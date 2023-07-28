using System;
using System.Collections.Generic;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003AD RID: 941
	internal class SoftShufflePool
	{
		// Token: 0x060014CF RID: 5327 RVA: 0x000557B7 File Offset: 0x00053BB7
		public SoftShufflePool(string key)
		{
			this.data.m_key = key.ToLower();
		}

		// Token: 0x060014D0 RID: 5328 RVA: 0x000557DB File Offset: 0x00053BDB
		public SoftShufflePool(SoftShufflePoolData poolData)
		{
			this.data = poolData;
			this.ValidateData();
			this.ArrangeElements();
		}

		// Token: 0x170001EF RID: 495
		// (get) Token: 0x060014D1 RID: 5329 RVA: 0x00055802 File Offset: 0x00053C02
		public string Key
		{
			get
			{
				return this.data.m_key;
			}
		}

		// Token: 0x060014D2 RID: 5330 RVA: 0x00055810 File Offset: 0x00053C10
		public void ValidateData()
		{
			if (string.IsNullOrEmpty(this.data.m_key))
			{
				Log.Warning<string>("[SoftShufflePool::ValidateData]Invalid pool key {0}", this.data.m_key);
				this.data.m_key = "default_key";
			}
			if (this.Marker < 0 || this.Marker >= this.data.m_elements.Count)
			{
				Log.Warning<int, int>("[SoftShufflePool::ValidateData]Invalid marker pos {0} (while elements count is {1})", this.Marker, this.data.m_elements.Count);
				this.Marker = 0;
			}
			bool flag = true;
			bool[] array = new bool[this.Elements.Count];
			for (int i = 0; i < this.Elements.Count; i++)
			{
				int pos = this.Elements[i].Pos;
				if (pos < 0 || pos >= this.Elements.Count)
				{
					Log.Warning<int>("[SoftShufflePool::ValidateData] Invalid position {0} found", pos);
					flag = false;
					break;
				}
				if (array[pos])
				{
					Log.Warning("[SoftShufflePool::ValidateData] Duplicate positions found");
					flag = false;
					break;
				}
				array[pos] = true;
			}
			if (flag)
			{
				bool[] array2 = array;
				for (int j = 0; j < array2.Length; j++)
				{
					if (!array2[j])
					{
						Log.Warning("[SoftShufflePool::ValidateData] Hole in positions found");
						flag = false;
						break;
					}
				}
			}
			if (!flag)
			{
				this.IndexElements();
			}
		}

		// Token: 0x060014D3 RID: 5331 RVA: 0x00055980 File Offset: 0x00053D80
		private void IndexElements()
		{
			if (this.HaveDuplicateIds(this.Elements))
			{
				Log.Warning("[SoftShufflePool::IndexElements] Duplicate elements in content found, aborting the indexing");
				return;
			}
			for (int i = 0; i < this.Elements.Count; i++)
			{
				this.Elements[i].Pos = i;
			}
		}

		// Token: 0x060014D4 RID: 5332 RVA: 0x000559D8 File Offset: 0x00053DD8
		public bool ArrangeElements()
		{
			List<SoftShufflePoolElement> list = new List<SoftShufflePoolElement>(this.Elements);
			bool[] array = new bool[this.Elements.Count];
			for (int i = 0; i < list.Count; i++)
			{
				int pos = list[i].Pos;
				if (array[pos])
				{
					this.Elements = list;
					Log.Warning("[SoftShufflePool::ArrangeElements] Duplicate positions found");
					return false;
				}
				this.Elements[pos] = list[i];
				array[pos] = true;
			}
			return true;
		}

		// Token: 0x060014D5 RID: 5333 RVA: 0x00055A60 File Offset: 0x00053E60
		public void SyncWithContent(List<SoftShufflePoolElement> content)
		{
			if (this.HaveDuplicateIds(content))
			{
				Log.Warning("[SoftShufflePool::SyncWithContent] Duplicate elements in content found, aborting the sync");
				return;
			}
			if (content.Count == 0)
			{
				this.Init();
				return;
			}
			if (this.Elements.Count == 0)
			{
				this.Elements.AddRange(content);
				Utils.Shuffle<SoftShufflePoolElement>(this.Elements);
			}
			else
			{
				using (List<SoftShufflePoolElement>.Enumerator enumerator = content.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						SoftShufflePoolElement el = enumerator.Current;
						SoftShufflePoolElement softShufflePoolElement = this.Elements.Find((SoftShufflePoolElement x) => x.Key == el.Key);
						if (softShufflePoolElement == null || softShufflePoolElement.Empty())
						{
							this.Elements.Insert(this.Marker, el);
						}
					}
				}
				List<SoftShufflePoolElement> list = new List<SoftShufflePoolElement>(this.Elements);
				using (List<SoftShufflePoolElement>.Enumerator enumerator2 = list.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						SoftShufflePoolElement el = enumerator2.Current;
						SoftShufflePoolElement softShufflePoolElement2 = content.Find((SoftShufflePoolElement x) => x.Key == el.Key);
						if (softShufflePoolElement2 == null || softShufflePoolElement2.Empty())
						{
							this.Elements.Remove(el);
							if (this.Marker > 0)
							{
								if (this.Marker < this.Elements.Count)
								{
									this.Marker--;
								}
								else
								{
									this.Marker = 0;
								}
							}
						}
					}
				}
			}
			this.IndexElements();
		}

		// Token: 0x060014D6 RID: 5334 RVA: 0x00055C28 File Offset: 0x00054028
		private SoftShufflePoolElement GetNextElementInternal(bool saveToDB)
		{
			SoftShufflePoolElement softShufflePoolElement = null;
			if (this.Marker < this.Elements.Count)
			{
				softShufflePoolElement = this.Elements[this.Marker];
				softShufflePoolElement.UsageCount++;
			}
			this.Marker++;
			if (this.Marker >= this.Elements.Count)
			{
				this.SoftShuffle();
				this.Marker = 0;
			}
			if (saveToDB)
			{
				this.SaveToDB();
			}
			return softShufflePoolElement;
		}

		// Token: 0x060014D7 RID: 5335 RVA: 0x00055CAB File Offset: 0x000540AB
		private SoftShufflePoolElement GetNextElementBackCompatibility()
		{
			return this.Elements[SoftShufflePool.m_rand.Next(0, this.Elements.Count - 1)];
		}

		// Token: 0x060014D8 RID: 5336 RVA: 0x00055CD0 File Offset: 0x000540D0
		public SoftShufflePoolElement GetNextElement(bool softShuffle)
		{
			if (!softShuffle)
			{
				return this.GetNextElementBackCompatibility();
			}
			return this.GetNextElementInternal(true);
		}

		// Token: 0x060014D9 RID: 5337 RVA: 0x00055CE6 File Offset: 0x000540E6
		public SoftShufflePoolElement GetNextElementEmulate()
		{
			return this.GetNextElementInternal(false);
		}

		// Token: 0x060014DA RID: 5338 RVA: 0x00055CEF File Offset: 0x000540EF
		private void Init()
		{
			this.Elements.Clear();
			this.Marker = 0;
			this.SoftShuffleIdx = 0;
		}

		// Token: 0x060014DB RID: 5339 RVA: 0x00055D0C File Offset: 0x0005410C
		private void SoftShuffle()
		{
			if (this.Elements.Count < 3)
			{
				return;
			}
			int num = this.SoftShuffleIdx % 2;
			int num2 = (this.Elements.Count - num) / 2;
			for (int i = 0; i < num2; i++)
			{
				if (SoftShufflePool.m_rand.Next(1, 100) <= 50)
				{
					SoftShufflePoolElement value = this.Elements[i * 2 + num];
					this.Elements[i * 2 + num] = this.Elements[i * 2 + num + 1];
					this.Elements[i * 2 + num + 1] = value;
				}
			}
			this.IndexElements();
			this.SoftShuffleIdx++;
		}

		// Token: 0x060014DC RID: 5340 RVA: 0x00055DC4 File Offset: 0x000541C4
		private void SaveToDB()
		{
			ServicesManager.GetService<IDALService>().MissionSystem.SaveSoftShufflePool(this.data);
		}

		// Token: 0x060014DD RID: 5341 RVA: 0x00055DDC File Offset: 0x000541DC
		private bool HaveDuplicateIds(List<SoftShufflePoolElement> elements)
		{
			int count = elements.Count;
			for (int i = 0; i < count - 1; i++)
			{
				for (int j = i + 1; j < count; j++)
				{
					if (elements[i].Key == elements[j].Key)
					{
						return true;
					}
				}
			}
			return false;
		}

		// Token: 0x170001F0 RID: 496
		// (get) Token: 0x060014DE RID: 5342 RVA: 0x00055E3D File Offset: 0x0005423D
		// (set) Token: 0x060014DF RID: 5343 RVA: 0x00055E4A File Offset: 0x0005424A
		private int Marker
		{
			get
			{
				return this.data.m_marker;
			}
			set
			{
				this.data.m_marker = value;
			}
		}

		// Token: 0x170001F1 RID: 497
		// (get) Token: 0x060014E0 RID: 5344 RVA: 0x00055E58 File Offset: 0x00054258
		// (set) Token: 0x060014E1 RID: 5345 RVA: 0x00055E65 File Offset: 0x00054265
		private int SoftShuffleIdx
		{
			get
			{
				return this.data.m_softShuffleIdx;
			}
			set
			{
				this.data.m_softShuffleIdx = value;
			}
		}

		// Token: 0x170001F2 RID: 498
		// (get) Token: 0x060014E2 RID: 5346 RVA: 0x00055E73 File Offset: 0x00054273
		// (set) Token: 0x060014E3 RID: 5347 RVA: 0x00055E80 File Offset: 0x00054280
		private List<SoftShufflePoolElement> Elements
		{
			get
			{
				return this.data.m_elements;
			}
			set
			{
				this.data.m_elements = value;
			}
		}

		// Token: 0x040009CE RID: 2510
		private const int SWAP_PROBABIILITY = 50;

		// Token: 0x040009CF RID: 2511
		private SoftShufflePoolData data = new SoftShufflePoolData();

		// Token: 0x040009D0 RID: 2512
		private static Random m_rand = new Random((int)DateTime.Now.Ticks);
	}
}

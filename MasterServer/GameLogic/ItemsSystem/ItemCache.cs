using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000349 RID: 841
	[Service]
	[Singleton]
	public class ItemCache : ServiceModule, IItemCache, IDBUpdateListener
	{
		// Token: 0x060012CF RID: 4815 RVA: 0x0004BC32 File Offset: 0x0004A032
		public ItemCache(IDBUpdateService dbUpdateService)
		{
			this.m_dbUpdateService = dbUpdateService;
		}

		// Token: 0x14000038 RID: 56
		// (add) Token: 0x060012D0 RID: 4816 RVA: 0x0004BC44 File Offset: 0x0004A044
		// (remove) Token: 0x060012D1 RID: 4817 RVA: 0x0004BC7C File Offset: 0x0004A07C
		public event Action<IEnumerable<SItem>> ItemsCacheUpdated;

		// Token: 0x170001B4 RID: 436
		// (get) Token: 0x060012D2 RID: 4818 RVA: 0x0004BCB2 File Offset: 0x0004A0B2
		// (set) Token: 0x060012D3 RID: 4819 RVA: 0x0004BCCD File Offset: 0x0004A0CD
		private bool CacheValid
		{
			get
			{
				return this.m_cacheValid && ServicesManager.ExecutionPhase >= ExecutionPhase.Starting;
			}
			set
			{
				this.m_cacheValid = value;
			}
		}

		// Token: 0x170001B5 RID: 437
		// (get) Token: 0x060012D4 RID: 4820 RVA: 0x0004BCD6 File Offset: 0x0004A0D6
		private IDALService DAL
		{
			get
			{
				return ServicesManager.GetService<IDALService>();
			}
		}

		// Token: 0x060012D5 RID: 4821 RVA: 0x0004BCDD File Offset: 0x0004A0DD
		public override void Init()
		{
			this.CacheValid = false;
			this.m_dbUpdateService.RegisterListener(this);
		}

		// Token: 0x060012D6 RID: 4822 RVA: 0x0004BCF2 File Offset: 0x0004A0F2
		public override void Stop()
		{
			this.m_allItems = null;
			this.m_allItemsByName = null;
			this.m_allActiveItems = null;
			this.m_allActiveItemsByName = null;
			this.m_dbUpdateService.UnregisterListener(this);
		}

		// Token: 0x060012D7 RID: 4823 RVA: 0x0004BD1C File Offset: 0x0004A11C
		public int GetItemsHash()
		{
			if (this.m_itemsHash == null)
			{
				this.CalculateHash();
			}
			return this.m_itemsHash.Value;
		}

		// Token: 0x060012D8 RID: 4824 RVA: 0x0004BD40 File Offset: 0x0004A140
		private void CalculateHash()
		{
			SortedList<ulong, int> sortedList = new SortedList<ulong, int>();
			Dictionary<ulong, SItem> allItems = this.GetAllItems();
			foreach (SItem sitem in allItems.Values)
			{
				if (sitem.Active)
				{
					sortedList.Add(sitem.ID, sitem.GetHashCode());
				}
			}
			CRC32 crc = new CRC32();
			crc.GetHash(this.GetListBuffer(sortedList));
			this.m_itemsHash = new int?((int)crc.CRCVal);
		}

		// Token: 0x060012D9 RID: 4825 RVA: 0x0004BDE8 File Offset: 0x0004A1E8
		private void CheckCache()
		{
			if (!this.CacheValid)
			{
				this.UpdateCache();
			}
		}

		// Token: 0x060012DA RID: 4826 RVA: 0x0004BDFC File Offset: 0x0004A1FC
		public void UpdateCache()
		{
			Dictionary<ulong, SItem> dictionary = new Dictionary<ulong, SItem>();
			Dictionary<string, SItem> dictionary2 = new Dictionary<string, SItem>();
			Dictionary<ulong, SItem> dictionary3 = new Dictionary<ulong, SItem>();
			Dictionary<string, SItem> dictionary4 = new Dictionary<string, SItem>();
			Dictionary<ulong, SEquipItem> dictionary5 = new Dictionary<ulong, SEquipItem>();
			foreach (SItem sitem in this.DAL.ItemSystem.GetAllItems())
			{
				dictionary.Add(sitem.ID, sitem);
			}
			foreach (SItem sitem2 in dictionary.Values)
			{
				try
				{
					dictionary2.Add(sitem2.Name, sitem2);
				}
				catch (ArgumentException innerException)
				{
					throw new ApplicationException(string.Format("Item with name {0} has a duplicate by name.", sitem2.Name), innerException);
				}
				if (sitem2.Active)
				{
					dictionary3.Add(sitem2.ID, sitem2);
					dictionary4.Add(sitem2.Name, sitem2);
				}
			}
			foreach (SEquipItem sequipItem in this.DAL.ItemSystem.GetDefaultProfileItems())
			{
				if (dictionary3.ContainsKey(sequipItem.ItemID))
				{
					dictionary5.Add(sequipItem.ProfileItemID, sequipItem);
				}
			}
			Interlocked.Exchange<Dictionary<ulong, SItem>>(ref this.m_allItems, dictionary);
			Interlocked.Exchange<Dictionary<string, SItem>>(ref this.m_allItemsByName, dictionary2);
			Interlocked.Exchange<Dictionary<ulong, SItem>>(ref this.m_allActiveItems, dictionary3);
			Interlocked.Exchange<Dictionary<string, SItem>>(ref this.m_allActiveItemsByName, dictionary4);
			Interlocked.Exchange<Dictionary<ulong, SEquipItem>>(ref this.m_allDefaultActiveItems, dictionary5);
			this.CacheValid = true;
			this.ItemsCacheUpdated.SafeInvoke(this.m_allItems.Values);
		}

		// Token: 0x060012DB RID: 4827 RVA: 0x0004C00C File Offset: 0x0004A40C
		private byte[] GetListBuffer(SortedList<ulong, int> hash_list)
		{
			byte[] array = new byte[hash_list.Count * 4];
			uint num = 0U;
			foreach (KeyValuePair<ulong, int> keyValuePair in hash_list)
			{
				Array.Copy(BitConverter.GetBytes(keyValuePair.Value), 0L, array, (long)((ulong)num), 4L);
				num += 4U;
			}
			return array;
		}

		// Token: 0x060012DC RID: 4828 RVA: 0x0004C088 File Offset: 0x0004A488
		public Dictionary<ulong, SItem> GetAllItems()
		{
			return this.GetAllItems(true);
		}

		// Token: 0x060012DD RID: 4829 RVA: 0x0004C091 File Offset: 0x0004A491
		public Dictionary<string, SItem> GetAllItemsByName()
		{
			return this.GetAllItemsByName(true);
		}

		// Token: 0x060012DE RID: 4830 RVA: 0x0004C09A File Offset: 0x0004A49A
		public bool TryGetItem(string name, out SItem item)
		{
			return this.TryGetItem(name, true, out item);
		}

		// Token: 0x060012DF RID: 4831 RVA: 0x0004C0A5 File Offset: 0x0004A4A5
		public Dictionary<ulong, SItem> GetAllItems(bool activeOnly)
		{
			this.CheckCache();
			return (!activeOnly) ? this.m_allItems : this.m_allActiveItems;
		}

		// Token: 0x060012E0 RID: 4832 RVA: 0x0004C0C4 File Offset: 0x0004A4C4
		public Dictionary<string, SItem> GetAllItemsByName(bool activeOnly)
		{
			this.CheckCache();
			return (!activeOnly) ? this.m_allItemsByName : this.m_allActiveItemsByName;
		}

		// Token: 0x060012E1 RID: 4833 RVA: 0x0004C0E3 File Offset: 0x0004A4E3
		public bool TryGetItem(string name, bool activeOnly, out SItem item)
		{
			return this.GetAllItemsByName(activeOnly).TryGetValue(name, out item);
		}

		// Token: 0x060012E2 RID: 4834 RVA: 0x0004C0F3 File Offset: 0x0004A4F3
		public Dictionary<ulong, SEquipItem> GetDefaultProfileItems()
		{
			this.CheckCache();
			return this.m_allDefaultActiveItems;
		}

		// Token: 0x060012E3 RID: 4835 RVA: 0x0004C104 File Offset: 0x0004A504
		public bool InsertItems(Stream inputStream)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(inputStream);
				this.CheckCache();
				Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>();
				IEnumerator enumerator = xmlDocument.DocumentElement.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode xmlNode = (XmlNode)obj;
						if (xmlNode.NodeType == XmlNodeType.Element)
						{
							XmlElement xmlElement = xmlNode as XmlElement;
							bool flag = xmlElement.GetAttribute("testcontent") == "1";
							if (Resources.DebugContentEnabled || !flag)
							{
								bool flag2 = false;
								string attribute = xmlElement.GetAttribute("name");
								SItem sitem;
								bool flag3 = !this.m_allItemsByName.TryGetValue(attribute, out sitem);
								if (flag3)
								{
									sitem = new SItem
									{
										Name = attribute
									};
								}
								else
								{
									dictionary.Add(sitem.ID, 0);
								}
								string attribute2 = xmlElement.GetAttribute("slots");
								bool flag4 = xmlElement.GetAttribute("active") == "1";
								bool flag5 = xmlElement.GetAttribute("locked") == "1";
								bool flag6 = xmlElement.GetAttribute("shopcontent") == "1";
								string attribute3 = xmlElement.GetAttribute("type");
								if (attribute2 != sitem.Slots || flag4 != sitem.Active || flag5 != sitem.Locked || flag6 != sitem.ShopContent || attribute3 != sitem.Type)
								{
									flag2 = true;
									sitem.Slots = attribute2;
									sitem.Active = flag4;
									sitem.Locked = flag5;
									sitem.ShopContent = flag6;
									sitem.Type = attribute3;
								}
								if (flag3)
								{
									this.DAL.ItemSystem.AddItem(sitem);
									Log.Info<string>("Inserting new item '{0}'", attribute);
								}
								else if (flag2)
								{
									this.DAL.ItemSystem.UpdateItem(sitem);
									Log.Info<string>("Updating existing item '{0}'", attribute);
								}
							}
						}
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				foreach (SItem sitem2 in this.m_allItems.Values)
				{
					if (!dictionary.ContainsKey(sitem2.ID))
					{
						this.DAL.ItemSystem.ActivateItem(sitem2.ID, false);
						Log.Info<string>("Disable missed item '{0}'", sitem2.Name);
					}
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
				return false;
			}
			return true;
		}

		// Token: 0x060012E4 RID: 4836 RVA: 0x0004C420 File Offset: 0x0004A820
		public bool OnDBUpdateStage(IDBUpdateService updater, DBUpdateStage stage)
		{
			if (stage == DBUpdateStage.PostUpdate)
			{
				this.UpdateCache();
			}
			return true;
		}

		// Token: 0x040008B2 RID: 2226
		private Dictionary<ulong, SItem> m_allItems;

		// Token: 0x040008B3 RID: 2227
		private Dictionary<string, SItem> m_allItemsByName;

		// Token: 0x040008B4 RID: 2228
		private Dictionary<ulong, SItem> m_allActiveItems;

		// Token: 0x040008B5 RID: 2229
		private Dictionary<string, SItem> m_allActiveItemsByName;

		// Token: 0x040008B6 RID: 2230
		private Dictionary<ulong, SEquipItem> m_allDefaultActiveItems;

		// Token: 0x040008B7 RID: 2231
		private int? m_itemsHash;

		// Token: 0x040008B8 RID: 2232
		private bool m_cacheValid;

		// Token: 0x040008B9 RID: 2233
		private readonly IDBUpdateService m_dbUpdateService;
	}
}

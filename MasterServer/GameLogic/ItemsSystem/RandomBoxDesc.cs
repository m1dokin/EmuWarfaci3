using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.GameLogic.RandomBoxValidationSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000362 RID: 866
	public class RandomBoxDesc : IItemsContainer
	{
		// Token: 0x170001C5 RID: 453
		// (get) Token: 0x06001368 RID: 4968 RVA: 0x0004F638 File Offset: 0x0004DA38
		// (set) Token: 0x06001369 RID: 4969 RVA: 0x0004F640 File Offset: 0x0004DA40
		public string Name { get; set; }

		// Token: 0x0600136A RID: 4970 RVA: 0x0004F64C File Offset: 0x0004DA4C
		public bool IsValid()
		{
			bool flag = true;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (RandomBoxDesc.Group group in this.Groups)
			{
				HashSet<string> hashSet2 = new HashSet<string>();
				foreach (RandomBoxDesc.Choice choice in group.Choices)
				{
					if (hashSet2.Add(choice.Name))
					{
						flag &= hashSet.Add(choice.Name);
						if (!flag)
						{
							throw new RandomBoxValidationException(string.Format("RandomBox {0} contains same item {1} in different groups", this.Name, choice.Name));
						}
					}
				}
			}
			return true;
		}

		// Token: 0x0600136B RID: 4971 RVA: 0x0004F740 File Offset: 0x0004DB40
		public bool HasItemNamed(string name)
		{
			return this.Groups.SelectMany((RandomBoxDesc.Group group) => group.Choices).Any((RandomBoxDesc.Choice choice) => choice.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
		}

		// Token: 0x04000907 RID: 2311
		public List<RandomBoxDesc.Group> Groups = new List<RandomBoxDesc.Group>();

		// Token: 0x02000363 RID: 867
		public class Choice : GenericItemBase
		{
			// Token: 0x0600136D RID: 4973 RVA: 0x0004F79C File Offset: 0x0004DB9C
			public Choice(IDictionary<string, string> @params) : base(@params)
			{
				string text;
				if (@params.TryGetValue("weight", out text))
				{
					this.m_weight = new float?(float.Parse(text));
				}
				if (@params.TryGetValue("top_prize_token", out text))
				{
					this.TopPrizeToken = text;
				}
				this.WinLimit = ((!@params.TryGetValue("win_limit", out text)) ? 0 : int.Parse(text));
			}

			// Token: 0x170001C6 RID: 454
			// (get) Token: 0x0600136E RID: 4974 RVA: 0x0004F810 File Offset: 0x0004DC10
			// (set) Token: 0x0600136F RID: 4975 RVA: 0x0004F818 File Offset: 0x0004DC18
			public string TopPrizeToken { get; private set; }

			// Token: 0x170001C7 RID: 455
			// (get) Token: 0x06001370 RID: 4976 RVA: 0x0004F821 File Offset: 0x0004DC21
			// (set) Token: 0x06001371 RID: 4977 RVA: 0x0004F829 File Offset: 0x0004DC29
			public int WinLimit { get; private set; }

			// Token: 0x170001C8 RID: 456
			// (get) Token: 0x06001372 RID: 4978 RVA: 0x0004F832 File Offset: 0x0004DC32
			public bool HasWeight
			{
				get
				{
					return this.m_weight != null;
				}
			}

			// Token: 0x06001373 RID: 4979 RVA: 0x0004F83F File Offset: 0x0004DC3F
			public bool HasTopPrizeTokenDefined()
			{
				return !string.IsNullOrEmpty(this.TopPrizeToken);
			}

			// Token: 0x06001374 RID: 4980 RVA: 0x0004F84F File Offset: 0x0004DC4F
			[Obsolete("Get your dirty hands off this method - it's used for testing purposes only!!!")]
			public void __setWinLimitTo(int value)
			{
				this.WinLimit = value;
			}

			// Token: 0x170001C9 RID: 457
			// (get) Token: 0x06001375 RID: 4981 RVA: 0x0004F858 File Offset: 0x0004DC58
			// (set) Token: 0x06001376 RID: 4982 RVA: 0x0004F865 File Offset: 0x0004DC65
			public float Weight
			{
				get
				{
					return this.m_weight.Value;
				}
				set
				{
					this.m_weight = new float?(value);
				}
			}

			// Token: 0x04000909 RID: 2313
			public const string WeightParam = "weight";

			// Token: 0x0400090A RID: 2314
			public const string TopPrizeTokenParam = "top_prize_token";

			// Token: 0x0400090B RID: 2315
			public const string WinLimitParam = "win_limit";

			// Token: 0x0400090C RID: 2316
			private float? m_weight;
		}

		// Token: 0x02000364 RID: 868
		public class Group
		{
			// Token: 0x0400090F RID: 2319
			public string Type;

			// Token: 0x04000910 RID: 2320
			public List<RandomBoxDesc.Choice> Choices = new List<RandomBoxDesc.Choice>();
		}
	}
}

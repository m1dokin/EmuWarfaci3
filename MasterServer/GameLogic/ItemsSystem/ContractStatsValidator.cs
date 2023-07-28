using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.GameLogic.ContractSystem;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200032D RID: 813
	[Service]
	[Singleton]
	internal class ContractStatsValidator : IItemStatsValidator
	{
		// Token: 0x06001251 RID: 4689 RVA: 0x00049140 File Offset: 0x00047540
		public ContractStatsValidator(IItemCache itemCache, IItemStats itemStats)
		{
			this.m_itemCache = itemCache;
			this.m_itemStats = itemStats;
			ConfigSection section = Resources.ModuleSettings.GetSection("Contracts");
			this.ContractSetSize = uint.Parse(section.Get("SetSize"));
		}

		// Token: 0x170001AC RID: 428
		// (get) Token: 0x06001252 RID: 4690 RVA: 0x00049187 File Offset: 0x00047587
		// (set) Token: 0x06001253 RID: 4691 RVA: 0x0004918F File Offset: 0x0004758F
		public uint ContractSetSize { get; private set; }

		// Token: 0x06001254 RID: 4692 RVA: 0x00049198 File Offset: 0x00047598
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			Dictionary<uint, List<ContractDesc>> contractsDesc = this.m_itemStats.GetContractsDesc();
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			bool flag = false;
			HashSet<string> hashSet = new HashSet<string>();
			foreach (KeyValuePair<uint, List<ContractDesc>> keyValuePair in contractsDesc)
			{
				if ((long)keyValuePair.Value.Count < (long)((ulong)this.ContractSetSize))
				{
					throw new ContractValidationException(string.Format("Contract set {0} has incorrect contact count {1}, must be {2}", keyValuePair.Key, keyValuePair.Value.Count, this.ContractSetSize));
				}
				HashSet<EContractType> hashSet2 = new HashSet<EContractType>();
				using (List<ContractDesc>.Enumerator enumerator2 = keyValuePair.Value.GetEnumerator())
				{
					while (enumerator2.MoveNext())
					{
						ContractDesc contract = enumerator2.Current;
						if (!hashSet2.Add(contract.Type))
						{
							throw new ContractValidationException(string.Format("Contract {0} in set {1} has duplicate  type {2}", contract.Name, contract.Id, contract.Type));
						}
						if (!hashSet.Add(contract.Name))
						{
							throw new ContractValidationException(string.Format("Contract {0} in set {1} has contract with same name", contract.Name, contract.Id));
						}
						if (allItemsByName.All((KeyValuePair<string, SItem> x) => x.Key != contract.Reward.Name))
						{
							throw new ContractValidationException(string.Format("Contract {0} in set {1} has incorrect reward {2}", contract.Name, contract.Id, contract.Reward.Name));
						}
						StoreOffer storeOffer = offers.FirstOrDefault((StoreOffer x) => x.Content.Item.Name == contract.Name);
						if (storeOffer != null && storeOffer.Type != OfferType.Expiration)
						{
							throw new ContractValidationException(string.Format("Contract {0} in set {1} has incorrect offer type {2}, must be {3}", new object[]
							{
								contract.Name,
								contract.Id,
								storeOffer.Type,
								OfferType.Expiration
							}));
						}
						contract.IsActive = (storeOffer != null);
						flag |= contract.IsActive;
					}
				}
				int num = keyValuePair.Value.Count((ContractDesc x) => x.IsActive);
				if (num > 0 && (long)num < (long)((ulong)this.ContractSetSize))
				{
					Log.Warning<uint, int, uint>("Contract set {0}, has incorrect amount of offers {1}, must be {2}", keyValuePair.Key, num, this.ContractSetSize);
				}
			}
			if (!flag)
			{
				Log.Warning("All contracts are inactive");
			}
		}

		// Token: 0x0400086E RID: 2158
		private readonly IItemStats m_itemStats;

		// Token: 0x0400086F RID: 2159
		private readonly IItemCache m_itemCache;
	}
}

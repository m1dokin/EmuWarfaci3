using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000341 RID: 833
	[QueryAttributes(TagName = "extend_item")]
	internal class ExtendItemQuery : BaseQuery
	{
		// Token: 0x060012AB RID: 4779 RVA: 0x0004B070 File Offset: 0x00049470
		public ExtendItemQuery(ICatalogService catalogService, IProfileItems profileItems, IItemService itemService)
		{
			this.m_catalogService = catalogService;
			this.m_profileItemsService = profileItems;
			this.m_itemService = itemService;
		}

		// Token: 0x060012AC RID: 4780 RVA: 0x0004B090 File Offset: 0x00049490
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ExtendItemQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					int supplierId = int.Parse(request.GetAttribute("supplier_id"));
					ulong offerId = ulong.Parse(request.GetAttribute("offer_id"));
					ulong num = ulong.Parse(request.GetAttribute("item_id"));
					long offerHash = long.Parse(request.GetAttribute("hash"));
					TransactionStatus transactionStatus = TransactionStatus.OK;
					try
					{
						this.m_itemService.ExtendItem(user, supplierId, offerId, offerHash, num);
					}
					catch (ItemServiceNotEnoughtMoneyException)
					{
						transactionStatus = TransactionStatus.NOT_ENOUGH_MONEY;
					}
					catch (ItemServiceHashMismatchException)
					{
						transactionStatus = TransactionStatus.HASH_MISMATCH;
					}
					catch (ItemServiceException e)
					{
						transactionStatus = TransactionStatus.INTERNAL_ERROR;
						Log.Error(e);
					}
					if (transactionStatus != TransactionStatus.INTERNAL_ERROR)
					{
						List<CustomerAccount> customerAccounts = this.m_catalogService.GetCustomerAccounts(user.UserID);
						Dictionary<Currency, long> dictionary = new Dictionary<Currency, long>
						{
							{
								Currency.GameMoney,
								-1L
							},
							{
								Currency.CryMoney,
								-1L
							},
							{
								Currency.CrownMoney,
								-1L
							}
						};
						foreach (CustomerAccount customerAccount in customerAccounts)
						{
							dictionary[customerAccount.Currency] = (long)customerAccount.Money;
						}
						if (transactionStatus == TransactionStatus.OK)
						{
							SProfileItem profileItem = this.m_profileItemsService.GetProfileItem(user.ProfileID, num);
							response.SetAttribute("durability", profileItem.DurabilityPoints.ToString());
							response.SetAttribute("total_durability", profileItem.TotalDurabilityPoints.ToString());
							response.SetAttribute("expiration_time_utc", profileItem.ExpirationTimeUTC.ToString());
							response.SetAttribute("seconds_left", profileItem.SecondsLeft.ToString());
						}
						response.SetAttribute("cry_money", dictionary[Currency.CryMoney].ToString());
						response.SetAttribute("game_money", dictionary[Currency.GameMoney].ToString());
						response.SetAttribute("crown_money", dictionary[Currency.CrownMoney].ToString());
						string name = "error_status";
						int num2 = (int)transactionStatus;
						response.SetAttribute(name, num2.ToString(CultureInfo.InvariantCulture));
						result = 0;
					}
					else
					{
						result = -1;
					}
				}
			}
			return result;
		}

		// Token: 0x0400089F RID: 2207
		private readonly ICatalogService m_catalogService;

		// Token: 0x040008A0 RID: 2208
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x040008A1 RID: 2209
		private readonly IItemService m_itemService;
	}
}

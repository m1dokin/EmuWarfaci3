using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.DAL.Exceptions;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000012 RID: 18
	internal class ECatalog : IECatalog
	{
		// Token: 0x06000089 RID: 137 RVA: 0x00005014 File Offset: 0x00003214
		public ECatalog(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600008A RID: 138 RVA: 0x00005065 File Offset: 0x00003265
		private ECatConnectionPool ConnectionPool
		{
			get
			{
				return this.m_dal.ConnectionPool;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00005074 File Offset: 0x00003274
		public DALResultMulti<StoreOffer> GetStoreOffers()
		{
			DALStats stats = new DALStats();
			CacheProxy.Options<StoreOffer> options = new CacheProxy.Options<StoreOffer>
			{
				stats = stats,
				get_data_stream = delegate()
				{
					Dictionary<ulong, StoreOffer> dictionary = new Dictionary<ulong, StoreOffer>();
					using (MySqlAccessor mySqlAccessor = new MySqlAccessor(this.ConnectionPool, stats))
					{
						using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL ECatGetStoreItems()", new object[0]))
						{
							while (dbdataReader.Read())
							{
								ulong storeID = ulong.Parse(dbdataReader["store_id"].ToString());
								StoreOffer storeOffer = new StoreOffer();
								storeOffer.StoreID = storeID;
								storeOffer.Category = dbdataReader["category"].ToString();
								storeOffer.Status = dbdataReader["offer_status"].ToString().ToLower();
								storeOffer.Discount = uint.Parse(dbdataReader["discount"].ToString());
								storeOffer.Rank = int.Parse(dbdataReader["rank"].ToString());
								storeOffer.Content.DurabilityPoints = int.Parse(dbdataReader["durability_points"].ToString());
								storeOffer.Content.RepairCost = dbdataReader["repair_cost"].ToString();
								storeOffer.Content.ExpirationTime = TimeUtils.UTCTimestampToTimeSpan(ulong.Parse(dbdataReader["expiration_time"].ToString()));
								storeOffer.Content.Quantity = ulong.Parse(dbdataReader["quantity"].ToString());
								storeOffer.Content.Item = new CatalogItem
								{
									ID = ulong.Parse(dbdataReader["catalog_id"].ToString()),
									Name = dbdataReader["item_name"].ToString(),
									Active = ParseUtils.ParseBool(dbdataReader["active"].ToString()),
									Stackable = ParseUtils.ParseBool(dbdataReader["stackable"].ToString()),
									Type = dbdataReader["type"].ToString()
								};
								storeOffer.Type = (OfferType)Enum.Parse(typeof(OfferType), dbdataReader["offer_type"].ToString());
								dictionary.Add(storeOffer.StoreID, storeOffer);
							}
						}
						using (DBDataReader dbdataReader2 = mySqlAccessor.ExecuteReader("CALL ECatGetOfferPrices()", new object[0]))
						{
							while (dbdataReader2.Read())
							{
								ulong key = ulong.Parse(dbdataReader2["store_id"].ToString());
								StoreOffer storeOffer2;
								if (dictionary.TryGetValue(key, out storeOffer2))
								{
									PriceTag price;
									price.Currency = (Currency)uint.Parse(dbdataReader2["currency_id"].ToString());
									price.KeyCatalogName = ((!(dbdataReader2["key_catalog_id"].ToString() != "0")) ? string.Empty : dbdataReader2["item_name"].ToString());
									price.Price = ulong.Parse(dbdataReader2["price"].ToString());
									storeOffer2.AddOriginalPrice(price);
								}
							}
						}
					}
					return new List<StoreOffer>(dictionary.Values);
				}
			};
			return this.m_dal.CacheProxy.GetStream<StoreOffer>(options);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x000050D0 File Offset: 0x000032D0
		public DALResultMulti<CatalogItem> GetCatalogItems()
		{
			CacheProxy.Options<CatalogItem> options = new CacheProxy.Options<CatalogItem>
			{
				connection_pool = this.ConnectionPool,
				db_query = "CALL ECatGetCatalogItems()",
				db_serializer = this.m_catalogItemSerializer
			};
			return this.m_dal.CacheProxy.GetStream<CatalogItem>(options);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x0000511C File Offset: 0x0000331C
		public DALResult<ulong> AddItem(string name, string description, int max_amount, bool stackable, string type)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("SELECT ECatAddItem(?name, ?desc, ?max, ?stackable, ?type)", new object[]
			{
				"?name",
				name,
				"?desc",
				description,
				"?max",
				max_amount,
				"?stackable",
				(!stackable) ? 0 : 1,
				"?type",
				type
			});
			ulong val = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x0600008E RID: 142 RVA: 0x000051CC File Offset: 0x000033CC
		public DALResultVoid RemoveFromStore(ulong store_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatRemoveFromStore(?sid)", new object[]
			{
				"?sid",
				store_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600008F RID: 143 RVA: 0x00005220 File Offset: 0x00003420
		public DALResultVoid ActivateItem(ulong store_id, bool active)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatActivateItem(?sid, ?active)", new object[]
			{
				"?sid",
				store_id,
				"?active",
				(!active) ? "0" : "1"
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000090 RID: 144 RVA: 0x00005294 File Offset: 0x00003494
		public DALResultVoid UpdateOffer(StoreOffer offer)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatUpdateStore(?sid, ?cid, ?cat, ?exp, ?dur, ?rep, ?quantity, ?status, ?discount, ?rank)", new object[]
			{
				"?sid",
				offer.StoreID,
				"?cid",
				offer.Content.Item.ID,
				"?cat",
				offer.Category,
				"?exp",
				TimeUtils.TimeSpanToUTCTimestamp(offer.Content.ExpirationTime),
				"?dur",
				offer.Content.DurabilityPoints,
				"?rep",
				offer.Content.RepairCost,
				"?quantity",
				offer.Content.Quantity,
				"?status",
				offer.Status,
				"?discount",
				offer.Discount,
				"?rank",
				offer.Rank
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000091 RID: 145 RVA: 0x000053D4 File Offset: 0x000035D4
		public DALResultVoid UpdatePrice(ulong store_id, PriceTag price)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatUpdatePrice(?sid, ?curr, ?key_name, ?price)", new object[]
			{
				"?sid",
				store_id,
				"?curr",
				(int)price.Currency,
				"?key_name",
				price.KeyCatalogName,
				"?price",
				price.Price
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000092 RID: 146 RVA: 0x00005468 File Offset: 0x00003668
		public DALResultMulti<CustomerItem> GetCustomerItems(ulong customer_id)
		{
			CacheProxy.Options<CustomerItem> options = new CacheProxy.Options<CustomerItem>
			{
				connection_pool = this.ConnectionPool,
				db_serializer = this.m_customerItemSerializer
			};
			options.query("CALL ECatGetCustomerItems(?cid, ?realmid)", new object[]
			{
				"?cid",
				customer_id,
				"?realmid",
				Resources.RealmId
			});
			return this.m_dal.CacheProxy.GetStream<CustomerItem>(options);
		}

		// Token: 0x06000093 RID: 147 RVA: 0x000054E0 File Offset: 0x000036E0
		public DALResultVoid SetMoney(ulong customer_id, Currency currency, ulong money)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("CALL ECatSetMoney(?cid, ?realmid, ?curr, ?money)", new object[]
			{
				"?cid",
				customer_id,
				"?realmid",
				(currency != Currency.CryMoney) ? Resources.RealmId : Resources.GlobalRealmId,
				"?curr",
				(int)currency,
				"?money",
				money
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000094 RID: 148 RVA: 0x00005584 File Offset: 0x00003784
		public DALResultMulti<CustomerAccount> GetCustomerAccounts(ulong customer_id)
		{
			CacheProxy.Options<CustomerAccount> options = new CacheProxy.Options<CustomerAccount>
			{
				connection_pool = this.ConnectionPool,
				db_serializer = this.m_customerAccountSerializer,
				db_transaction = true
			};
			options.query("CALL ECatGetCustomerAccounts(?cid, ?realmid, ?globalrealmid)", new object[]
			{
				"?cid",
				customer_id,
				"?realmid",
				Resources.RealmId,
				"?globalrealmid",
				Resources.GlobalRealmId
			});
			return this.m_dal.CacheProxy.GetStream<CustomerAccount>(options);
		}

		// Token: 0x06000095 RID: 149 RVA: 0x00005618 File Offset: 0x00003818
		public DALResult<AddCustomerItemResponse> AddCustomerItem(ulong customer_id, OfferItem item, bool stackingEnabled, bool ignoreLimit)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("SELECT ECatAddCustomerItem(?cid, ?realmid, ?catid, ?exp, ?dur, ?repcost, ?quant, ?stacking, ?ignore_limit)", new object[]
			{
				"?cid",
				customer_id,
				"?realmid",
				Resources.RealmId,
				"?catid",
				item.Item.ID,
				"?exp",
				TimeUtils.TimeSpanToUTCTimestamp(item.ExpirationTime),
				"?dur",
				item.DurabilityPoints,
				"?quant",
				item.Quantity,
				"?repcost",
				item.RepairCost,
				"?stacking",
				Convert.ToByte(stackingEnabled),
				"?ignore_limit",
				Convert.ToByte(ignoreLimit)
			});
			ulong num = ulong.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			AddCustomerItemResponse val = new AddCustomerItemResponse
			{
				Status = ((num >= 18446744073709551610UL) ? this.ToTransactionStatus(num) : TransactionStatus.OK),
				Items = new List<ulong>
				{
					num
				}
			};
			return new DALResult<AddCustomerItemResponse>(val, setOptions.stats);
		}

		// Token: 0x06000096 RID: 150 RVA: 0x00005794 File Offset: 0x00003994
		public DALResult<PurchaseOfferResponse> AddCustomerBoxItems(ulong customer_id, ulong box_instance_id, IEnumerable<OfferItem> items, bool stackingEnabled)
		{
			ECatalog.<AddCustomerBoxItems>c__AnonStorey1 <AddCustomerBoxItems>c__AnonStorey = new ECatalog.<AddCustomerBoxItems>c__AnonStorey1();
			<AddCustomerBoxItems>c__AnonStorey.items = items;
			<AddCustomerBoxItems>c__AnonStorey.customer_id = customer_id;
			<AddCustomerBoxItems>c__AnonStorey.stackingEnabled = stackingEnabled;
			<AddCustomerBoxItems>c__AnonStorey.box_instance_id = box_instance_id;
			<AddCustomerBoxItems>c__AnonStorey.$this = this;
			DALStats dalstats = new DALStats();
			<AddCustomerBoxItems>c__AnonStorey.response = default(PurchaseOfferResponse);
			<AddCustomerBoxItems>c__AnonStorey.response.Status = TransactionStatus.INVALID_REQUEST;
			<AddCustomerBoxItems>c__AnonStorey.response.Items = new List<KeyValuePair<TransactionStatus, ulong>>();
			DALResult<PurchaseOfferResponse> result;
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(this.ConnectionPool, dalstats))
			{
				acc.Transaction(delegate()
				{
					<AddCustomerBoxItems>c__AnonStorey.response.Status = TransactionStatus.OK;
					foreach (OfferItem offerItem in <AddCustomerBoxItems>c__AnonStorey.items)
					{
						TransactionStatus transactionStatus = TransactionStatus.OK;
						ulong num = ulong.Parse(acc.ExecuteScalar("SELECT ECatAddCustomerItem(?cid, ?realmid, ?catid, ?exp, ?dur, ?repcost, ?quant, ?stacking, ?ignore_limit)", new object[]
						{
							"?cid",
							<AddCustomerBoxItems>c__AnonStorey.customer_id,
							"?realmid",
							Resources.RealmId,
							"?catid",
							offerItem.Item.ID,
							"?exp",
							TimeUtils.TimeSpanToUTCTimestamp(offerItem.ExpirationTime),
							"?dur",
							offerItem.DurabilityPoints,
							"?repcost",
							offerItem.RepairCost,
							"?quant",
							offerItem.Quantity,
							"?stacking",
							Convert.ToByte(<AddCustomerBoxItems>c__AnonStorey.stackingEnabled),
							"?ignore_limit",
							0
						}).ToString());
						if (num >= 18446744073709551610UL)
						{
							transactionStatus = <AddCustomerBoxItems>c__AnonStorey.$this.ToTransactionStatus(num);
							if (transactionStatus != TransactionStatus.LIMIT_REACHED)
							{
								<AddCustomerBoxItems>c__AnonStorey.response.Status = transactionStatus;
								return false;
							}
						}
						<AddCustomerBoxItems>c__AnonStorey.response.Items.Add(new KeyValuePair<TransactionStatus, ulong>(transactionStatus, num));
					}
					acc.ExecuteNonQuery("CALL ECatDeleteCustomerItem(?cid, ?realmid, ?inst)", new object[]
					{
						"?cid",
						<AddCustomerBoxItems>c__AnonStorey.customer_id,
						"?realmid",
						Resources.RealmId,
						"?inst",
						<AddCustomerBoxItems>c__AnonStorey.box_instance_id
					});
					return true;
				});
				result = new DALResult<PurchaseOfferResponse>(<AddCustomerBoxItems>c__AnonStorey.response, dalstats);
			}
			return result;
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00005870 File Offset: 0x00003A70
		public DALResult<List<PurchaseOfferResponse>> PurchaseOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback)
		{
			return this.PurchaseOffers(customerId, supplierId, offers, offerHash, true, stackingEnabled, paymentCallback);
		}

		// Token: 0x06000098 RID: 152 RVA: 0x00005882 File Offset: 0x00003A82
		public DALResult<List<PurchaseOfferResponse>> PurchaseIngameCoinsOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, IPaymentCallback paymentCallback)
		{
			return this.PurchaseOffers(customerId, supplierId, offers, offerHash, false, false, paymentCallback);
		}

		// Token: 0x06000099 RID: 153 RVA: 0x00005894 File Offset: 0x00003A94
		private DALResult<List<PurchaseOfferResponse>> PurchaseOffers(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool needCheckHash, bool stackingEnabled, IPaymentCallback paymentCallback)
		{
			DALStats stats = new DALStats();
			List<PurchaseOfferResponse> list = new List<PurchaseOfferResponse>(offers.Count);
			foreach (ulong offerId in offers)
			{
				PurchaseOfferResponse item = this.PurchaseOffer(customerId, supplierId, offerId, stats, offerHash, needCheckHash, stackingEnabled, paymentCallback);
				list.Add(item);
				if (item.Status != TransactionStatus.OK)
				{
					break;
				}
			}
			return new DALResult<List<PurchaseOfferResponse>>(list, stats);
		}

		// Token: 0x0600009A RID: 154 RVA: 0x00005928 File Offset: 0x00003B28
		private PurchaseOfferResponse PurchaseOffer(ulong customerId, int supplierId, ulong offerId, DALStats stats, long offerHash, bool needCheckHash, bool stackingEnabled, IPaymentCallback paymentCallback)
		{
			Func<MySqlAccessorTransaction, object> purchaseAction = (MySqlAccessorTransaction acc) => acc.ExecuteScalar("SELECT ECatOfferPurchase(?cid, ?realmid, ?globalrealmid, ?sid, ?hash, ?needCheckHash, ?stacking)", new object[]
			{
				"?cid",
				customerId,
				"?realmid",
				Resources.RealmId,
				"?globalrealmid",
				Resources.GlobalRealmId,
				"?sid",
				offerId,
				"?hash",
				offerHash,
				"?needCheckHash",
				(!needCheckHash) ? "False" : "True",
				"?stacking",
				Convert.ToUInt16(stackingEnabled)
			});
			return this.PurchaseOfferImpl(customerId, supplierId, offerId, stats, purchaseAction, paymentCallback);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x00005988 File Offset: 0x00003B88
		private PurchaseOfferResponse PurchaseOfferImpl(ulong customerId, int supplierId, ulong offerId, DALStats stats, Func<MySqlAccessorTransaction, object> purchaseAction, IPaymentCallback paymentCallback)
		{
			PurchaseOfferResponse response = new PurchaseOfferResponse
			{
				OfferId = offerId,
				Status = TransactionStatus.INVALID_REQUEST,
				Items = new List<KeyValuePair<TransactionStatus, ulong>>()
			};
			PurchaseOfferResponse response2;
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(this.ConnectionPool, stats))
			{
				ulong instanceId = ulong.MaxValue;
				MySqlAccessor.TransactionDelegateBool deleg = delegate()
				{
					object obj = purchaseAction(acc);
					instanceId = ulong.Parse(obj.ToString());
					if (instanceId >= 18446744073709551610UL)
					{
						response.Status = this.ToTransactionStatus(instanceId);
						return false;
					}
					PaymentCallbackResult paymentCallbackResult = PaymentCallbackResult.Ok;
					try
					{
						if (paymentCallback != null)
						{
							paymentCallbackResult = paymentCallback.SpendMoneyByOfferId(customerId, supplierId, offerId);
						}
					}
					catch
					{
						response.Status = TransactionStatus.INTERNAL_ERROR;
						throw;
					}
					if (paymentCallbackResult != PaymentCallbackResult.Ok)
					{
						response.Status = TransactionStatus.NOT_ENOUGH_MONEY;
						return false;
					}
					response.Status = TransactionStatus.OK;
					return true;
				};
				if (acc.Transaction(deleg, DBAccessMode.Master))
				{
					response.Items.Add(new KeyValuePair<TransactionStatus, ulong>(response.Status, instanceId));
				}
				response2 = response;
			}
			return response2;
		}

		// Token: 0x0600009C RID: 156 RVA: 0x00005AAC File Offset: 0x00003CAC
		public DALResult<List<PurchaseOfferResponse>> PurchaseOffersWithKeys(ulong customerId, int supplierId, IList<ulong> offers, long offerHash, bool stackingEnabled, IPaymentCallback paymentCallback)
		{
			DALStats stats = new DALStats();
			List<PurchaseOfferResponse> list = new List<PurchaseOfferResponse>(offers.Count);
			using (IEnumerator<ulong> enumerator = offers.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					ulong offer = enumerator.Current;
					Func<MySqlAccessorTransaction, object> purchaseAction = (MySqlAccessorTransaction acc) => acc.ExecuteScalar("SELECT ECatOfferPurchaseWithKeys(?cid, ?realmid, ?globalrealmid, ?sid, ?hash, ?stacking)", new object[]
					{
						"?cid",
						customerId,
						"?realmid",
						Resources.RealmId,
						"?globalrealmid",
						Resources.GlobalRealmId,
						"?sid",
						offer,
						"?hash",
						offerHash,
						"?stacking",
						Convert.ToUInt16(stackingEnabled)
					});
					PurchaseOfferResponse item = this.PurchaseOfferImpl(customerId, supplierId, offer, stats, purchaseAction, paymentCallback);
					list.Add(item);
					if (item.Status != TransactionStatus.OK)
					{
						break;
					}
				}
			}
			return new DALResult<List<PurchaseOfferResponse>>(list, stats);
		}

		// Token: 0x0600009D RID: 157 RVA: 0x00005B88 File Offset: 0x00003D88
		public DALResult<ConsumeItemResponse> ConsumeItem(ulong customerId, ulong itemId, ushort quantity)
		{
			ConsumeItemResponse val = default(ConsumeItemResponse);
			val.Status = TransactionStatus.INVALID_REQUEST;
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("SELECT ECatConsumeItem(?cid, ?realmid, ?inst, ?quantity)", new object[]
			{
				"?cid",
				customerId,
				"?realmid",
				Resources.RealmId,
				"?inst",
				itemId,
				"?quantity",
				quantity
			});
			object obj = this.m_dal.CacheProxy.SetScalar(setOptions);
			ulong num = ulong.Parse(obj.ToString());
			if (num < 18446744073709551610UL)
			{
				val.Status = TransactionStatus.OK;
				val.ItemsLeft = (ushort)num;
			}
			else
			{
				val.Status = this.ToTransactionStatus(num);
			}
			return new DALResult<ConsumeItemResponse>(val, setOptions.stats);
		}

		// Token: 0x0600009E RID: 158 RVA: 0x00005C74 File Offset: 0x00003E74
		public DALResultVoid RepairPermanentItem(ulong customerId, ulong itemId, int durability, int totalDurability)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("CALL ECatRepairPermanentItem(?iid, ?curdur, ?totdur)", new object[]
			{
				"?iid",
				itemId,
				"?curdur",
				durability,
				"?totdur",
				totalDurability
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0600009F RID: 159 RVA: 0x00005CF4 File Offset: 0x00003EF4
		public DALResultVoid UpdateCatalogItemDurability(ulong customerId, ulong catalogId, int addDurability)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("CALL ECatUpdateCatalogItemDurability(?cid, ?iid, ?dur)", new object[]
			{
				"?cid",
				customerId,
				"?iid",
				catalogId,
				"?dur",
				addDurability
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000A0 RID: 160 RVA: 0x00005D74 File Offset: 0x00003F74
		public DALResult<MoneyUpdateResult> SpendMoney(ulong customerId, Currency currency, ulong ammount, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback)
		{
			DALResult<MoneyUpdateResultMulti> dalresult = this.SpendMoney(customerId, new KeyValuePair<Currency, ulong>[]
			{
				new KeyValuePair<Currency, ulong>(currency, ammount)
			}, catalogId, spendMoneyReason, paymentCallback);
			return new DALResult<MoneyUpdateResult>(new MoneyUpdateResult
			{
				Status = dalresult.Value.Status,
				Money = dalresult.Value.Money[0].Value
			}, dalresult.Stats);
		}

		// Token: 0x060000A1 RID: 161 RVA: 0x00005DF0 File Offset: 0x00003FF0
		public DALResult<MoneyUpdateResultMulti> SpendMoney(ulong customerId, IEnumerable<KeyValuePair<Currency, ulong>> money, ulong catalogId, SpendMoneyReason spendMoneyReason, IPaymentCallback paymentCallback)
		{
			ECatalog.<SpendMoney>c__AnonStorey9 <SpendMoney>c__AnonStorey = new ECatalog.<SpendMoney>c__AnonStorey9();
			<SpendMoney>c__AnonStorey.money = money;
			<SpendMoney>c__AnonStorey.customerId = customerId;
			<SpendMoney>c__AnonStorey.catalogId = catalogId;
			<SpendMoney>c__AnonStorey.paymentCallback = paymentCallback;
			<SpendMoney>c__AnonStorey.spendMoneyReason = spendMoneyReason;
			<SpendMoney>c__AnonStorey.$this = this;
			DALStats dalstats = new DALStats();
			<SpendMoney>c__AnonStorey.res = default(MoneyUpdateResultMulti);
			<SpendMoney>c__AnonStorey.res.Status = TransactionStatus.OK;
			<SpendMoney>c__AnonStorey.res.Money = new List<KeyValuePair<Currency, ulong>>();
			foreach (KeyValuePair<Currency, ulong> keyValuePair in <SpendMoney>c__AnonStorey.money)
			{
				<SpendMoney>c__AnonStorey.res.Money.Add(new KeyValuePair<Currency, ulong>(keyValuePair.Key, 0UL));
			}
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(this.ConnectionPool, dalstats))
			{
				MySqlAccessor.TransactionDelegateBool deleg = delegate()
				{
					int num = 0;
					foreach (KeyValuePair<Currency, ulong> keyValuePair2 in <SpendMoney>c__AnonStorey.money)
					{
						object obj = acc.ExecuteScalar("SELECT ECatSpendMoney(?cid, ?realmid, ?curr, ?amm, ?sid)", new object[]
						{
							"?cid",
							<SpendMoney>c__AnonStorey.customerId,
							"?realmid",
							(keyValuePair2.Key != Currency.CryMoney) ? Resources.RealmId : Resources.GlobalRealmId,
							"?curr",
							(int)keyValuePair2.Key,
							"?amm",
							keyValuePair2.Value,
							"?sid",
							<SpendMoney>c__AnonStorey.catalogId
						});
						ulong num2 = ulong.Parse(obj.ToString());
						if (num2 >= 18446744073709551610UL)
						{
							<SpendMoney>c__AnonStorey.res.Status = <SpendMoney>c__AnonStorey.$this.ToTransactionStatus(num2);
							return false;
						}
						if (keyValuePair2.Key == Currency.CryMoney)
						{
							PaymentCallbackResult paymentCallbackResult = PaymentCallbackResult.Ok;
							try
							{
								if (<SpendMoney>c__AnonStorey.paymentCallback != null)
								{
									paymentCallbackResult = <SpendMoney>c__AnonStorey.paymentCallback.SpendMoney(<SpendMoney>c__AnonStorey.customerId, keyValuePair2.Value, <SpendMoney>c__AnonStorey.spendMoneyReason);
								}
							}
							catch
							{
								<SpendMoney>c__AnonStorey.res.Status = TransactionStatus.INTERNAL_ERROR;
								throw;
							}
							if (paymentCallbackResult != PaymentCallbackResult.Ok)
							{
								<SpendMoney>c__AnonStorey.res.Status = TransactionStatus.NOT_ENOUGH_MONEY;
								return false;
							}
						}
						<SpendMoney>c__AnonStorey.res.Money[num++] = new KeyValuePair<Currency, ulong>(keyValuePair2.Key, num2);
					}
					return true;
				};
				acc.Transaction(deleg);
			}
			return new DALResult<MoneyUpdateResultMulti>(<SpendMoney>c__AnonStorey.res, dalstats);
		}

		// Token: 0x060000A2 RID: 162 RVA: 0x00005F40 File Offset: 0x00004140
		public DALResultVoid DeleteCustomerItem(ulong customer_id, ulong instance_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("CALL ECatDeleteCustomerItem(?cid, ?realmid, ?inst)", new object[]
			{
				"?cid",
				customer_id,
				"?realmid",
				Resources.RealmId,
				"?inst",
				instance_id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00005FC4 File Offset: 0x000041C4
		public DALResultVoid AddMoney(ulong customerId, Currency currencyId, ulong money, string transactionId, TimeSpan lifeTime)
		{
			ECatalog.<AddMoney>c__AnonStoreyB <AddMoney>c__AnonStoreyB = new ECatalog.<AddMoney>c__AnonStoreyB();
			<AddMoney>c__AnonStoreyB.currencyId = currencyId;
			<AddMoney>c__AnonStoreyB.transactionId = transactionId;
			<AddMoney>c__AnonStoreyB.customerId = customerId;
			<AddMoney>c__AnonStoreyB.lifeTime = lifeTime;
			<AddMoney>c__AnonStoreyB.money = money;
			DALStats dalstats = new DALStats();
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(this.ConnectionPool, dalstats))
			{
				if (!acc.Transaction(delegate()
				{
					if (<AddMoney>c__AnonStoreyB.currencyId == Currency.CryMoney && !string.IsNullOrEmpty(<AddMoney>c__AnonStoreyB.transactionId))
					{
						object obj = acc.ExecuteScalar("SELECT ECatStoreTransaction(?cid, ?transactionId, ?lifeTime)", new object[]
						{
							"?cid",
							<AddMoney>c__AnonStoreyB.customerId,
							"?transactionId",
							<AddMoney>c__AnonStoreyB.transactionId,
							"?lifeTime",
							(uint)Math.Ceiling(<AddMoney>c__AnonStoreyB.lifeTime.TotalSeconds)
						});
						ulong num = ulong.Parse(obj.ToString());
						if (num == 18446744073709551615UL)
						{
							return false;
						}
					}
					acc.ExecuteNonQuery("CALL ECatAddMoney(?cid, ?realmid, ?curr, ?money)", new object[]
					{
						"?cid",
						<AddMoney>c__AnonStoreyB.customerId,
						"?realmid",
						(<AddMoney>c__AnonStoreyB.currencyId != Currency.CryMoney) ? Resources.RealmId : Resources.GlobalRealmId,
						"?curr",
						(uint)<AddMoney>c__AnonStoreyB.currencyId,
						"?money",
						<AddMoney>c__AnonStoreyB.money
					});
					return true;
				}))
				{
					throw new DalException(string.Format("Collision detected for transactionId {0}, user {1}", <AddMoney>c__AnonStoreyB.transactionId, <AddMoney>c__AnonStoreyB.customerId));
				}
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00006094 File Offset: 0x00004294
		private TransactionStatus ToTransactionStatus(ulong code)
		{
			code = ulong.MaxValue - code;
			if (code >= 0UL && code <= 5UL)
			{
				switch ((int)code)
				{
				case 0:
					return TransactionStatus.NOT_ENOUGH_MONEY;
				case 1:
					return TransactionStatus.INVALID_REQUEST;
				case 2:
					return TransactionStatus.OUT_OF_STORE;
				case 3:
					return TransactionStatus.LIMIT_REACHED;
				case 4:
					return TransactionStatus.KEY_TIME_OUT;
				case 5:
					return TransactionStatus.HASH_MISMATCH;
				}
			}
			return TransactionStatus.INVALID_REQUEST;
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x000060EC File Offset: 0x000042EC
		public DALResult<bool> TryLockUpdaterPermission(string onlineId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			bool val = false;
			try
			{
				setOptions.query("SELECT ECatLockUpdaterPermission(?oid)", new object[]
				{
					"?oid",
					onlineId
				});
				object obj = this.m_dal.CacheProxy.SetScalar(setOptions);
				val = (int.Parse(obj.ToString()) != 0);
			}
			catch (TransactionError)
			{
				val = true;
			}
			return new DALResult<bool>(val, setOptions.stats);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x00006180 File Offset: 0x00004380
		public DALResultVoid UnlockUpdaterPermission(string onlineId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatUnlockUpdaterPermission(?oid)", new object[]
			{
				"?oid",
				onlineId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x000061D0 File Offset: 0x000043D0
		public DALResultVoid ResetUpdaterPermission()
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatResetUpdaterPermission()", new object[0]);
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000A8 RID: 168 RVA: 0x00006214 File Offset: 0x00004414
		public DALResult<bool> BackupLogs(TimeSpan logRecordLifetime, TimeSpan dbTimeout, int batchSize)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_cmd_timeout = (int)dbTimeout.TotalSeconds
			};
			setOptions.query("SELECT ECatBackupLogs(?record_lifetime_min, ?batch_size)", new object[]
			{
				"?record_lifetime_min",
				(int)logRecordLifetime.TotalMinutes,
				"?batch_size",
				batchSize
			});
			bool val = int.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString()) > 0;
			return new DALResult<bool>(val, setOptions.stats);
		}

		// Token: 0x060000A9 RID: 169 RVA: 0x000062A8 File Offset: 0x000044A8
		public DALResult<bool> DebugExpireItem(ulong customerId, ulong itemId, uint seconds)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("SELECT ECatExpireCustomerItem(?cid, ?realmid, ?inst, ?time)", new object[]
			{
				"?cid",
				customerId,
				"?realmid",
				Resources.RealmId,
				"?inst",
				itemId,
				"?time",
				seconds
			});
			object obj = this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<bool>(int.Parse(obj.ToString()) == 0, setOptions.stats);
		}

		// Token: 0x060000AA RID: 170 RVA: 0x00006354 File Offset: 0x00004554
		public DALResultVoid DebugResetCustomerItems(ulong customer_id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("CALL ECatDebugResetCustomerItems(?cid, ?realmid)", new object[]
			{
				"?cid",
				customer_id,
				"?realmid",
				Resources.RealmId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000AB RID: 171 RVA: 0x000063C4 File Offset: 0x000045C4
		public DALResult<string> GetTotalDataVersionStamp()
		{
			DALStats dalstats = new DALStats();
			DALResult<string> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(this.ConnectionPool, dalstats))
			{
				object obj = mySqlAccessor.ExecuteScalar("CALL ECatGetTotalDataVersionStamp()", new object[0]);
				result = new DALResult<string>(obj.ToString(), dalstats);
			}
			return result;
		}

		// Token: 0x060000AC RID: 172 RVA: 0x00006428 File Offset: 0x00004628
		public DALResultMulti<SVersionStamp> GetDataVersionStamps()
		{
			DALStats dalstats = new DALStats();
			DALResultMulti<SVersionStamp> result;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(this.ConnectionPool, dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("CALL ECatGetDataVersionStamps()", new object[0]))
				{
					IEnumerable<SVersionStamp> val = SerializeHelper.Deserialize<SVersionStamp>(dbdataReader, this.m_versionStampSerializer);
					result = new DALResultMulti<SVersionStamp>(val, dalstats);
				}
			}
			return result;
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000064B0 File Offset: 0x000046B0
		public DALResultVoid SetDataVersionStamp(string group, string hash)
		{
			DALStats dalstats = new DALStats();
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(this.ConnectionPool, dalstats))
			{
				mySqlAccessor.ExecuteNonQuery("CALL ECatSetDataVersionStamp(?group, ?hash)", new object[]
				{
					"?group",
					group,
					"?hash",
					hash
				});
			}
			return new DALResultVoid(dalstats);
		}

		// Token: 0x060000AE RID: 174 RVA: 0x00006524 File Offset: 0x00004724
		public DALResultMulti<EcatLogHistory> DebugGetLogHistory(ulong customerId)
		{
			CacheProxy.Options<EcatLogHistory> options = new CacheProxy.Options<EcatLogHistory>
			{
				connection_pool = this.ConnectionPool,
				db_serializer = this.m_ecatLogHistorySerializer
			};
			options.query("CALL ECatDebugGetLogHistory(?cid)", new object[]
			{
				"?cid",
				customerId
			});
			return this.m_dal.CacheProxy.GetStream<EcatLogHistory>(options);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x00006584 File Offset: 0x00004784
		public DALResultVoid DebugGenEcatRecords(uint count, uint dayInterval)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatLogDebugGenerateRecords(?count, ?dayInterval)", new object[]
			{
				"?count",
				count,
				"?dayInterval",
				dayInterval
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x060000B0 RID: 176 RVA: 0x000065EC File Offset: 0x000047EC
		public DALResultVoid DebugClearGiveMoneyTransactionHistory(ulong customerId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL ECatClearTransactionHistory(?cid)", new object[]
			{
				"?cid",
				customerId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000041 RID: 65
		private const ulong FIRST_ERRCODE = 18446744073709551610UL;

		// Token: 0x04000042 RID: 66
		private readonly DAL m_dal;

		// Token: 0x04000043 RID: 67
		private readonly CatalogItemSerializer m_catalogItemSerializer = new CatalogItemSerializer();

		// Token: 0x04000044 RID: 68
		private readonly CustomerItemSerializer m_customerItemSerializer = new CustomerItemSerializer();

		// Token: 0x04000045 RID: 69
		private readonly CustomerAccountSerializer m_customerAccountSerializer = new CustomerAccountSerializer();

		// Token: 0x04000046 RID: 70
		private readonly VersionStampSerializer m_versionStampSerializer = new VersionStampSerializer();

		// Token: 0x04000047 RID: 71
		private readonly EcatLogHistorySerializer m_ecatLogHistorySerializer = new EcatLogHistorySerializer();
	}
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GameLogic.ContractSystem;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200036D RID: 877
	[Service]
	[Singleton]
	internal class ItemStats : ServiceModule, IItemStats
	{
		// Token: 0x06001399 RID: 5017 RVA: 0x0004FA9E File Offset: 0x0004DE9E
		public ItemStats(IItemCache itemCache, ITagService tagService, IConfigProvider<XmlDocument> configProvider)
		{
			this.m_itemCache = itemCache;
			this.m_tagService = tagService;
			this.m_configProvider = configProvider;
		}

		// Token: 0x14000042 RID: 66
		// (add) Token: 0x0600139A RID: 5018 RVA: 0x0004FABC File Offset: 0x0004DEBC
		// (remove) Token: 0x0600139B RID: 5019 RVA: 0x0004FAF4 File Offset: 0x0004DEF4
		public event Action ItemStatsUpdated;

		// Token: 0x0600139C RID: 5020 RVA: 0x0004FB2C File Offset: 0x0004DF2C
		public override void Start()
		{
			base.Start();
			XmlDocument itemsData = this.m_configProvider.Get();
			this.UpdateStats(itemsData);
		}

		// Token: 0x0600139D RID: 5021 RVA: 0x0004FB52 File Offset: 0x0004DF52
		public Dictionary<ulong, StackableItemStats> GetStackableItemStats()
		{
			return this.m_stackableItemStats;
		}

		// Token: 0x0600139E RID: 5022 RVA: 0x0004FB5C File Offset: 0x0004DF5C
		public StackableItemStats GetStackableItemStats(ulong itemId)
		{
			StackableItemStats result;
			this.m_stackableItemStats.TryGetValue(itemId, out result);
			return result;
		}

		// Token: 0x0600139F RID: 5023 RVA: 0x0004FB79 File Offset: 0x0004DF79
		public bool IsVipItem(ulong item_id)
		{
			return this.m_vipItems.Contains(item_id);
		}

		// Token: 0x060013A0 RID: 5024 RVA: 0x0004FB87 File Offset: 0x0004DF87
		public bool IsBoosterItem(ulong item_id)
		{
			return this.m_boosterDescs.ContainsKey(item_id);
		}

		// Token: 0x060013A1 RID: 5025 RVA: 0x0004FB98 File Offset: 0x0004DF98
		public RandomBoxDesc GetRandomBoxDesc(ulong item_id)
		{
			RandomBoxDesc result;
			this.m_randomBoxDescs.TryGetValue(item_id, out result);
			return result;
		}

		// Token: 0x060013A2 RID: 5026 RVA: 0x0004FBB8 File Offset: 0x0004DFB8
		public BundleDesc GetBundleDesc(ulong itemId)
		{
			BundleDesc result;
			this.m_bundleDescs.TryGetValue(itemId, out result);
			return result;
		}

		// Token: 0x060013A3 RID: 5027 RVA: 0x0004FBD8 File Offset: 0x0004DFD8
		public BoosterDesc GetBoosterDesc(ulong item_id)
		{
			BoosterDesc result;
			this.m_boosterDescs.TryGetValue(item_id, out result);
			return result;
		}

		// Token: 0x060013A4 RID: 5028 RVA: 0x0004FBF8 File Offset: 0x0004DFF8
		public MetaGameDesc GetMetaGameDesc(ulong itemId)
		{
			MetaGameDesc result;
			this.m_metaGameDescs.TryGetValue(itemId, out result);
			return result;
		}

		// Token: 0x060013A5 RID: 5029 RVA: 0x0004FC15 File Offset: 0x0004E015
		public Dictionary<uint, List<ContractDesc>> GetContractsDesc()
		{
			return this.m_contracts;
		}

		// Token: 0x060013A6 RID: 5030 RVA: 0x0004FC1D File Offset: 0x0004E01D
		public IList<RandomBoxDesc> GetRandomBoxesDesc()
		{
			return this.m_randomBoxDescs.Values.ToList<RandomBoxDesc>();
		}

		// Token: 0x060013A7 RID: 5031 RVA: 0x0004FC2F File Offset: 0x0004E02F
		public IList<BundleDesc> GetBundlesDesc()
		{
			return this.m_bundleDescs.Values.ToList<BundleDesc>();
		}

		// Token: 0x060013A8 RID: 5032 RVA: 0x0004FC41 File Offset: 0x0004E041
		public IEnumerable<MetaGameDesc> GetMetaGameDescs()
		{
			return this.m_metaGameDescs.Values.ToList<MetaGameDesc>();
		}

		// Token: 0x060013A9 RID: 5033 RVA: 0x0004FC54 File Offset: 0x0004E054
		public ContractDesc GetContractByName(string name)
		{
			ContractDesc contractDesc = null;
			foreach (KeyValuePair<uint, List<ContractDesc>> keyValuePair in this.m_contracts)
			{
				contractDesc = keyValuePair.Value.FirstOrDefault((ContractDesc x) => x.Name == name);
				if (contractDesc != null)
				{
					break;
				}
			}
			return contractDesc;
		}

		// Token: 0x060013AA RID: 5034 RVA: 0x0004FCE0 File Offset: 0x0004E0E0
		public bool IsItemAvailableForUser(string itemName, UserInfo.User user)
		{
			SItem sitem;
			return !string.IsNullOrEmpty(itemName) && this.m_itemCache.GetAllItemsByName().TryGetValue(itemName, out sitem) && this.IsItemAvailableForUser(sitem.ID, user);
		}

		// Token: 0x060013AB RID: 5035 RVA: 0x0004FD20 File Offset: 0x0004E120
		public bool IsItemAvailableForUser(ulong itemId, UserInfo.User user)
		{
			if (user == null)
			{
				return true;
			}
			TaggedItemDesc taggedItemDesc = this.GetTaggedItemDesc(itemId);
			if (taggedItemDesc == null)
			{
				return true;
			}
			UserTags userTags = this.m_tagService.GetUserTags(user.UserID);
			return taggedItemDesc.Filter.Check(userTags);
		}

		// Token: 0x060013AC RID: 5036 RVA: 0x0004FD64 File Offset: 0x0004E164
		public SItem GetAccessItemByMisisonType(string missionType)
		{
			SItem result;
			this.m_accessItemDescs.TryGetValue(missionType, out result);
			return result;
		}

		// Token: 0x060013AD RID: 5037 RVA: 0x0004FD84 File Offset: 0x0004E184
		public TaggedItemDesc GetTaggedItemDesc(ulong itemId)
		{
			TaggedItemDesc result;
			this.m_taggedItemsDescs.TryGetValue(itemId, out result);
			return result;
		}

		// Token: 0x060013AE RID: 5038 RVA: 0x0004FDA4 File Offset: 0x0004E1A4
		private void UpdateStats(XmlDocument itemsData)
		{
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			Dictionary<ulong, StackableItemStats> dictionary = new Dictionary<ulong, StackableItemStats>();
			Dictionary<ulong, RandomBoxDesc> dictionary2 = new Dictionary<ulong, RandomBoxDesc>();
			Dictionary<ulong, BoosterDesc> dictionary3 = new Dictionary<ulong, BoosterDesc>();
			Dictionary<ulong, TaggedItemDesc> dictionary4 = new Dictionary<ulong, TaggedItemDesc>();
			Dictionary<string, SItem> dictionary5 = new Dictionary<string, SItem>();
			Dictionary<uint, List<ContractDesc>> dictionary6 = new Dictionary<uint, List<ContractDesc>>();
			Dictionary<ulong, MetaGameDesc> dictionary7 = new Dictionary<ulong, MetaGameDesc>();
			Dictionary<ulong, BundleDesc> dictionary8 = new Dictionary<ulong, BundleDesc>();
			List<ulong> list = new List<ulong>();
			IEnumerator enumerator = itemsData.DocumentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					string attribute = xmlElement.GetAttribute("name");
					SItem sitem;
					if (!allItemsByName.TryGetValue(attribute, out sitem))
					{
						Log.Info<string>("Skip non-active item {0}", attribute);
					}
					else if (xmlElement.FirstChild != null && xmlElement.FirstChild.FirstChild != null)
					{
						XmlElement xmlElement2 = (XmlElement)xmlElement.FirstChild.FirstChild;
						foreach (XmlElement xmlElement3 in xmlElement2.ChildNodes.OfType<XmlElement>())
						{
							if (xmlElement3.Name == "mmo_stats")
							{
								if (this.IsVipItem(xmlElement3))
								{
									list.Add(sitem.ID);
								}
								StackableItemStats stackableItemStats = this.ParseStackableStats(sitem, xmlElement3);
								if (stackableItemStats != null)
								{
									dictionary.Add(sitem.ID, stackableItemStats);
								}
								TaggedItemDesc taggedItemDesc = this.ParseItemTagsStats(sitem.ID, xmlElement3);
								if (taggedItemDesc != null)
								{
									dictionary4.Add(sitem.ID, taggedItemDesc);
								}
							}
							else if (xmlElement3.Name == "random_box")
							{
								RandomBoxDesc randomBoxDesc = ItemStats.ParseRandomBox(sitem, xmlElement3);
								if (randomBoxDesc.IsValid())
								{
									dictionary2.Add(sitem.ID, randomBoxDesc);
								}
							}
							else if (xmlElement3.Name == "GameParams" && sitem.IsBoosterItem)
							{
								BoosterDesc value = this.ParseBooster(xmlElement3);
								dictionary3.Add(sitem.ID, value);
							}
							else if (xmlElement3.Name == "contract")
							{
								ContractDesc contractDesc = this.ParseContract(sitem.ID, attribute, xmlElement3);
								List<ContractDesc> list2;
								if (dictionary6.TryGetValue(contractDesc.Id, out list2))
								{
									list2.Add(contractDesc);
								}
								else
								{
									dictionary6.Add(contractDesc.Id, new List<ContractDesc>
									{
										contractDesc
									});
								}
							}
							else if (xmlElement3.Name == "access")
							{
								string attribute2 = xmlElement3.GetAttribute("mission_type");
								dictionary5.Add(attribute2, sitem);
							}
							else if (xmlElement3.Name == "metagame_stats")
							{
								MetaGameDesc value2 = this.ParseMetaGameStats(sitem, xmlElement3);
								dictionary7.Add(sitem.ID, value2);
							}
							else if (xmlElement3.Name == "bundle")
							{
								dictionary8.Add(sitem.ID, ItemStats.ParseBundle(sitem, xmlElement3));
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
			IEnumerator enumerator3 = itemsData.DocumentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator3.MoveNext())
				{
					object obj2 = enumerator3.Current;
					XmlElement xmlElement4 = (XmlElement)obj2;
					string attribute3 = xmlElement4.GetAttribute("name");
					SItem sitem2;
					if (!allItemsByName.TryGetValue(attribute3, out sitem2))
					{
						Log.Info<string>("Skip non-active item {0}", attribute3);
					}
					else if (xmlElement4.FirstChild != null && xmlElement4.FirstChild.FirstChild != null)
					{
						XmlElement xmlElement5 = (XmlElement)xmlElement4.FirstChild.FirstChild;
						IEnumerator enumerator4 = xmlElement5.ChildNodes.GetEnumerator();
						try
						{
							while (enumerator4.MoveNext())
							{
								object obj3 = enumerator4.Current;
								XmlNode xmlNode = (XmlNode)obj3;
								if (xmlNode.NodeType == XmlNodeType.Element)
								{
									XmlElement xmlElement6 = (XmlElement)xmlNode;
									if (xmlElement6.Name == "content")
									{
										this.ParseContentStats(xmlElement6, sitem2.ID, dictionary3);
									}
								}
							}
						}
						finally
						{
							IDisposable disposable2;
							if ((disposable2 = (enumerator4 as IDisposable)) != null)
							{
								disposable2.Dispose();
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable3;
				if ((disposable3 = (enumerator3 as IDisposable)) != null)
				{
					disposable3.Dispose();
				}
			}
			Interlocked.Exchange<Dictionary<ulong, StackableItemStats>>(ref this.m_stackableItemStats, dictionary);
			Interlocked.Exchange<Dictionary<ulong, RandomBoxDesc>>(ref this.m_randomBoxDescs, dictionary2);
			Interlocked.Exchange<Dictionary<ulong, BoosterDesc>>(ref this.m_boosterDescs, dictionary3);
			Interlocked.Exchange<Dictionary<ulong, TaggedItemDesc>>(ref this.m_taggedItemsDescs, dictionary4);
			Interlocked.Exchange<List<ulong>>(ref this.m_vipItems, list);
			Interlocked.Exchange<Dictionary<uint, List<ContractDesc>>>(ref this.m_contracts, dictionary6);
			Interlocked.Exchange<Dictionary<string, SItem>>(ref this.m_accessItemDescs, dictionary5);
			Interlocked.Exchange<Dictionary<ulong, MetaGameDesc>>(ref this.m_metaGameDescs, dictionary7);
			Interlocked.Exchange<Dictionary<ulong, BundleDesc>>(ref this.m_bundleDescs, dictionary8);
			this.ItemStatsUpdated.SafeInvoke();
		}

		// Token: 0x060013AF RID: 5039 RVA: 0x00050318 File Offset: 0x0004E718
		private StackableItemStats ParseStackableStats(SItem item, XmlElement statElement)
		{
			bool flag = false;
			ushort num = 0;
			IEnumerator enumerator = statElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						string attribute = xmlElement.GetAttribute("name");
						if (attribute == "stackable")
						{
							flag = (xmlElement.GetAttribute("value") == "1");
						}
						else if (attribute == "max_buy_amount")
						{
							num = ushort.Parse(xmlElement.GetAttribute("value"));
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
			if (flag && num == 0)
			{
				throw new ApplicationException("Invalid max buy amount for stackable item");
			}
			return new StackableItemStats
			{
				IsStackable = flag,
				MaxBuyAmount = num
			};
		}

		// Token: 0x060013B0 RID: 5040 RVA: 0x00050424 File Offset: 0x0004E824
		private bool IsVipItem(XmlElement stat_el)
		{
			IEnumerator enumerator = stat_el.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						string attribute = xmlElement.GetAttribute("name");
						if (attribute == "vip")
						{
							return int.Parse(xmlElement.GetAttribute("value")) > 0;
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
			return false;
		}

		// Token: 0x060013B1 RID: 5041 RVA: 0x000504D8 File Offset: 0x0004E8D8
		private static RandomBoxDesc ParseRandomBox(SItem item, XmlNode randomBoxNode)
		{
			RandomBoxDesc randomBoxDesc = new RandomBoxDesc
			{
				Name = item.Name
			};
			foreach (XmlElement xmlElement in randomBoxNode.ChildNodes.OfType<XmlElement>())
			{
				RandomBoxDesc.Group group = new RandomBoxDesc.Group
				{
					Type = xmlElement.GetAttribute("type")
				};
				float num = 0f;
				foreach (XmlElement xmlElement2 in xmlElement.ChildNodes.OfType<XmlElement>())
				{
					RandomBoxDesc.Choice choice = new RandomBoxDesc.Choice(xmlElement2.Attributes.OfType<XmlAttribute>().ToDictionary((XmlAttribute p) => p.Name, (XmlAttribute p) => p.Value));
					if (!choice.HasWeight || choice.Weight <= 0f)
					{
						throw new Exception("Incorrect choice weight for random box");
					}
					num += choice.Weight;
					group.Choices.Add(choice);
				}
				if (group.Choices.Count <= 1)
				{
					throw new Exception(string.Format("Incorrect number of choices ({0}) in random box group for {1}", group.Choices.Count, randomBoxDesc.Name));
				}
				foreach (RandomBoxDesc.Choice choice2 in group.Choices)
				{
					choice2.Weight /= num;
				}
				randomBoxDesc.Groups.Add(group);
			}
			return randomBoxDesc;
		}

		// Token: 0x060013B2 RID: 5042 RVA: 0x0005070C File Offset: 0x0004EB0C
		private static BundleDesc ParseBundle(SItem item, XmlNode bundleNode)
		{
			BundleDesc bundleDesc = new BundleDesc
			{
				Name = item.Name
			};
			bundleDesc.Items.AddRange(from el in bundleNode.ChildNodes.OfType<XmlElement>()
			select new
			{
				el = el,
				@params = el.Attributes.OfType<XmlAttribute>().ToDictionary((XmlAttribute p) => p.Name, (XmlAttribute p) => p.Value)
			} into <>__TranspIdent4
			select new
			{
				<>__TranspIdent4 = <>__TranspIdent4,
				bundledItem = new BundleDesc.BundledItem(<>__TranspIdent4.@params)
			} into <>__TranspIdent5
			select <>__TranspIdent5.bundledItem);
			return bundleDesc;
		}

		// Token: 0x060013B3 RID: 5043 RVA: 0x000507AC File Offset: 0x0004EBAC
		private BoosterDesc ParseBooster(XmlElement rb_el)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerator enumerator = rb_el.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						dictionary.Add(xmlElement.GetAttribute("name"), xmlElement.GetAttribute("value"));
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
			float xp = 0f;
			float vp = 0f;
			float gm = 0f;
			float ic = 0f;
			BoosterStackOption so = BoosterStackOption.SumStackOption;
			string s;
			if (dictionary.TryGetValue("xpBoost", out s))
			{
				xp = float.Parse(s);
			}
			if (dictionary.TryGetValue("vpBoost", out s))
			{
				vp = float.Parse(s);
			}
			if (dictionary.TryGetValue("gmBoost", out s))
			{
				gm = float.Parse(s);
			}
			if (dictionary.TryGetValue("icBoost", out s))
			{
				ic = float.Parse(s);
			}
			if (dictionary.TryGetValue("BoostStackOption", out s))
			{
				so = (BoosterStackOption)uint.Parse(s);
			}
			return new BoosterDesc(xp, vp, gm, ic, so);
		}

		// Token: 0x060013B4 RID: 5044 RVA: 0x000508FC File Offset: 0x0004ECFC
		private void ParseContentStats(XmlNode element, ulong itemID, Dictionary<ulong, BoosterDesc> booster_descs)
		{
			Dictionary<string, SItem> allItemsByName = this.m_itemCache.GetAllItemsByName();
			IEnumerator enumerator = element.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					string value = xmlNode.Attributes["name"].Value;
					SItem sitem;
					BoosterDesc value2;
					if (allItemsByName.TryGetValue(value, out sitem) && booster_descs.TryGetValue(sitem.ID, out value2))
					{
						booster_descs.Add(itemID, value2);
					}
					if (xmlNode.HasChildNodes)
					{
						this.ParseContentStats(xmlNode, itemID, booster_descs);
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
		}

		// Token: 0x060013B5 RID: 5045 RVA: 0x000509C0 File Offset: 0x0004EDC0
		private ContractDesc ParseContract(ulong itemId, string name, XmlNode element)
		{
			uint num = uint.Parse(element.Attributes["id"].Value);
			uint totalProgress = uint.Parse(element.Attributes["amount"].Value);
			EContractType type = (EContractType)Enum.Parse(typeof(EContractType), element.Attributes["diff"].Value, true);
			IContractReward reward = null;
			IEnumerator enumerator = element.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					if (!(xmlElement.Name != "Reward"))
					{
						string value = xmlElement.Attributes["name"].Value;
						if (xmlElement.HasAttribute("amount"))
						{
							reward = new ContractRewardMoney(value, uint.Parse(xmlElement.Attributes["amount"].Value));
							break;
						}
						throw new ContractValidationException(string.Format("Contract {0} in set {1}, has incorect reward section", name, num));
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
			return new ContractDesc(num, itemId, name, totalProgress, type, reward);
		}

		// Token: 0x060013B6 RID: 5046 RVA: 0x00050B18 File Offset: 0x0004EF18
		private TaggedItemDesc ParseItemTagsStats(ulong itemId, XmlElement statsElement)
		{
			UserTags userTags = new UserTags(null);
			IEnumerator enumerator = statsElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlElement.GetAttribute("name") == "tag_filter")
						{
							string attribute = xmlElement.GetAttribute("value");
							if (!string.IsNullOrEmpty(attribute))
							{
								userTags.Add(new UserTags(attribute));
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
			return (!userTags.HasAny()) ? null : new TaggedItemDesc(userTags);
		}

		// Token: 0x060013B7 RID: 5047 RVA: 0x00050BF0 File Offset: 0x0004EFF0
		private MetaGameDesc ParseMetaGameStats(SItem item, XmlElement statsElement)
		{
			MetaGameDesc metaGameDesc = new MetaGameDesc(item.Name);
			IEnumerator enumerator = statsElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						IEnumerator enumerator2 = xmlElement.Attributes.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlAttribute xmlAttribute = (XmlAttribute)obj2;
								string key = string.Format("{0}.{1}", xmlElement.Name, xmlAttribute.Name);
								metaGameDesc.Add(key, xmlAttribute.Value);
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator2 as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			return metaGameDesc;
		}

		// Token: 0x04000920 RID: 2336
		private Dictionary<ulong, StackableItemStats> m_stackableItemStats;

		// Token: 0x04000921 RID: 2337
		private Dictionary<ulong, RandomBoxDesc> m_randomBoxDescs;

		// Token: 0x04000922 RID: 2338
		private Dictionary<ulong, BoosterDesc> m_boosterDescs;

		// Token: 0x04000923 RID: 2339
		private Dictionary<ulong, TaggedItemDesc> m_taggedItemsDescs;

		// Token: 0x04000924 RID: 2340
		private Dictionary<string, SItem> m_accessItemDescs;

		// Token: 0x04000925 RID: 2341
		private Dictionary<ulong, MetaGameDesc> m_metaGameDescs;

		// Token: 0x04000926 RID: 2342
		private Dictionary<ulong, BundleDesc> m_bundleDescs;

		// Token: 0x04000927 RID: 2343
		private Dictionary<uint, List<ContractDesc>> m_contracts;

		// Token: 0x04000928 RID: 2344
		private List<ulong> m_vipItems;

		// Token: 0x04000929 RID: 2345
		private readonly IConfigProvider<XmlDocument> m_configProvider;

		// Token: 0x0400092A RID: 2346
		private readonly IItemCache m_itemCache;

		// Token: 0x0400092B RID: 2347
		private readonly ITagService m_tagService;
	}
}

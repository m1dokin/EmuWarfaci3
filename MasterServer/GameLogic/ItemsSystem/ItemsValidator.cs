using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000780 RID: 1920
	[Service]
	[Singleton]
	internal class ItemsValidator : ServiceModule, IItemsValidator
	{
		// Token: 0x060027C7 RID: 10183 RVA: 0x000A9D10 File Offset: 0x000A8110
		public bool CheckProfileItems(Dictionary<ulong, SProfileItem> profile, ulong profileID)
		{
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>(profile.Count);
			foreach (ulong key in profile.Keys)
			{
				SProfileItem sprofileItem = profile[key];
				if (!sprofileItem.IsExpired)
				{
					dictionary.Add(key, sprofileItem);
				}
			}
			return this.ValidateEquip(profile, dictionary, profileID);
		}

		// Token: 0x060027C8 RID: 10184 RVA: 0x000A9D98 File Offset: 0x000A8198
		public bool CheckProfileItems(ulong profileID)
		{
			IProfileItems service = ServicesManager.GetService<IProfileItems>();
			Dictionary<ulong, SProfileItem> profileItems = service.GetProfileItems(profileID);
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>(profileItems.Count);
			foreach (ulong key in profileItems.Keys)
			{
				SProfileItem sprofileItem = profileItems[key];
				if (!sprofileItem.IsExpired)
				{
					dictionary.Add(key, sprofileItem);
				}
			}
			return this.ValidateEquip(profileItems, dictionary, profileID);
		}

		// Token: 0x060027C9 RID: 10185 RVA: 0x000A9E30 File Offset: 0x000A8230
		private bool ValidateEquip(Dictionary<ulong, SProfileItem> current_equip, Dictionary<ulong, SProfileItem> new_equip, ulong profileId)
		{
			if (this.ValidationVerbose == 1)
			{
				this.DumpEquip(profileId, current_equip, "current equip");
				this.DumpEquip(profileId, new_equip, "new equip");
			}
			foreach (SProfileItem item in new_equip.Values)
			{
				this.ValidateItemEquip(item, current_equip);
			}
			this.checkAmmount(current_equip, new_equip);
			Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>();
			foreach (SlotInfo slotInfo in this.Inventory)
			{
				dictionary.Add(slotInfo.id, slotInfo.maxCount);
			}
			foreach (SProfileItem sprofileItem in new_equip.Values)
			{
				ItemsValidator.ItemTemplate itemTemplate = this.Templates[sprofileItem.ItemID];
				if (itemTemplate.slotRules.Count != 0)
				{
					for (int i = 0; i < ItemsValidator.classNames.Count; i++)
					{
						string b = ItemsValidator.classNames[i];
						ulong num = sprofileItem.SlotIDs & 31UL << i * 5;
						if (num != 0UL)
						{
							bool flag = false;
							foreach (ItemsValidator.SlotRule slotRule in itemTemplate.slotRules)
							{
								if (!(slotRule.classId != b) && slotRule.requiredSlots.Contains(num))
								{
									bool flag2 = true;
									foreach (ulong key in slotRule.requiredSlots)
									{
										if (!dictionary.ContainsKey(key) || dictionary[key] <= 0)
										{
											flag2 = false;
											break;
										}
									}
									if (flag2)
									{
										foreach (ulong num2 in slotRule.requiredSlots)
										{
											Dictionary<ulong, int> dictionary2;
											ulong key2;
											(dictionary2 = dictionary)[key2 = num2] = dictionary2[key2] - 1;
										}
										flag = true;
										break;
									}
								}
							}
							if (!flag)
							{
								throw new ValidationException(string.Format("Empty space not found for item {0} [slotId={1}]", itemTemplate.name, num));
							}
						}
					}
				}
			}
			foreach (SlotInfo slotInfo2 in this.Inventory)
			{
				if (dictionary.ContainsKey(slotInfo2.id) && slotInfo2.maxCount - dictionary[slotInfo2.id] < slotInfo2.minCount)
				{
					throw new ValidationException(string.Format("Mandatory items are required for slot {0} in class {1}", slotInfo2.name, slotInfo2.classId));
				}
			}
			return true;
		}

		// Token: 0x060027CA RID: 10186 RVA: 0x000AA244 File Offset: 0x000A8644
		private void checkAmmount(Dictionary<ulong, SProfileItem> current_equip, Dictionary<ulong, SProfileItem> new_equip)
		{
			Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>(current_equip.Count);
			foreach (SProfileItem sprofileItem in current_equip.Values)
			{
				int num = 0;
				dictionary.TryGetValue(sprofileItem.ItemID, out num);
				num++;
				dictionary[sprofileItem.ItemID] = num;
			}
			foreach (SProfileItem sprofileItem2 in new_equip.Values)
			{
				if (dictionary[sprofileItem2.ItemID] <= 0)
				{
					throw new ValidationException(string.Format("Amount test failed on {0}", sprofileItem2.ItemID));
				}
				Dictionary<ulong, int> dictionary2;
				ulong itemID;
				(dictionary2 = dictionary)[itemID = sprofileItem2.ItemID] = dictionary2[itemID] - 1;
			}
		}

		// Token: 0x060027CB RID: 10187 RVA: 0x000AA360 File Offset: 0x000A8760
		public override void Init()
		{
		}

		// Token: 0x060027CC RID: 10188 RVA: 0x000AA364 File Offset: 0x000A8764
		public override void Start()
		{
			string path = Path.Combine(Resources.GetResourcesDirectory(), "default_slots.xml");
			using (FileStream fileStream = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				this.UpdateDefaultSlots(fileStream);
			}
			this.UpdateCache();
		}

		// Token: 0x060027CD RID: 10189 RVA: 0x000AA3BC File Offset: 0x000A87BC
		public void UpdateCache()
		{
			Dictionary<ulong, ItemsValidator.ItemTemplate> dictionary = new Dictionary<ulong, ItemsValidator.ItemTemplate>();
			IItemCache service = ServicesManager.GetService<IItemCache>();
			Dictionary<ulong, SItem> allItems = service.GetAllItems();
			foreach (SItem sitem in allItems.Values)
			{
				ItemsValidator.ItemTemplate value = new ItemsValidator.ItemTemplate(sitem.Name, new List<ItemsValidator.SlotRule>(), string.Empty);
				try
				{
					List<ItemsValidator.SlotRule> list = this.ParseSlots(sitem.Slots);
					foreach (ItemsValidator.SlotRule item in list)
					{
						if (!value.classes.Contains(item.classId))
						{
							value.classes += item.classId;
						}
						value.slotRules.Add(item);
					}
					dictionary.Add(sitem.ID, value);
				}
				catch (ApplicationException ex)
				{
					Log.Error(string.Format("{0} from {1}.xml not found in default_slots.xml", ex.Message, sitem.Name));
					throw;
				}
			}
			this.Templates = dictionary;
		}

		// Token: 0x060027CE RID: 10190 RVA: 0x000AA514 File Offset: 0x000A8914
		private bool IsValidSlotDef(ulong slotId, string slotName, Dictionary<string, SlotDefData> existingSlotsDef)
		{
			if (slotId == 0UL)
			{
				return false;
			}
			foreach (KeyValuePair<string, SlotDefData> keyValuePair in existingSlotsDef)
			{
				if (keyValuePair.Key == slotName || keyValuePair.Value.id == slotId)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x060027CF RID: 10191 RVA: 0x000AA5A0 File Offset: 0x000A89A0
		private void UpdateDefaultSlots(Stream defaultSlots)
		{
			List<SlotInfo> list = new List<SlotInfo>();
			Dictionary<string, SlotDefData> dictionary = new Dictionary<string, SlotDefData>();
			XmlTextReader xmlTextReader = new XmlTextReader(defaultSlots);
			if (xmlTextReader.Read())
			{
				string className = string.Empty;
				int num = -1;
				while (xmlTextReader.Read())
				{
					if (xmlTextReader.NodeType == XmlNodeType.Element)
					{
						if (xmlTextReader.Name == "slot_def")
						{
							ulong num2 = ulong.Parse(xmlTextReader.GetAttribute("id"));
							string attribute = xmlTextReader.GetAttribute("name");
							bool alwaysEquip_ = xmlTextReader.GetAttribute("always_equip") == "1";
							if (!this.IsValidSlotDef(num2, attribute, dictionary))
							{
								throw new ValidationException(string.Format("Incorrect slot ({0}, {1}) definition", attribute, num2));
							}
							dictionary.Add(attribute, new SlotDefData(num2, alwaysEquip_));
						}
						else
						{
							if (xmlTextReader.Name == "class")
							{
								className = xmlTextReader.GetAttribute("name");
								num = ItemsValidator.classNames.FindIndex((string A) => A == className);
							}
							if (xmlTextReader.Name == "slot")
							{
								if (className == string.Empty || num == -1)
								{
									throw new ValidationException("Class name not assigned");
								}
								SlotInfo item = new SlotInfo(string.Empty, 0UL, string.Empty, 0, 0);
								item.classId = className;
								item.classIndex = ItemsValidator.classNames.IndexOf(className);
								item.name = xmlTextReader.GetAttribute("name");
								item.minCount = int.Parse(xmlTextReader.GetAttribute("min"));
								item.maxCount = int.Parse(xmlTextReader.GetAttribute("max"));
								if (!dictionary.ContainsKey(item.name))
								{
									throw new ValidationException(string.Format("Invalid slot name {0}", item.name));
								}
								item.id = dictionary[item.name].id << 5 * num;
								list.Add(item);
							}
						}
					}
				}
			}
			this.Inventory = list;
			this.UpdateSlotsCache();
			this.SlotsDef = dictionary;
		}

		// Token: 0x060027D0 RID: 10192 RVA: 0x000AA7F0 File Offset: 0x000A8BF0
		public bool FixSlotIds(ulong profileID, ulong itemId, ref ulong slotIds)
		{
			if (slotIds != 0UL)
			{
				return false;
			}
			IProfileItems service = ServicesManager.GetService<IProfileItems>();
			SProfileItem profileItem = service.GetProfileItem(profileID, itemId);
			if (profileItem == null)
			{
				return false;
			}
			List<ItemsValidator.SlotRule> list = this.ParseSlots(profileItem.GameItem.Slots);
			foreach (ItemsValidator.SlotRule slotRule in list)
			{
				foreach (ulong num in slotRule.requiredSlots)
				{
					ulong slotId = num >> 5 * slotRule.classIndex;
					if (this.IsAlwaysEquip(slotId))
					{
						slotIds |= num;
					}
				}
			}
			return slotIds != 0UL;
		}

		// Token: 0x060027D1 RID: 10193 RVA: 0x000AA8EC File Offset: 0x000A8CEC
		public ulong GetSlotIds(SItem item)
		{
			ulong num = 0UL;
			List<ItemsValidator.SlotRule> list = this.ParseSlots(item.Slots);
			foreach (ItemsValidator.SlotRule slotRule in list)
			{
				foreach (ulong num2 in slotRule.requiredSlots)
				{
					ulong slotId = num2 >> 5 * slotRule.classIndex;
					if (this.IsAlwaysEquip(slotId))
					{
						num |= num2;
					}
				}
			}
			return num;
		}

		// Token: 0x060027D2 RID: 10194 RVA: 0x000AA9B8 File Offset: 0x000A8DB8
		public void FixAndUpdateSlotIds(ulong profileID, ulong itemId, ulong attachedTo)
		{
			ulong slotIds = 0UL;
			if (!this.FixSlotIds(profileID, itemId, ref slotIds))
			{
				return;
			}
			IProfileItems service = ServicesManager.GetService<IProfileItems>();
			service.UpdateProfileItem(profileID, itemId, slotIds, attachedTo, string.Empty);
		}

		// Token: 0x060027D3 RID: 10195 RVA: 0x000AA9F0 File Offset: 0x000A8DF0
		private bool IsAlwaysEquip(ulong slotId)
		{
			foreach (KeyValuePair<string, SlotDefData> keyValuePair in this.SlotsDef)
			{
				if (keyValuePair.Value.id == slotId)
				{
					return keyValuePair.Value.alwaysEquip;
				}
			}
			return false;
		}

		// Token: 0x060027D4 RID: 10196 RVA: 0x000AAA74 File Offset: 0x000A8E74
		private List<ItemsValidator.SlotRule> ParseSlots(string slotsStr)
		{
			List<ItemsValidator.SlotRule> list = new List<ItemsValidator.SlotRule>();
			string[] array = slotsStr.Split(new char[]
			{
				';'
			});
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(new char[]
				{
					':'
				});
				if (array2.Length == 2)
				{
					ItemsValidator.SlotRule item = new ItemsValidator.SlotRule(array2[0]);
					string[] array3 = array2[1].Split(new char[]
					{
						','
					});
					for (int j = 0; j < array3.Length; j++)
					{
						SlotDefData slotDefData;
						if (!this.SlotsDef.TryGetValue(array3[j], out slotDefData))
						{
							throw new ApplicationException(string.Format("Slot {0}", array3[j]));
						}
						item.requiredSlots.Add(slotDefData.id << 5 * item.classIndex);
					}
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x060027D5 RID: 10197 RVA: 0x000AAB5C File Offset: 0x000A8F5C
		private bool ValidateItemEquip(SProfileItem item, Dictionary<ulong, SProfileItem> current_equipment)
		{
			if (item.IsExpired)
			{
				throw new ValidationException(string.Format("Cannot equip expired item {0}", item.ProfileItemID));
			}
			foreach (SProfileItem sprofileItem in current_equipment.Values)
			{
				if (sprofileItem.ItemID == item.ItemID)
				{
					ItemsValidator.ItemTemplate itemTemplate;
					if (!this.Templates.TryGetValue(item.ItemID, out itemTemplate))
					{
						throw new ValidationException(string.Format("Template {0} not found", item.ItemID));
					}
					int num = 0;
					while (num < ItemsValidator.classNames.Count && itemTemplate.classes != string.Empty)
					{
						if ((item.SlotIDs & 31UL << 5 * num) != 0UL && !itemTemplate.classes.Contains(ItemsValidator.classNames[num]))
						{
							throw new ValidationException(string.Format("Item {0} not supported by assigned class {1}", itemTemplate.name, num));
						}
						num++;
					}
					return true;
				}
			}
			throw new ValidationException(string.Format("Item instance {0} not found in profile", item.ItemID));
		}

		// Token: 0x060027D6 RID: 10198 RVA: 0x000AACC4 File Offset: 0x000A90C4
		private void DumpEquip(ulong profileId, Dictionary<ulong, SProfileItem> equip, string type)
		{
			Log.Info<ulong>("================ Validation dump '{0}' (profile {1}) ================", profileId);
			foreach (SProfileItem sprofileItem in equip.Values)
			{
				Log.Info("Key: {0} {1} {2} {3}", new object[]
				{
					sprofileItem.ItemID,
					sprofileItem.Status,
					sprofileItem.SlotIDs,
					sprofileItem.Config
				});
			}
			Log.Info("================ Validation dump equip end ================");
		}

		// Token: 0x170003B3 RID: 947
		// (get) Token: 0x060027D7 RID: 10199 RVA: 0x000AAD74 File Offset: 0x000A9174
		// (set) Token: 0x060027D8 RID: 10200 RVA: 0x000AAD7C File Offset: 0x000A917C
		public int ValidationVerbose
		{
			get
			{
				return this.m_validationVerbose;
			}
			set
			{
				this.m_validationVerbose = value;
			}
		}

		// Token: 0x060027D9 RID: 10201 RVA: 0x000AAD88 File Offset: 0x000A9188
		public IEnumerable<SlotInfo> GetSlotsInfo(ulong slot_ids)
		{
			List<SlotInfo> list = new List<SlotInfo>();
			for (int i = 0; i < ItemsValidator.classNames.Count; i++)
			{
				SlotInfo slotInfoPerClass = this.GetSlotInfoPerClass(slot_ids, i);
				list.Add(slotInfoPerClass);
			}
			return list;
		}

		// Token: 0x060027DA RID: 10202 RVA: 0x000AADC8 File Offset: 0x000A91C8
		public SlotInfo GetSlotInfoPerClass(ulong slot_ids, int classIndex)
		{
			ulong slotId = this.GetSlotId(slot_ids, classIndex);
			return this.m_slotsCache[slotId];
		}

		// Token: 0x060027DB RID: 10203 RVA: 0x000AADEC File Offset: 0x000A91EC
		private ulong GetSlotId(ulong slotIds, int classIndex)
		{
			ulong num = 31UL & slotIds >> classIndex * 5;
			return num << classIndex * 5;
		}

		// Token: 0x060027DC RID: 10204 RVA: 0x000AAE10 File Offset: 0x000A9210
		private void UpdateSlotsCache()
		{
			this.m_slotsCache.Clear();
			foreach (SlotInfo value in this.Inventory)
			{
				for (int i = 0; i < ItemsValidator.classNames.Count; i++)
				{
					ulong slotId = this.GetSlotId(value.id, i);
					this.m_slotsCache[slotId] = value;
				}
			}
		}

		// Token: 0x170003B4 RID: 948
		// (get) Token: 0x060027DD RID: 10205 RVA: 0x000AAEA8 File Offset: 0x000A92A8
		public IEnumerable<SlotInfo> DefaultInventory
		{
			get
			{
				return this.Inventory;
			}
		}

		// Token: 0x040014C9 RID: 5321
		private readonly Dictionary<ulong, SlotInfo> m_slotsCache = new Dictionary<ulong, SlotInfo>();

		// Token: 0x040014CA RID: 5322
		public static List<string> classNames = new List<string>
		{
			"R",
			"H",
			"S",
			"M",
			"E"
		};

		// Token: 0x040014CB RID: 5323
		private List<SlotInfo> Inventory = new List<SlotInfo>();

		// Token: 0x040014CC RID: 5324
		private int m_validationVerbose;

		// Token: 0x040014CD RID: 5325
		public const int SLOT_SIZE = 5;

		// Token: 0x040014CE RID: 5326
		private const ulong SLOT_MASK = 31UL;

		// Token: 0x040014CF RID: 5327
		private Dictionary<ulong, ItemsValidator.ItemTemplate> Templates = new Dictionary<ulong, ItemsValidator.ItemTemplate>();

		// Token: 0x040014D0 RID: 5328
		private Dictionary<string, SlotDefData> SlotsDef = new Dictionary<string, SlotDefData>();

		// Token: 0x02000781 RID: 1921
		public struct SlotRule
		{
			// Token: 0x060027DF RID: 10207 RVA: 0x000AAF00 File Offset: 0x000A9300
			public SlotRule(string _classId)
			{
				this.classId = _classId;
				this.requiredSlots = new List<ulong>();
				this.classIndex = ItemsValidator.classNames.FindIndex((string A) => A == _classId);
				if (this.classIndex == -1)
				{
					throw new ValidationException(string.Format("Invalid slot name {0}", this.classId));
				}
			}

			// Token: 0x060027E0 RID: 10208 RVA: 0x000AAF70 File Offset: 0x000A9370
			public void Dump()
			{
				Log.Info<string>("Class: {0}", this.classId);
				if (this.requiredSlots != null)
				{
					Log.Info("Required slots dump");
					foreach (ulong p in this.requiredSlots)
					{
						Log.Info<ulong>("Slot: {0}", p);
					}
				}
			}

			// Token: 0x040014D1 RID: 5329
			public string classId;

			// Token: 0x040014D2 RID: 5330
			public int classIndex;

			// Token: 0x040014D3 RID: 5331
			public List<ulong> requiredSlots;
		}

		// Token: 0x02000782 RID: 1922
		private struct ItemTemplate
		{
			// Token: 0x060027E1 RID: 10209 RVA: 0x000AB00E File Offset: 0x000A940E
			public ItemTemplate(string _name, List<ItemsValidator.SlotRule> _slotRules, string _classes)
			{
				this.name = _name;
				this.slotRules = _slotRules;
				this.classes = _classes;
			}

			// Token: 0x060027E2 RID: 10210 RVA: 0x000AB028 File Offset: 0x000A9428
			public void Dump()
			{
				Log.Info<string>("Name: {0}", this.name);
				Log.Info<string>("Classes: {0}", this.classes);
				if (this.slotRules != null)
				{
					Log.Info("Slot rules dump");
					foreach (ItemsValidator.SlotRule slotRule in this.slotRules)
					{
						slotRule.Dump();
					}
				}
			}

			// Token: 0x040014D4 RID: 5332
			public string name;

			// Token: 0x040014D5 RID: 5333
			public List<ItemsValidator.SlotRule> slotRules;

			// Token: 0x040014D6 RID: 5334
			public string classes;
		}
	}
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000779 RID: 1913
	public static class DefaultProfile
	{
		// Token: 0x060027AE RID: 10158 RVA: 0x000A9580 File Offset: 0x000A7980
		public static bool ResetProfileItems(ulong profileId)
		{
			return DefaultProfile.ResetProfileItems(profileId, DefaultProfile.ResetType.ResetIfChanged);
		}

		// Token: 0x060027AF RID: 10159 RVA: 0x000A958C File Offset: 0x000A798C
		public static bool ResetProfileItems(ulong profileId, DefaultProfile.ResetType resetType)
		{
			ProfileProxy profile = new ProfileProxy(profileId);
			return DefaultProfile.ResetProfileItems(profile, resetType);
		}

		// Token: 0x060027B0 RID: 10160 RVA: 0x000A95A7 File Offset: 0x000A79A7
		internal static bool ResetProfileItems(ProfileProxy profile)
		{
			return DefaultProfile.ResetProfileItems(profile, DefaultProfile.ResetType.ResetIfChanged);
		}

		// Token: 0x060027B1 RID: 10161 RVA: 0x000A95B0 File Offset: 0x000A79B0
		internal static bool ResetProfileItems(ProfileProxy profile, DefaultProfile.ResetType resetType)
		{
			try
			{
				IItemCache service = ServicesManager.GetService<IItemCache>();
				Dictionary<ulong, SEquipItem> defaultProfileItems = service.GetDefaultProfileItems();
				Dictionary<ulong, SProfileItem> profileDefaultItems = profile.GetProfileDefaultItems();
				if (resetType == DefaultProfile.ResetType.ResetIfChanged)
				{
					bool flag = defaultProfileItems.Count != profileDefaultItems.Count;
					if (!flag)
					{
						Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>();
						foreach (SProfileItem sprofileItem in profileDefaultItems.Values)
						{
							if (dictionary.ContainsKey(sprofileItem.ItemID))
							{
								Dictionary<ulong, int> dictionary2;
								ulong itemID;
								(dictionary2 = dictionary)[itemID = sprofileItem.ItemID] = dictionary2[itemID] + 1;
							}
							else
							{
								dictionary.Add(sprofileItem.ItemID, 1);
							}
						}
						foreach (SEquipItem sequipItem in defaultProfileItems.Values)
						{
							if (dictionary.ContainsKey(sequipItem.ItemID))
							{
								Dictionary<ulong, int> dictionary2;
								ulong itemID2;
								(dictionary2 = dictionary)[itemID2 = sequipItem.ItemID] = dictionary2[itemID2] - 1;
							}
						}
						foreach (int num in dictionary.Values)
						{
							if (num != 0)
							{
								flag = true;
								break;
							}
						}
					}
					if (!flag)
					{
						return false;
					}
				}
				Log.Info<ulong, DefaultProfile.ResetType>("Reset profile {0} to default setup by reason {1}", profile.ProfileID, resetType);
				IProfileItems service2 = ServicesManager.GetService<IProfileItems>();
				service2.DeleteDefaultItems(profile.ProfileID);
				service2.AddDefaultItems(profile.ProfileID);
				Dictionary<ulong, SProfileItem> profileItems = profile.GetProfileItems((SProfileItem PI) => !PI.IsDefault && PI.SlotIDs != 0UL);
				foreach (KeyValuePair<ulong, SProfileItem> keyValuePair in profileItems)
				{
					ulong key = keyValuePair.Key;
					SProfileItem value = keyValuePair.Value;
					Log.Info<ulong, ulong>("Reset profile {0} unequip non default item {1}", profile.ProfileID, value.ItemID);
					profile.UpdateProfileItem(key, 0UL, 0UL, value.Config);
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
			return true;
		}

		// Token: 0x060027B2 RID: 10162 RVA: 0x000A9898 File Offset: 0x000A7C98
		public static bool InsertDefaultProfile(Stream inputStream)
		{
			try
			{
				List<DefaultProfile.ProfileItem> list = new List<DefaultProfile.ProfileItem>();
				DefaultProfile.ReadItemsXML(inputStream, ref list);
				List<DefaultProfile.ProfileItem> list2 = new List<DefaultProfile.ProfileItem>();
				DefaultProfile.ReadItemsSQL(ref list2);
				bool flag = false;
				foreach (DefaultProfile.ProfileItem item in list)
				{
					if (!list2.Remove(item))
					{
						flag = true;
						break;
					}
				}
				if (list2.Count != 0 || flag)
				{
					DefaultProfile.InsertProfileItems(list);
				}
			}
			catch (Exception ex)
			{
				Log.Error<string, string>("(Exception) {0}\n{1}", ex.Message, ex.StackTrace);
				return false;
			}
			return true;
		}

		// Token: 0x060027B3 RID: 10163 RVA: 0x000A9968 File Offset: 0x000A7D68
		private static void InsertProfileItems(List<DefaultProfile.ProfileItem> defaultItemsXML)
		{
			IDALService service = ServicesManager.GetService<IDALService>();
			service.ItemSystem.ClearDefaultProfileItems();
			Dictionary<string, ulong> dictionary = new Dictionary<string, ulong>();
			foreach (SItem sitem in service.ItemSystem.GetAllItems())
			{
				if (sitem.Active)
				{
					dictionary.Add(sitem.Name, sitem.ID);
				}
			}
			foreach (DefaultProfile.ProfileItem profileItem in defaultItemsXML)
			{
				ulong itemId = 0UL;
				if (!dictionary.TryGetValue(profileItem.name, out itemId))
				{
					throw new ApplicationException(string.Format("Default profile item '{0}' not found in items table", profileItem.name));
				}
				service.ItemSystem.AddDefaultProfileItem(profileItem.id, itemId, profileItem.slotIds, profileItem.config);
			}
		}

		// Token: 0x060027B4 RID: 10164 RVA: 0x000A9A8C File Offset: 0x000A7E8C
		private static void ReadItemsSQL(ref List<DefaultProfile.ProfileItem> defaultItems)
		{
			IItemCache service = ServicesManager.GetService<IItemCache>();
			Dictionary<ulong, SItem> allItems = service.GetAllItems(false);
			Dictionary<ulong, SEquipItem> defaultProfileItems = service.GetDefaultProfileItems();
			foreach (SEquipItem sequipItem in defaultProfileItems.Values)
			{
				SItem sitem = allItems[sequipItem.ItemID];
				defaultItems.Add(new DefaultProfile.ProfileItem(sequipItem.ProfileItemID, sitem.Name, sequipItem.SlotIDs, sequipItem.Config));
			}
		}

		// Token: 0x060027B5 RID: 10165 RVA: 0x000A9B2C File Offset: 0x000A7F2C
		private static void ReadItemsXML(Stream inputStream, ref List<DefaultProfile.ProfileItem> defaultItemsXML)
		{
			XmlTextReader xmlTextReader = new XmlTextReader(inputStream);
			if (!xmlTextReader.Read())
			{
				throw new ApplicationException(string.Format("[ReadItemsXML]Stream cannot be read", new object[0]));
			}
			while (xmlTextReader.Read())
			{
				if (xmlTextReader.NodeType == XmlNodeType.Element && !(xmlTextReader.Name != "item"))
				{
					DefaultProfile.ProfileItem item = default(DefaultProfile.ProfileItem);
					item.id = ulong.Parse(xmlTextReader.GetAttribute("id"));
					item.name = xmlTextReader.GetAttribute("name");
					item.config = xmlTextReader.GetAttribute("config");
					item.slotIds = ulong.Parse(xmlTextReader.GetAttribute("slot"));
					defaultItemsXML.Add(item);
				}
			}
		}

		// Token: 0x0200077A RID: 1914
		public enum ResetType
		{
			// Token: 0x040014BB RID: 5307
			ResetToDefault,
			// Token: 0x040014BC RID: 5308
			ResetIfChanged
		}

		// Token: 0x0200077B RID: 1915
		private struct ProfileItem
		{
			// Token: 0x060027B7 RID: 10167 RVA: 0x000A9C16 File Offset: 0x000A8016
			public ProfileItem(ulong _id, string _name, ulong _slotIds, string _config)
			{
				this.name = _name;
				this.slotIds = _slotIds;
				this.config = _config;
				this.id = _id;
			}

			// Token: 0x040014BD RID: 5309
			public ulong id;

			// Token: 0x040014BE RID: 5310
			public ulong slotIds;

			// Token: 0x040014BF RID: 5311
			public string name;

			// Token: 0x040014C0 RID: 5312
			public string config;
		}
	}
}

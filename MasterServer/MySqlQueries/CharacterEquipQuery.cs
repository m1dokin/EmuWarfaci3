using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.Users;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200082F RID: 2095
	[QueryAttributes(TagName = "setcharacter")]
	internal class CharacterEquipQuery : BaseQuery
	{
		// Token: 0x06002B62 RID: 11106 RVA: 0x000BB17D File Offset: 0x000B957D
		public CharacterEquipQuery(IProfileItems profileItems, IItemsValidator itemsValidator, ILogService logService, IDALService dalService, IClassChangingService classChanging, IItemStats itemStats)
		{
			this.m_profileItems = profileItems;
			this.m_itemsValidator = itemsValidator;
			this.m_logService = logService;
			this.m_dalService = dalService;
			this.m_classChanging = classChanging;
			this.m_itemStats = itemStats;
		}

		// Token: 0x06002B63 RID: 11107 RVA: 0x000BB1B4 File Offset: 0x000B95B4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "CharacterEquipQuery"))
			{
				uint classId = uint.Parse(request.GetAttribute("current_class"));
				bool flag = false;
				try
				{
					ClassChangeStatus classChangeStatus = this.m_classChanging.ChangePlayersClass(user, classId);
					if (classChangeStatus != ClassChangeStatus.Ok)
					{
						Log.Warning(string.Format("[CharacterEquipQuery] Invalid class for profile {0}", user.ProfileID));
						flag = true;
					}
					else if (request.ChildNodes.Count > 0)
					{
						this.ApplyEquipChanges(user, request);
					}
				}
				catch (Exception ex)
				{
					Log.Warning<string>("[CharacterEquipQuery] {0}", ex.Message);
					flag = true;
				}
				if (flag)
				{
					XmlElement xmlElement = response.OwnerDocument.CreateElement("character");
					response.AppendChild(xmlElement);
					ProfileProxy profile = new ProfileProxy(user);
					ProfileReader profileReader = new ProfileReader(profile);
					profileReader.ReadProfileItems(xmlElement);
					profileReader.ReadUnlockedItems(xmlElement);
				}
				result = 0;
			}
			return result;
		}

		// Token: 0x06002B64 RID: 11108 RVA: 0x000BB2D8 File Offset: 0x000B96D8
		private void ApplyEquipChanges(UserInfo.User user, XmlElement request)
		{
			int num = 0;
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(user.ProfileID);
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(user.ProfileID);
			Dictionary<ulong, SProfileItem> dictionary = this.ParseDiffItems(request, profileItems);
			if (!this.CheckEquip(dictionary, profileItems, user))
			{
				throw new ApplicationException(string.Format("Items validation failed for profile {0}", user.ProfileID));
			}
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				foreach (SProfileItem sprofileItem in dictionary.Values)
				{
					if (!this.IsDynamicItem(sprofileItem))
					{
						num++;
						this.m_itemsValidator.FixSlotIds(user.ProfileID, sprofileItem.ProfileItemID, ref sprofileItem.EquipItem.SlotIDs);
						this.m_profileItems.UpdateProfileItem(user.ProfileID, sprofileItem.ProfileItemID, sprofileItem.EquipItem.SlotIDs, sprofileItem.EquipItem.AttachedTo, sprofileItem.EquipItem.Config);
						logGroup.ItemEquipLog(profileInfo.UserID, profileInfo.Id, profileInfo.Nickname, profileInfo.RankInfo.RankId, sprofileItem.ProfileItemID, sprofileItem.GameItem.Name, sprofileItem.EquipItem.SlotIDs);
					}
				}
			}
			Log.Info<ulong, int>("CharacterEquip({0}) successfully executed [{1} items saved]", user.ProfileID, num);
		}

		// Token: 0x06002B65 RID: 11109 RVA: 0x000BB488 File Offset: 0x000B9888
		private Dictionary<ulong, SProfileItem> ParseDiffItems(XmlElement parentNode, Dictionary<ulong, SProfileItem> profile_items)
		{
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>();
			IEnumerator enumerator = parentNode.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = (XmlElement)xmlNode;
						if (xmlElement.Name != "item")
						{
							Log.Warning<string>("Malformed xml element '{0}' in CharacterEquip", xmlElement.Name);
						}
						else
						{
							ulong num = ulong.Parse(xmlElement.GetAttribute("id"));
							string attribute = xmlElement.GetAttribute("config");
							SProfileItem item;
							if (!profile_items.TryGetValue(num, out item))
							{
								throw new ValidationException(string.Format("Can't find item with ID: {0}. Perhaps it is expired.", num));
							}
							SProfileItem sprofileItem = new SProfileItem(item);
							sprofileItem.EquipItem.Config = attribute;
							sprofileItem.EquipItem.SlotIDs = ulong.Parse(xmlElement.GetAttribute("slot"));
							sprofileItem.EquipItem.AttachedTo = ulong.Parse(xmlElement.GetAttribute("attached_to"));
							dictionary.Add(sprofileItem.ProfileItemID, sprofileItem);
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
			return dictionary;
		}

		// Token: 0x06002B66 RID: 11110 RVA: 0x000BB5E0 File Offset: 0x000B99E0
		private bool IsDynamicItem(SProfileItem profileItem)
		{
			return profileItem.Status == EProfileItemStatus.REWARD || profileItem.GameItem.Type == "attachment";
		}

		// Token: 0x06002B67 RID: 11111 RVA: 0x000BB608 File Offset: 0x000B9A08
		private bool CheckEquip(Dictionary<ulong, SProfileItem> diff_items, Dictionary<ulong, SProfileItem> curr_equip, UserInfo.User user)
		{
			Dictionary<ulong, SProfileItem> dictionary = new Dictionary<ulong, SProfileItem>();
			foreach (SProfileItem sprofileItem in diff_items.Values)
			{
				if (sprofileItem.SlotIDs != 0UL)
				{
					dictionary.Add(sprofileItem.ProfileItemID, sprofileItem);
				}
			}
			foreach (SProfileItem sprofileItem2 in curr_equip.Values)
			{
				if (sprofileItem2.SlotIDs != 0UL && !diff_items.ContainsKey(sprofileItem2.ProfileItemID))
				{
					dictionary.Add(sprofileItem2.ProfileItemID, sprofileItem2);
				}
			}
			foreach (SProfileItem sprofileItem3 in diff_items.Values)
			{
				if (!this.m_itemStats.IsItemAvailableForUser(sprofileItem3.ItemID, user))
				{
					throw new ValidationException(string.Format("Profile {0} tries to equip tag-restircted item {1}", user.ProfileID, sprofileItem3.ItemID));
				}
				if (sprofileItem3.IsExpired)
				{
					throw new ValidationException(string.Format("Profile {0} tries to equip expired item {1}", user.ProfileID, sprofileItem3.ItemID));
				}
			}
			return this.m_itemsValidator.CheckProfileItems(dictionary, user.ProfileID);
		}

		// Token: 0x04001715 RID: 5909
		private readonly IProfileItems m_profileItems;

		// Token: 0x04001716 RID: 5910
		private readonly IItemsValidator m_itemsValidator;

		// Token: 0x04001717 RID: 5911
		private readonly ILogService m_logService;

		// Token: 0x04001718 RID: 5912
		private readonly IDALService m_dalService;

		// Token: 0x04001719 RID: 5913
		private readonly IClassChangingService m_classChanging;

		// Token: 0x0400171A RID: 5914
		private readonly IItemStats m_itemStats;
	}
}

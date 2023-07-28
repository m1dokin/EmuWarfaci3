using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200082E RID: 2094
	[QueryAttributes(TagName = "getprofile")]
	internal class GetProfileInfoQuery : BaseQuery
	{
		// Token: 0x06002B5F RID: 11103 RVA: 0x000BAA74 File Offset: 0x000B8E74
		public GetProfileInfoQuery(IDALService dalService, IClanService clanService, IItemsValidator itemsValidator, IProfileItems profileItems, IBoosterService boosterService, IGameRoomManager gameRoomManager, ISessionStorage sessionStorage, ISkillService skillService)
		{
			this.m_dalService = dalService;
			this.m_clanService = clanService;
			this.m_itemsValidator = itemsValidator;
			this.m_profileItems = profileItems;
			this.m_boosterService = boosterService;
			this.m_gameRoomManager = gameRoomManager;
			this.m_sessionStorage = sessionStorage;
			this.m_skillService = skillService;
		}

		// Token: 0x06002B60 RID: 11104 RVA: 0x000BAAC4 File Offset: 0x000B8EC4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetProfileInfoQuery"))
			{
				string text;
				if (!base.GetServerID(fromJid, out text))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("session_id");
					if (!this.m_sessionStorage.ValidateSession(fromJid, attribute))
					{
						Log.Warning<string, string>("Ignoring get profile info request from server {0} which has incorrect session id {1}", fromJid, attribute);
						result = -1;
					}
					else if (!request.HasAttribute("id"))
					{
						Log.Error("Get profile: root has not attribute 'id'");
						result = -1;
					}
					else
					{
						ulong profileId = ulong.Parse(request.GetAttribute("id"));
						UserInfo.User user = base.UserRepository.GetUser(profileId);
						if (user == null)
						{
							Log.Warning<ulong, string>("Can't find user {0} for server {1}", profileId, text);
							result = -1;
						}
						else
						{
							IGameRoom room = this.m_gameRoomManager.GetRoomByServer(text);
							if (room == null)
							{
								Log.Warning<string>("Can't find room for server {0}", text);
								result = -1;
							}
							else
							{
								SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
								try
								{
									this.m_itemsValidator.CheckProfileItems(profileId);
								}
								catch (ValidationException e)
								{
									Log.Error(e);
									DefaultProfile.ResetProfileItems(profileId, DefaultProfile.ResetType.ResetToDefault);
								}
								Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(profileId, EquipOptions.ActiveOnly | EquipOptions.FilterByTags);
								ClanMember memberInfo = this.m_clanService.GetMemberInfo(profileId);
								ClanInfo clanInfo = (memberInfo != null) ? this.m_clanService.GetClanInfo(memberInfo.ClanID) : null;
								XmlElement profileNode = response.OwnerDocument.CreateElement("profile");
								profileNode.SetAttribute("nickname", profileInfo.Nickname);
								profileNode.SetAttribute("user_id", profileInfo.UserID.ToString(CultureInfo.InvariantCulture));
								profileNode.SetAttribute("gender", profileInfo.Gender);
								profileNode.SetAttribute("height", profileInfo.Height.ToString(CultureInfo.InvariantCulture));
								profileNode.SetAttribute("fatness", (profileInfo.Fatness - 1f).ToString(CultureInfo.InvariantCulture));
								profileNode.SetAttribute("head", profileInfo.Head);
								profileNode.SetAttribute("current_class", profileInfo.CurrentClass.ToString(CultureInfo.InvariantCulture));
								profileNode.SetAttribute("experience", profileInfo.RankInfo.Points.ToString(CultureInfo.InvariantCulture));
								profileNode.SetAttribute("preset", "DefaultPreset");
								profileNode.SetAttribute("clanName", (clanInfo != null) ? clanInfo.Name : string.Empty);
								profileNode.SetAttribute("unlocked_classes", ((int)user.ProfileProgression.ClassUnlocked).ToString(CultureInfo.InvariantCulture));
								room.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
								{
									RoomPlayer player = r.GetPlayer(profileId);
									if (player == null)
									{
										throw new Exception(string.Format("Can't find player {0} in room {1}", profileId, room.ID));
									}
									profileNode.SetAttribute("group_id", player.GroupID);
								});
								SkillType skillTypeByRoomType = SkillTypeHelper.GetSkillTypeByRoomType(room.Type);
								XmlElement xmlElement = response.OwnerDocument.CreateElement("skill");
								Skill skill = this.m_skillService.GetSkill(profileId, skillTypeByRoomType);
								xmlElement.SetAttribute("type", skillTypeByRoomType.ToString());
								xmlElement.SetAttribute("value", skill.Value.ToString());
								profileNode.AppendChild(xmlElement);
								Dictionary<BoosterType, float> boosters = this.m_boosterService.GetBoosters(profileId);
								bool flag = this.m_boosterService.HasVipItem(profileId);
								XmlElement xmlElement2 = response.OwnerDocument.CreateElement("boosts");
								xmlElement2.SetAttribute("xp_boost", boosters[BoosterType.XPBooster].ToString(CultureInfo.InvariantCulture));
								xmlElement2.SetAttribute("vp_boost", boosters[BoosterType.VPBooster].ToString(CultureInfo.InvariantCulture));
								xmlElement2.SetAttribute("gm_boost", boosters[BoosterType.GMBooster].ToString(CultureInfo.InvariantCulture));
								xmlElement2.SetAttribute("ic_boost", boosters[BoosterType.ICBooster].ToString(CultureInfo.InvariantCulture));
								xmlElement2.SetAttribute("is_vip", (!flag) ? "0" : "1");
								profileNode.AppendChild(xmlElement2);
								CustomParams data = this.m_sessionStorage.GetData<CustomParams>(attribute, ESessionData.Restrictions);
								XmlElement xmlElement3 = response.OwnerDocument.CreateElement("items");
								foreach (SProfileItem sprofileItem in profileItems.Values)
								{
									if (GetProfileInfoQuery.NeedSendToDedicatedServer(sprofileItem, data))
									{
										XmlElement xml = ServerItem.GetXml(sprofileItem, response.OwnerDocument, "item");
										xmlElement3.AppendChild(xml);
									}
								}
								profileNode.AppendChild(xmlElement3);
								response.AppendChild(profileNode);
								response.SetAttribute("id", profileId.ToString(CultureInfo.InvariantCulture));
								Log.Info<ulong, int>("GetProfileInfo({0}) successfully selected [items: {1}]", profileId, xmlElement3.ChildNodes.Count);
								result = 0;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06002B61 RID: 11105 RVA: 0x000BB09C File Offset: 0x000B949C
		private static bool NeedSendToDedicatedServer(SProfileItem profileItem, CustomParams restrictions)
		{
			if (profileItem.GameItem.IsCoinItem)
			{
				return true;
			}
			if (profileItem.GameItem.IsAccessItem)
			{
				return true;
			}
			if (profileItem.SlotIDs == 0UL)
			{
				return false;
			}
			if (restrictions != null)
			{
				for (int i = 0; i < 5; i++)
				{
					if (!restrictions.IsItemAllowed(profileItem, i))
					{
						return false;
					}
				}
			}
			return !profileItem.IsExpired;
		}

		// Token: 0x0400170D RID: 5901
		private readonly IDALService m_dalService;

		// Token: 0x0400170E RID: 5902
		private readonly IClanService m_clanService;

		// Token: 0x0400170F RID: 5903
		private readonly IItemsValidator m_itemsValidator;

		// Token: 0x04001710 RID: 5904
		private readonly IProfileItems m_profileItems;

		// Token: 0x04001711 RID: 5905
		private readonly IBoosterService m_boosterService;

		// Token: 0x04001712 RID: 5906
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04001713 RID: 5907
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04001714 RID: 5908
		private readonly ISkillService m_skillService;
	}
}

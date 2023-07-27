using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.DAL.CustomRules;
using MasterServer.DAL.PlayerStats;
using MasterServer.DAL.RatingSystem;
using MasterServer.DAL.Utils;
using OLAPHypervisor;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200001B RID: 27
	internal class ColdProfileDataXMLSerializer : IDataSerializer<ColdProfileData>
	{
		// Token: 0x060000F3 RID: 243 RVA: 0x000096D0 File Offset: 0x000078D0
		public void Serialize(ColdProfileData data, TextWriter wr)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("profile");
			xmlElement.AppendChild(ColdProfileDataXMLSerializer.SerializeItems(xmlDocument, data.EquipItems));
			xmlElement.AppendChild(this.SerializeUnlockItems(xmlDocument, data.UnlockItems));
			xmlElement.AppendChild(this.SerializeAchievements(xmlDocument, data.Achievements));
			xmlElement.AppendChild(this.SerializeSponsorPoints(xmlDocument, data.SponsorPoints));
			xmlElement.AppendChild(this.SerializePersistentSettings(xmlDocument, data.PersistentSettings));
			if (data.ProfileContract != null)
			{
				xmlElement.AppendChild(this.SerializeProfileContract(xmlDocument, data.ProfileContract));
			}
			if (data.PlayerStatistics != null)
			{
				xmlElement.AppendChild(this.SerializePlayerStatistics(xmlDocument, data.PlayerStatistics));
			}
			if (!data.ProfileProgression.IsEmpty())
			{
				xmlElement.AppendChild(this.SerializeProfileProgression(xmlDocument, data.ProfileProgression));
			}
			xmlElement.AppendChild(this.SerializeCustomRulesState(xmlDocument, data.CustomRuleStates));
			xmlElement.AppendChild(this.SerializeSkillInfos(xmlDocument, data.SkillInfos));
			if (!data.RatingInfo.IsEmpty())
			{
				xmlElement.AppendChild(this.SerializeRatingInfo(xmlDocument, data.RatingInfo));
			}
			xmlElement.AppendChild(this.SerializeRatingGameBanInfo(xmlDocument, data.RatingGamePlayerBan));
			wr.Write(xmlElement.OuterXml);
		}

		// Token: 0x060000F4 RID: 244 RVA: 0x00009820 File Offset: 0x00007A20
		public ColdProfileData Deserialize(TextReader rd, DBVersion version)
		{
			ColdProfileData coldProfileData = new ColdProfileData();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(rd);
			XmlElement documentElement = xmlDocument.DocumentElement;
			XmlElement its_el = (XmlElement)documentElement.SelectSingleNode("equip_items");
			coldProfileData.EquipItems = ColdProfileDataXMLSerializer.DeserializeItems(its_el);
			XmlElement xmlElement = (XmlElement)documentElement.SelectSingleNode("unlock_items");
			if (xmlElement != null)
			{
				coldProfileData.UnlockItems = ColdProfileDataXMLSerializer.DeserializeUnlockItems(xmlElement);
			}
			XmlElement xmlElement2 = (XmlElement)documentElement.SelectSingleNode("achievements");
			if (xmlElement2 != null)
			{
				coldProfileData.Achievements = ColdProfileDataXMLSerializer.DeserializeAchievements(xmlElement2);
			}
			XmlElement xmlElement3 = (XmlElement)documentElement.SelectSingleNode("sponsor_points");
			if (xmlElement3 != null)
			{
				coldProfileData.SponsorPoints = ColdProfileDataXMLSerializer.DeserializeSponsorPoints(xmlElement3);
			}
			XmlElement xmlElement4 = (XmlElement)documentElement.SelectSingleNode("persistent_settings");
			if (xmlElement4 != null)
			{
				coldProfileData.PersistentSettings = ColdProfileDataXMLSerializer.DeserializePersistentSettings(xmlElement4);
			}
			XmlElement xmlElement5 = (XmlElement)documentElement.SelectSingleNode("contract");
			if (xmlElement5 != null)
			{
				coldProfileData.ProfileContract = ColdProfileDataXMLSerializer.DeserializeProfileContract(xmlElement5);
			}
			XmlElement xmlElement6 = (XmlElement)documentElement.SelectSingleNode("player_statistics");
			if (xmlElement6 != null)
			{
				coldProfileData.PlayerStatistics = this.DeserializePlayerStatistics(xmlElement6);
			}
			XmlElement xmlElement7 = (XmlElement)documentElement.SelectSingleNode("profile_progression");
			if (xmlElement7 != null)
			{
				coldProfileData.ProfileProgression = ColdProfileDataXMLSerializer.DeserializeProfileProgression(xmlElement7, version);
			}
			XmlElement xmlElement8 = (XmlElement)documentElement.SelectSingleNode("custom_rules_state");
			if (xmlElement8 != null)
			{
				coldProfileData.CustomRuleStates = this.DeserializeCustomRulesState(xmlElement8);
			}
			if (version > new DBVersion(2, 71))
			{
				XmlElement xmlElement9 = (XmlElement)documentElement.SelectSingleNode("skill_infos");
				if (xmlElement9 != null)
				{
					coldProfileData.SkillInfos = this.DeserializeProfileSkillInfos(xmlElement9);
				}
			}
			XmlElement xmlElement10 = (XmlElement)documentElement.SelectSingleNode("rating_info");
			if (xmlElement10 != null)
			{
				coldProfileData.RatingInfo = this.DeserializeProfileRatingInfo(xmlElement10);
			}
			XmlElement xmlElement11 = (XmlElement)documentElement.SelectSingleNode("rating_game_player_ban_info");
			if (xmlElement11 != null)
			{
				coldProfileData.RatingGamePlayerBan = this.DeserializeRatingGameBanInfo(xmlElement11);
			}
			return coldProfileData;
		}

		// Token: 0x060000F5 RID: 245 RVA: 0x00009A20 File Offset: 0x00007C20
		private static XmlElement SerializeItems(XmlDocument doc, IEnumerable<SEquipItem> items)
		{
			XmlElement xmlElement = doc.CreateElement("equip_items");
			foreach (SEquipItem it in items)
			{
				XmlElement newChild = ColdProfileDataXMLSerializer.SerializeItem(doc, it);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000F6 RID: 246 RVA: 0x00009A8C File Offset: 0x00007C8C
		private XmlNode SerializeUnlockItems(XmlDocument doc, IEnumerable<ulong> unlockItems)
		{
			XmlElement xmlElement = doc.CreateElement("unlock_items");
			foreach (ulong unlockItem in unlockItems)
			{
				XmlNode newChild = this.SerializeUnlockItem(doc, unlockItem);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000F7 RID: 247 RVA: 0x00009AF8 File Offset: 0x00007CF8
		private XmlNode SerializeAchievements(XmlDocument doc, IEnumerable<AchievementInfo> achievements)
		{
			XmlElement xmlElement = doc.CreateElement("achievements");
			foreach (AchievementInfo achievement in achievements)
			{
				XmlNode newChild = this.SerializeAchievement(doc, achievement);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000F8 RID: 248 RVA: 0x00009B64 File Offset: 0x00007D64
		private XmlNode SerializeSponsorPoints(XmlDocument doc, IEnumerable<SSponsorPoints> sponsorPoints)
		{
			XmlElement xmlElement = doc.CreateElement("sponsor_points");
			foreach (SSponsorPoints sponsorPoint in sponsorPoints)
			{
				XmlNode newChild = this.SerializeSponsorPoint(doc, sponsorPoint);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000F9 RID: 249 RVA: 0x00009BD0 File Offset: 0x00007DD0
		private XmlNode SerializePersistentSettings(XmlDocument doc, IEnumerable<SPersistentSettings> persistentSettings)
		{
			XmlElement xmlElement = doc.CreateElement("persistent_settings");
			foreach (SPersistentSettings setting in persistentSettings)
			{
				XmlNode newChild = this.SerializePersistentSetting(doc, setting);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000FA RID: 250 RVA: 0x00009C3C File Offset: 0x00007E3C
		private XmlNode SerializePlayerStatistics(XmlDocument doc, PlayerStatistics playerStats)
		{
			XmlElement xmlElement = doc.CreateElement("player_statistics");
			xmlElement.SetAttribute("version", playerStats.Version.ToString());
			foreach (Measure measure in playerStats.Measures)
			{
				XmlNode newChild = this.SerializePlayerStat(doc, measure);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000FB RID: 251 RVA: 0x00009CCC File Offset: 0x00007ECC
		private XmlNode SerializeCustomRulesState(XmlDocument doc, IEnumerable<CustomRuleRawState> states)
		{
			XmlElement xmlElement = doc.CreateElement("custom_rules_state");
			foreach (CustomRuleRawState rawState in states)
			{
				XmlNode newChild = this.SerializeCustomRuleState(doc, rawState);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000FC RID: 252 RVA: 0x00009D38 File Offset: 0x00007F38
		private XmlNode SerializeSkillInfos(XmlDocument doc, IEnumerable<SkillInfo> skillInfoCollection)
		{
			XmlElement xmlElement = doc.CreateElement("skill_infos");
			foreach (XmlNode newChild in from skillInfo in skillInfoCollection
			select this.SerializeSkillInfo(doc, skillInfo))
			{
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x060000FD RID: 253 RVA: 0x00009DC4 File Offset: 0x00007FC4
		private XmlNode SerializeRatingInfo(XmlDocument doc, RatingInfo ratingInfo)
		{
			XmlElement xmlElement = doc.CreateElement("rating_info");
			xmlElement.SetAttribute("rating_points", ratingInfo.RatingPoints.ToString());
			xmlElement.SetAttribute("win_streak", ratingInfo.WinStreak.ToString());
			xmlElement.SetAttribute("season_id", ratingInfo.SeasonId);
			return xmlElement;
		}

		// Token: 0x060000FE RID: 254 RVA: 0x00009E2C File Offset: 0x0000802C
		private XmlNode SerializeRatingGameBanInfo(XmlDocument doc, RatingGamePlayerBanInfo banInfo)
		{
			XmlElement xmlElement = doc.CreateElement("rating_game_player_ban_info");
			xmlElement.SetAttribute("ban_timeout", banInfo.BanTimeout.ToString());
			xmlElement.SetAttribute("unban_time", banInfo.UnbanTime.ToString(CultureInfo.InvariantCulture));
			return xmlElement;
		}

		// Token: 0x060000FF RID: 255 RVA: 0x00009E83 File Offset: 0x00008083
		private static List<SEquipItem> DeserializeItems(XmlElement its_el)
		{
			IEnumerable<XmlElement> source = its_el.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache0 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache0 = new Func<XmlElement, SEquipItem>(ColdProfileDataXMLSerializer.DeserializeItem);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache0).ToList<SEquipItem>();
		}

		// Token: 0x06000100 RID: 256 RVA: 0x00009EB7 File Offset: 0x000080B7
		private static List<ulong> DeserializeUnlockItems(XmlElement unlockItemsEl)
		{
			IEnumerable<XmlElement> source = unlockItemsEl.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache1 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache1 = new Func<XmlElement, ulong>(ColdProfileDataXMLSerializer.DeserializeUnlockItem);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache1).ToList<ulong>();
		}

		// Token: 0x06000101 RID: 257 RVA: 0x00009EEB File Offset: 0x000080EB
		private static List<AchievementInfo> DeserializeAchievements(XmlElement achievenetsElement)
		{
			IEnumerable<XmlElement> source = achievenetsElement.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache2 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache2 = new Func<XmlElement, AchievementInfo>(ColdProfileDataXMLSerializer.DeserializeAchievement);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache2).ToList<AchievementInfo>();
		}

		// Token: 0x06000102 RID: 258 RVA: 0x00009F1F File Offset: 0x0000811F
		private static List<SSponsorPoints> DeserializeSponsorPoints(XmlElement sponsorPointsElement)
		{
			IEnumerable<XmlElement> source = sponsorPointsElement.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache3 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache3 = new Func<XmlElement, SSponsorPoints>(ColdProfileDataXMLSerializer.DeserializeSponsorPoint);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache3).ToList<SSponsorPoints>();
		}

		// Token: 0x06000103 RID: 259 RVA: 0x00009F53 File Offset: 0x00008153
		private static List<SPersistentSettings> DeserializePersistentSettings(XmlElement persistentSettingsElement)
		{
			IEnumerable<XmlElement> source = persistentSettingsElement.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache4 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache4 = new Func<XmlElement, SPersistentSettings>(ColdProfileDataXMLSerializer.DeserializePersistentSetting);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache4).ToList<SPersistentSettings>();
		}

		// Token: 0x06000104 RID: 260 RVA: 0x00009F88 File Offset: 0x00008188
		private PlayerStatistics DeserializePlayerStatistics(XmlElement playerStatisticsElement)
		{
			return new PlayerStatistics
			{
				Version = DBVersion.Parse(playerStatisticsElement.GetAttribute("version")),
				Measures = (from XmlElement el in playerStatisticsElement.ChildNodes
				select this.DeserializePlayerStat(el)).ToList<Measure>()
			};
		}

		// Token: 0x06000105 RID: 261 RVA: 0x00009FD9 File Offset: 0x000081D9
		private List<CustomRuleRawState> DeserializeCustomRulesState(XmlElement rulesElement)
		{
			IEnumerable<XmlElement> source = rulesElement.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache5 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache5 = new Func<XmlElement, CustomRuleRawState>(ColdProfileDataXMLSerializer.DeserializeCustomRuleState);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache5).ToList<CustomRuleRawState>();
		}

		// Token: 0x06000106 RID: 262 RVA: 0x0000A00D File Offset: 0x0000820D
		private List<SkillInfo> DeserializeProfileSkillInfos(XmlNode skillsElement)
		{
			IEnumerable<XmlElement> source = skillsElement.ChildNodes.Cast<XmlElement>();
			if (ColdProfileDataXMLSerializer.<>f__mg$cache6 == null)
			{
				ColdProfileDataXMLSerializer.<>f__mg$cache6 = new Func<XmlElement, SkillInfo>(ColdProfileDataXMLSerializer.DeserializeProfileSkillInfo);
			}
			return source.Select(ColdProfileDataXMLSerializer.<>f__mg$cache6).ToList<SkillInfo>();
		}

		// Token: 0x06000107 RID: 263 RVA: 0x0000A044 File Offset: 0x00008244
		private RatingInfo DeserializeProfileRatingInfo(XmlElement ratingElement)
		{
			RatingInfo result = default(RatingInfo);
			result.RatingPoints = uint.Parse(ratingElement.GetAttribute("rating_points"));
			if (ratingElement.HasAttribute("season_id"))
			{
				result.SeasonId = ratingElement.GetAttribute("season_id");
			}
			if (ratingElement.HasAttribute("win_streak"))
			{
				result.WinStreak = uint.Parse(ratingElement.GetAttribute("win_streak"));
			}
			return result;
		}

		// Token: 0x06000108 RID: 264 RVA: 0x0000A0BC File Offset: 0x000082BC
		private RatingGamePlayerBanInfo DeserializeRatingGameBanInfo(XmlElement ratingGameBanInfo)
		{
			return new RatingGamePlayerBanInfo
			{
				BanTimeout = ulong.Parse(ratingGameBanInfo.GetAttribute("ban_timeout")),
				UnbanTime = DateTime.Parse(ratingGameBanInfo.GetAttribute("unban_time"), new DateTimeFormatInfo())
			};
		}

		// Token: 0x06000109 RID: 265 RVA: 0x0000A104 File Offset: 0x00008304
		private static XmlElement SerializeItem(XmlDocument doc, SEquipItem it)
		{
			XmlElement xmlElement = doc.CreateElement("it");
			xmlElement.SetAttribute("item", it.ItemID.ToString());
			xmlElement.SetAttribute("attached_to", it.AttachedTo.ToString());
			xmlElement.SetAttribute("slot_ids", it.SlotIDs.ToString());
			xmlElement.SetAttribute("config", it.Config);
			xmlElement.SetAttribute("status", it.Status.ToString());
			if (it.CatalogID != 0UL)
			{
				xmlElement.SetAttribute("catalogId", it.CatalogID.ToString());
			}
			return xmlElement;
		}

		// Token: 0x0600010A RID: 266 RVA: 0x0000A1C8 File Offset: 0x000083C8
		private XmlNode SerializeUnlockItem(XmlDocument doc, ulong unlockItem)
		{
			XmlElement xmlElement = doc.CreateElement("unlock_item");
			xmlElement.SetAttribute("id", unlockItem.ToString());
			return xmlElement;
		}

		// Token: 0x0600010B RID: 267 RVA: 0x0000A1FC File Offset: 0x000083FC
		private XmlNode SerializeAchievement(XmlDocument doc, AchievementInfo achievement)
		{
			XmlElement xmlElement = doc.CreateElement("achievement");
			xmlElement.SetAttribute("id", achievement.ID.ToString());
			xmlElement.SetAttribute("progress", achievement.Progress.ToString());
			xmlElement.SetAttribute("completion_time", achievement.CompletionTime.ToString());
			return xmlElement;
		}

		// Token: 0x0600010C RID: 268 RVA: 0x0000A270 File Offset: 0x00008470
		private XmlNode SerializeSponsorPoint(XmlDocument doc, SSponsorPoints sponsorPoint)
		{
			XmlElement xmlElement = doc.CreateElement("sponsor_points");
			xmlElement.SetAttribute("sponsor_id", sponsorPoint.SponsorID.ToString());
			xmlElement.SetAttribute("next_unlock_item_id", sponsorPoint.NextUnlockItemId.ToString());
			xmlElement.SetAttribute("points", sponsorPoint.RankInfo.Points.ToString());
			xmlElement.SetAttribute("stage_id", sponsorPoint.RankInfo.RankId.ToString());
			xmlElement.SetAttribute("stage_start", sponsorPoint.RankInfo.RankStart.ToString());
			xmlElement.SetAttribute("next_stage_start", sponsorPoint.RankInfo.NextRankStart.ToString());
			return xmlElement;
		}

		// Token: 0x0600010D RID: 269 RVA: 0x0000A34C File Offset: 0x0000854C
		private XmlNode SerializePersistentSetting(XmlDocument doc, SPersistentSettings setting)
		{
			XmlElement xmlElement = doc.CreateElement("setting");
			xmlElement.SetAttribute("group", setting.Group);
			xmlElement.SetAttribute("settings", setting.Settings);
			return xmlElement;
		}

		// Token: 0x0600010E RID: 270 RVA: 0x0000A38C File Offset: 0x0000858C
		private XmlNode SerializePlayerStat(XmlDocument doc, Measure measure)
		{
			XmlElement xmlElement = doc.CreateElement("stat");
			xmlElement.SetAttribute("row_count", measure.RowCount.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("value", measure.Value.ToString(CultureInfo.InvariantCulture));
			foreach (KeyValuePair<string, string> keyValuePair in measure.Dimensions)
			{
				XmlElement xmlElement2 = doc.CreateElement("dimension");
				xmlElement2.SetAttribute("key", keyValuePair.Key);
				xmlElement2.SetAttribute("value", keyValuePair.Value);
				xmlElement.AppendChild(xmlElement2);
			}
			return xmlElement;
		}

		// Token: 0x0600010F RID: 271 RVA: 0x0000A45C File Offset: 0x0000865C
		private XmlNode SerializeProfileContract(XmlDocument doc, ProfileContract contract)
		{
			XmlElement xmlElement = doc.CreateElement("contract");
			xmlElement.SetAttribute("profile_id", contract.ProfileId.ToString());
			xmlElement.SetAttribute("rotation_id", contract.RotationId.ToString());
			xmlElement.SetAttribute("profile_item_id", contract.ProfileItemId.ToString());
			xmlElement.SetAttribute("contract_name", contract.ContractName);
			xmlElement.SetAttribute("current", contract.CurrentProgress.ToString());
			xmlElement.SetAttribute("total", contract.TotalProgress.ToString());
			xmlElement.SetAttribute("rotation_time", TimeUtils.LocalTimeToUTCTimestamp(contract.RotationTimeUTC).ToString());
			xmlElement.SetAttribute("status", ((int)contract.Status).ToString());
			return xmlElement;
		}

		// Token: 0x06000110 RID: 272 RVA: 0x0000A558 File Offset: 0x00008758
		private XmlNode SerializeProfileProgression(XmlDocument doc, SProfileProgression progression)
		{
			XmlElement xmlElement = doc.CreateElement("profile_progression");
			xmlElement.SetAttribute("profile_id", progression.ProfileId.ToString());
			xmlElement.SetAttribute("mission_unlocked", progression.MissionUnlocked.ToString());
			xmlElement.SetAttribute("mission_pass_counter", progression.MissionPassCounter.ToString());
			xmlElement.SetAttribute("zombie_pass_counter", progression.ZombieMissionPassCounter.ToString());
			xmlElement.SetAttribute("campaign_pass_counter", progression.CampaignPassCounter.ToString());
			xmlElement.SetAttribute("volcano_campaign_pass_counter", progression.VolcanoCampaignPasCounter.ToString());
			xmlElement.SetAttribute("anubis_campaign_pass_counter", progression.AnubisCampaignPassCounter.ToString());
			xmlElement.SetAttribute("zombietower_campaign_pass_counter", progression.ZombieTowerCampaignPassCounter.ToString());
			xmlElement.SetAttribute("icebreaker_campaign_pass_counter", progression.IceBreakerCampaignPassCounter.ToString());
			xmlElement.SetAttribute("tutorial_unlocked", progression.TutorialUnlocked.ToString());
			xmlElement.SetAttribute("tutorial_passed", progression.TutorialPassed.ToString());
			xmlElement.SetAttribute("class_unlocked", progression.ClassUnlocked.ToString());
			return xmlElement;
		}

		// Token: 0x06000111 RID: 273 RVA: 0x0000A6D0 File Offset: 0x000088D0
		private XmlNode SerializeCustomRuleState(XmlDocument doc, CustomRuleRawState rawState)
		{
			XmlElement xmlElement = doc.CreateElement("state");
			xmlElement.SetAttribute("profile_id", rawState.Key.ProfileID.ToString());
			xmlElement.SetAttribute("rule_id", rawState.Key.RuleID.ToString());
			xmlElement.SetAttribute("version", rawState.Key.Version.ToString());
			xmlElement.SetAttribute("last_update", TimeUtils.LocalTimeToUTCTimestamp(rawState.LastUpdateTimeUtc).ToString());
			xmlElement.SetAttribute("rule_type", rawState.RuleType.ToString());
			xmlElement.SetAttribute("data", Convert.ToBase64String(rawState.Data ?? new byte[0]));
			return xmlElement;
		}

		// Token: 0x06000112 RID: 274 RVA: 0x0000A7C0 File Offset: 0x000089C0
		private XmlNode SerializeSkillInfo(XmlDocument doc, SkillInfo skill)
		{
			XmlElement xmlElement = doc.CreateElement("profile_skill");
			xmlElement.SetAttribute("type", skill.Type);
			xmlElement.SetAttribute("points", skill.Points.ToString());
			xmlElement.SetAttribute("curveCoef", skill.CurveCoef.ToString());
			return xmlElement;
		}

		// Token: 0x06000113 RID: 275 RVA: 0x0000A828 File Offset: 0x00008A28
		private static SEquipItem DeserializeItem(XmlElement el)
		{
			SEquipItem sequipItem = new SEquipItem();
			sequipItem.ItemID = ulong.Parse(el.GetAttribute("item"));
			sequipItem.AttachedTo = ulong.Parse(el.GetAttribute("attached_to"));
			sequipItem.SlotIDs = ulong.Parse(el.GetAttribute("slot_ids"));
			sequipItem.Config = el.GetAttribute("config");
			sequipItem.Status = (EProfileItemStatus)Enum.Parse(typeof(EProfileItemStatus), el.GetAttribute("status"), true);
			if (el.HasAttribute("catalogId"))
			{
				sequipItem.CatalogID = ulong.Parse(el.GetAttribute("catalogId"));
			}
			return sequipItem;
		}

		// Token: 0x06000114 RID: 276 RVA: 0x0000A8DB File Offset: 0x00008ADB
		private static ulong DeserializeUnlockItem(XmlElement xmlElement)
		{
			return ulong.Parse(xmlElement.GetAttribute("id"));
		}

		// Token: 0x06000115 RID: 277 RVA: 0x0000A8F0 File Offset: 0x00008AF0
		private static AchievementInfo DeserializeAchievement(XmlElement xmlElement)
		{
			return new AchievementInfo
			{
				ID = int.Parse(xmlElement.GetAttribute("id")),
				Progress = int.Parse(xmlElement.GetAttribute("progress")),
				CompletionTime = ulong.Parse(xmlElement.GetAttribute("completion_time"))
			};
		}

		// Token: 0x06000116 RID: 278 RVA: 0x0000A94C File Offset: 0x00008B4C
		private static SSponsorPoints DeserializeSponsorPoint(XmlElement xmlElement)
		{
			SSponsorPoints result = default(SSponsorPoints);
			result.SponsorID = uint.Parse(xmlElement.GetAttribute("sponsor_id"));
			result.NextUnlockItemId = ulong.Parse(xmlElement.GetAttribute("next_unlock_item_id"));
			result.RankInfo = default(SRankInfo);
			result.RankInfo.Points = (ulong)uint.Parse(xmlElement.GetAttribute("points"));
			result.RankInfo.RankId = (int)byte.Parse(xmlElement.GetAttribute("stage_id"));
			result.RankInfo.RankStart = ulong.Parse(xmlElement.GetAttribute("stage_start"));
			result.RankInfo.NextRankStart = ulong.Parse(xmlElement.GetAttribute("next_stage_start"));
			return result;
		}

		// Token: 0x06000117 RID: 279 RVA: 0x0000AA14 File Offset: 0x00008C14
		private static SPersistentSettings DeserializePersistentSetting(XmlElement xmlElement)
		{
			return new SPersistentSettings
			{
				Group = xmlElement.GetAttribute("group"),
				Settings = xmlElement.GetAttribute("settings")
			};
		}

		// Token: 0x06000118 RID: 280 RVA: 0x0000AA50 File Offset: 0x00008C50
		private Measure DeserializePlayerStat(XmlElement statElement)
		{
			Measure result = default(Measure);
			result.Dimensions = new SortedList<string, string>();
			result.RowCount = long.Parse(statElement.GetAttribute("row_count"));
			result.Value = long.Parse(statElement.GetAttribute("value"));
			IEnumerator enumerator = statElement.SelectNodes("dimension").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					string value = xmlNode.Attributes["key"].Value;
					string value2 = xmlNode.Attributes["value"].Value;
					result.Dimensions.Add(value, value2);
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
			return result;
		}

		// Token: 0x06000119 RID: 281 RVA: 0x0000AB34 File Offset: 0x00008D34
		private static ProfileContract DeserializeProfileContract(XmlElement xmlElement)
		{
			ProfileContract profileContract = new ProfileContract();
			profileContract.ProfileId = ulong.Parse(xmlElement.GetAttribute("profile_id"));
			profileContract.RotationId = uint.Parse(xmlElement.GetAttribute("rotation_id"));
			profileContract.ProfileItemId = ulong.Parse(xmlElement.GetAttribute("profile_item_id"));
			profileContract.ContractName = xmlElement.GetAttribute("contract_name");
			profileContract.CurrentProgress = uint.Parse(xmlElement.GetAttribute("current"));
			profileContract.TotalProgress = uint.Parse(xmlElement.GetAttribute("total"));
			ulong utc = ulong.Parse(xmlElement.GetAttribute("rotation_time"));
			profileContract.RotationTimeUTC = TimeUtils.UTCTimestampToUTCTime(utc);
			return profileContract;
		}

		// Token: 0x0600011A RID: 282 RVA: 0x0000ABE4 File Offset: 0x00008DE4
		private static SProfileProgression DeserializeProfileProgression(XmlElement xmlElement, DBVersion version)
		{
			SProfileProgression result = default(SProfileProgression);
			result.ProfileId = ulong.Parse(xmlElement.GetAttribute("profile_id"));
			if (version < new DBVersion(2, 64))
			{
				result.MissionPassCounter = int.Parse(xmlElement.GetAttribute("mission_unlocked"));
				result.ZombieMissionPassCounter = ((!xmlElement.HasAttribute("zombie_mission_unlocked")) ? 0 : int.Parse(xmlElement.GetAttribute("zombie_mission_unlocked")));
			}
			else
			{
				result.MissionUnlocked = int.Parse(xmlElement.GetAttribute("mission_unlocked"));
				result.MissionPassCounter = int.Parse(xmlElement.GetAttribute("mission_pass_counter"));
				result.ZombieMissionPassCounter = ((!xmlElement.HasAttribute("zombie_pass_counter")) ? 0 : int.Parse(xmlElement.GetAttribute("zombie_pass_counter")));
			}
			result.CampaignPassCounter = ((!xmlElement.HasAttribute("campaign_pass_counter")) ? 0 : int.Parse(xmlElement.GetAttribute("campaign_pass_counter")));
			result.VolcanoCampaignPasCounter = ((!xmlElement.HasAttribute("volcano_campaign_pass_counter")) ? 0 : int.Parse(xmlElement.GetAttribute("volcano_campaign_pass_counter")));
			result.AnubisCampaignPassCounter = ((!xmlElement.HasAttribute("anubis_campaign_pass_counter")) ? 0 : int.Parse(xmlElement.GetAttribute("anubis_campaign_pass_counter")));
			result.ZombieTowerCampaignPassCounter = ((!xmlElement.HasAttribute("zombietower_campaign_pass_counter")) ? 0 : int.Parse(xmlElement.GetAttribute("zombietower_campaign_pass_counter")));
			result.IceBreakerCampaignPassCounter = ((!xmlElement.HasAttribute("icebreaker_campaign_pass_counter")) ? 0 : int.Parse(xmlElement.GetAttribute("icebreaker_campaign_pass_counter")));
			result.TutorialUnlocked = int.Parse(xmlElement.GetAttribute("tutorial_unlocked"));
			result.TutorialPassed = int.Parse(xmlElement.GetAttribute("tutorial_passed"));
			result.ClassUnlocked = int.Parse(xmlElement.GetAttribute("class_unlocked"));
			return result;
		}

		// Token: 0x0600011B RID: 283 RVA: 0x0000ADF0 File Offset: 0x00008FF0
		private static CustomRuleRawState DeserializeCustomRuleState(XmlElement xmlElement)
		{
			return new CustomRuleRawState
			{
				Key = new CustomRuleRawState.KeyData
				{
					ProfileID = ulong.Parse(xmlElement.GetAttribute("profile_id")),
					RuleID = ulong.Parse(xmlElement.GetAttribute("rule_id")),
					Version = uint.Parse(xmlElement.GetAttribute("version"))
				},
				LastUpdateTimeUtc = TimeUtils.UTCTimestampToUTCTime(ulong.Parse(xmlElement.GetAttribute("last_update"))),
				RuleType = byte.Parse(xmlElement.GetAttribute("rule_type")),
				Data = Convert.FromBase64String(xmlElement.GetAttribute("data"))
			};
		}

		// Token: 0x0600011C RID: 284 RVA: 0x0000AEA0 File Offset: 0x000090A0
		private static SkillInfo DeserializeProfileSkillInfo(XmlElement elem)
		{
			return new SkillInfo
			{
				Type = elem.GetAttribute("type"),
				Points = double.Parse(elem.GetAttribute("points")),
				CurveCoef = double.Parse(elem.GetAttribute("curveCoef"))
			};
		}

		// Token: 0x0400005B RID: 91
		[CompilerGenerated]
		private static Func<XmlElement, SEquipItem> <>f__mg$cache0;

		// Token: 0x0400005C RID: 92
		[CompilerGenerated]
		private static Func<XmlElement, ulong> <>f__mg$cache1;

		// Token: 0x0400005D RID: 93
		[CompilerGenerated]
		private static Func<XmlElement, AchievementInfo> <>f__mg$cache2;

		// Token: 0x0400005E RID: 94
		[CompilerGenerated]
		private static Func<XmlElement, SSponsorPoints> <>f__mg$cache3;

		// Token: 0x0400005F RID: 95
		[CompilerGenerated]
		private static Func<XmlElement, SPersistentSettings> <>f__mg$cache4;

		// Token: 0x04000060 RID: 96
		[CompilerGenerated]
		private static Func<XmlElement, CustomRuleRawState> <>f__mg$cache5;

		// Token: 0x04000061 RID: 97
		[CompilerGenerated]
		private static Func<XmlElement, SkillInfo> <>f__mg$cache6;
	}
}

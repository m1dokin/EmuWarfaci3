using System;
using System.Collections.Generic;
using MasterServer.DAL.CustomRules;
using MasterServer.DAL.PlayerStats;
using MasterServer.DAL.RatingSystem;
using MasterServer.DAL.Utils;
using MasterServer.Database;
using OLAPHypervisor;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x0200001C RID: 28
	internal class ColdStorageSystem : IColdStorageSystem
	{
		// Token: 0x0600011E RID: 286 RVA: 0x0000AF1F File Offset: 0x0000911F
		public ColdStorageSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x0600011F RID: 287 RVA: 0x0000AF30 File Offset: 0x00009130
		public DALResult<bool?> IsProfileCold(ulong profile_id)
		{
			DALStats dalstats = new DALStats();
			bool? val;
			using (MySqlAccessor mySqlAccessor = new MySqlAccessor(dalstats))
			{
				using (DBDataReader dbdataReader = mySqlAccessor.ExecuteReader("SELECT cold FROM profiles WHERE id = ?pid", new object[]
				{
					"?pid",
					profile_id
				}))
				{
					val = ((!dbdataReader.Read()) ? null : new bool?(dbdataReader[0].ToString() == "1"));
				}
			}
			return new DALResult<bool?>(val, dalstats);
		}

		// Token: 0x06000120 RID: 288 RVA: 0x0000AFEC File Offset: 0x000091EC
		public DALResult<TouchProfileResult> TouchProfile(ulong profile_id, DBVersion current_schema)
		{
			ColdStorageSystem.<TouchProfile>c__AnonStorey1 <TouchProfile>c__AnonStorey = new ColdStorageSystem.<TouchProfile>c__AnonStorey1();
			<TouchProfile>c__AnonStorey.profile_id = profile_id;
			<TouchProfile>c__AnonStorey.$this = this;
			DALStats dalstats = new DALStats();
			<TouchProfile>c__AnonStorey.result = new TouchProfileResult
			{
				Status = ETouchProfileResult.NotExists
			};
			DALResult<TouchProfileResult> result;
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(dalstats))
			{
				if (!acc.Transaction(delegate()
				{
					using (DBDataReader dbdataReader = acc.ExecuteReader("SELECT cold, UNIX_TIMESTAMP(last_seen) FROM profiles WHERE id = ?pid FOR UPDATE", new object[]
					{
						"?pid",
						<TouchProfile>c__AnonStorey.profile_id
					}))
					{
						if (!dbdataReader.Read())
						{
							return false;
						}
						<TouchProfile>c__AnonStorey.result.Status = ((!(dbdataReader[0].ToString() == "1")) ? ETouchProfileResult.IsHot : ETouchProfileResult.RestoredFromCold);
						<TouchProfile>c__AnonStorey.result.PreviousLastSeenTimeUTC = ulong.Parse(dbdataReader[1].ToString());
					}
					acc.ExecuteNonQuery("CALL UpdateLastSeenDate(?pid)", new object[]
					{
						"?pid",
						<TouchProfile>c__AnonStorey.profile_id
					});
					if (<TouchProfile>c__AnonStorey.result.Status == ETouchProfileResult.IsHot)
					{
						return true;
					}
					DBVersion dbversion;
					byte[] data;
					using (DBDataReader dbdataReader2 = acc.ExecuteReader("CALL LoadColdProfileData(?pid)", new object[]
					{
						"?pid",
						<TouchProfile>c__AnonStorey.profile_id
					}))
					{
						if (!dbdataReader2.Read())
						{
							throw new Exception(string.Format("Failed to load cold data for profile {0}", <TouchProfile>c__AnonStorey.profile_id));
						}
						dbversion = DBVersion.Parse(dbdataReader2["version"].ToString());
						data = (byte[])dbdataReader2["data"];
					}
					<TouchProfile>c__AnonStorey.result.DataVersion = dbversion;
					DataArchiveSerializer<ColdProfileData> dataArchiveSerializer = new DataArchiveSerializer<ColdProfileData>(new ColdProfileDataXMLSerializer());
					ColdProfileData profile_data = dataArchiveSerializer.Deserialize(data, dbversion);
					<TouchProfile>c__AnonStorey.$this.RestoreDataFromColdStorage(acc, <TouchProfile>c__AnonStorey.profile_id, profile_data);
					acc.ExecuteNonQuery("CALL DeleteColdProfileData(?pid)", new object[]
					{
						"?pid",
						<TouchProfile>c__AnonStorey.profile_id
					});
					acc.ExecuteNonQuery("UPDATE profiles SET cold = 0 WHERE id = ?pid", new object[]
					{
						"?pid",
						<TouchProfile>c__AnonStorey.profile_id
					});
					return true;
				}))
				{
					throw new TransactionError(string.Format("Can't move profile {0} to hot on schema version {1}", <TouchProfile>c__AnonStorey.profile_id, current_schema));
				}
				result = new DALResult<TouchProfileResult>(<TouchProfile>c__AnonStorey.result, dalstats);
			}
			return result;
		}

		// Token: 0x06000121 RID: 289 RVA: 0x0000B0C0 File Offset: 0x000092C0
		private void RestoreDataFromColdStorage(MySqlAccessorTransaction acc, ulong profile_id, ColdProfileData profile_data)
		{
			this.RestoreEquipItems(acc, profile_id, profile_data);
			this.RestoreUnlockItems(acc, profile_id, profile_data);
			this.RestoreAchievement(acc, profile_id, profile_data);
			this.RestoreSponsorPoints(acc, profile_id, profile_data);
			this.RestorePersistentSettings(acc, profile_id, profile_data);
			this.RestoreProfileContract(acc, profile_id, profile_data);
			this.RestoreProfileStatisticsData(acc, profile_id, profile_data);
			this.RestoreProfileProgressionData(acc, profile_id, profile_data);
			this.RestoreCustomRulesState(acc, profile_id, profile_data);
			this.RestoreProfileSkillData(acc, profile_id, profile_data);
			this.RestoreProfileRatingData(acc, profile_id, profile_data);
			this.RestoreProfileRatingBanData(acc, profile_id, profile_data);
		}

		// Token: 0x06000122 RID: 290 RVA: 0x0000B13C File Offset: 0x0000933C
		private void RestoreEquipItems(MySqlAccessorTransaction acc, ulong profile_id, ColdProfileData profile_data)
		{
			foreach (SEquipItem sequipItem in profile_data.EquipItems)
			{
				acc.ExecuteNonQuery("SELECT AddItem(?pid, ?item, ?attached_to, ?slots, ?config, ?status, ?cat)", new object[]
				{
					"?pid",
					profile_id,
					"?item",
					sequipItem.ItemID,
					"?attached_to",
					sequipItem.AttachedTo,
					"?slots",
					sequipItem.SlotIDs,
					"?config",
					sequipItem.Config,
					"?status",
					sequipItem.Status.ToString().ToLower(),
					"?cat",
					sequipItem.CatalogID
				});
			}
		}

		// Token: 0x06000123 RID: 291 RVA: 0x0000B244 File Offset: 0x00009444
		private void RestoreUnlockItems(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			foreach (ulong num in profileData.UnlockItems)
			{
				acc.ExecuteNonQuery("CALL UnlockItem(?pid, ?item)", new object[]
				{
					"?pid",
					profileId,
					"?item",
					num
				});
			}
		}

		// Token: 0x06000124 RID: 292 RVA: 0x0000B2D0 File Offset: 0x000094D0
		private void RestoreAchievement(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			foreach (AchievementInfo achievementInfo in profileData.Achievements)
			{
				acc.ExecuteNonQuery("CALL SetAchievementProgress(?pid, ?achv, ?newprgs, ?maxprgs, ?cmpltd)", new object[]
				{
					"?pid",
					profileId,
					"?achv",
					achievementInfo.ID,
					"?newprgs",
					achievementInfo.Progress,
					"?maxprgs",
					(achievementInfo.CompletionTime == 0UL) ? int.MaxValue : achievementInfo.Progress,
					"?cmpltd",
					achievementInfo.CompletionTime
				});
			}
		}

		// Token: 0x06000125 RID: 293 RVA: 0x0000B3C0 File Offset: 0x000095C0
		private void RestoreSponsorPoints(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			foreach (SSponsorPoints ssponsorPoints in profileData.SponsorPoints)
			{
				acc.ExecuteNonQuery("SELECT SetSponsorInfo(?pid, ?spid, ?oldSpPts, ?sppts, ?spStage, ?spStageStart, ?spNextStageStart)", new object[]
				{
					"?pid",
					profileId,
					"?spid",
					ssponsorPoints.SponsorID,
					"?oldSpPts",
					0,
					"?sppts",
					ssponsorPoints.RankInfo.Points,
					"?spStage",
					ssponsorPoints.RankInfo.RankId,
					"?spStageStart",
					ssponsorPoints.RankInfo.RankStart,
					"?spNextStageStart",
					ssponsorPoints.RankInfo.NextRankStart
				});
				acc.ExecuteNonQuery("CALL SetNextUnlockItem(?pid, ?spid, ?nuitd)", new object[]
				{
					"?pid",
					profileId,
					"?spid",
					ssponsorPoints.SponsorID,
					"?nuitd",
					ssponsorPoints.NextUnlockItemId
				});
			}
		}

		// Token: 0x06000126 RID: 294 RVA: 0x0000B534 File Offset: 0x00009734
		private void RestorePersistentSettings(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			foreach (SPersistentSettings spersistentSettings in profileData.PersistentSettings)
			{
				acc.ExecuteNonQuery("CALL SetPersistentSettings(?pid, ?grp, ?stng)", new object[]
				{
					"?pid",
					profileId,
					"?grp",
					spersistentSettings.Group,
					"?stng",
					spersistentSettings.Settings
				});
			}
		}

		// Token: 0x06000127 RID: 295 RVA: 0x0000B5D0 File Offset: 0x000097D0
		private void RestoreProfileContract(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			if (profileData.ProfileContract == null)
			{
				return;
			}
			acc.ExecuteNonQuery("CALL SetContractInfo(?pid, ?rid, ?pitem, ?name, ?progress_current, ?progress_total, ?rotation_time)", new object[]
			{
				"?pid",
				profileData.ProfileContract.ProfileId,
				"?rid",
				profileData.ProfileContract.RotationId,
				"?pitem",
				profileData.ProfileContract.ProfileItemId,
				"?name",
				profileData.ProfileContract.ContractName,
				"?progress_current",
				profileData.ProfileContract.CurrentProgress,
				"?progress_total",
				profileData.ProfileContract.TotalProgress,
				"?rotation_time",
				TimeUtils.LocalTimeToUTCTimestamp(profileData.ProfileContract.RotationTimeUTC)
			});
		}

		// Token: 0x06000128 RID: 296 RVA: 0x0000B6C0 File Offset: 0x000098C0
		private void RestoreProfileStatisticsData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			if (profileData.PlayerStatistics == null)
			{
				return;
			}
			DataArchiveSerializer<PlayerStatistics> dataArchiveSerializer = new DataArchiveSerializer<PlayerStatistics>(new PlayerStatisticsDataSerializer());
			byte[] array = dataArchiveSerializer.Serialize(profileData.PlayerStatistics);
			this.m_dal.ValidateFixedSizeColumnData("profile_stats", "stats", array.Length);
			acc.ExecuteNonQuery("CALL UpdatePlayerStats(?pid, ?data)", new object[]
			{
				"?pid",
				profileId,
				"?data",
				array
			});
		}

		// Token: 0x06000129 RID: 297 RVA: 0x0000B738 File Offset: 0x00009938
		private void RestoreProfileProgressionData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			if (!profileData.ProfileProgression.IsEmpty())
			{
				acc.ExecuteNonQuery("CALL SetProfileProgression(?pid, ?m_unlc, ?m_pass, ?zm_unlc, ?c_unlc, ?vc_unlc, ?ab_pass, ?zt_pass, ?ib_pass, ?tut_unlc, ?tut_psd, ?cl_unlc)", new object[]
				{
					"?pid",
					profileData.ProfileProgression.ProfileId,
					"?m_unlc",
					profileData.ProfileProgression.MissionUnlocked,
					"?m_pass",
					profileData.ProfileProgression.MissionPassCounter,
					"?zm_unlc",
					profileData.ProfileProgression.ZombieMissionPassCounter,
					"?c_unlc",
					profileData.ProfileProgression.CampaignPassCounter,
					"?vc_unlc",
					profileData.ProfileProgression.VolcanoCampaignPasCounter,
					"?ab_pass",
					profileData.ProfileProgression.AnubisCampaignPassCounter,
					"?zt_pass",
					profileData.ProfileProgression.ZombieTowerCampaignPassCounter,
					"?ib_pass",
					profileData.ProfileProgression.IceBreakerCampaignPassCounter,
					"?tut_unlc",
					profileData.ProfileProgression.TutorialUnlocked,
					"?tut_psd",
					profileData.ProfileProgression.TutorialPassed,
					"?cl_unlc",
					profileData.ProfileProgression.ClassUnlocked
				});
			}
		}

		// Token: 0x0600012A RID: 298 RVA: 0x0000B8BC File Offset: 0x00009ABC
		private void RestoreCustomRulesState(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			foreach (CustomRuleRawState customRuleRawState in profileData.CustomRuleStates)
			{
				acc.ExecuteNonQuery("CALL SetCustomRuleState(?pid, ?rid, ?vsn, ?rt, ?data)", new object[]
				{
					"?pid",
					customRuleRawState.Key.ProfileID,
					"?rid",
					customRuleRawState.Key.RuleID,
					"?vsn",
					customRuleRawState.Key.Version,
					"?rt",
					customRuleRawState.RuleType,
					"?data",
					customRuleRawState.Data
				});
			}
		}

		// Token: 0x0600012B RID: 299 RVA: 0x0000B9A8 File Offset: 0x00009BA8
		private void RestoreProfileSkillData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			foreach (SkillInfo skillInfo in profileData.SkillInfos)
			{
				acc.ExecuteNonQuery("CALL AddSkill(?pid, ?skill_type, ?skill_change, ?current_curve_coef)", new object[]
				{
					"?pid",
					profileId,
					"?skill_type",
					skillInfo.Type,
					"?skill_change",
					skillInfo.Points,
					"?current_curve_coef",
					skillInfo.CurveCoef
				});
			}
		}

		// Token: 0x0600012C RID: 300 RVA: 0x0000BA60 File Offset: 0x00009C60
		private void RestoreProfileRatingData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			RatingInfo ratingInfo = profileData.RatingInfo;
			if (!ratingInfo.IsEmpty())
			{
				acc.ExecuteNonQuery("CALL AddPvpRatingPoints(?pid, ?new_points, ?new_wins, ?new_season_id)", new object[]
				{
					"?pid",
					profileId,
					"?new_points",
					ratingInfo.RatingPoints,
					"?new_wins",
					ratingInfo.WinStreak,
					"?new_season_id",
					ratingInfo.SeasonId
				});
			}
		}

		// Token: 0x0600012D RID: 301 RVA: 0x0000BAE4 File Offset: 0x00009CE4
		private void RestoreProfileRatingBanData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			RatingGamePlayerBanInfo ratingGamePlayerBan = profileData.RatingGamePlayerBan;
			double num = Math.Max((ratingGamePlayerBan.UnbanTime - DateTime.UtcNow).TotalSeconds, 0.0);
			acc.ExecuteNonQuery("CALL BanPlayerForRatingGames(?pid, ?unban_time)", new object[]
			{
				"?pid",
				profileId,
				"?unban_time",
				num
			});
		}

		// Token: 0x0600012E RID: 302 RVA: 0x0000BB54 File Offset: 0x00009D54
		public DALResult<bool> MoveProfileToCold(ulong profile_id, TimeSpan threshold, DBVersion current_schema)
		{
			ColdStorageSystem.<MoveProfileToCold>c__AnonStorey3 <MoveProfileToCold>c__AnonStorey = new ColdStorageSystem.<MoveProfileToCold>c__AnonStorey3();
			<MoveProfileToCold>c__AnonStorey.profile_id = profile_id;
			<MoveProfileToCold>c__AnonStorey.threshold = threshold;
			<MoveProfileToCold>c__AnonStorey.current_schema = current_schema;
			<MoveProfileToCold>c__AnonStorey.$this = this;
			DALStats dalstats = new DALStats();
			<MoveProfileToCold>c__AnonStorey.moved = false;
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(dalstats))
			{
				acc.Transaction(delegate()
				{
					using (DBDataReader dbdataReader = acc.ExecuteReader("SELECT cold, UNIX_TIMESTAMP(last_seen) as last_see FROM profiles WHERE id = ?pid FOR UPDATE", new object[]
					{
						"?pid",
						<MoveProfileToCold>c__AnonStorey.profile_id
					}))
					{
						if (!dbdataReader.Read())
						{
							return false;
						}
						bool flag = dbdataReader[0].ToString() == "1";
						ulong utc = ulong.Parse(dbdataReader[1].ToString());
						DateTime d = TimeUtils.UTCTimestampToUTCTime(utc);
						if (flag || (!<MoveProfileToCold>c__AnonStorey.threshold.Equals(TimeSpan.Zero) && DateTime.UtcNow - d < <MoveProfileToCold>c__AnonStorey.threshold))
						{
							return false;
						}
					}
					ColdProfileData data = <MoveProfileToCold>c__AnonStorey.$this.GatherProfileData(acc, <MoveProfileToCold>c__AnonStorey.profile_id);
					DataArchiveSerializer<ColdProfileData> dataArchiveSerializer = new DataArchiveSerializer<ColdProfileData>(new ColdProfileDataXMLSerializer());
					byte[] array = dataArchiveSerializer.Serialize(data);
					<MoveProfileToCold>c__AnonStorey.$this.m_dal.ValidateFixedSizeColumnData("profiles_cold", "data", array.Length);
					acc.ExecuteNonQuery("CALL SaveColdProfileData(?pid, ?data, ?ver)", new object[]
					{
						"?pid",
						<MoveProfileToCold>c__AnonStorey.profile_id,
						"?data",
						array,
						"?ver",
						<MoveProfileToCold>c__AnonStorey.current_schema.ToString()
					});
					acc.ExecuteNonQuery("UPDATE profiles SET cold = 1 WHERE id = ?pid", new object[]
					{
						"?pid",
						<MoveProfileToCold>c__AnonStorey.profile_id
					});
					<MoveProfileToCold>c__AnonStorey.$this.CleanObsoleteProfileData(acc, <MoveProfileToCold>c__AnonStorey.profile_id);
					<MoveProfileToCold>c__AnonStorey.moved = true;
					return true;
				});
			}
			return new DALResult<bool>(<MoveProfileToCold>c__AnonStorey.moved, dalstats);
		}

		// Token: 0x0600012F RID: 303 RVA: 0x0000BBFC File Offset: 0x00009DFC
		private ColdProfileData GatherProfileData(MySqlAccessorTransaction acc, ulong profileId)
		{
			ColdProfileData coldProfileData = new ColdProfileData();
			this.GatherEquipItemsProfileData(acc, profileId, coldProfileData);
			this.GatherUnlockItemsProfileData(acc, profileId, coldProfileData);
			this.GatherAchievementProfileData(acc, profileId, coldProfileData);
			this.GatherSponsorPointsProfileData(acc, profileId, coldProfileData);
			this.GatherPersistentSettingsProfileData(acc, profileId, coldProfileData);
			this.GatherContractProfileData(acc, profileId, coldProfileData);
			this.GatherProfileStatisticsData(acc, profileId, coldProfileData);
			this.GatherProfileProgressionData(acc, profileId, coldProfileData);
			this.GatherCustomRulesState(acc, profileId, coldProfileData);
			this.GatherProfileSkillData(acc, profileId, coldProfileData);
			this.GatherProfileRatingData(acc, profileId, coldProfileData);
			this.GatherProfileRatingGameBanData(acc, profileId, coldProfileData);
			return coldProfileData;
		}

		// Token: 0x06000130 RID: 304 RVA: 0x0000BC7C File Offset: 0x00009E7C
		private void CleanObsoleteProfileData(MySqlAccessorTransaction acc, ulong profileId)
		{
			this.CleanProfileFirstWinOfDayByModeData(acc, profileId);
		}

		// Token: 0x06000131 RID: 305 RVA: 0x0000BC88 File Offset: 0x00009E88
		private void GatherEquipItemsProfileData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetProfileItems(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				EquipItemSerializer equipItemSerializer = new EquipItemSerializer();
				while (dbdataReader.Read())
				{
					SEquipItem item;
					equipItemSerializer.Deserialize(dbdataReader, out item);
					profileData.EquipItems.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_items WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000132 RID: 306 RVA: 0x0000BD28 File Offset: 0x00009F28
		private void GatherUnlockItemsProfileData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetUnlockedItems(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				UInt64FieldSerializer uint64FieldSerializer = new UInt64FieldSerializer("id");
				while (dbdataReader.Read())
				{
					ulong item;
					uint64FieldSerializer.Deserialize(dbdataReader, out item);
					profileData.UnlockItems.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_items_unlock WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000133 RID: 307 RVA: 0x0000BDD0 File Offset: 0x00009FD0
		private void GatherAchievementProfileData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetProfileAchievements(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				AchievementInfoSerializer achievementInfoSerializer = new AchievementInfoSerializer();
				while (dbdataReader.Read())
				{
					AchievementInfo item;
					achievementInfoSerializer.Deserialize(dbdataReader, out item);
					profileData.Achievements.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_achievements WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000134 RID: 308 RVA: 0x0000BE70 File Offset: 0x0000A070
		private void GatherSponsorPointsProfileData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetProfileSponsorPoints(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				SponsorPointsSerializer sponsorPointsSerializer = new SponsorPointsSerializer();
				while (dbdataReader.Read())
				{
					SSponsorPoints item;
					sponsorPointsSerializer.Deserialize(dbdataReader, out item);
					profileData.SponsorPoints.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_sponsor_points WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000135 RID: 309 RVA: 0x0000BF10 File Offset: 0x0000A110
		private void GatherPersistentSettingsProfileData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetPersistentSettings(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				PersistentSettingsSerializer persistentSettingsSerializer = new PersistentSettingsSerializer();
				while (dbdataReader.Read())
				{
					SPersistentSettings item;
					persistentSettingsSerializer.Deserialize(dbdataReader, out item);
					profileData.PersistentSettings.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM persistent_settings WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000136 RID: 310 RVA: 0x0000BFB0 File Offset: 0x0000A1B0
		private void GatherContractProfileData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetContractInfo(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				ProfileContractSerializer profileContractSerializer = new ProfileContractSerializer();
				while (dbdataReader.Read())
				{
					ProfileContract profileContract;
					profileContractSerializer.Deserialize(dbdataReader, out profileContract);
					profileData.ProfileContract = profileContract;
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_contracts WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000137 RID: 311 RVA: 0x0000C04C File Offset: 0x0000A24C
		private void GatherProfileStatisticsData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetPlayerStats(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				byte[] array = null;
				DataArchiveSerializer<PlayerStatistics> dataArchiveSerializer = new DataArchiveSerializer<PlayerStatistics>(new PlayerStatisticsDataSerializer());
				if (dbdataReader.Read())
				{
					array = (byte[])dbdataReader["stats"];
				}
				profileData.PlayerStatistics = ((array == null) ? new PlayerStatistics
				{
					Measures = new List<Measure>()
				} : dataArchiveSerializer.Deserialize(array, DBVersion.Zero));
			}
			acc.ExecuteNonQuery("DELETE FROM profile_stats WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000138 RID: 312 RVA: 0x0000C11C File Offset: 0x0000A31C
		private void GatherProfileProgressionData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetProfileProgression(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				if (dbdataReader.Read())
				{
					ProfileProgressionSerializer profileProgressionSerializer = new ProfileProgressionSerializer();
					profileProgressionSerializer.Deserialize(dbdataReader, out profileData.ProfileProgression);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_progression WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x06000139 RID: 313 RVA: 0x0000C1B0 File Offset: 0x0000A3B0
		private void GatherCustomRulesState(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetCustomRulesState(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				CustomRuleStateSerializer customRuleStateSerializer = new CustomRuleStateSerializer();
				while (dbdataReader.Read())
				{
					CustomRuleRawState item;
					customRuleStateSerializer.Deserialize(dbdataReader, out item);
					profileData.CustomRuleStates.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM custom_rules_state WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x0600013A RID: 314 RVA: 0x0000C250 File Offset: 0x0000A450
		private void GatherProfileSkillData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetSkillInfos(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				SkillInfoSerializer skillInfoSerializer = new SkillInfoSerializer();
				while (dbdataReader.Read())
				{
					SkillInfo item;
					skillInfoSerializer.Deserialize(dbdataReader, out item);
					profileData.SkillInfos.Add(item);
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_skill WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x0600013B RID: 315 RVA: 0x0000C2F0 File Offset: 0x0000A4F0
		private void GatherProfileRatingData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetPvpRatingPoints(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				if (dbdataReader.Read())
				{
					RatingInfoSerializer ratingInfoSerializer = new RatingInfoSerializer();
					RatingInfo ratingInfo;
					ratingInfoSerializer.Deserialize(dbdataReader, out ratingInfo);
					profileData.RatingInfo = ratingInfo;
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_pvp_rating WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x0600013C RID: 316 RVA: 0x0000C388 File Offset: 0x0000A588
		private void GatherProfileRatingGameBanData(MySqlAccessorTransaction acc, ulong profileId, ColdProfileData profileData)
		{
			using (DBDataReader dbdataReader = acc.ExecuteReader("CALL GetPlayerRatingUnbanTime(?pid)", new object[]
			{
				"?pid",
				profileId
			}))
			{
				if (dbdataReader.Read())
				{
					RatingGamePlayerBanInfoSerializer ratingGamePlayerBanInfoSerializer = new RatingGamePlayerBanInfoSerializer();
					RatingGamePlayerBanInfo ratingGamePlayerBan;
					ratingGamePlayerBanInfoSerializer.Deserialize(dbdataReader, out ratingGamePlayerBan);
					profileData.RatingGamePlayerBan = ratingGamePlayerBan;
				}
			}
			acc.ExecuteNonQuery("DELETE FROM profile_pvp_rating_game_ban WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x0600013D RID: 317 RVA: 0x0000C420 File Offset: 0x0000A620
		private void CleanProfileFirstWinOfDayByModeData(MySqlAccessorTransaction acc, ulong profileId)
		{
			acc.ExecuteNonQuery("DELETE FROM first_win_of_day_by_mode WHERE profile_id = ?pid", new object[]
			{
				"?pid",
				profileId
			});
		}

		// Token: 0x0600013E RID: 318 RVA: 0x0000C448 File Offset: 0x0000A648
		public DALResultMulti<ulong> GetUnusedProfiles(TimeSpan threshold, int limit)
		{
			CacheProxy.Options<ulong> options = new CacheProxy.Options<ulong>
			{
				db_serializer = new UInt64FieldSerializer("id")
			};
			options.query("CALL GetExpiredProfiles(?exptm, ?lmt)", new object[]
			{
				"?exptm",
				threshold.TotalMinutes,
				"?lmt",
				limit
			});
			return this.m_dal.CacheProxy.GetStream<ulong>(options);
		}

		// Token: 0x0600013F RID: 319 RVA: 0x0000C4B8 File Offset: 0x0000A6B8
		public DALResultMulti<ulong> GetColdProfiles(int limit)
		{
			CacheProxy.Options<ulong> options = new CacheProxy.Options<ulong>
			{
				db_serializer = new UInt64FieldSerializer("profile_id")
			};
			options.query("CALL GetColdProfiles(?lim)", new object[]
			{
				"?lim",
				limit
			});
			return this.m_dal.CacheProxy.GetStream<ulong>(options);
		}

		// Token: 0x06000140 RID: 320 RVA: 0x0000C510 File Offset: 0x0000A710
		public DALResult<ColdProfileData> GetColdProfileData(ulong profile_id, DBVersion current_schema)
		{
			ColdStorageSystem.<GetColdProfileData>c__AnonStorey5 <GetColdProfileData>c__AnonStorey = new ColdStorageSystem.<GetColdProfileData>c__AnonStorey5();
			<GetColdProfileData>c__AnonStorey.profile_id = profile_id;
			DALStats dalstats = new DALStats();
			<GetColdProfileData>c__AnonStorey.coldProfileData = null;
			using (MySqlAccessorTransaction acc = new MySqlAccessorTransaction(dalstats))
			{
				acc.Transaction(delegate()
				{
					using (DBDataReader dbdataReader = acc.ExecuteReader("SELECT cold FROM profiles WHERE id = ?pid FOR UPDATE", new object[]
					{
						"?pid",
						<GetColdProfileData>c__AnonStorey.profile_id
					}))
					{
						if (!dbdataReader.Read())
						{
							return false;
						}
						if (dbdataReader[0].ToString() != "1")
						{
							return true;
						}
					}
					DBVersion version;
					byte[] data;
					using (DBDataReader dbdataReader2 = acc.ExecuteReader("CALL LoadColdProfileData(?pid)", new object[]
					{
						"?pid",
						<GetColdProfileData>c__AnonStorey.profile_id
					}))
					{
						if (!dbdataReader2.Read())
						{
							throw new Exception(string.Format("Failed to load cold data for profile {0}", <GetColdProfileData>c__AnonStorey.profile_id));
						}
						version = DBVersion.Parse(dbdataReader2["version"].ToString());
						data = (byte[])dbdataReader2["data"];
					}
					DataArchiveSerializer<ColdProfileData> dataArchiveSerializer = new DataArchiveSerializer<ColdProfileData>(new ColdProfileDataXMLSerializer());
					<GetColdProfileData>c__AnonStorey.coldProfileData = dataArchiveSerializer.Deserialize(data, version);
					return true;
				});
			}
			return new DALResult<ColdProfileData>(<GetColdProfileData>c__AnonStorey.coldProfileData, dalstats);
		}

		// Token: 0x04000062 RID: 98
		private readonly DAL m_dal;
	}
}

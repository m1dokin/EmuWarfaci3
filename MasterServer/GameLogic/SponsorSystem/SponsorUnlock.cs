using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.Users;

namespace MasterServer.GameLogic.SponsorSystem
{
	// Token: 0x020007BE RID: 1982
	[Service]
	[Singleton]
	internal class SponsorUnlock : ServiceModule, ISponsorUnlock, IDBUpdateListener, IRewardProcessor
	{
		// Token: 0x0600289F RID: 10399 RVA: 0x000AF0DC File Offset: 0x000AD4DC
		public SponsorUnlock(IDALService dalService, IProfileItems profileItemsService, IItemCache itemCacheService)
		{
			this.m_dalService = dalService;
			this.m_profileItemsService = profileItemsService;
			this.m_itemCacheService = itemCacheService;
		}

		// Token: 0x140000AE RID: 174
		// (add) Token: 0x060028A0 RID: 10400 RVA: 0x000AF11C File Offset: 0x000AD51C
		// (remove) Token: 0x060028A1 RID: 10401 RVA: 0x000AF154 File Offset: 0x000AD554
		public event Func<UnlockItemInfo, bool> ItemUnlocked;

		// Token: 0x060028A2 RID: 10402 RVA: 0x000AF18C File Offset: 0x000AD58C
		public override void Init()
		{
			string filename = Path.Combine(Resources.GetResourcesDirectory(), "sponsors_data.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			this.m_sponsors = SponsorUnlock.ReadData(xmlDocument);
		}

		// Token: 0x060028A3 RID: 10403 RVA: 0x000AF1C2 File Offset: 0x000AD5C2
		public override void Start()
		{
			this.UpdateCache();
		}

		// Token: 0x060028A4 RID: 10404 RVA: 0x000AF1CA File Offset: 0x000AD5CA
		public bool OnDBUpdateStage(IDBUpdateService updater, DBUpdateStage stage)
		{
			return true;
		}

		// Token: 0x060028A5 RID: 10405 RVA: 0x000AF1D0 File Offset: 0x000AD5D0
		public void UpdateCache()
		{
			Dictionary<string, SItem> allItemsByName = this.m_itemCacheService.GetAllItemsByName();
			List<string> list = new List<string>();
			foreach (SponsorUnlock.SponsorData sponsorData in this.m_sponsors.Values)
			{
				foreach (KeyValuePair<string, SponsorUnlock.SponsorItem> keyValuePair in sponsorData.items)
				{
					SItem sitem;
					if (!allItemsByName.TryGetValue(keyValuePair.Key, out sitem))
					{
						list.Add(keyValuePair.Key);
						Log.Warning<string>("Sponsor unlock item '{0}' is either not exist or disabled", keyValuePair.Key);
					}
					else
					{
						keyValuePair.Value.id = sitem.ID;
					}
				}
				foreach (string key in list)
				{
					sponsorData.items.Remove(key);
				}
				list.Clear();
			}
		}

		// Token: 0x060028A6 RID: 10406 RVA: 0x000AF324 File Offset: 0x000AD724
		public void ValidateProgression(ProfileProxy profile)
		{
			IEnumerable<SSponsorPoints> sponsorPoints = profile.SponsorPoints;
			Set<ulong> set = new Set<ulong>();
			foreach (SItem sitem in profile.UnlockedItems.Values)
			{
				set.Add(sitem.ID);
			}
			foreach (KeyValuePair<uint, SponsorUnlock.SponsorData> keyValuePair in this.m_sponsors)
			{
				SponsorUnlock.SponsorData value = keyValuePair.Value;
				SSponsorPoints sp = this.FindSponsorPointsData(value.id, sponsorPoints);
				if (sp.RankInfo.RankStart == 0UL && sp.RankInfo.NextRankStart == 0UL)
				{
					SRankInfo new_sp = RankCurveUtils.CalculateRankInfo(sp.RankInfo.Points, value.stages);
					profile.SetSponsorInfo(sp.SponsorID, sp.RankInfo.Points, new_sp);
				}
				else
				{
					this.TryFixupSponsorPoints(profile, set, sp, value);
				}
				this.TryFixupNextUnlock(profile, set, sp, value);
			}
		}

		// Token: 0x060028A7 RID: 10407 RVA: 0x000AF470 File Offset: 0x000AD870
		public bool IsItemPresentInSponsors(string itemName)
		{
			foreach (SponsorUnlock.SponsorData sponsorData in this.m_sponsors.Values)
			{
				if (sponsorData.items.ContainsKey(itemName))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x060028A8 RID: 10408 RVA: 0x000AF4E8 File Offset: 0x000AD8E8
		public RewardOutputData ProcessRewardData(ulong userId, RewardProcessorState state, MissionContext missionContext, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			if (state != RewardProcessorState.Process)
			{
				return aggRewardData;
			}
			RewardOutputData output = this.RewardSponsorPoints(userId, aggRewardData, logGroup);
			return this.AddSponsorPoints(userId, output, logGroup);
		}

		// Token: 0x060028A9 RID: 10409 RVA: 0x000AF518 File Offset: 0x000AD918
		private static Dictionary<uint, SponsorUnlock.SponsorData> ReadData(XmlDocument doc)
		{
			Dictionary<uint, SponsorUnlock.SponsorData> dictionary = new Dictionary<uint, SponsorUnlock.SponsorData>();
			XmlElement documentElement = doc.DocumentElement;
			if (documentElement.Name != "sponsors_data")
			{
				throw new ServiceModuleException("Sponsors data XML is corrupted");
			}
			IEnumerator enumerator = documentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						if (xmlElement.Name == "sponsor_data")
						{
							string attribute = xmlElement.GetAttribute("id");
							uint i = uint.Parse(attribute);
							SponsorUnlock.SponsorData sponsorData = new SponsorUnlock.SponsorData(i);
							IEnumerator enumerator2 = xmlElement.ChildNodes.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									XmlNode xmlNode2 = (XmlNode)obj2;
									if (xmlNode2.NodeType == XmlNodeType.Element)
									{
										xmlElement = (xmlNode2 as XmlElement);
										if (xmlElement.Name == "stages")
										{
											sponsorData.stages.Add(0UL);
											IEnumerator enumerator3 = xmlElement.ChildNodes.GetEnumerator();
											try
											{
												while (enumerator3.MoveNext())
												{
													object obj3 = enumerator3.Current;
													XmlNode xmlNode3 = (XmlNode)obj3;
													if (xmlNode3.NodeType == XmlNodeType.Element)
													{
														XmlElement xmlElement2 = xmlNode3 as XmlElement;
														if (xmlElement2.Name.StartsWith("stage_"))
														{
															if (!xmlElement2.HasAttribute("value"))
															{
																break;
															}
															string attribute2 = xmlElement2.GetAttribute("value");
															uint num = uint.Parse(attribute2);
															sponsorData.stages.Add((ulong)num);
														}
													}
												}
											}
											finally
											{
												IDisposable disposable;
												if ((disposable = (enumerator3 as IDisposable)) != null)
												{
													disposable.Dispose();
												}
											}
											sponsorData.stages.Sort();
										}
										else if (xmlElement.Name == "sponsor_unlocks")
										{
											IEnumerator enumerator4 = xmlElement.ChildNodes.GetEnumerator();
											try
											{
												while (enumerator4.MoveNext())
												{
													object obj4 = enumerator4.Current;
													XmlNode xmlNode4 = (XmlNode)obj4;
													if (xmlNode4.NodeType == XmlNodeType.Element)
													{
														XmlElement xmlElement3 = xmlNode4 as XmlElement;
														if (xmlElement3.Name == "item")
														{
															string attribute3 = xmlElement3.GetAttribute("name");
															string attribute4 = xmlElement3.GetAttribute("type");
															SponsorUnlock.SponsorItem sponsorItem = new SponsorUnlock.SponsorItem(attribute3, attribute4);
															IEnumerator enumerator5 = xmlElement3.ChildNodes.GetEnumerator();
															try
															{
																while (enumerator5.MoveNext())
																{
																	object obj5 = enumerator5.Current;
																	XmlNode xmlNode5 = (XmlNode)obj5;
																	if (xmlNode5.NodeType == XmlNodeType.Element)
																	{
																		XmlElement xmlElement4 = xmlNode5 as XmlElement;
																		if (xmlElement4.Name.StartsWith("weight"))
																		{
																			string s = xmlElement4.Name.Substring(6);
																			int key;
																			if (int.TryParse(s, out key))
																			{
																				if (!xmlElement4.HasAttribute("value"))
																				{
																					break;
																				}
																				string attribute5 = xmlElement4.GetAttribute("value");
																				uint num2 = uint.Parse(attribute5);
																				if (num2 == 0U)
																				{
																					num2 = 1U;
																					Log.Warning<string, string>("Zero weight is present in sponsors_data.xml for item '{0}', in column '{1}'; auto-replaced on 1", attribute3, xmlElement4.Name);
																				}
																				sponsorItem.weigths.Add(key, num2);
																			}
																		}
																	}
																}
															}
															finally
															{
																IDisposable disposable2;
																if ((disposable2 = (enumerator5 as IDisposable)) != null)
																{
																	disposable2.Dispose();
																}
															}
															sponsorData.items.Add(sponsorItem.name, sponsorItem);
														}
													}
												}
											}
											finally
											{
												IDisposable disposable3;
												if ((disposable3 = (enumerator4 as IDisposable)) != null)
												{
													disposable3.Dispose();
												}
											}
										}
									}
								}
							}
							finally
							{
								IDisposable disposable4;
								if ((disposable4 = (enumerator2 as IDisposable)) != null)
								{
									disposable4.Dispose();
								}
							}
							dictionary.Add(sponsorData.id, sponsorData);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable5;
				if ((disposable5 = (enumerator as IDisposable)) != null)
				{
					disposable5.Dispose();
				}
			}
			return dictionary;
		}

		// Token: 0x060028AA RID: 10410 RVA: 0x000AF988 File Offset: 0x000ADD88
		private bool ProcessSponsorPoints(ulong profileId, uint sponsorId, ulong sponsorPoints, uint gainedSponsorPoints, ref Dictionary<ulong, int> unlockList, ref ulong nextUnlock)
		{
			Dictionary<uint, SponsorUnlock.SponsorData> sponsors = this.m_sponsors;
			Dictionary<ulong, SItem> unlockedItems = this.m_profileItemsService.GetUnlockedItems(profileId);
			Set<ulong> set = new Set<ulong>(unlockedItems.Keys);
			SponsorUnlock.SponsorData sponsorData = sponsors[sponsorId];
			ulong num = sponsorPoints + (ulong)gainedSponsorPoints;
			int num2 = 0;
			int num3 = 0;
			int num4 = sponsorData.stages.Count - 1;
			for (int i = 1; i <= num4; i++)
			{
				ulong num5 = sponsorData.stages[i];
				if (num >= num5)
				{
					num3 = i;
				}
				if (sponsorPoints >= num5)
				{
					num2 = i;
				}
			}
			for (int j = num2 + 1; j <= num3; j++)
			{
				unlockList.Add(nextUnlock, j);
				set.Add(nextUnlock);
				nextUnlock = 0UL;
				if (j != num4)
				{
					nextUnlock = this.GetRandomUnlock(sponsorId, j + 1, set);
					if (nextUnlock == 0UL)
					{
						Log.Warning<ulong, int>("Failed to get sponsor item for profile {0} on unlock stage {1}", profileId, j);
						return false;
					}
				}
			}
			if (num3 != num2 && num3 >= num4 && num2 < num4)
			{
				int num6 = 1 + this.m_dalService.RewardsSystem.GetSponsorPoints(profileId).Count((SSponsorPoints sponsor) => sponsor.RankInfo.RankStart >= sponsor.RankInfo.NextRankStart);
				IStatsTracker service = ServicesManager.GetService<IStatsTracker>();
				service.ResetStatistics(profileId, EStatsEvent.SPONSOR_PROGRESS, num6);
			}
			return true;
		}

		// Token: 0x060028AB RID: 10411 RVA: 0x000AFAE8 File Offset: 0x000ADEE8
		private SponsorUnlock.SponsorItem GetSponsorItem(uint sponsorId, ulong itemId)
		{
			return this.m_sponsors[sponsorId].items[this.m_itemCacheService.GetAllItems()[itemId].Name];
		}

		// Token: 0x060028AC RID: 10412 RVA: 0x000AFB18 File Offset: 0x000ADF18
		private ulong GetRandomUnlock(uint sponsorId, int unlockStage, Set<ulong> unlockBans)
		{
			Dictionary<uint, SponsorUnlock.SponsorData> sponsors = this.m_sponsors;
			int num = 0;
			List<SponsorUnlock.SponsorItem> list = new List<SponsorUnlock.SponsorItem>();
			SponsorUnlock.SponsorData sponsorData = sponsors[sponsorId];
			foreach (SponsorUnlock.SponsorItem sponsorItem in sponsorData.items.Values)
			{
				if (!unlockBans.Contains(sponsorItem.id) && sponsorItem.weigths[unlockStage] > 0U)
				{
					num += (int)sponsorItem.weigths[unlockStage];
					list.Add(sponsorItem);
				}
			}
			int num2 = 0;
			object itemRandom = this.m_itemRandom;
			lock (itemRandom)
			{
				num2 = this.m_itemRandom.Next(num);
			}
			foreach (SponsorUnlock.SponsorItem sponsorItem2 in list)
			{
				num2 -= (int)sponsorItem2.weigths[unlockStage];
				if (num2 <= 0)
				{
					return sponsorItem2.id;
				}
			}
			return 0UL;
		}

		// Token: 0x060028AD RID: 10413 RVA: 0x000AFC78 File Offset: 0x000AE078
		private int GetStageFromUnlocks(ProfileProxy profile, Dictionary<string, SponsorUnlock.SponsorItem> items, Set<ulong> unlocked)
		{
			Dictionary<ulong, SItem> unlockedItems = profile.UnlockedItems;
			foreach (SItem sitem in unlockedItems.Values)
			{
				unlocked.Add(sitem.ID);
			}
			Set<ulong> set = new Set<ulong>();
			foreach (SponsorUnlock.SponsorItem sponsorItem in items.Values)
			{
				set.Add(sponsorItem.id);
			}
			return unlocked.Intersect(set).Count + 1;
		}

		// Token: 0x060028AE RID: 10414 RVA: 0x000AFD4C File Offset: 0x000AE14C
		private SSponsorPoints FindSponsorPointsData(uint sponsorId, IEnumerable<SSponsorPoints> profileSponsorPoints)
		{
			SSponsorPoints ssponsorPoints = new SSponsorPoints(sponsorId);
			foreach (SSponsorPoints result in profileSponsorPoints)
			{
				if (result.SponsorID == sponsorId)
				{
					return result;
				}
			}
			return new SSponsorPoints(sponsorId);
		}

		// Token: 0x060028AF RID: 10415 RVA: 0x000AFDC0 File Offset: 0x000AE1C0
		private void TryFixupSponsorPoints(ProfileProxy profile, Set<ulong> unlocked, SSponsorPoints sp, SponsorUnlock.SponsorData data)
		{
			int stageFromUnlocks = this.GetStageFromUnlocks(profile, data.items, unlocked);
			if (stageFromUnlocks < data.stages.Count)
			{
				if (sp.RankInfo.RankStart != data.stages[stageFromUnlocks - 1] || sp.RankInfo.NextRankStart != data.stages[stageFromUnlocks] || sp.RankInfo.RankId != stageFromUnlocks || sp.RankInfo.Points > sp.RankInfo.NextRankStart || sp.RankInfo.Points < sp.RankInfo.RankStart)
				{
					SRankInfo rankInfo = sp.RankInfo;
					rankInfo.RankId = stageFromUnlocks;
					rankInfo.Points = RankCurveUtils.CalculatePoints(rankInfo, data.stages);
					rankInfo.RankStart = data.stages[stageFromUnlocks - 1];
					rankInfo.NextRankStart = data.stages[stageFromUnlocks];
					Log.Info("Fixup profile {0} sponsor {1} points {2} -> {3}, stage {4} -> {5}  (stage start {6}, next stage start {7})", new object[]
					{
						profile.ProfileID,
						data.id,
						sp.RankInfo.Points,
						rankInfo.Points,
						sp.RankInfo.RankId,
						stageFromUnlocks,
						rankInfo.RankStart,
						rankInfo.NextRankStart
					});
					if (!profile.SetSponsorInfo(sp.SponsorID, sp.RankInfo.Points, rankInfo))
					{
						Log.Warning<ulong>("Failed to fixup profile {0}", profile.ProfileID);
					}
				}
			}
			else if (stageFromUnlocks == data.stages.Count && (sp.RankInfo.RankStart != data.stages[stageFromUnlocks - 1] || sp.RankInfo.NextRankStart != data.stages[stageFromUnlocks - 1]))
			{
				SRankInfo rankInfo2 = sp.RankInfo;
				rankInfo2.RankId = stageFromUnlocks;
				rankInfo2.Points = RankCurveUtils.CalculatePoints(rankInfo2, data.stages);
				rankInfo2.RankStart = data.stages[stageFromUnlocks - 1];
				rankInfo2.NextRankStart = rankInfo2.RankStart;
				Log.Info("Fixup profile {0} sponsor {1} points {2} -> {3}, stage {4} -> {5}  (stage start {6}, next stage start {7})", new object[]
				{
					profile.ProfileID,
					data.id,
					sp.RankInfo.Points,
					rankInfo2.Points,
					sp.RankInfo.RankId,
					stageFromUnlocks,
					rankInfo2.RankStart,
					rankInfo2.NextRankStart
				});
				if (!profile.SetSponsorInfo(sp.SponsorID, sp.RankInfo.Points, rankInfo2))
				{
					Log.Warning<ulong>("Failed to fixup profile {0}", profile.ProfileID);
				}
			}
		}

		// Token: 0x060028B0 RID: 10416 RVA: 0x000B00DC File Offset: 0x000AE4DC
		private void TryFixupNextUnlock(ProfileProxy profile, Set<ulong> unlocked, SSponsorPoints sp, SponsorUnlock.SponsorData data)
		{
			if (!this.m_itemCacheService.GetAllItems().ContainsKey(sp.NextUnlockItemId) || !data.items.ContainsKey(this.m_itemCacheService.GetAllItems()[sp.NextUnlockItemId].Name) || unlocked.Contains(sp.NextUnlockItemId))
			{
				int rankId = data.CalculateStageInfo(sp.RankInfo.Points).RankId;
				if (rankId == data.stages.Count)
				{
					if (sp.NextUnlockItemId != 0UL)
					{
						profile.SetNextUnlockItem(data.id, 0UL);
					}
					return;
				}
				Set<ulong> set = new Set<ulong>();
				foreach (SponsorUnlock.SponsorItem sponsorItem in data.items.Values)
				{
					set.Add(sponsorItem.id);
				}
				Set<ulong> unlockBans = unlocked.Intersect(set);
				ulong randomUnlock = this.GetRandomUnlock(data.id, rankId, unlockBans);
				Log.Info<uint, ulong, ulong>("Performing sponsor {0} next unlock fix-up for profile {1}. Setting next unlock to {2}", data.id, profile.ProfileID, randomUnlock);
				profile.SetNextUnlockItem(data.id, randomUnlock);
			}
		}

		// Token: 0x060028B1 RID: 10417 RVA: 0x000B0234 File Offset: 0x000AE634
		private SRankInfo CalculateStageInfo(uint sponsorId, ulong sponsorPoints)
		{
			return this.m_sponsors[sponsorId].CalculateStageInfo(sponsorPoints);
		}

		// Token: 0x060028B2 RID: 10418 RVA: 0x000B0248 File Offset: 0x000AE648
		private RewardOutputData RewardSponsorPoints(ulong userId, RewardOutputData aggRewardData, ILogGroup logGroup)
		{
			ulong num = 0UL;
			ulong nextUnlockItemId = 0UL;
			foreach (SSponsorPoints ssponsorPoints in this.m_dalService.RewardsSystem.GetSponsorPoints(aggRewardData.profileId))
			{
				if (ssponsorPoints.SponsorID == aggRewardData.SponsorData.sponsorId)
				{
					num = ssponsorPoints.RankInfo.Points;
					nextUnlockItemId = ssponsorPoints.NextUnlockItemId;
					break;
				}
			}
			Dictionary<ulong, int> dictionary = new Dictionary<ulong, int>();
			List<SponsorDataUpdate.ItemIDs> list = new List<SponsorDataUpdate.ItemIDs>();
			if (this.ProcessSponsorPoints(aggRewardData.profileId, aggRewardData.SponsorData.sponsorId, num, aggRewardData.gainedSponsorPoints, ref dictionary, ref nextUnlockItemId))
			{
				foreach (KeyValuePair<ulong, int> keyValuePair in dictionary)
				{
					ulong key = keyValuePair.Key;
					int value = keyValuePair.Value;
					try
					{
						SponsorUnlock.SponsorItem sponsorItem = this.GetSponsorItem(aggRewardData.SponsorData.sponsorId, key);
						this.m_profileItemsService.UnlockItem(aggRewardData.profileId, key);
						SItem sitem = this.m_itemCacheService.GetAllItems(false)[key];
						bool flag = false;
						if (!sitem.IsAttachmentItem && this.ItemUnlocked != null)
						{
							flag |= this.ItemUnlocked(new UnlockItemInfo(aggRewardData.userId, sponsorItem.name, logGroup));
						}
						logGroup.ItemUnlockedLog(userId, aggRewardData.profileId, key, sitem.Name, LogGroup.ProduceType.Reward);
						Log.Info("Unlocked item {0} for profile {1} on step {2} for sponsor {3}", new object[]
						{
							key,
							aggRewardData.profileId,
							value,
							aggRewardData.SponsorData.sponsorId
						});
						ulong num2 = 0UL;
						SponsorUnlock.SponsorItem.RewardType rewardType = sponsorItem.rewardType;
						if (rewardType == SponsorUnlock.SponsorItem.RewardType.GIVE_PERMANENT)
						{
							num2 = ulong.MaxValue - key;
							Log.Info("Give item {0} for profile {1} on step {2} for sponsor {3}", new object[]
							{
								key,
								aggRewardData.profileId,
								value,
								aggRewardData.SponsorData.sponsorId
							});
						}
						if (num2 != 0UL)
						{
							logGroup.ItemGivenLog(userId, aggRewardData.profileId, key, num2, sitem.Name, LogGroup.ProduceType.Reward);
						}
						if (!flag)
						{
							list.Add(new SponsorDataUpdate.ItemIDs(key, num2));
						}
					}
					catch (Exception ex)
					{
						Log.Error("Failed to unlock item {0} for profile {1} sponsor {2}: {3}", new object[]
						{
							key,
							aggRewardData.profileId,
							aggRewardData.SponsorData.sponsorId,
							ex.ToString()
						});
					}
				}
				this.m_dalService.RewardsSystem.SetNextUnlockItem(aggRewardData.profileId, aggRewardData.SponsorData.sponsorId, nextUnlockItemId);
			}
			aggRewardData.SponsorData = new SponsorDataUpdate
			{
				sponsorId = aggRewardData.SponsorData.sponsorId,
				totalSponsorPoints = num + (ulong)aggRewardData.gainedSponsorPoints,
				nextUnlockItemId = nextUnlockItemId,
				unlockedItems = list
			};
			return aggRewardData;
		}

		// Token: 0x060028B3 RID: 10419 RVA: 0x000B05E0 File Offset: 0x000AE9E0
		private RewardOutputData AddSponsorPoints(ulong userId, RewardOutputData output, ILogGroup logGroup)
		{
			SSponsorPoints current_sponsor_pts = default(SSponsorPoints);
			SRankInfo stageInfo = default(SRankInfo);
			ulong profileID = output.profileId;
			ulong calculatedSponsorPts = (ulong)output.gainedSponsorPoints;
			uint gainedSponsorPts = 0U;
			uint sponsorID = output.SponsorData.sponsorId;
			Utils.Retry(delegate
			{
				IEnumerable<SSponsorPoints> sponsorPoints = this.m_dalService.RewardsSystem.GetSponsorPoints(profileID);
				current_sponsor_pts = sponsorPoints.FirstOrDefault((SSponsorPoints x) => x.SponsorID == sponsorID);
				ulong sponsorPoints2 = current_sponsor_pts.RankInfo.Points + calculatedSponsorPts;
				stageInfo = this.CalculateStageInfo(sponsorID, sponsorPoints2);
				gainedSponsorPts = (uint)Math.Min(stageInfo.Points - current_sponsor_pts.RankInfo.Points, calculatedSponsorPts);
				return this.m_dalService.RewardsSystem.SetSponsorInfo(profileID, sponsorID, current_sponsor_pts.RankInfo.Points, stageInfo);
			});
			output.gainedSponsorPoints = gainedSponsorPts;
			output.gainedSponsorPointsBooster = ((gainedSponsorPts <= 0U) ? 0U : output.gainedSponsorPointsBooster);
			if (logGroup != null)
			{
				logGroup.SponsorPointsLog(userId, profileID, sponsorID, gainedSponsorPts, current_sponsor_pts.RankInfo.RankId, stageInfo.Points, stageInfo.RankId);
			}
			return output;
		}

		// Token: 0x0400158D RID: 5517
		public const uint INVALID_SPONSOR = 4294967295U;

		// Token: 0x0400158E RID: 5518
		private readonly Random m_itemRandom = new Random(DateTime.Now.Millisecond);

		// Token: 0x0400158F RID: 5519
		private readonly IDALService m_dalService;

		// Token: 0x04001590 RID: 5520
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x04001591 RID: 5521
		private readonly IItemCache m_itemCacheService;

		// Token: 0x04001592 RID: 5522
		private Dictionary<uint, SponsorUnlock.SponsorData> m_sponsors;

		// Token: 0x020007BF RID: 1983
		private class SponsorItem
		{
			// Token: 0x060028B5 RID: 10421 RVA: 0x000B06F0 File Offset: 0x000AEAF0
			public SponsorItem(string itemName, string unlockType)
			{
				this.id = 0UL;
				this.name = itemName;
				if (!string.IsNullOrEmpty(unlockType))
				{
					this.rewardType = (SponsorUnlock.SponsorItem.RewardType)Enum.Parse(typeof(SponsorUnlock.SponsorItem.RewardType), unlockType, true);
				}
				else
				{
					this.rewardType = SponsorUnlock.SponsorItem.RewardType.UNLOCK;
				}
			}

			// Token: 0x04001595 RID: 5525
			public SponsorUnlock.SponsorItem.RewardType rewardType;

			// Token: 0x04001596 RID: 5526
			private string rewardDetails;

			// Token: 0x04001597 RID: 5527
			public ulong id;

			// Token: 0x04001598 RID: 5528
			public string name;

			// Token: 0x04001599 RID: 5529
			public Dictionary<int, uint> weigths = new Dictionary<int, uint>();

			// Token: 0x020007C0 RID: 1984
			public enum RewardType
			{
				// Token: 0x0400159B RID: 5531
				UNLOCK,
				// Token: 0x0400159C RID: 5532
				GIVE_PERMANENT
			}
		}

		// Token: 0x020007C1 RID: 1985
		private class SponsorData
		{
			// Token: 0x060028B6 RID: 10422 RVA: 0x000B0750 File Offset: 0x000AEB50
			public SponsorData(uint i)
			{
				this.id = i;
			}

			// Token: 0x060028B7 RID: 10423 RVA: 0x000B0775 File Offset: 0x000AEB75
			public SRankInfo CalculateStageInfo(ulong points)
			{
				return RankCurveUtils.CalculateRankInfo(points, this.stages);
			}

			// Token: 0x0400159D RID: 5533
			public uint id;

			// Token: 0x0400159E RID: 5534
			public List<ulong> stages = new List<ulong>();

			// Token: 0x0400159F RID: 5535
			public Dictionary<string, SponsorUnlock.SponsorItem> items = new Dictionary<string, SponsorUnlock.SponsorItem>();
		}
	}
}

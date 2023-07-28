using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services.Regions;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.PlayerStats;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.PunishmentSystem;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SponsorSystem;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.Payment;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using Ninject;

namespace MasterServer.MySqlQueries
{
	// Token: 0x02000683 RID: 1667
	[QueryAttributes(TagName = "join_channel", QoSClass = "login")]
	internal class JoinChannelQuery : BaseChannelQuery
	{
		// Token: 0x06002311 RID: 8977 RVA: 0x000941C0 File Offset: 0x000925C0
		public JoinChannelQuery(ISessionInfoService sessionInfoService, IAuthService authService, IColdStorageService coldStorageService, IDALService dalService, IProfileProgressionService profileProgressionService, IUserRepository userRepository, IPlayerStatsService playerStatsService, IItemsReimbursement itemsReimbursement, IItemsPurchase itemsPurchase, ISponsorUnlock sponsorUnlock, IItemsExpiration itemsExpiration, IItemsValidator itemsValidator, IFriendsService friendsService, INotificationService notificationService, IAbuseReportService abuseReportService, IOnlineVariables onlineVariables, ILogService logService, IPunishmentService punishmentService, IRankSystem rankSystem, [Optional] IPaymentService paymentService, INicknameProvider nicknameProvider, IExternalNicknameSyncService externalNicknameSyncService, IClientVersionsManagementService clientVersionsManagementService, IRegionsService regionsService, IRatingSeasonService ratingSeasonService, IProfileValidationService profileValidationService) : base(sessionInfoService, dalService, profileProgressionService, userRepository, logService, rankSystem, onlineVariables, paymentService, abuseReportService, itemsPurchase, sponsorUnlock, clientVersionsManagementService, regionsService)
		{
			this.m_coldStorageService = coldStorageService;
			this.m_playerStatsService = playerStatsService;
			this.m_itemsReimbursement = itemsReimbursement;
			this.m_itemsExpiration = itemsExpiration;
			this.m_itemsValidator = itemsValidator;
			this.m_friendsService = friendsService;
			this.m_notificationService = notificationService;
			this.m_punishmentService = punishmentService;
			this.m_nicknameProvider = nicknameProvider;
			this.m_externalNicknameSyncService = externalNicknameSyncService;
			this.m_authService = authService;
			this.m_ratingSeasonService = ratingSeasonService;
			this.m_profileValidationService = profileValidationService;
		}

		// Token: 0x06002312 RID: 8978 RVA: 0x00094254 File Offset: 0x00092654
		public override async Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "JoinChannelQuery"))
			{
				string fromJid = query.online_id;
				if (!request.HasAttribute("profile_id") || !request.HasAttribute("user_id") || !request.HasAttribute("version"))
				{
					Log.Error("Join channel, invalid query parameters");
					result = -1;
				}
				else if (!this.CheckUserLimit())
				{
					Log.Warning<string>("Too many users on the channel, reject join for {0}", fromJid);
					result = 4;
				}
				else
				{
					string regionId = request.GetAttribute("region_id");
					ulong userId = ulong.Parse(request.GetAttribute("user_id"));
					if (!base.CheckRegionId(userId, regionId))
					{
						result = -1;
					}
					else
					{
						ulong profileId = ulong.Parse(request.GetAttribute("profile_id"));
						string buildType = request.GetAttribute("build_type");
						string hardwareId = request.GetAttribute("hw_id");
						uint cpuVendor = uint.Parse(request.GetAttribute("cpu_vendor"));
						uint cpuFamily = uint.Parse(request.GetAttribute("cpu_family"));
						uint cpuModel = uint.Parse(request.GetAttribute("cpu_model"));
						uint cpuStepping = uint.Parse(request.GetAttribute("cpu_stepping"));
						uint cpuSpeed = uint.Parse(request.GetAttribute("cpu_speed"));
						uint cpuNumCores = uint.Parse(request.GetAttribute("cpu_num_cores"));
						uint gpuVendorId = uint.Parse(request.GetAttribute("gpu_vendor_id"));
						uint gpuDeviceId = uint.Parse(request.GetAttribute("gpu_device_id"));
						uint physicalMemory = uint.Parse(request.GetAttribute("physical_memory"));
						uint os64 = uint.Parse(request.GetAttribute("os_64"));
						uint osVer = uint.Parse(request.GetAttribute("os_ver"));
						SessionInfo sessionInfo = await this.m_sessionInfoService.GetSessionInfoByOnlineIdAsync(fromJid, 3);
						if (!base.CheckBootstrap(sessionInfo))
						{
							Log.Warning<ulong, UserTags>("User {0} has invalid bootstrap configuration '{1}'", userId, sessionInfo.Tags);
							result = 2;
						}
						else if (sessionInfo.UserID == 0UL)
						{
							result = 8;
						}
						else if (sessionInfo.UserID != userId)
						{
							Log.Error<ulong, string>("Join channel, user authentication failed for user {0} from {1}", userId, fromJid);
							result = -1;
						}
						else
						{
							ClientVersion version;
							ClientVersion.TryParse(request.GetAttribute("version"), out version);
							if (!this.m_clientVersionsManagementService.Validate(version))
							{
								Log.Warning<ulong, ClientVersion>("User {0} has unsupported client version {1}", userId, version);
								result = 2;
							}
							else
							{
								TouchProfileResult cold_storage_result = this.m_coldStorageService.TouchProfile(profileId, Resources.LatestDbUpdateVersion);
								if (cold_storage_result.Status == ETouchProfileResult.NotExists)
								{
									Log.Info<ulong>("Profile {0} does not exist in game database", profileId);
									result = 1;
								}
								else
								{
									base.FlushCachedProfileData(userId, profileId, false);
									string nickname;
									try
									{
										nickname = this.m_nicknameProvider.GetNickname(userId, profileId);
									}
									catch (NicknameProviderException e)
									{
										Log.Error(e);
										return 6;
									}
									if (!this.m_externalNicknameSyncService.SyncNickname(profileId, nickname))
									{
										Log.Warning<ulong>("Can't resolve nickname collision while User {0} joining channel", userId);
										result = 7;
									}
									else
									{
										SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
										if (profileInfo.UserID != userId)
										{
											Log.Warning<ulong, ulong, string>("Failed to join user {0} which sent incorrect profile id {1}, from {2}", userId, profileId, fromJid);
											result = 1;
										}
										else
										{
											List<SUserAccessLevel> accessLevel = this.m_authService.GetAccessLevel(userId);
											SUserAccessLevel userAccessLevel = SUserAccessLevel.GetUserAccessLevel(userId, sessionInfo.IPAddress, accessLevel);
											ProfileProgressionInfo progression = this.m_profileProgressionService.GetProgression(profileId);
											UserInfo.User userInfo = this.m_userRepository.Make(profileId, fromJid, string.Empty, sessionInfo.IPAddress, buildType, regionId, profileInfo, userAccessLevel.accessLevel, sessionInfo.LoginTime, progression, version);
											ProfileProxy profile = new ProfileProxy(userInfo);
											ProfileReader profReader = new ProfileReader(profile);
											this.CheckCurrentClass(profile);
											this.CheckCurrentHead(profileInfo);
											if (!base.CheckRankRestrictions(profile))
											{
												Log.Warning<ulong, int>("Failed to join user {0} with incorrect rank {1}", userId, profile.ProfileInfo.RankInfo.RankId);
												result = 5;
											}
											else if (!this.CheckBanned(profile))
											{
												Log.Error<ulong, string>("User {0} banned, from {1}", userId, fromJid);
												result = 3;
											}
											else
											{
												using (UserLoginContext userLoginContext = new UserLoginContext(this.m_userRepository, userInfo, ELoginType.Ordinary, cold_storage_result.PreviousLastSeenTime))
												{
													this.m_playerStatsService.InitPlayerStats(profileId);
													List<string> list = this.m_itemsReimbursement.ProcessReimbursement(userId, profileId);
													this.m_itemsPurchase.SyncProfileItemsWithCatalog(profile);
													this.m_sponsorUnlock.ValidateProgression(profile);
													this.m_itemsExpiration.ExpireItemsByDate(profile);
													try
													{
														this.m_itemsValidator.CheckProfileItems(profile.ProfileItems, profile.ProfileID);
													}
													catch (ValidationException e2)
													{
														Log.Error(e2);
														DefaultProfile.ResetProfileItems(profile, DefaultProfile.ResetType.ResetToDefault);
													}
													XmlElement xmlElement = response.OwnerDocument.CreateElement("character");
													response.AppendChild(xmlElement);
													profReader.FillCharacterInfo(xmlElement);
													profReader.FillProfileTags(xmlElement);
													profReader.ReadProfileMoney(xmlElement);
													profReader.ReadProfileItems(xmlElement);
													profReader.ReadExpiredItems(xmlElement);
													profReader.ReadUnlockedItems(xmlElement);
													profReader.ReadSponsorInfo(xmlElement);
													profReader.ReadChatChannels(xmlElement);
													profReader.ReadProgression(xmlElement);
													profReader.ReadLoginBonusProgress(xmlElement);
													this.m_friendsService.SendFriendListUpdate(profileId, fromJid);
													foreach (string data in list)
													{
														SNotification snotification = new SNotification
														{
															Type = ENotificationType.Message,
															ConfirmationType = EConfirmationType.None,
															ExpirationTimeUTC = DateTime.UtcNow,
															Data = Utils.CreateByteArrayFromType<string>(data)
														};
														xmlElement.AppendChild(snotification.ToXml(this.m_notificationService, xmlElement.OwnerDocument));
													}
													profReader.ReadPendingNotifications(xmlElement);
													this.m_onlineVariables.WriteVariables(xmlElement, OnlineVariableDestination.Client);
													TimeSpan totalOnlineTime = this.m_abuseReportService.GetTotalOnlineTime(userInfo.ProfileID, true);
													ulong rankPoints = ulong.Parse(xmlElement.GetAttribute("experience"));
													ulong gameMoney = ulong.Parse(xmlElement.GetAttribute("game_money"));
													ulong cryMoney = ulong.Parse(xmlElement.GetAttribute("cry_money"));
													ulong crownMoney = ulong.Parse(xmlElement.GetAttribute("crown_money"));
													Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(profileId);
													this.m_logService.Event.CharacterLoginLog(profile.UserID, profile.UserInfo.IP, profile.ProfileID, profile.Nickname, profile.ProfileInfo.RankInfo.RankId, rankPoints, playerRating.Level, playerRating.SeasonId, profile.ProfileInfo.CreateTime, totalOnlineTime, gameMoney, cryMoney, crownMoney, fromJid, string.Empty, sessionInfo.Tags.ToString(), hardwareId, cpuVendor, cpuFamily, cpuModel, cpuStepping, cpuSpeed, cpuNumCores, gpuVendorId, gpuDeviceId, physicalMemory, version.ToString(), regionId, os64, osVer);
													if (this.m_paymentService != null)
													{
														try
														{
															ulong money = this.m_paymentService.GetMoney(userId);
															base.Manager.Request("update_cry_money", fromJid, new object[]
															{
																money
															});
														}
														catch (PaymentServiceException e3)
														{
															Log.Error(e3);
														}
													}
													if (!userLoginContext.Commit())
													{
														result = -1;
													}
													else
													{
														result = 0;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x06002313 RID: 8979 RVA: 0x000942A1 File Offset: 0x000926A1
		private bool CheckUserLimit()
		{
			return !this.m_userRepository.IsJoinUsersLimitReached();
		}

		// Token: 0x06002314 RID: 8980 RVA: 0x000942B1 File Offset: 0x000926B1
		private bool CheckBanned(ProfileProxy profile)
		{
			return !this.m_punishmentService.IsBanned(profile);
		}

		// Token: 0x06002315 RID: 8981 RVA: 0x000942C4 File Offset: 0x000926C4
		private void CheckCurrentClass(ProfileProxy profile)
		{
			if (!profile.UserInfo.ProfileProgression.IsClassUnlocked((int)profile.ProfileInfo.CurrentClass))
			{
				this.m_dalService.ProfileSystem.SetProfileCurClass(profile.ProfileID, SProfileInfo.DEFAULT_CLASS_ID);
			}
		}

		// Token: 0x06002316 RID: 8982 RVA: 0x0009430F File Offset: 0x0009270F
		private void CheckCurrentHead(SProfileInfo profile)
		{
			if (!this.m_profileValidationService.IsHeadValid(profile.Head))
			{
				this.m_dalService.ProfileSystem.UpdateProfileHead(profile.Id, string.Empty);
			}
		}

		// Token: 0x040011BC RID: 4540
		private const int E_INVALID_NICKNAME = 6;

		// Token: 0x040011BD RID: 4541
		private const int E_INTERNAL_NICKNAME_COLLISION = 7;

		// Token: 0x040011BE RID: 4542
		private const int E_SESSION_INFO_REQUEST_FAILED = 8;

		// Token: 0x040011BF RID: 4543
		private const int GET_SESSION_INFO_RETRIES = 3;

		// Token: 0x040011C0 RID: 4544
		private readonly IColdStorageService m_coldStorageService;

		// Token: 0x040011C1 RID: 4545
		private readonly IPlayerStatsService m_playerStatsService;

		// Token: 0x040011C2 RID: 4546
		private readonly IItemsReimbursement m_itemsReimbursement;

		// Token: 0x040011C3 RID: 4547
		private readonly IItemsExpiration m_itemsExpiration;

		// Token: 0x040011C4 RID: 4548
		private readonly IItemsValidator m_itemsValidator;

		// Token: 0x040011C5 RID: 4549
		private readonly IFriendsService m_friendsService;

		// Token: 0x040011C6 RID: 4550
		private readonly INotificationService m_notificationService;

		// Token: 0x040011C7 RID: 4551
		private readonly IPunishmentService m_punishmentService;

		// Token: 0x040011C8 RID: 4552
		private readonly INicknameProvider m_nicknameProvider;

		// Token: 0x040011C9 RID: 4553
		private readonly IExternalNicknameSyncService m_externalNicknameSyncService;

		// Token: 0x040011CA RID: 4554
		private readonly IAuthService m_authService;

		// Token: 0x040011CB RID: 4555
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x040011CC RID: 4556
		private readonly IProfileValidationService m_profileValidationService;
	}
}

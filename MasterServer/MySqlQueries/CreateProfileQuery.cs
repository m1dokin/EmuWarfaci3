using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Services.Regions;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.GameInterface;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SponsorSystem;
using MasterServer.Platform.Nickname;
using MasterServer.Platform.Payment;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Platform.ProfanityCheck;
using MasterServer.Users;
using Ninject;

namespace MasterServer.MySqlQueries
{
	// Token: 0x02000682 RID: 1666
	[QueryAttributes(TagName = "create_profile")]
	internal class CreateProfileQuery : BaseChannelQuery
	{
		// Token: 0x0600230F RID: 8975 RVA: 0x000934C8 File Offset: 0x000918C8
		public CreateProfileQuery(ISessionInfoService sessionInfoService, IDALService dalService, IProfileProgressionService profileProgressionService, IUserRepository userRepository, ILogService logService, IRankSystem rankSystem, IOnlineVariables onlineVariables, [Optional] IPaymentService paymentService, IProfileValidationService profileValidationService, IProfanityCheckService profanityCheckService, IAbuseReportService abuseReportService, IItemsPurchase itemsPurchase, ISponsorUnlock sponsorUnlock, IProfileItems profileItemsService, INicknameProvider nicknameProvider, INicknameReservationService nicknameReservation, IExternalNicknameSyncService externalNicknameSyncService, IClientVersionsManagementService clientVersionsManagementService, IRegionsService regionsService, IRatingSeasonService ratingSeasonService) : base(sessionInfoService, dalService, profileProgressionService, userRepository, logService, rankSystem, onlineVariables, paymentService, abuseReportService, itemsPurchase, sponsorUnlock, clientVersionsManagementService, regionsService)
		{
			this.m_profileValidationService = profileValidationService;
			this.m_profanityCheckService = profanityCheckService;
			this.m_profileItemsService = profileItemsService;
			this.m_nicknameProvider = nicknameProvider;
			this.m_nicknameReservationService = nicknameReservation;
			this.m_externalNicknameSyncService = externalNicknameSyncService;
			this.m_ratingSeasonService = ratingSeasonService;
		}

		// Token: 0x06002310 RID: 8976 RVA: 0x0009352C File Offset: 0x0009192C
		public override async Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "CreateProfileQuery"))
			{
				string fromJid = query.online_id;
				if (!request.HasAttribute("user_id") || !request.HasAttribute("token") || !request.HasAttribute("nickname") || !request.HasAttribute("version"))
				{
					Log.Error("Create profile, invalid query parameters");
					result = -1;
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
						string token = request.GetAttribute("token");
						string nickname = request.GetAttribute("nickname");
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
						string head = (!request.HasAttribute("head")) ? string.Empty : request.GetAttribute("head");
						ulong profileId = 0UL;
						if (Resources.DebugQueriesEnabled)
						{
							profileId = ((!request.HasAttribute("profile_id")) ? 0UL : ulong.Parse(request.GetAttribute("profile_id")));
						}
						SessionInfo sessionInfo = await this.m_sessionInfoService.GetSessionInfoByOnlineIdAsync(fromJid);
						if (!base.CheckBootstrap(sessionInfo))
						{
							Log.Warning<ulong, UserTags>("User {0} has invalid bootstrap configuration '{1}'", userId, sessionInfo.Tags);
							result = 4;
						}
						else if (sessionInfo.UserID != userId)
						{
							Log.Error<ulong, string>("Create profile, user authentication failed for user {0} from {1}", userId, fromJid);
							result = -1;
						}
						else
						{
							ClientVersion version;
							ClientVersion.TryParse(request.GetAttribute("version"), out version);
							if (!this.m_clientVersionsManagementService.Validate(version))
							{
								Log.Warning<ulong, ClientVersion>("User {0} has unsupported client version {1}", userId, version);
								result = 4;
							}
							else
							{
								try
								{
									string nickname2 = this.m_nicknameProvider.GetNickname(userId, profileId);
									if (!string.IsNullOrEmpty(nickname2))
									{
										nickname = nickname2;
									}
								}
								catch (NicknameProviderException e)
								{
									Log.Error(e);
									return 2;
								}
								if (this.m_profileValidationService.ValidateNickname(nickname) != NameValidationResult.NoError)
								{
									Log.Error<ulong>("Failed to create profile for user {0}: invalid nickname", userId);
									result = 2;
								}
								else
								{
									ProfanityCheckResult profanityCheckResult = this.m_profanityCheckService.CheckProfileName(userId, nickname);
									if (profanityCheckResult != ProfanityCheckResult.Failed)
									{
										if (profanityCheckResult != ProfanityCheckResult.Reserved)
										{
											NameReservationResult nameReservationResult = this.m_nicknameReservationService.ReserveNickname(userId, nickname);
											if (nameReservationResult != NameReservationResult.OK)
											{
												Log.Warning<ulong, NameReservationResult>("Failed to reserve nickname for user {0}: {1}", userId, nameReservationResult);
												result = ((nameReservationResult != NameReservationResult.ALREADY_EXIST) ? -1 : 1);
											}
											else if (!this.m_externalNicknameSyncService.SyncNickname(profileId, nickname))
											{
												Log.Warning<ulong>("Failed to create profile for user {0}: internal nickname collision can't be resolved", userId);
												result = 7;
											}
											else if (!this.m_profileValidationService.IsHeadValid(head))
											{
												Log.Error<ulong, string>("Failed to create profile for user {0}: invalid head {1}", userId, head);
												result = 8;
											}
											else
											{
												if (profileId != 0UL)
												{
													this.m_dalService.ProfileSystem.CreateProfile(profileId, userId, nickname);
												}
												else
												{
													profileId = this.m_dalService.ProfileSystem.CreateProfile(userId, nickname, head);
												}
												if (profileId == 0UL)
												{
													Log.Error<string, ulong>("Failed to create profile '{0}' for user {1}", nickname, userId);
													result = 1;
												}
												else
												{
													SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
													if (string.IsNullOrEmpty(profileInfo.Nickname))
													{
														Log.Warning<ulong>("Profile {0} failed to get created profile info", profileId);
														result = -1;
													}
													else
													{
														this.m_profileItemsService.AddDefaultItems(profileId);
														ProfileProgressionInfo progression = this.m_profileProgressionService.InitProgression(profileId);
														UserInfo.User userInfo = this.m_userRepository.Make(profileId, fromJid, token, sessionInfo.IPAddress, buildType, regionId, profileInfo, AccessLevel.Basic, sessionInfo.LoginTime, progression, version);
														ProfileProxy profile = new ProfileProxy(userInfo);
														ProfileReader profReader = new ProfileReader(profile);
														using (UserLoginContext userLoginContext = new UserLoginContext(this.m_userRepository, userInfo, ELoginType.NewProfile, DateTime.MinValue))
														{
															this.m_itemsPurchase.SyncProfileItemsWithCatalog(profile);
															this.m_sponsorUnlock.ValidateProgression(profile);
															response.SetAttribute("profile_id", profileId.ToString());
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
															this.m_onlineVariables.WriteVariables(xmlElement, OnlineVariableDestination.Client);
															this.m_logService.Event.CharacterCreationLog(userId, sessionInfo.IPAddress, profileId, nickname);
															Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(profileId);
															Dictionary<Currency, ulong> dictionary = new Dictionary<Currency, ulong>
															{
																{
																	Currency.GameMoney,
																	0UL
																},
																{
																	Currency.CryMoney,
																	0UL
																},
																{
																	Currency.CrownMoney,
																	0UL
																}
															};
															try
															{
																foreach (CustomerAccount customerAccount in profile.Accounts)
																{
																	dictionary[customerAccount.Currency] = customerAccount.Money;
																}
															}
															catch (PaymentServiceException e2)
															{
																Log.Error(e2);
															}
															this.m_logService.Event.CharacterLoginLog(profile.UserID, profile.UserInfo.IP, profile.ProfileID, profile.Nickname, profile.ProfileInfo.RankInfo.RankId, profile.ProfileInfo.RankInfo.Points, playerRating.Level, playerRating.SeasonId, profile.ProfileInfo.CreateTime, TimeSpan.Zero, dictionary[Currency.GameMoney], dictionary[Currency.CryMoney], dictionary[Currency.CrownMoney], fromJid, token, sessionInfo.Tags.ToString(), hardwareId, cpuVendor, cpuFamily, cpuModel, cpuStepping, cpuSpeed, cpuNumCores, gpuVendorId, gpuDeviceId, physicalMemory, version.ToString(), regionId, os64, osVer);
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
										else
										{
											Log.Error<ulong>("Failed to create profile for user {0}: nickname reserved", userId);
											result = 3;
										}
									}
									else
									{
										Log.Error<ulong>("Failed to create profile for user {0}: invalid nickname", userId);
										result = 2;
									}
								}
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040011AF RID: 4527
		private const int E_ALLREADY_EXIST = 1;

		// Token: 0x040011B0 RID: 4528
		private const int E_INVALID_NICKNAME = 2;

		// Token: 0x040011B1 RID: 4529
		private const int E_RESERVED_NICKNAME = 3;

		// Token: 0x040011B2 RID: 4530
		private new const int E_VERSION_MISMATCH = 4;

		// Token: 0x040011B3 RID: 4531
		private const int E_INTERNAL_NICKNAME_COLLISION = 7;

		// Token: 0x040011B4 RID: 4532
		private const int E_INVALID_HEAD = 8;

		// Token: 0x040011B5 RID: 4533
		private readonly IProfileValidationService m_profileValidationService;

		// Token: 0x040011B6 RID: 4534
		private readonly IProfanityCheckService m_profanityCheckService;

		// Token: 0x040011B7 RID: 4535
		private readonly IProfileItems m_profileItemsService;

		// Token: 0x040011B8 RID: 4536
		private readonly INicknameProvider m_nicknameProvider;

		// Token: 0x040011B9 RID: 4537
		private readonly INicknameReservationService m_nicknameReservationService;

		// Token: 0x040011BA RID: 4538
		private readonly IExternalNicknameSyncService m_externalNicknameSyncService;

		// Token: 0x040011BB RID: 4539
		private readonly IRatingSeasonService m_ratingSeasonService;
	}
}

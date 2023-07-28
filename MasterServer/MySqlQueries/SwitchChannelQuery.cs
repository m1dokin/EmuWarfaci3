using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Services.Regions;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.SponsorSystem;
using MasterServer.Platform.Payment;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;
using Ninject;

namespace MasterServer.MySqlQueries
{
	// Token: 0x0200067D RID: 1661
	[QueryAttributes(TagName = "switch_channel")]
	internal class SwitchChannelQuery : BaseChannelQuery
	{
		// Token: 0x06002304 RID: 8964 RVA: 0x00092824 File Offset: 0x00090C24
		public SwitchChannelQuery(ISessionInfoService sessionInfoService, IAuthService authService, IDALService dalService, IProfileProgressionService profileProgressionService, IUserRepository userRepository, ILogService logService, IRankSystem rankSystem, IOnlineVariables onlineVariables, [Optional] IPaymentService paymentService, IAbuseReportService abuseReportService, IItemsPurchase itemsPurchase, ISponsorUnlock sponsorUnlock, IClientVersionsManagementService clientVersionsManagementService, IRegionsService regionsService) : base(sessionInfoService, dalService, profileProgressionService, userRepository, logService, rankSystem, onlineVariables, paymentService, abuseReportService, itemsPurchase, sponsorUnlock, clientVersionsManagementService, regionsService)
		{
			this.m_authService = authService;
		}

		// Token: 0x06002305 RID: 8965 RVA: 0x00092858 File Offset: 0x00090C58
		public override async Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SwitchChannelQuery"))
			{
				string fromJid = query.online_id;
				if (!request.HasAttribute("profile_id") || !request.HasAttribute("user_id") || !request.HasAttribute("version"))
				{
					Log.Error("Switch channel, invalid query parameters");
					result = -1;
				}
				else if (this.m_userRepository.IsOnlineUsersLimitReached())
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
						SessionInfo sessionInfo = await this.m_sessionInfoService.GetSessionInfoByOnlineIdAsync(fromJid);
						if (!base.CheckBootstrap(sessionInfo))
						{
							Log.Warning<ulong, UserTags>("User {0} has invalid bootstrap configuration '{1}'", userId, sessionInfo.Tags);
							result = 2;
						}
						else if (sessionInfo.UserID != userId)
						{
							Log.Error<ulong, string>("Switch channel, user authentication failed for user {0} from {1}", userId, fromJid);
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
								base.FlushCachedProfileData(userId, profileId, true);
								SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
								if (string.IsNullOrEmpty(profileInfo.Nickname))
								{
									Log.Info<ulong>("Profile {0} does not exist in game database", profileId);
									result = 1;
								}
								else if (profileInfo.UserID != userId)
								{
									Log.Warning<ulong, ulong, string>("Failed to switch channel for user {0} which sent incorrect profile id {1}, from {2}", userId, profileId, fromJid);
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
									if (!base.CheckRankRestrictions(profile))
									{
										Log.Warning<ulong, int>("Failed to join user {0} with incorrect rank {1}", userId, profile.ProfileInfo.RankInfo.RankId);
										result = 5;
									}
									else
									{
										using (UserLoginContext userLoginContext = new UserLoginContext(this.m_userRepository, userInfo, ELoginType.SwitchChannel, DateTime.Now))
										{
											XmlElement xmlElement = response.OwnerDocument.CreateElement("character");
											response.AppendChild(xmlElement);
											profReader.FillCharacterInfo(xmlElement);
											profReader.ReadChatChannels(xmlElement);
											profReader.ReadPendingNotifications(xmlElement);
											profReader.ReadProgression(xmlElement);
											this.m_onlineVariables.WriteVariables(xmlElement, OnlineVariableDestination.Client);
											this.m_logService.Event.ChannelSwitch(userId, profileId, fromJid, string.Empty, sessionInfo.Tags.ToString());
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
												catch (PaymentServiceException e)
												{
													Log.Error(e);
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
			return result;
		}

		// Token: 0x040011A5 RID: 4517
		private readonly IAuthService m_authService;
	}
}

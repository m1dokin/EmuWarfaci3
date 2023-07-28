using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameLogic.PerformanceSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000262 RID: 610
	internal class ClanDebugCommands
	{
		// Token: 0x02000263 RID: 611
		[ConsoleCmdAttributes(CmdName = "clan_info", ArgsSize = 1, Help = "Clan info by profileId")]
		public class ClanInfoCmd : IConsoleCmd
		{
			// Token: 0x06000D61 RID: 3425 RVA: 0x00035392 File Offset: 0x00033792
			public ClanInfoCmd(IClanService clanService)
			{
				this.m_clanService = clanService;
			}

			// Token: 0x06000D62 RID: 3426 RVA: 0x000353A4 File Offset: 0x000337A4
			public void ExecuteCmd(string[] args)
			{
				Log.Info("Clan info");
				ulong num = ulong.Parse(args[1]);
				ClanMember memberInfo = this.m_clanService.GetMemberInfo(num);
				if (memberInfo != null)
				{
					ClanInfo clanInfo = this.m_clanService.GetClanInfo(memberInfo.ClanID);
					Log.Info("ID: {0}, Name: {1}, MembersCount: {2}, ClanPoints: {3}", new object[]
					{
						clanInfo.ClanID,
						clanInfo.Name,
						clanInfo.MembersCount,
						clanInfo.ClanPoints
					});
					foreach (ClanMember clanMember in this.m_clanService.GetClanMembers(memberInfo.ClanID))
					{
						Log.Info<string, ulong, EClanRole>("Nickname {0}, ClanPoints: {1}, ClanRole: {2}", clanMember.Nickname, clanMember.ClanPoints, clanMember.ClanRole);
					}
				}
				else
				{
					Log.Info<ulong>("User with profileId {0} not clan member", num);
				}
			}

			// Token: 0x04000628 RID: 1576
			private readonly IClanService m_clanService;
		}

		// Token: 0x02000264 RID: 612
		[ConsoleCmdAttributes(CmdName = "clan_add_clanpoints", ArgsSize = 2, Help = "Add clan points to clan by profile ID")]
		public class ClanAddPointsCmd : IConsoleCmd
		{
			// Token: 0x06000D63 RID: 3427 RVA: 0x000354B0 File Offset: 0x000338B0
			public ClanAddPointsCmd(IClanService clanService)
			{
				this.m_clanService = clanService;
			}

			// Token: 0x06000D64 RID: 3428 RVA: 0x000354C0 File Offset: 0x000338C0
			public void ExecuteCmd(string[] args)
			{
				ulong target_id = ulong.Parse(args[1]);
				this.m_clanService.AddClanPoints(target_id, ulong.Parse(args[2]));
				Log.Info("Add Clanpoints done");
			}

			// Token: 0x04000629 RID: 1577
			private readonly IClanService m_clanService;
		}

		// Token: 0x02000265 RID: 613
		[ConsoleCmdAttributes(CmdName = "clan_fixup", ArgsSize = 1, Help = "Fix clan data by clan ID")]
		public class ClanValidateCmd : IConsoleCmd
		{
			// Token: 0x06000D65 RID: 3429 RVA: 0x000354F4 File Offset: 0x000338F4
			public ClanValidateCmd(IDALService dalService)
			{
				this.m_dalService = dalService;
			}

			// Token: 0x06000D66 RID: 3430 RVA: 0x00035504 File Offset: 0x00033904
			public void ExecuteCmd(string[] args)
			{
				if (args.Length == 1)
				{
					uint p = this.m_dalService.ClanSystem.FixupClans();
					Log.Info<uint>("Clans fixed: {0}", p);
				}
				else
				{
					bool flag = this.m_dalService.ClanSystem.FixupClan(ulong.Parse(args[1]));
					Log.Info((!flag) ? "Nothing to fix" : "Clan has been fixed");
				}
			}

			// Token: 0x0400062A RID: 1578
			private readonly IDALService m_dalService;
		}

		// Token: 0x02000266 RID: 614
		[DebugCommand]
		[ConsoleCmdAttributes(CmdName = "debug_clan_generate", ArgsSize = 1, Help = "Generate N fake clans. Max(N) = 9999 clans")]
		public class ClanGenerateCmd : IConsoleCmd
		{
			// Token: 0x06000D67 RID: 3431 RVA: 0x0003556E File Offset: 0x0003396E
			public ClanGenerateCmd(IDALService dalService)
			{
				this.m_dalService = dalService;
			}

			// Token: 0x06000D68 RID: 3432 RVA: 0x00035580 File Offset: 0x00033980
			public void ExecuteCmd(string[] args)
			{
				uint num = uint.Parse(args[1]);
				if (num > 9999U)
				{
					Log.Warning<uint>("It's impossible to create {0} clans. See the command definition", num);
					return;
				}
				Log.Info("Clan generation started");
				this.m_dalService.ClanSystem.DebugGenerateClans(num);
				Log.Info("Clan generation finished");
			}

			// Token: 0x0400062B RID: 1579
			private readonly IDALService m_dalService;

			// Token: 0x0400062C RID: 1580
			private const uint maxClansToCreate = 9999U;
		}

		// Token: 0x02000267 RID: 615
		[ConsoleCmdAttributes(CmdName = "clan_create", ArgsSize = 3, Help = "Create clan, params profileId, clan name, clan description (optional)")]
		public class ClanCreateCmd : IConsoleCmd
		{
			// Token: 0x06000D69 RID: 3433 RVA: 0x000355D2 File Offset: 0x000339D2
			public ClanCreateCmd(IClanService clanService)
			{
				this.m_clanService = clanService;
			}

			// Token: 0x06000D6A RID: 3434 RVA: 0x000355E4 File Offset: 0x000339E4
			public void ExecuteCmd(string[] args)
			{
				ulong clanOwnerId = ulong.Parse(args[1]);
				string clanName = args[2];
				string description = Convert.ToBase64String(Encoding.UTF8.GetBytes((args.Length <= 3) ? string.Empty : args[3]));
				ulong num = 0UL;
				EClanCreationStatus p = this.m_clanService.CreateClan(clanOwnerId, ref num, clanName, description);
				Log.Info<EClanCreationStatus>("Create clan : {0}", p);
			}

			// Token: 0x0400062D RID: 1581
			private readonly IClanService m_clanService;
		}

		// Token: 0x02000268 RID: 616
		[ConsoleCmdAttributes(CmdName = "clan_invite", ArgsSize = 3, Help = "Invite to clan, params initiator pid, target pid [, target nickname]")]
		public class ClanInviteCmd : IConsoleCmd
		{
			// Token: 0x06000D6B RID: 3435 RVA: 0x00035645 File Offset: 0x00033A45
			public ClanInviteCmd(IClanService clanService, IUserRepository userRepository)
			{
				this.m_clanService = clanService;
				this.m_userRepository = userRepository;
			}

			// Token: 0x06000D6C RID: 3436 RVA: 0x0003565C File Offset: 0x00033A5C
			public void ExecuteCmd(string[] args)
			{
				ulong profileId = ulong.Parse(args[1]);
				ulong targetId = ulong.Parse(args[2]);
				UserInfo.User user = this.m_userRepository.GetUser(profileId);
				EInviteStatus result = this.m_clanService.Invite(user, targetId, (args.Length <= 3) ? string.Empty : args[3]).Result;
				Log.Info<EInviteStatus>("Invite to clan : {0}", result);
			}

			// Token: 0x0400062E RID: 1582
			private readonly IClanService m_clanService;

			// Token: 0x0400062F RID: 1583
			private readonly IUserRepository m_userRepository;
		}

		// Token: 0x02000269 RID: 617
		[ConsoleCmdAttributes(CmdName = "clan_leaderboard", ArgsSize = 2, Help = "Get clan leaderboard position by profile ID, show [N] positions before your clanusage: clan_leaderboard [profileId] [numberOfPositionToShow]")]
		public class ClanLeadarboardCmd : IConsoleCmd
		{
			// Token: 0x06000D6D RID: 3437 RVA: 0x000356BC File Offset: 0x00033ABC
			public ClanLeadarboardCmd(IClanPerformanceService clanPerformanceService, IClanService clanService, IDALService dalService)
			{
				this.m_clanPerformanceService = clanPerformanceService;
				this.m_clanService = clanService;
				this.m_dalService = dalService;
			}

			// Token: 0x06000D6E RID: 3438 RVA: 0x000356DC File Offset: 0x00033ADC
			public void ExecuteCmd(string[] args)
			{
				ulong profile_id = ulong.Parse(args[1]);
				int num = int.Parse(args[2]);
				ClanMember memberInfo = this.m_clanService.GetMemberInfo(profile_id);
				if (memberInfo != null)
				{
					ClanInfo clan_info = this.m_dalService.ClanSystem.GetClanInfo(memberInfo.ClanID);
					IEnumerable<ulong> clansForLeaderboardPrediction = this.m_dalService.ClanSystem.GetClansForLeaderboardPrediction();
					int num2 = 0;
					foreach (ulong num3 in clansForLeaderboardPrediction)
					{
						num2++;
						if (num3 == memberInfo.ClanPoints)
						{
							break;
						}
					}
					MasterRecord performanceMasterRecord = this.m_dalService.PerformanceSystem.GetPerformanceMasterRecord("clan.%", cache_domains.clan_performance_master_record);
					ClanPerformanceInfo clanPerformance = this.m_clanPerformanceService.GetClanPerformance(memberInfo.ClanID);
					Log.Info<int, int>("DB position: {0}; Predicted position: {1}", num2, clanPerformance.Position);
					foreach (MasterRecord.Record record in performanceMasterRecord.Records)
					{
						if (record.StatSamples.Count == 0)
						{
							Log.Warning("Record doesn't contains prediction polynomial coefficients");
							break;
						}
						int num4 = record.StatSamples[0].Samples.FindIndex((KeyValuePair<float, float> kv) => (ulong)kv.Value == clan_info.ClanPoints);
						int count = record.StatSamples[0].Samples.Count;
						num = ((count <= num) ? count : (num + 1));
						int index = (num4 >= num) ? (num4 - num) : 0;
						foreach (KeyValuePair<float, float> keyValuePair in record.StatSamples[0].Samples.GetRange(index, num))
						{
							Log.Info<float, float>("Position: {0}, Clan Points {1}", keyValuePair.Key, keyValuePair.Value);
						}
					}
				}
				else
				{
					Log.Info("Not in clan");
				}
			}

			// Token: 0x04000630 RID: 1584
			private readonly IClanPerformanceService m_clanPerformanceService;

			// Token: 0x04000631 RID: 1585
			private readonly IClanService m_clanService;

			// Token: 0x04000632 RID: 1586
			private readonly IDALService m_dalService;
		}

		// Token: 0x0200026A RID: 618
		[DebugCommand]
		[ConsoleCmdAttributes(CmdName = "debug_clan_delete_all", ArgsSize = 0, Help = "Delete all clans")]
		public class ClanDeleteAllCmd : IConsoleCmd
		{
			// Token: 0x06000D6F RID: 3439 RVA: 0x00035983 File Offset: 0x00033D83
			public ClanDeleteAllCmd(IDALService dalService)
			{
				this.m_dalService = dalService;
			}

			// Token: 0x06000D70 RID: 3440 RVA: 0x00035992 File Offset: 0x00033D92
			public void ExecuteCmd(string[] args)
			{
				Log.Info("Starting delete of all clans");
				this.m_dalService.ClanSystem.DebugDeleteAllClans();
				Log.Info("Finished delete of all clans");
			}

			// Token: 0x04000633 RID: 1587
			private readonly IDALService m_dalService;
		}

		// Token: 0x0200026B RID: 619
		[ConsoleCmdAttributes(CmdName = "lb_refresh_master_records", ArgsSize = 0, Help = "Refresh master record for clans")]
		public class RefreshMasterRecordsCmd : IConsoleCmd
		{
			// Token: 0x06000D71 RID: 3441 RVA: 0x000359B8 File Offset: 0x00033DB8
			public RefreshMasterRecordsCmd(IClanPerformanceService clanPerformanceService)
			{
				this.m_clanPerformanceService = clanPerformanceService;
			}

			// Token: 0x06000D72 RID: 3442 RVA: 0x000359C7 File Offset: 0x00033DC7
			public void ExecuteCmd(string[] args)
			{
				this.m_clanPerformanceService.ForceRefreshMasterRecord();
				Log.Info("Refresh master record done");
			}

			// Token: 0x04000634 RID: 1588
			private readonly IClanPerformanceService m_clanPerformanceService;
		}

		// Token: 0x0200026C RID: 620
		[ConsoleCmdAttributes(CmdName = "lb_refresh_performance", ArgsSize = 0, Help = "Force refresh of clan leaderboard")]
		public class RefreshPerformanceLBCmd : IConsoleCmd
		{
			// Token: 0x06000D73 RID: 3443 RVA: 0x000359DE File Offset: 0x00033DDE
			public RefreshPerformanceLBCmd(IClanPerformanceService clanPerformanceService)
			{
				this.m_clanPerformanceService = clanPerformanceService;
			}

			// Token: 0x06000D74 RID: 3444 RVA: 0x000359ED File Offset: 0x00033DED
			public void ExecuteCmd(string[] args)
			{
				this.m_clanPerformanceService.RefreshLeaderboard();
				Log.Info("Refresh leaderboard done");
			}

			// Token: 0x04000635 RID: 1589
			private readonly IClanPerformanceService m_clanPerformanceService;
		}
	}
}

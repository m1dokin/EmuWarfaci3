using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E5 RID: 741
	internal static class GameInterfaceCmd
	{
		// Token: 0x060010BF RID: 4287 RVA: 0x00041E1C File Offset: 0x0004021C
		static GameInterfaceCmd()
		{
			GameInterfaceCmd.Commands = (from m in typeof(GameInterfaceCmd).GetMethods(BindingFlags.Static | BindingFlags.Public)
			where m.Name.StartsWith("gi_")
			select m).ToDictionary((MethodInfo m) => m.Name, (MethodInfo m) => new GameInterfaceCmd.GameInterfaceMethodInfo(m));
		}

		// Token: 0x060010C0 RID: 4288 RVA: 0x00041E78 File Offset: 0x00040278
		public static string Execute(AccessLevel access_level, string cmd)
		{
			string[] array = ConsoleCmdManager.ParseCmdLine(cmd);
			GameInterfaceCmd.GameInterfaceMethodInfo gameInterfaceMethodInfo;
			if (!GameInterfaceCmd.Commands.TryGetValue(array[0], out gameInterfaceMethodInfo))
			{
				throw new Exception(string.Format("Unrecognized command '{0}'", cmd));
			}
			IGameInterface service = ServicesManager.GetService<IGameInterface>();
			ILogService service2 = ServicesManager.GetService<ILogService>();
			ILogGroup logGroup = service2.CreateGroup();
			string result;
			using (IGameInterfaceContext gameInterfaceContext = service.CreateContext(access_level, logGroup))
			{
				logGroup.GameInterfaceCmd(string.Join(" ", array));
				result = gameInterfaceMethodInfo.Invoke(gameInterfaceContext, array.Skip(1).ToArray<string>());
			}
			return result;
		}

		// Token: 0x060010C1 RID: 4289 RVA: 0x00041F20 File Offset: 0x00040320
		public static TimeSpan GetTimeSpan(string ts)
		{
			string[] array = ts.Split(new char[]
			{
				':'
			});
			int days = int.Parse(array[0]);
			int hours = int.Parse(array[1]);
			int minutes = int.Parse(array[2]);
			return new TimeSpan(days, hours, minutes, 0);
		}

		// Token: 0x060010C2 RID: 4290 RVA: 0x00041F63 File Offset: 0x00040363
		public static IEnumerable<ulong> GetProfiles(string profilesList)
		{
			return GameInterfaceCmd.GetIDList(profilesList);
		}

		// Token: 0x060010C3 RID: 4291 RVA: 0x00041F6B File Offset: 0x0004036B
		public static IEnumerable<ulong> GetUsers(string usersList)
		{
			return GameInterfaceCmd.GetIDList(usersList);
		}

		// Token: 0x060010C4 RID: 4292 RVA: 0x00041F74 File Offset: 0x00040374
		public static void SetGiCommandsTimeout(IEnumerable<string> commands, TimeSpan timeout)
		{
			commands.SafeForEach(delegate(string c)
			{
				GameInterfaceCmd.GameInterfaceMethodInfo gameInterfaceMethodInfo;
				if (GameInterfaceCmd.Commands.TryGetValue(c, out gameInterfaceMethodInfo))
				{
					gameInterfaceMethodInfo.Timeout = timeout;
				}
			});
		}

		// Token: 0x060010C5 RID: 4293 RVA: 0x00041FA0 File Offset: 0x000403A0
		public static void SetGiCommandsShouldThrow(IEnumerable<string> commands, bool shouldThrow)
		{
			commands.SafeForEach(delegate(string c)
			{
				GameInterfaceCmd.GameInterfaceMethodInfo gameInterfaceMethodInfo;
				if (GameInterfaceCmd.Commands.TryGetValue(c, out gameInterfaceMethodInfo))
				{
					gameInterfaceMethodInfo.ShouldThrow = shouldThrow;
				}
			});
		}

		// Token: 0x060010C6 RID: 4294 RVA: 0x00041FCC File Offset: 0x000403CC
		public static bool ShouldGiCommandThrow(string command)
		{
			GameInterfaceCmd.GameInterfaceMethodInfo gameInterfaceMethodInfo;
			return GameInterfaceCmd.Commands.TryGetValue(command, out gameInterfaceMethodInfo) && gameInterfaceMethodInfo.ShouldThrow;
		}

		// Token: 0x060010C7 RID: 4295 RVA: 0x00041FF4 File Offset: 0x000403F4
		public static TimeSpan? GetGiCommandTimeout(string command)
		{
			GameInterfaceCmd.GameInterfaceMethodInfo gameInterfaceMethodInfo;
			return (!GameInterfaceCmd.Commands.TryGetValue(command, out gameInterfaceMethodInfo)) ? null : new TimeSpan?(gameInterfaceMethodInfo.Timeout);
		}

		// Token: 0x060010C8 RID: 4296 RVA: 0x0004202C File Offset: 0x0004042C
		public static int gi_test(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.TotalOnlineUsers();
		}

		// Token: 0x060010C9 RID: 4297 RVA: 0x00042034 File Offset: 0x00040434
		public static int gi_total_online(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.TotalOnlineUsers();
		}

		// Token: 0x060010CA RID: 4298 RVA: 0x0004203C File Offset: 0x0004043C
		public static string gi_server_status(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetServerStatus();
		}

		// Token: 0x060010CB RID: 4299 RVA: 0x00042044 File Offset: 0x00040444
		public static string gi_quit(IGameInterfaceContext ctx, string[] args)
		{
			ctx.Quit();
			return "quiting";
		}

		// Token: 0x060010CC RID: 4300 RVA: 0x00042054 File Offset: 0x00040454
		public static string gi_get_login_ip(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileId = ulong.Parse(args[0]);
			IPAddress userIPByProfileId = ctx.GetUserIPByProfileId(profileId);
			return userIPByProfileId.Equals(IPAddress.None) ? string.Empty : userIPByProfileId.ToString();
		}

		// Token: 0x060010CD RID: 4301 RVA: 0x00042094 File Offset: 0x00040494
		public static string gi_server_online(IGameInterfaceContext ctx, string[] args)
		{
			Dictionary<string, int> dictionary = ctx.ServerOnlineUsers();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<string, int> keyValuePair in dictionary)
			{
				stringBuilder.AppendFormat("{0} {1}\n", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060010CE RID: 4302 RVA: 0x00042118 File Offset: 0x00040518
		public static bool gi_send_plain_text_notification(IGameInterfaceContext ctx, string[] args)
		{
			bool flag = true;
			string notification = Uri.UnescapeDataString(args[1]);
			foreach (ulong profileId in GameInterfaceCmd.GetProfiles(args[0]))
			{
				flag &= ctx.SendPlainTextNotification(profileId, notification, GameInterfaceCmd.GetTimeSpan(args[2]));
			}
			return flag;
		}

		// Token: 0x060010CF RID: 4303 RVA: 0x0004218C File Offset: 0x0004058C
		public static string gi_get_announcement(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetAnnouncements();
		}

		// Token: 0x060010D0 RID: 4304 RVA: 0x00042194 File Offset: 0x00040594
		public static string gi_get_active_announcement(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetActiveAnnouncements();
		}

		// Token: 0x060010D1 RID: 4305 RVA: 0x0004219C File Offset: 0x0004059C
		public static bool gi_add_announcement(IGameInterfaceContext ctx, string[] args)
		{
			int count = args.Count<string>();
			Func<int, bool> func = (int idx) => idx >= count || args[idx] == "-";
			Announcement announcement = new Announcement
			{
				Message = Uri.UnescapeDataString(args[0]),
				StartTimeUTC = DateTime.Parse(args[1]).ToUniversalTime(),
				EndTimeUTC = DateTime.Parse(args[2]).ToUniversalTime(),
				IsSystem = (args[3] == "1"),
				RepeatTimes = ((!func(4)) ? uint.Parse(args[4]) : 1U),
				Target = (ulong)((!func(5)) ? uint.Parse(args[5]) : 0U),
				Server = ((!func(6) && !args[6].Equals("all", StringComparison.OrdinalIgnoreCase)) ? args[6] : string.Empty),
				Channel = ((!func(7) && !args[7].Equals("all", StringComparison.OrdinalIgnoreCase)) ? Enum.Parse(typeof(Resources.ChannelType), args[7], true).ToString() : string.Empty),
				Place = ((!func(8)) ? ((EAnnouncementPlace)Enum.Parse(typeof(EAnnouncementPlace), args[8], true)) : EAnnouncementPlace.None)
			};
			return announcement.RepeatTimes > 0U && announcement.StartTimeUTC < announcement.EndTimeUTC && ctx.AddAnnouncement(announcement);
		}

		// Token: 0x060010D2 RID: 4306 RVA: 0x00042378 File Offset: 0x00040778
		public static bool gi_modify_announcement(IGameInterfaceContext ctx, string[] args)
		{
			int count = args.Count<string>();
			ulong id = ulong.Parse(args[0].ToString());
			Announcement announcement = ctx.GetAnnouncement(id);
			if (announcement == null)
			{
				return false;
			}
			Func<int, bool> func = (int idx) => idx >= count || args[idx] == "-";
			announcement.Message = ((!func(1)) ? Uri.UnescapeDataString(args[1]) : announcement.Message);
			announcement.StartTimeUTC = ((!func(2)) ? DateTime.Parse(args[2]).ToUniversalTime() : announcement.StartTimeUTC);
			announcement.EndTimeUTC = ((!func(3)) ? DateTime.Parse(args[3]).ToUniversalTime() : announcement.EndTimeUTC);
			announcement.IsSystem = ((!func(4)) ? (args[4] == "1") : announcement.IsSystem);
			announcement.RepeatTimes = ((!func(5)) ? uint.Parse(args[5]) : announcement.RepeatTimes);
			announcement.Target = ((!func(6)) ? ((ulong)uint.Parse(args[6])) : announcement.Target);
			if (args[7].Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				announcement.Server = string.Empty;
			}
			else
			{
				announcement.Server = ((!func(7)) ? args[7] : announcement.Server);
			}
			if (args[8].Equals("all", StringComparison.OrdinalIgnoreCase))
			{
				announcement.Channel = string.Empty;
			}
			else
			{
				announcement.Channel = ((!func(8)) ? args[8] : announcement.Channel);
			}
			announcement.Place = ((!func(9)) ? ((EAnnouncementPlace)Enum.Parse(typeof(EAnnouncementPlace), args[9], true)) : announcement.Place);
			return announcement.StartTimeUTC < announcement.EndTimeUTC && ctx.ModifyAnnouncement(announcement);
		}

		// Token: 0x060010D3 RID: 4307 RVA: 0x000425E0 File Offset: 0x000409E0
		public static bool gi_delete_announcement(IGameInterfaceContext ctx, string[] args)
		{
			ulong id = ulong.Parse(args[0].ToString());
			return ctx.DeleteAnnouncement(id);
		}

		// Token: 0x060010D4 RID: 4308 RVA: 0x00042604 File Offset: 0x00040A04
		public static bool gi_notify_money_given(IGameInterfaceContext ctx, string[] args)
		{
			bool flag = true;
			foreach (ulong userId in GameInterfaceCmd.GetUsers(args[0]))
			{
				flag &= ctx.NotifyMoneyGiven(userId, (Currency)int.Parse(args[1]), ulong.Parse(args[2]));
			}
			return flag;
		}

		// Token: 0x060010D5 RID: 4309 RVA: 0x00042678 File Offset: 0x00040A78
		public static string gi_give_money(IGameInterfaceContext ctx, string[] args)
		{
			ulong userId = ulong.Parse(args[0]);
			bool notify = true;
			Func<int, string> skipOrGetParam = (int i) => (!string.Equals(args[i], "-")) ? args[i] : string.Empty;
			Func<int, string> func = (int i) => (args.Length <= i) ? string.Empty : skipOrGetParam(i);
			if (args.Length >= 4)
			{
				notify = ParseUtils.ParseBool(args[3]);
			}
			return ctx.GiveMoney(userId, (Currency)int.Parse(args[1]), ulong.Parse(args[2]), func(4), notify, func(5), func(6)).ToString();
		}

		// Token: 0x060010D6 RID: 4310 RVA: 0x00042724 File Offset: 0x00040B24
		public static bool gi_spend_money(IGameInterfaceContext ctx, string[] args)
		{
			bool flag = true;
			foreach (ulong userId in GameInterfaceCmd.GetUsers(args[0]))
			{
				flag &= ctx.SpendMoney(userId, (Currency)int.Parse(args[1]), ulong.Parse(args[2]), (args.Length <= 3) ? string.Empty : args[3]);
			}
			return flag;
		}

		// Token: 0x060010D7 RID: 4311 RVA: 0x000427AC File Offset: 0x00040BAC
		public static bool gi_give_randombox(IGameInterfaceContext ctx, string[] args)
		{
			Func<int, string> func = (int idx) => (args.Length <= idx) ? string.Empty : args[idx];
			bool flag = true;
			foreach (ulong userId in GameInterfaceCmd.GetUsers(func(0)))
			{
				flag &= ctx.GiveItem(userId, func(1), OfferType.Regular, string.Empty, func(2), func(3), true, false);
			}
			return flag;
		}

		// Token: 0x060010D8 RID: 4312 RVA: 0x00042850 File Offset: 0x00040C50
		public static bool gi_give_item(IGameInterfaceContext ctx, string[] args)
		{
			Func<int, string> skipOrGetParam = (int i) => (!string.Equals(args[i], "-")) ? args[i] : string.Empty;
			Func<int, string> func = (int i) => (args.Length <= i) ? string.Empty : skipOrGetParam(i);
			Func<int, bool> func2 = (int i) => args.Length <= i || string.Equals(args[i], "-") || ParseUtils.ParseBool(args[i]);
			Func<int, bool> func3 = (int i) => args.Length > i && ParseUtils.ParseBool(args[i]);
			bool flag = true;
			foreach (ulong userId in GameInterfaceCmd.GetUsers(args[0]))
			{
				string text = args[2].ToLower();
				if (text != null)
				{
					if (text == "p" || text == "permanent")
					{
						flag &= ctx.GiveItem(userId, args[1], OfferType.Permanent, string.Empty, func(3), func(5), func2(4), false);
						continue;
					}
					if (text == "r" || text == "regular")
					{
						flag &= ctx.GiveItem(userId, args[1], OfferType.Regular, string.Empty, func(3), func(5), func2(4), func3(6));
						continue;
					}
				}
				flag &= ctx.GiveItem(userId, args[1], args[2] + " " + args[3], func(4), func(6), func2(5));
			}
			return flag;
		}

		// Token: 0x060010D9 RID: 4313 RVA: 0x00042A20 File Offset: 0x00040E20
		public static bool gi_extend_item(IGameInterfaceContext ctx, string[] args)
		{
			bool flag = true;
			foreach (ulong userId in GameInterfaceCmd.GetUsers(args[0]))
			{
				flag &= ctx.GiveItem(userId, args[1], args[2] + " " + args[3], (args.Length <= 4) ? string.Empty : args[4], string.Empty, true);
			}
			return flag;
		}

		// Token: 0x060010DA RID: 4314 RVA: 0x00042AB0 File Offset: 0x00040EB0
		public static bool gi_remove_item(IGameInterfaceContext ctx, string[] args)
		{
			if (args.Length < 2)
			{
				return false;
			}
			bool flag = true;
			foreach (ulong userId in GameInterfaceCmd.GetUsers(args[0]))
			{
				ulong customerItemId;
				if (ulong.TryParse(args[1], out customerItemId))
				{
					flag &= ctx.RemoveItem(userId, customerItemId, (args.Length < 3) ? string.Empty : args[2]);
				}
				else
				{
					bool flag2 = args.Length >= 3 && args[2] == "all";
					int num = (!flag2) ? 3 : 4;
					string reason = (args.Length < num) ? string.Empty : args[num - 1];
					flag &= ctx.RemoveItem(userId, args[1], flag2, reason);
				}
			}
			return flag;
		}

		// Token: 0x060010DB RID: 4315 RVA: 0x00042BA0 File Offset: 0x00040FA0
		public static bool gi_remove_permanent_item(IGameInterfaceContext ctx, string[] args)
		{
			if (args.Length < 2)
			{
				return false;
			}
			bool flag = true;
			string reason = (args.Length <= 2) ? string.Empty : args[2];
			foreach (ulong userId in GameInterfaceCmd.GetUsers(args[0]))
			{
				flag &= ctx.RemovePermanentItem(userId, args[1], reason);
			}
			return flag;
		}

		// Token: 0x060010DC RID: 4316 RVA: 0x00042C28 File Offset: 0x00041028
		public static bool gi_remove_profile_item(IGameInterfaceContext ctx, string[] args)
		{
			bool flag = true;
			if (args.Length < 2)
			{
				throw new ArgumentException(string.Format("Incorrect amount of arguments", new object[0]));
			}
			ulong num;
			if (!ulong.TryParse(args[0], out num))
			{
				throw new ArgumentException(string.Format("Incorrect profileId, '{0}' can't be parsed to ulong", num));
			}
			string reason = (args.Count<string>() <= 2) ? string.Empty : args[2];
			ulong profileItemId;
			if (ulong.TryParse(args[1], out profileItemId))
			{
				flag &= ctx.RemoveProfileItem(num, profileItemId, reason);
			}
			return flag;
		}

		// Token: 0x060010DD RID: 4317 RVA: 0x00042CB4 File Offset: 0x000410B4
		public static int gi_give_coins(IGameInterfaceContext ctx, string[] args)
		{
			string message = string.Empty;
			bool notify = true;
			if (args.Length >= 3)
			{
				notify = ParseUtils.ParseBool(args[2]);
				if (notify && args.Length >= 4)
				{
					message = args[3];
				}
			}
			string reason = (args.Length <= 4) ? "-" : args[4];
			return GameInterfaceCmd.GetUsers(args[0]).Sum((ulong pid) => ctx.GiveCoins(pid, ushort.Parse(args[1]), message, reason, notify));
		}

		// Token: 0x060010DE RID: 4318 RVA: 0x00042D74 File Offset: 0x00041174
		public static int gi_give_consumable(IGameInterfaceContext ctx, string[] args)
		{
			string reason = string.Empty;
			string message = string.Empty;
			bool notify = true;
			if (args.Length >= 4)
			{
				notify = ParseUtils.ParseBool(args[3]);
				if (args.Length >= 5)
				{
					if (notify)
					{
						message = args[4];
						if (args.Length >= 6)
						{
							reason = args[5];
						}
					}
					else
					{
						reason = args[4];
					}
				}
			}
			return GameInterfaceCmd.GetUsers(args[0]).Sum((ulong pid) => ctx.GiveConsumable(pid, args[1], ushort.Parse(args[2]), message, reason, notify));
		}

		// Token: 0x060010DF RID: 4319 RVA: 0x00042E48 File Offset: 0x00041248
		public static bool gi_unlock_item(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.UnlockItem(pid, args[1]));
		}

		// Token: 0x060010E0 RID: 4320 RVA: 0x00042E88 File Offset: 0x00041288
		public static bool gi_unlock_all_items(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.UnlockAllItems(pid));
		}

		// Token: 0x060010E1 RID: 4321 RVA: 0x00042EBC File Offset: 0x000412BC
		public static bool gi_ban_player(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.BanPlayer(pid, GameInterfaceCmd.GetTimeSpan(args[1]), (args.Length <= 2) ? string.Empty : args[2]));
		}

		// Token: 0x060010E2 RID: 4322 RVA: 0x00042EFC File Offset: 0x000412FC
		public static bool gi_cancel_ban_player(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.CancelBanPlayer(pid));
		}

		// Token: 0x060010E3 RID: 4323 RVA: 0x00042F30 File Offset: 0x00041330
		public static bool gi_mute_player(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.MutePlayer(pid, GameInterfaceCmd.GetTimeSpan(args[1])));
		}

		// Token: 0x060010E4 RID: 4324 RVA: 0x00042F70 File Offset: 0x00041370
		public static bool gi_cancel_mute_player(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.CancelMutePlayer(pid));
		}

		// Token: 0x060010E5 RID: 4325 RVA: 0x00042FA4 File Offset: 0x000413A4
		public static bool gi_kick_player(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.KickPlayer(pid));
		}

		// Token: 0x060010E6 RID: 4326 RVA: 0x00042FD8 File Offset: 0x000413D8
		public static bool gi_force_logout(IGameInterfaceContext ctx, string[] args)
		{
			return GameInterfaceCmd.GetProfiles(args[0]).Aggregate(true, (bool current, ulong pid) => current & ctx.ForceLogout(pid));
		}

		// Token: 0x060010E7 RID: 4327 RVA: 0x0004300C File Offset: 0x0004140C
		public static bool gi_block_purchase(IGameInterfaceContext ctx, string[] args)
		{
			ulong userId = ulong.Parse(args[0]);
			ctx.BlockPurchase(userId, (args.Length <= 1) ? string.Empty : args[1]);
			return true;
		}

		// Token: 0x060010E8 RID: 4328 RVA: 0x00043040 File Offset: 0x00041440
		public static bool gi_unblock_purchase(IGameInterfaceContext ctx, string[] args)
		{
			ulong userId = ulong.Parse(args[0]);
			ctx.UnblockPurchase(userId, (args.Length <= 1) ? string.Empty : args[1]);
			return true;
		}

		// Token: 0x060010E9 RID: 4329 RVA: 0x00043074 File Offset: 0x00041474
		public static ulong gi_get_user_id(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetUserID(ulong.Parse(args[0]));
		}

		// Token: 0x060010EA RID: 4330 RVA: 0x00043084 File Offset: 0x00041484
		public static ulong gi_get_user_id_by_nick(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileIDByNickname = ctx.GetProfileIDByNickname(args[0]);
			return ctx.GetUserID(profileIDByNickname);
		}

		// Token: 0x060010EB RID: 4331 RVA: 0x000430A2 File Offset: 0x000414A2
		public static ulong gi_get_profile_id(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileID(ulong.Parse(args[0]));
		}

		// Token: 0x060010EC RID: 4332 RVA: 0x000430B2 File Offset: 0x000414B2
		public static ulong gi_get_profile_id_by_nick(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileIDByNickname(args[0]);
		}

		// Token: 0x060010ED RID: 4333 RVA: 0x000430C0 File Offset: 0x000414C0
		public static string gi_get_profile(IGameInterfaceContext ctx, string[] args)
		{
			ulong num = ulong.Parse(args[0]);
			SProfileInfo profileInfo = ctx.GetProfileInfo(num);
			return (profileInfo.Id == 0UL) ? string.Format("Profile with Id {0} doesn't exist.", num) : profileInfo.ToString();
		}

		// Token: 0x060010EE RID: 4334 RVA: 0x0004310E File Offset: 0x0004150E
		public static string gi_get_profile_items(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileItems(ulong.Parse(args[0]));
		}

		// Token: 0x060010EF RID: 4335 RVA: 0x0004311E File Offset: 0x0004151E
		public static string gi_get_default_items(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetDefaultItems();
		}

		// Token: 0x060010F0 RID: 4336 RVA: 0x00043126 File Offset: 0x00041526
		public static string gi_get_profile_unlocked_items(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileUnlockedItems(ulong.Parse(args[0]));
		}

		// Token: 0x060010F1 RID: 4337 RVA: 0x00043136 File Offset: 0x00041536
		public static string gi_get_profile_achievements(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileAchievements(ulong.Parse(args[0]));
		}

		// Token: 0x060010F2 RID: 4338 RVA: 0x00043146 File Offset: 0x00041546
		public static string gi_get_profile_sponsor_points(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileSponsorPoints(ulong.Parse(args[0]));
		}

		// Token: 0x060010F3 RID: 4339 RVA: 0x00043156 File Offset: 0x00041556
		public static string gi_get_profile_persistent_settings(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfilePersistentSettings(ulong.Parse(args[0]));
		}

		// Token: 0x060010F4 RID: 4340 RVA: 0x00043166 File Offset: 0x00041566
		public static string gi_get_profile_contracts(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileContract(ulong.Parse(args[0]));
		}

		// Token: 0x060010F5 RID: 4341 RVA: 0x00043178 File Offset: 0x00041578
		public static string gi_get_profile_clan(IGameInterfaceContext ctx, string[] args)
		{
			ClanInfo profileClan = ctx.GetProfileClan(ulong.Parse(args[0]));
			return (profileClan == null) ? string.Empty : profileClan.ToString();
		}

		// Token: 0x060010F6 RID: 4342 RVA: 0x000431AC File Offset: 0x000415AC
		public static bool gi_reset_profile(IGameInterfaceContext ctx, string[] args)
		{
			bool full = args.Length == 2;
			ctx.ResetProfile(ulong.Parse(args[0]), full);
			return true;
		}

		// Token: 0x060010F7 RID: 4343 RVA: 0x000431D0 File Offset: 0x000415D0
		public static bool gi_flush_user_profile(IGameInterfaceContext ctx, string[] args)
		{
			ctx.FlushUserProfile(ulong.Parse(args[0]));
			return true;
		}

		// Token: 0x060010F8 RID: 4344 RVA: 0x000431E1 File Offset: 0x000415E1
		public static bool gi_set_access_level(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.SetAccessLevel(new SUserAccessLevel(ulong.Parse(args[0]), ulong.Parse(args[1]), (AccessLevel)int.Parse(args[2]), args[3]));
		}

		// Token: 0x060010F9 RID: 4345 RVA: 0x0004320C File Offset: 0x0004160C
		public static string gi_get_access_level(IGameInterfaceContext ctx, string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<SUserAccessLevel> list = (args.Length <= 0) ? ctx.GetAccessLevel() : ctx.GetAccessLevel(ulong.Parse(args[0]));
			if (list.Count == 0)
			{
				if (args.Length == 1)
				{
					list.Add(new SUserAccessLevel(0UL, ulong.Parse(args[0]), AccessLevel.Basic, "0.0.0.0/0"));
				}
				else
				{
					stringBuilder.Append("No users with access level higher than Basic");
				}
			}
			foreach (SUserAccessLevel suserAccessLevel in list)
			{
				stringBuilder.AppendFormat("db_id: {0}, user_id: {1}, access_level: {2}, ip_mask: {3}\n", new object[]
				{
					suserAccessLevel.db_id,
					suserAccessLevel.user_id,
					suserAccessLevel.accessLevel,
					suserAccessLevel.ip_mask
				});
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060010FA RID: 4346 RVA: 0x0004331C File Offset: 0x0004171C
		public static bool gi_remove_access_level(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.RemoveAccessLevel(ulong.Parse(args[0]), ulong.Parse(args[1]));
		}

		// Token: 0x060010FB RID: 4347 RVA: 0x00043334 File Offset: 0x00041734
		public static string gi_get_clan_info(IGameInterfaceContext ctx, string[] args)
		{
			ClanInfo clanInfo = ctx.GetClanInfo(ulong.Parse(args[0]));
			return (clanInfo == null) ? string.Empty : clanInfo.ToString();
		}

		// Token: 0x060010FC RID: 4348 RVA: 0x00043368 File Offset: 0x00041768
		public static string gi_get_clan_info_by_name(IGameInterfaceContext ctx, string[] args)
		{
			ClanInfo clanInfoByName = ctx.GetClanInfoByName(args[0]);
			return (clanInfoByName == null) ? string.Empty : clanInfoByName.ToString();
		}

		// Token: 0x060010FD RID: 4349 RVA: 0x00043395 File Offset: 0x00041795
		public static string gi_get_clan_desc(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetClanDesc(ulong.Parse(args[0]));
		}

		// Token: 0x060010FE RID: 4350 RVA: 0x000433A8 File Offset: 0x000417A8
		public static string gi_get_clan_members(IGameInterfaceContext ctx, string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			List<ClanMember> list = ctx.GetClanMembers(ulong.Parse(args[0])).ToList<ClanMember>();
			stringBuilder.AppendLine(string.Format("Members count = {0}", list.Count<ClanMember>()));
			foreach (ClanMember clanMember in list)
			{
				stringBuilder.AppendLine(clanMember.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060010FF RID: 4351 RVA: 0x00043444 File Offset: 0x00041844
		public static bool gi_remove_clan_member(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.RemoveClanMember(ulong.Parse(args[0]));
		}

		// Token: 0x06001100 RID: 4352 RVA: 0x00043454 File Offset: 0x00041854
		public static int gi_add_clan_member(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.AddClanMember(ulong.Parse(args[0]), ulong.Parse(args[1]));
		}

		// Token: 0x06001101 RID: 4353 RVA: 0x0004346C File Offset: 0x0004186C
		public static bool gi_set_clan_role(IGameInterfaceContext ctx, string[] args)
		{
			EClanRole eclanRole = (EClanRole)int.Parse(args[2]);
			return Enum.IsDefined(typeof(EClanRole), eclanRole) && ctx.SetClanRole(ulong.Parse(args[0]), ulong.Parse(args[1]), eclanRole);
		}

		// Token: 0x06001102 RID: 4354 RVA: 0x000434B5 File Offset: 0x000418B5
		public static int gi_create_clan(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.CreateClan(ulong.Parse(args[0]), args[1], args[2]);
		}

		// Token: 0x06001103 RID: 4355 RVA: 0x000434CB File Offset: 0x000418CB
		public static bool gi_remove_clan(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.RemoveClan(ulong.Parse(args[0]));
		}

		// Token: 0x06001104 RID: 4356 RVA: 0x000434DC File Offset: 0x000418DC
		public static string gi_get_abuse_by_date(IGameInterfaceContext ctx, string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			DateTime dateTime = DateTime.Parse(args[0]).ToUniversalTime();
			DateTime dateTime2 = DateTime.Parse(args[1]).ToUniversalTime();
			uint count = uint.Parse(args[2]);
			sbyte reportSource = (args.Length < 4) ? -1 : sbyte.Parse(args[3]);
			if (dateTime <= dateTime2)
			{
				IEnumerable<SAbuseHistory> abuseReportsByDate = ctx.GetAbuseReportsByDate(dateTime, dateTime2, reportSource, count);
				foreach (SAbuseHistory sabuseHistory in abuseReportsByDate)
				{
					stringBuilder.AppendLine(sabuseHistory.ToString());
				}
			}
			else
			{
				stringBuilder.AppendLine("Start time can't be bigger than end time");
			}
			return (stringBuilder.Length <= 0) ? "No result" : stringBuilder.ToString();
		}

		// Token: 0x06001105 RID: 4357 RVA: 0x000435D8 File Offset: 0x000419D8
		public static string gi_get_abuses_count(IGameInterfaceContext ctx, string[] args)
		{
			DateTime dateTime = DateTime.Parse(args[0]).ToUniversalTime();
			DateTime dateTime2 = DateTime.Parse(args[1]).ToUniversalTime();
			if (dateTime > dateTime2)
			{
				return "Start time can't be bigger than end time";
			}
			return ctx.GetAbusesCount(dateTime, dateTime2).ToString();
		}

		// Token: 0x06001106 RID: 4358 RVA: 0x00043630 File Offset: 0x00041A30
		public static string gi_get_abuse_to_user(IGameInterfaceContext ctx, string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<SAbuseHistory> abuseReportsToUser = ctx.GetAbuseReportsToUser(ulong.Parse(args[0]));
			foreach (SAbuseHistory sabuseHistory in abuseReportsToUser)
			{
				stringBuilder.AppendLine(sabuseHistory.ToString());
			}
			return (stringBuilder.Length <= 0) ? "No result" : stringBuilder.ToString();
		}

		// Token: 0x06001107 RID: 4359 RVA: 0x000436C4 File Offset: 0x00041AC4
		public static string gi_get_abuse_from_user(IGameInterfaceContext ctx, string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<SAbuseHistory> abuseReportsFromUser = ctx.GetAbuseReportsFromUser(ulong.Parse(args[0]));
			foreach (SAbuseHistory sabuseHistory in abuseReportsFromUser)
			{
				stringBuilder.AppendLine(sabuseHistory.ToString());
			}
			return (stringBuilder.Length <= 0) ? "No result" : stringBuilder.ToString();
		}

		// Token: 0x06001108 RID: 4360 RVA: 0x00043758 File Offset: 0x00041B58
		public static string gi_get_abuse_top(IGameInterfaceContext ctx, string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<SAbuseTopReport> topAbuseReports = ctx.GetTopAbuseReports(uint.Parse(args[0]));
			foreach (SAbuseTopReport sabuseTopReport in topAbuseReports)
			{
				stringBuilder.AppendLine(sabuseTopReport.ToString());
			}
			return (stringBuilder.Length <= 0) ? "No result" : stringBuilder.ToString();
		}

		// Token: 0x06001109 RID: 4361 RVA: 0x000437EC File Offset: 0x00041BEC
		public static string gi_remote_screenshot_by_pid(IGameInterfaceContext ctx, string[] args)
		{
			int num = args.Length;
			return ctx.MakeRemoteScreenShot(ulong.Parse(args[0]), num > 1 && args[1].Equals("f"), (num <= 2) ? 1 : int.Parse(args[2]), (num <= 3) ? -1f : float.Parse(args[3]), (num <= 4) ? -1f : float.Parse(args[4]));
		}

		// Token: 0x0600110A RID: 4362 RVA: 0x0004386C File Offset: 0x00041C6C
		public static string gi_remote_screenshot_by_nick(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileIDByNickname = ctx.GetProfileIDByNickname(args[0]);
			int num = args.Length;
			return ctx.MakeRemoteScreenShot(profileIDByNickname, num > 1 && args[1].Equals("f"), (num <= 2) ? 1 : int.Parse(args[2]), (num <= 3) ? -1f : float.Parse(args[3]), (num <= 4) ? -1f : float.Parse(args[4]));
		}

		// Token: 0x0600110B RID: 4363 RVA: 0x000438EF File Offset: 0x00041CEF
		public static string gi_get_player_stats_telem(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetPlayerStatFromTelem(ulong.Parse(args[0]));
		}

		// Token: 0x0600110C RID: 4364 RVA: 0x000438FF File Offset: 0x00041CFF
		public static string gi_get_player_stats(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetPlayerStat(ulong.Parse(args[0]));
		}

		// Token: 0x0600110D RID: 4365 RVA: 0x0004390F File Offset: 0x00041D0F
		public static bool gi_unlock_tutorial(IGameInterfaceContext ctx, string[] args)
		{
			ctx.UnlockTutorial(ulong.Parse(args[0]), args[1], true, string.Empty);
			return true;
		}

		// Token: 0x0600110E RID: 4366 RVA: 0x00043929 File Offset: 0x00041D29
		public static bool gi_unlock_class(IGameInterfaceContext ctx, string[] args)
		{
			ctx.UnlockClass(ulong.Parse(args[0]), args[1], true, string.Empty);
			return true;
		}

		// Token: 0x0600110F RID: 4367 RVA: 0x00043944 File Offset: 0x00041D44
		public static string gi_unlock_mission(IGameInterfaceContext ctx, string[] args)
		{
			IEnumerable<string> source = ctx.UnlockMission(ulong.Parse(args[0]), args[1], true);
			return string.Join(",", source.ToArray<string>());
		}

		// Token: 0x06001110 RID: 4368 RVA: 0x00043974 File Offset: 0x00041D74
		public static bool gi_unlock_achievement(IGameInterfaceContext ctx, string[] args)
		{
			ctx.UnlockAchievement(ulong.Parse(args[0]), uint.Parse(args[1]));
			return true;
		}

		// Token: 0x06001111 RID: 4369 RVA: 0x00043990 File Offset: 0x00041D90
		public static string gi_lock_achievement(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.LockHiddenAchievement(ulong.Parse(args[0]), uint.Parse(args[1])).ToString();
		}

		// Token: 0x06001112 RID: 4370 RVA: 0x000439C1 File Offset: 0x00041DC1
		public static string gi_get_all_achievements(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetAllAchievements();
		}

		// Token: 0x06001113 RID: 4371 RVA: 0x000439C9 File Offset: 0x00041DC9
		public static string gi_get_profile_progression(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetProfileProgression(ulong.Parse(args[0]));
		}

		// Token: 0x06001114 RID: 4372 RVA: 0x000439D9 File Offset: 0x00041DD9
		public static string gi_get_user_tags(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.GetUserTags(ulong.Parse(args[0]));
		}

		// Token: 0x06001115 RID: 4373 RVA: 0x000439E9 File Offset: 0x00041DE9
		public static bool gi_set_user_tags(IGameInterfaceContext ctx, string[] args)
		{
			ctx.SetUserTags(ulong.Parse(args[0]), args[1]);
			return true;
		}

		// Token: 0x06001116 RID: 4374 RVA: 0x000439FD File Offset: 0x00041DFD
		public static bool gi_remove_user_tags(IGameInterfaceContext ctx, string[] args)
		{
			ctx.RemoveUserTags(ulong.Parse(args[0]));
			return true;
		}

		// Token: 0x06001117 RID: 4375 RVA: 0x00043A0E File Offset: 0x00041E0E
		public static string gi_custom_rule_list(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.CustomRuleList();
		}

		// Token: 0x06001118 RID: 4376 RVA: 0x00043A18 File Offset: 0x00041E18
		public static string gi_custom_rule_add(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.CustomRuleAdd(args[0]).ToString();
		}

		// Token: 0x06001119 RID: 4377 RVA: 0x00043A3C File Offset: 0x00041E3C
		public static string gi_custom_rule_enable(IGameInterfaceContext ctx, string[] args)
		{
			string text = args[args.Length - 1];
			bool flag = text.Contains("clean");
			bool enable = int.Parse(args[args.Length - ((!flag) ? 1 : 2)]) != 0;
			if (GameInterfaceCmd.<>f__mg$cache0 == null)
			{
				GameInterfaceCmd.<>f__mg$cache0 = new Func<string, ulong>(ulong.Parse);
			}
			IEnumerable<ulong> ruleId = args.Select(GameInterfaceCmd.<>f__mg$cache0).Take(args.Length - ((!flag) ? 1 : 2));
			return ctx.CustomRuleEnable(ruleId, enable, flag);
		}

		// Token: 0x0600111A RID: 4378 RVA: 0x00043ABF File Offset: 0x00041EBF
		public static string gi_custom_rule_remove(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.CustomRuleRemove(ulong.Parse(args[0]));
		}

		// Token: 0x0600111B RID: 4379 RVA: 0x00043AD0 File Offset: 0x00041ED0
		public static ulong gi_add_exp(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileId = ulong.Parse(args[0]);
			long amount = long.Parse(args[1]);
			return ctx.AddExp(profileId, amount, LevelChangeReason.GMOperation);
		}

		// Token: 0x0600111C RID: 4380 RVA: 0x00043AF8 File Offset: 0x00041EF8
		public static string gi_get_money(IGameInterfaceContext ctx, string[] args)
		{
			Dictionary<Currency, ulong> money = ctx.GetMoney(ulong.Parse(args[0]));
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KeyValuePair<Currency, ulong> keyValuePair in money)
			{
				stringBuilder.AppendFormat("{0} {1}\n", keyValuePair.Key, keyValuePair.Value);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600111D RID: 4381 RVA: 0x00043B88 File Offset: 0x00041F88
		public static string gi_get_profile_info_ex(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileId = ulong.Parse(args[0]);
			return ctx.GetProfileInfoEx(profileId).ToString();
		}

		// Token: 0x0600111E RID: 4382 RVA: 0x00043BB4 File Offset: 0x00041FB4
		public static string gi_get_all_items(IGameInterfaceContext ctx, string[] args)
		{
			Dictionary<ulong, SItem> allItems = ctx.GetAllItems();
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SItem sitem in allItems.Values)
			{
				stringBuilder.AppendLine(sitem.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x0600111F RID: 4383 RVA: 0x00043C2C File Offset: 0x0004202C
		public static string gi_get_friends(IGameInterfaceContext ctx, string[] args)
		{
			IEnumerable<SFriend> friends = ctx.GetFriends(ulong.Parse(args[0]));
			StringBuilder stringBuilder = new StringBuilder();
			foreach (SFriend sfriend in friends)
			{
				stringBuilder.AppendFormat("{0} {1}\n", sfriend.ProfileID, sfriend.Nickname);
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001120 RID: 4384 RVA: 0x00043CB4 File Offset: 0x000420B4
		public static string gi_recover_vouchers(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.RecoverVouchers();
		}

		// Token: 0x06001121 RID: 4385 RVA: 0x00043CBC File Offset: 0x000420BC
		public static string gi_reload_offers(IGameInterfaceContext ctx, string[] args)
		{
			return ctx.ReloadOffers();
		}

		// Token: 0x06001122 RID: 4386 RVA: 0x00043CC4 File Offset: 0x000420C4
		public static bool gi_load_offers(IGameInterfaceContext ctx, string[] args)
		{
			ctx.LoadOffers();
			return true;
		}

		// Token: 0x06001123 RID: 4387 RVA: 0x00043CD0 File Offset: 0x000420D0
		public static bool gi_set_observer(IGameInterfaceContext ctx, string[] args)
		{
			bool flag = true;
			foreach (ulong profileId in GameInterfaceCmd.GetProfiles(args[0]))
			{
				flag &= ctx.SetObserver(profileId, int.Parse(args[1]) != 0);
			}
			return flag;
		}

		// Token: 0x06001124 RID: 4388 RVA: 0x00043D40 File Offset: 0x00042140
		public static string gi_add_friend(IGameInterfaceContext ctx, string[] args)
		{
			ulong senderProfileID = ulong.Parse(args[0]);
			ulong targetProfileID = ulong.Parse(args[1]);
			return ctx.AddFriend(senderProfileID, targetProfileID);
		}

		// Token: 0x06001125 RID: 4389 RVA: 0x00043D68 File Offset: 0x00042168
		public static bool gi_remove_friend(IGameInterfaceContext ctx, string[] args)
		{
			ulong senderProfileID = ulong.Parse(args[0]);
			ulong targetProfileID = ulong.Parse(args[1]);
			ctx.RemoveFriend(senderProfileID, targetProfileID);
			return true;
		}

		// Token: 0x06001126 RID: 4390 RVA: 0x00043D90 File Offset: 0x00042190
		public static string gi_get_pending_friends(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileID = ulong.Parse(args[0]);
			return ctx.GetPendingFriends(profileID);
		}

		// Token: 0x06001127 RID: 4391 RVA: 0x00043DB0 File Offset: 0x000421B0
		public static bool gi_respond_to_notification(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileID = ulong.Parse(args[0]);
			ulong notifID = ulong.Parse(args[1]);
			bool accept = int.Parse(args[2]) == 1;
			ctx.RespondToInvitation(profileID, notifID, accept);
			return true;
		}

		// Token: 0x06001128 RID: 4392 RVA: 0x00043DE8 File Offset: 0x000421E8
		public static string gi_invite_clan_member(IGameInterfaceContext ctx, string[] args)
		{
			ulong senderProfileID = ulong.Parse(args[0]);
			ulong targetProfileID = ulong.Parse(args[1]);
			return ctx.InviteClanMember(senderProfileID, targetProfileID);
		}

		// Token: 0x06001129 RID: 4393 RVA: 0x00043E10 File Offset: 0x00042210
		public static bool gi_leave_clan(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileID = ulong.Parse(args[0]);
			return ctx.LeaveClan(profileID);
		}

		// Token: 0x0600112A RID: 4394 RVA: 0x00043E30 File Offset: 0x00042230
		public static string gi_get_pending_clans(IGameInterfaceContext ctx, string[] args)
		{
			ulong profileID = ulong.Parse(args[0]);
			return ctx.GetPendingClans(profileID);
		}

		// Token: 0x0600112B RID: 4395 RVA: 0x00043E50 File Offset: 0x00042250
		public static string gi_create_room(IGameInterfaceContext ctx, string[] args)
		{
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			string mission = args[2];
			int count = args.Count<string>();
			Func<int, bool> func = (int idx) => idx >= count || args[idx] == "-";
			bool manualStart = !func(3) && int.Parse(args[3]) > 0;
			bool allowJoin = !func(4) && int.Parse(args[4]) > 0;
			bool lockedSpectatorCamera = func(5) || int.Parse(args[5]) > 0;
			int roundLimit = (!func(6)) ? int.Parse(args[6]) : 11;
			int preRoundTime = (!func(7)) ? int.Parse(args[7]) : 30;
			bool overtimeMode = func(8) || int.Parse(args[8]) > 0;
			return ctx.CreateRoom(masterId, roomRef, new CreateRoomParam
			{
				Mission = mission,
				ManualStart = manualStart,
				AllowJoin = allowJoin,
				LockedSpectatorCamera = lockedSpectatorCamera,
				RoundLimit = roundLimit,
				PreRoundTime = preRoundTime,
				OvertimeMode = overtimeMode
			});
		}

		// Token: 0x0600112C RID: 4396 RVA: 0x00043FCC File Offset: 0x000423CC
		public static string gi_get_room_info_by_ref(IGameInterfaceContext ctx, string[] args)
		{
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			return ctx.GetRoomInfo(masterId, roomRef);
		}

		// Token: 0x0600112D RID: 4397 RVA: 0x00043FF0 File Offset: 0x000423F0
		public static string gi_add_player_to_room(IGameInterfaceContext ctx, string[] args)
		{
			if (args.Length < 3 || args.Length > 18)
			{
				return "It's an error in the number of arguments in call of gi_add_player_to_room. Stopping execution of GI command";
			}
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			IList<PlayerInfoForRoomOffer> list = GameInterfaceCmd.ParsePlayers(args.Skip(2).ToArray<string>());
			if (list.Count == 0)
			{
				return "There were no valid profile_id or nickname entered. Stopping execution of GI command";
			}
			int num = (from p in list
			group p by p.ProfileId).Count((IGrouping<ulong, PlayerInfoForRoomOffer> g) => g.Count<PlayerInfoForRoomOffer>() >= 2);
			int num2 = (from p in list
			where !string.IsNullOrEmpty(p.Nickname)
			group p by p.Nickname.ToLower()).Count((IGrouping<string, PlayerInfoForRoomOffer> g) => g.Count<PlayerInfoForRoomOffer>() >= 2);
			if (num > 1 || num2 > 1)
			{
				return "It was duplicates values in profile_id or nickname. Stopping execution of GI command";
			}
			return ctx.AddPlayerToRoom(masterId, roomRef, list);
		}

		// Token: 0x0600112E RID: 4398 RVA: 0x00044114 File Offset: 0x00042514
		public static string gi_remove_player_from_room(IGameInterfaceContext ctx, string[] args)
		{
			if (args.Length < 3 || args.Length > 18)
			{
				return "It's an error in the number of arguments in call of gi_add_player_to_room. Stopping execution of GI command";
			}
			string master = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			IList<PlayerInfoForRoomOffer> list = GameInterfaceCmd.ParsePlayers(args.Skip(2).ToArray<string>());
			if (list.Count == 0)
			{
				return "There were no valid profile_id or nickname entered. Stopping execution of GI command";
			}
			return ctx.RemovePlayerFromRoom(master, roomRef, list);
		}

		// Token: 0x0600112F RID: 4399 RVA: 0x00044174 File Offset: 0x00042574
		public static string gi_start_room(IGameInterfaceContext ctx, string[] args)
		{
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			int team1Score = (args.Length <= 2) ? 0 : int.Parse(args[2]);
			int team2Score = (args.Length <= 3) ? 0 : int.Parse(args[3]);
			return ctx.StartRoom(masterId, roomRef, team1Score, team2Score);
		}

		// Token: 0x06001130 RID: 4400 RVA: 0x000441C8 File Offset: 0x000425C8
		public static string gi_pause_session(IGameInterfaceContext ctx, string[] args)
		{
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			return ctx.PauseGameSession(masterId, roomRef);
		}

		// Token: 0x06001131 RID: 4401 RVA: 0x000441EC File Offset: 0x000425EC
		public static string gi_resume_session(IGameInterfaceContext ctx, string[] args)
		{
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			return ctx.ResumeGameSession(masterId, roomRef);
		}

		// Token: 0x06001132 RID: 4402 RVA: 0x00044210 File Offset: 0x00042610
		public static string gi_stop_session(IGameInterfaceContext ctx, string[] args)
		{
			string masterId = args[0];
			RoomReference roomRef = new RoomReference(args[1]);
			return ctx.StopGameSession(masterId, roomRef);
		}

		// Token: 0x06001133 RID: 4403 RVA: 0x00044234 File Offset: 0x00042634
		public static string gi_set_client_versions(IGameInterfaceContext ctx, string[] args)
		{
			string[] array = GameInterfaceCmd.GetVersionRegexString(args).ToArray<string>();
			ctx.SetSupportedClientVersions(array);
			return GameInterfaceCmd.DumpVersionsChangeList(ctx, array);
		}

		// Token: 0x06001134 RID: 4404 RVA: 0x0004425C File Offset: 0x0004265C
		public static string gi_add_client_versions(IGameInterfaceContext ctx, string[] args)
		{
			string[] versions = GameInterfaceCmd.GetVersionRegexString(args).ToArray<string>();
			ctx.AddSupportedClientVersions(versions);
			return GameInterfaceCmd.DumpVersionsChangeList(ctx, versions);
		}

		// Token: 0x06001135 RID: 4405 RVA: 0x00044284 File Offset: 0x00042684
		public static string gi_remove_client_versions(IGameInterfaceContext ctx, string[] args)
		{
			string[] array = GameInterfaceCmd.GetVersionRegexString(args).ToArray<string>();
			ctx.RemoveSupportedClientVersions(array);
			return GameInterfaceCmd.gi_dump_client_versions(ctx, array);
		}

		// Token: 0x06001136 RID: 4406 RVA: 0x000442AB File Offset: 0x000426AB
		public static string gi_dump_client_versions(IGameInterfaceContext ctx, string[] _)
		{
			return string.Format("Supported client versions: {0}", string.Join("; ", ctx.GetSupportedClientVersions().ToArray<string>()));
		}

		// Token: 0x06001137 RID: 4407 RVA: 0x000442CC File Offset: 0x000426CC
		public static bool gi_reload_client_versions(IGameInterfaceContext ctx, string[] _)
		{
			ctx.BroadcastClientVersionsReload();
			return true;
		}

		// Token: 0x06001138 RID: 4408 RVA: 0x000442D5 File Offset: 0x000426D5
		public static string gi_test_ttl(IGameInterfaceContext ctx, string[] _)
		{
			return "Test ttl executed";
		}

		// Token: 0x06001139 RID: 4409 RVA: 0x000442DC File Offset: 0x000426DC
		public static string gi_test_max_length(IGameInterfaceContext ctx, string[] prms)
		{
			if (prms.Length == 1)
			{
				return prms[0];
			}
			return "Test max length executed";
		}

		// Token: 0x0600113A RID: 4410 RVA: 0x000442F0 File Offset: 0x000426F0
		public static string gi_test_route(IGameInterfaceContext ctx, string[] _)
		{
			return "Test route executed";
		}

		// Token: 0x0600113B RID: 4411 RVA: 0x000442F7 File Offset: 0x000426F7
		public static string gi_test_access_level_admin(IGameInterfaceContext ctx, string[] _)
		{
			return ctx.TestAccessLevelAdmin();
		}

		// Token: 0x0600113C RID: 4412 RVA: 0x000442FF File Offset: 0x000426FF
		public static string gi_test_access_level_basic(IGameInterfaceContext ctx, string[] _)
		{
			return ctx.TestAccessLevelBasic();
		}

		// Token: 0x0600113D RID: 4413 RVA: 0x00044307 File Offset: 0x00042707
		public static string gi_test_access_level_moderator(IGameInterfaceContext ctx, string[] _)
		{
			return ctx.TestAccessLevelModerator();
		}

		// Token: 0x0600113E RID: 4414 RVA: 0x0004430F File Offset: 0x0004270F
		public static string gi_test_access_level_debug(IGameInterfaceContext ctx, string[] _)
		{
			return ctx.TestAccessLevelDebug();
		}

		// Token: 0x0600113F RID: 4415 RVA: 0x00044318 File Offset: 0x00042718
		public static string gi_get_rating_season_status(IGameInterfaceContext ctx, string[] _)
		{
			return ctx.GetRatingSeason();
		}

		// Token: 0x06001140 RID: 4416 RVA: 0x00044330 File Offset: 0x00042730
		public static string gi_get_profile_rating_points(IGameInterfaceContext ctx, string[] args)
		{
			ulong num = ulong.Parse(args[0]);
			uint profileRatingPoints = ctx.GetProfileRatingPoints(num);
			return string.Format("Player {0} has rating points = {1}", num, profileRatingPoints);
		}

		// Token: 0x06001141 RID: 4417 RVA: 0x00044364 File Offset: 0x00042764
		public static string gi_set_profile_rating_points(IGameInterfaceContext ctx, string[] args)
		{
			ulong num = ulong.Parse(args[0]);
			uint ratingPointsToSet = uint.Parse(args[1]);
			if (!ctx.SetProfileRatingPoints(num, ratingPointsToSet))
			{
				return "Unable to set rating points: player hasn't played this rating season yet";
			}
			uint profileRatingPoints = ctx.GetProfileRatingPoints(num);
			return string.Format("Player {0} now has rating points = {1}", num, profileRatingPoints);
		}

		// Token: 0x06001142 RID: 4418 RVA: 0x000443B4 File Offset: 0x000427B4
		public static string gi_get_top_rating_profiles(IGameInterfaceContext ctx, string[] args)
		{
			uint playersCount = uint.Parse(args[0]);
			IEnumerable<ulong> topRatingPlayers = ctx.GetTopRatingPlayers(playersCount);
			return string.Join<ulong>(",", topRatingPlayers);
		}

		// Token: 0x06001143 RID: 4419 RVA: 0x000443E0 File Offset: 0x000427E0
		public static string gi_rating_game_ban(IGameInterfaceContext ctx, string[] args)
		{
			ulong num = ulong.Parse(args[0]);
			TimeSpan banTimeOut = TimeSpan.Zero;
			ulong num2;
			if (args.Length > 1 && ulong.TryParse(args[1], out num2))
			{
				banTimeOut = TimeSpan.FromSeconds(num2);
			}
			string msg = (args.Length <= 2) ? string.Empty : args[2];
			bool flag = ctx.RatingGameBan(num, banTimeOut, msg);
			return string.Format("Player {0}; Banned for rating games: {1}", num, flag);
		}

		// Token: 0x06001144 RID: 4420 RVA: 0x00044458 File Offset: 0x00042858
		public static string gi_rating_game_unban(IGameInterfaceContext ctx, string[] args)
		{
			ulong num = ulong.Parse(args[0]);
			bool flag = ctx.RatingGameUnban(num);
			return string.Format("Player {0}; Banned for rating games: {1}", num, flag);
		}

		// Token: 0x06001145 RID: 4421 RVA: 0x0004448C File Offset: 0x0004288C
		public static bool gi_validate_authorization_token(IGameInterfaceContext ctx, string[] args)
		{
			ulong userId = ulong.Parse(args[0]);
			string tokenStr = args[1];
			return ctx.ValidateAuthorizationToken(userId, tokenStr);
		}

		// Token: 0x06001146 RID: 4422 RVA: 0x000444B0 File Offset: 0x000428B0
		private static string DumpVersionsChangeList(IGameInterfaceContext ctx, string[] versions)
		{
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<string> supportedClientVersions = ctx.GetSupportedClientVersions();
			string[] array = versions.Except(supportedClientVersions).ToArray<string>();
			if (array.Any<string>())
			{
				stringBuilder.AppendFormat("There were some unsupported versions: {0}", string.Join("; ", array));
				stringBuilder.AppendLine();
			}
			stringBuilder.Append(GameInterfaceCmd.gi_dump_client_versions(ctx, versions));
			return stringBuilder.ToString();
		}

		// Token: 0x06001147 RID: 4423 RVA: 0x00044514 File Offset: 0x00042914
		private static string SplitByColon(string val, out string group)
		{
			string[] array = val.Split(new char[]
			{
				':'
			});
			if (array.Count<string>() == 2)
			{
				group = array[1];
				return array[0];
			}
			group = RoomPlayer.DefaultGroup;
			return array[0];
		}

		// Token: 0x06001148 RID: 4424 RVA: 0x00044554 File Offset: 0x00042954
		private static IList<PlayerInfoForRoomOffer> ParsePlayers(string[] args)
		{
			List<PlayerInfoForRoomOffer> list = new List<PlayerInfoForRoomOffer>();
			foreach (string text in args)
			{
				string groupId;
				string text2 = GameInterfaceCmd.SplitByColon(text, out groupId);
				if (string.IsNullOrEmpty(text2))
				{
					Log.Warning("It's whitespace found instead profile_id or nickname");
				}
				else
				{
					PlayerInfoForRoomOffer item = default(PlayerInfoForRoomOffer);
					ulong profileId;
					if (text.StartsWith("pid#") && ulong.TryParse(text.Substring(4), out profileId))
					{
						item.ProfileId = profileId;
					}
					else
					{
						item.Nickname = text2;
					}
					item.GroupId = groupId;
					list.Add(item);
				}
			}
			return list;
		}

		// Token: 0x06001149 RID: 4425 RVA: 0x000445FA File Offset: 0x000429FA
		private static IEnumerable<string> GetVersionRegexString(IEnumerable<string> versions)
		{
			return from version in versions
			select string.Format("^{0}$", version);
		}

		// Token: 0x0600114A RID: 4426 RVA: 0x00044620 File Offset: 0x00042A20
		private static IEnumerable<ulong> GetIDList(string idsList)
		{
			List<ulong> list = new List<ulong>();
			foreach (string text in idsList.Split(new char[]
			{
				','
			}))
			{
				list.Add(ulong.Parse(text.Trim()));
			}
			return list;
		}

		// Token: 0x0400079B RID: 1947
		private static readonly Dictionary<string, GameInterfaceCmd.GameInterfaceMethodInfo> Commands = new Dictionary<string, GameInterfaceCmd.GameInterfaceMethodInfo>();

		// Token: 0x0400079C RID: 1948
		[CompilerGenerated]
		private static Func<string, ulong> <>f__mg$cache0;

		// Token: 0x020002E6 RID: 742
		private class GameInterfaceMethodInfo
		{
			// Token: 0x06001154 RID: 4436 RVA: 0x000446E2 File Offset: 0x00042AE2
			public GameInterfaceMethodInfo(MethodInfo method) : this(method, false, TimeSpan.FromSeconds(0.0))
			{
			}

			// Token: 0x06001155 RID: 4437 RVA: 0x000446FA File Offset: 0x00042AFA
			protected GameInterfaceMethodInfo(MethodInfo methodInfo, bool shouldThrow, TimeSpan timeout)
			{
				this.m_method = GameInterfaceCmd.GameInterfaceMethodInfo.CreateDelagate(methodInfo);
				this.Timeout = timeout;
				this.ShouldThrow = shouldThrow;
			}

			// Token: 0x17000191 RID: 401
			// (get) Token: 0x06001156 RID: 4438 RVA: 0x0004471C File Offset: 0x00042B1C
			// (set) Token: 0x06001157 RID: 4439 RVA: 0x00044724 File Offset: 0x00042B24
			public bool ShouldThrow { get; set; }

			// Token: 0x17000192 RID: 402
			// (get) Token: 0x06001158 RID: 4440 RVA: 0x0004472D File Offset: 0x00042B2D
			// (set) Token: 0x06001159 RID: 4441 RVA: 0x00044735 File Offset: 0x00042B35
			public TimeSpan Timeout { get; set; }

			// Token: 0x17000193 RID: 403
			// (get) Token: 0x0600115A RID: 4442 RVA: 0x00044740 File Offset: 0x00042B40
			public bool ShouldWait
			{
				get
				{
					return this.Timeout.TotalMilliseconds > 0.0;
				}
			}

			// Token: 0x0600115B RID: 4443 RVA: 0x00044766 File Offset: 0x00042B66
			public string Invoke(IGameInterfaceContext ctx, params string[] args)
			{
				if (this.ShouldThrow)
				{
					throw new GameInterfaceException();
				}
				if (this.ShouldWait)
				{
					Thread.Sleep(this.Timeout);
				}
				return this.m_method(ctx, args);
			}

			// Token: 0x0600115C RID: 4444 RVA: 0x0004479C File Offset: 0x00042B9C
			private static Func<IGameInterfaceContext, string[], string> CreateDelagate(MethodInfo methodInfo)
			{
				Type returnType = methodInfo.ReturnType;
				MethodInfo method = typeof(GameInterfaceCmd.GameInterfaceMethodInfo).GetMethod("DelegateHelper", BindingFlags.Static | BindingFlags.NonPublic);
				MethodInfo methodInfo2 = method.MakeGenericMethod(new Type[]
				{
					returnType
				});
				return (Func<IGameInterfaceContext, string[], string>)methodInfo2.Invoke(null, new object[]
				{
					methodInfo
				});
			}

			// Token: 0x0600115D RID: 4445 RVA: 0x000447F0 File Offset: 0x00042BF0
			private static Func<IGameInterfaceContext, string[], string> DelegateHelper<TReturn>(MethodInfo method)
			{
				Func<IGameInterfaceContext, string[], TReturn> func = (Func<IGameInterfaceContext, string[], TReturn>)Delegate.CreateDelegate(typeof(Func<IGameInterfaceContext, string[], TReturn>), method);
				return delegate(IGameInterfaceContext ctx, string[] args)
				{
					TReturn treturn = func(ctx, args);
					return treturn.ToString();
				};
			}

			// Token: 0x040007A3 RID: 1955
			private readonly Func<IGameInterfaceContext, string[], string> m_method;
		}
	}
}

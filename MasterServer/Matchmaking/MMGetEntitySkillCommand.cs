using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.Matchmaking.Data;
using MasterServer.Users;

namespace MasterServer.Matchmaking
{
	// Token: 0x0200050E RID: 1294
	[ConsoleCmdAttributes(CmdName = "mm_get_entity_skill", ArgsSize = 1, Help = "Get mm entity's skill.")]
	internal class MMGetEntitySkillCommand : IConsoleCmd
	{
		// Token: 0x06001C09 RID: 7177 RVA: 0x000714D0 File Offset: 0x0006F8D0
		public MMGetEntitySkillCommand(IUserProxyRepository userProxyRepository, ISkillService skillService)
		{
			this.m_userProxyRepository = userProxyRepository;
			this.m_skillService = skillService;
		}

		// Token: 0x06001C0A RID: 7178 RVA: 0x000714E8 File Offset: 0x0006F8E8
		public void ExecuteCmd(string[] args)
		{
			IEnumerable<string> source = args[1].Split(new char[]
			{
				','
			});
			if (MMGetEntitySkillCommand.<>f__mg$cache0 == null)
			{
				MMGetEntitySkillCommand.<>f__mg$cache0 = new Func<string, ulong>(ulong.Parse);
			}
			IEnumerable<ulong> groupList = source.Select(MMGetEntitySkillCommand.<>f__mg$cache0);
			StringBuilder stringBuilder = new StringBuilder();
			IEnumerable<SkillType> enumValues = Utils.GetEnumValues<SkillType>();
			IEnumerable<SkillType> enumerable = from st in enumValues
			where st != SkillType.None
			select st;
			foreach (SkillType skillType in enumerable)
			{
				MMEntityInfo mmentityInfo = this.CreateEntityInfo(groupList, skillType);
				if (mmentityInfo != null)
				{
					string arg = mmentityInfo.GetSkill().ToString(CultureInfo.InvariantCulture);
					stringBuilder.AppendFormat("{0} skill for group with participants '{1}' is: '{2}'", skillType, args[1], arg);
				}
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x06001C0B RID: 7179 RVA: 0x000715F0 File Offset: 0x0006F9F0
		private MMEntityInfo CreateEntityInfo(IEnumerable<ulong> groupList, SkillType skillType)
		{
			IEnumerable<UserInfo.User> users = this.m_userProxyRepository.GetUserOrProxyByProfileId(groupList, true);
			if (groupList.Any((ulong pid) => users.All((UserInfo.User user) => user.ProfileID != pid)))
			{
				Log.Error<string>("Can't get user information for skill calculation for all users '{0}'", string.Join(",", (from pid in groupList
				select pid.ToString(CultureInfo.InvariantCulture)).ToArray<string>()));
				return null;
			}
			List<MMPlayerInfo> players = (from u in users
			select new MMPlayerInfo
			{
				User = u,
				Skill = this.m_skillService.GetSkill(u.ProfileID, skillType)
			}).ToList<MMPlayerInfo>();
			return new MMEntityInfo
			{
				Players = players
			};
		}

		// Token: 0x04000D67 RID: 3431
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x04000D68 RID: 3432
		private readonly ISkillService m_skillService;

		// Token: 0x04000D69 RID: 3433
		[CompilerGenerated]
		private static Func<string, ulong> <>f__mg$cache0;
	}
}

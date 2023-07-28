using System;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.DebugQueries
{
	// Token: 0x02000217 RID: 535
	[DebugQuery]
	[QueryAttributes(TagName = "debug_add_experience")]
	internal class AddExperienceQuery : BaseQuery
	{
		// Token: 0x06000BA8 RID: 2984 RVA: 0x0002BEEA File Offset: 0x0002A2EA
		public AddExperienceQuery(IGameRoomManager roomManager, IRankSystem rankSystem, ILogService logService)
		{
			this.m_roomManager = roomManager;
			this.m_rankSystem = rankSystem;
			this.m_logService = logService;
		}

		// Token: 0x06000BA9 RID: 2985 RVA: 0x0002BF08 File Offset: 0x0002A308
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "AddExperienceQuery"))
			{
				UserInfo.User user;
				if (!base.GetClientInfo(fromJid, out user))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("experience");
					ulong gained_exp = ulong.Parse(attribute);
					gained_exp = this.m_rankSystem.AddExperience(user.ProfileID, gained_exp, LevelChangeReason.DebugQuery, this.m_logService.Event);
					IGameRoom roomByPlayer = this.m_roomManager.GetRoomByPlayer(user.ProfileID);
					if (roomByPlayer != null)
					{
						roomByPlayer.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
						{
							r.SignalPlayersChanged();
						});
					}
					response.SetAttribute("experience", gained_exp.ToString());
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x04000565 RID: 1381
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x04000566 RID: 1382
		private readonly IRankSystem m_rankSystem;

		// Token: 0x04000567 RID: 1383
		private readonly ILogService m_logService;
	}
}

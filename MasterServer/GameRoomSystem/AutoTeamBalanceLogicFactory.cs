using System;
using HK2Net;
using MasterServer.GameLogic.GameModes;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200044F RID: 1103
	[Service]
	[Singleton]
	internal class AutoTeamBalanceLogicFactory : IAutoTeamBalanceLogicFactory
	{
		// Token: 0x06001766 RID: 5990 RVA: 0x00060F29 File Offset: 0x0005F329
		public AutoTeamBalanceLogicFactory(IGameModesSystem gameModesSystem)
		{
			this.m_gameModesSystem = gameModesSystem;
		}

		// Token: 0x06001767 RID: 5991 RVA: 0x00060F38 File Offset: 0x0005F338
		public IAutoTeamBalanceLogic GetTeamBalancer(GameRoomType roomType)
		{
			switch (roomType)
			{
			case GameRoomType.PvE_Private:
				break;
			case GameRoomType.PvP_Public:
			case GameRoomType.PvP_AutoStart:
				goto IL_59;
			default:
				if (roomType != GameRoomType.PvE_AutoStart && roomType != GameRoomType.PvE)
				{
					if (roomType != GameRoomType.PvP_Rating && roomType != GameRoomType.PvP)
					{
						throw new Exception(string.Format("Can't find balancer for {0}", roomType));
					}
					goto IL_59;
				}
				break;
			case GameRoomType.PvP_ClanWar:
				return new ClanWarAutoTeamBalanceLogic(this.m_gameModesSystem);
			}
			return new PveAutoTeamBalanceLogic(this.m_gameModesSystem);
			IL_59:
			return new PvPAutoTeamBalanceLogic(this.m_gameModesSystem);
		}

		// Token: 0x04000B42 RID: 2882
		private readonly IGameModesSystem m_gameModesSystem;
	}
}

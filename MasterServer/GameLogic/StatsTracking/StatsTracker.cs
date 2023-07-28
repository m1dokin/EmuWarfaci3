using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.GameLogic.RewardSystem;

namespace MasterServer.GameLogic.StatsTracking
{
	// Token: 0x020005D3 RID: 1491
	[Service]
	[Singleton]
	public class StatsTracker : ServiceModule, IStatsTracker
	{
		// Token: 0x06001FCE RID: 8142 RVA: 0x00081788 File Offset: 0x0007FB88
		public void ChangeStatistics(ulong profileId, EStatsEvent eventId, object eventParams)
		{
			List<IStatsFilter> list = new List<IStatsFilter>();
			int value = 1;
			switch (eventId)
			{
			case EStatsEvent.SESSION_END:
				list.Add(new SessionOutcomeFilter((SessionOutcome)eventParams));
				goto IL_8E;
			case EStatsEvent.MONEY_AWARDED:
			case EStatsEvent.CRYMONEY_AWARDED:
			case EStatsEvent.CROWN_COLLECTED:
			case EStatsEvent.ADD_FRIEND:
			case EStatsEvent.HIDDEN:
				value = Convert.ToInt32(eventParams);
				goto IL_8E;
			case EStatsEvent.LEADER_BOARD_UPDATE:
				list.Add(new LeaderBoardPositionFilter((uint)eventParams));
				goto IL_8E;
			}
			throw new Exception(string.Format("Event '{0}' isn't handled by ChangeStatistics", eventId.ToString()));
			IL_8E:
			if (this.OnStatisticsChanged != null)
			{
				this.OnStatisticsChanged(profileId, eventId, list, value);
			}
		}

		// Token: 0x06001FCF RID: 8143 RVA: 0x00081840 File Offset: 0x0007FC40
		public void ResetStatistics(ulong profileId, EStatsEvent eventId, object eventParams)
		{
			List<IStatsFilter> filters = new List<IStatsFilter>();
			int value;
			if (eventId != EStatsEvent.RANK_CHANGED)
			{
				if (eventId != EStatsEvent.SPONSOR_PROGRESS)
				{
					throw new Exception(string.Format("Event '{0}' isn't handled by ResetStatistics", eventId.ToString()));
				}
				value = Convert.ToInt32(eventParams);
			}
			else
			{
				value = Convert.ToInt32((uint)eventParams);
			}
			if (this.OnStatisticsReset != null)
			{
				this.OnStatisticsReset(profileId, eventId, filters, value);
			}
		}

		// Token: 0x06001FD0 RID: 8144 RVA: 0x000818BC File Offset: 0x0007FCBC
		public static bool IsFiltersMatch(List<IStatsFilter> candidate, List<IStatsFilter> restrictions)
		{
			foreach (IStatsFilter statsFilter in candidate)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (IStatsFilter statsFilter2 in restrictions)
				{
					if (statsFilter.FilterId == statsFilter2.FilterId)
					{
						flag = true;
						flag2 = (flag2 || statsFilter.Compare(statsFilter2));
						if (flag2)
						{
							break;
						}
					}
				}
				if (!flag)
				{
					flag2 = true;
				}
				if (!flag2)
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x1400007C RID: 124
		// (add) Token: 0x06001FD1 RID: 8145 RVA: 0x0008199C File Offset: 0x0007FD9C
		// (remove) Token: 0x06001FD2 RID: 8146 RVA: 0x000819D4 File Offset: 0x0007FDD4
		public event StatsChangedDelegate OnStatisticsChanged;

		// Token: 0x1400007D RID: 125
		// (add) Token: 0x06001FD3 RID: 8147 RVA: 0x00081A0C File Offset: 0x0007FE0C
		// (remove) Token: 0x06001FD4 RID: 8148 RVA: 0x00081A44 File Offset: 0x0007FE44
		public event StatsChangedDelegate OnStatisticsReset;
	}
}

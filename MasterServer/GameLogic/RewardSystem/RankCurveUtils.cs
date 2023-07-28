using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005AD RID: 1453
	public static class RankCurveUtils
	{
		// Token: 0x06001F25 RID: 7973 RVA: 0x0007E84C File Offset: 0x0007CC4C
		public static SRankInfo CalculateRankInfo(ulong points, List<ulong> curve)
		{
			SRankInfo result = new SRankInfo(1, 0UL, curve[0], curve[1]);
			int count = curve.Count;
			ulong num = curve[count - 1];
			if (points < num)
			{
				byte b = 1;
				while ((int)b < count)
				{
					if (points < curve[(int)b])
					{
						result.RankId = (int)b;
						result.Points = points;
						result.RankStart = curve[(int)(b - 1)];
						result.NextRankStart = curve[(int)b];
						break;
					}
					b += 1;
				}
			}
			else
			{
				result.RankId = count;
				result.Points = num;
				result.RankStart = num;
				result.NextRankStart = num;
			}
			return result;
		}

		// Token: 0x06001F26 RID: 7974 RVA: 0x0007E904 File Offset: 0x0007CD04
		public static ulong CalculatePoints(SRankInfo rankInfo, List<ulong> curve)
		{
			if (rankInfo.RankId > curve.Count || rankInfo.RankId < 1)
			{
				throw new Exception(string.Format("Rank is out of bounds maxRank {0}, rank {1}", curve.Count, rankInfo.RankId));
			}
			int num = curve.Count - 1;
			if (rankInfo.RankId < num)
			{
				ulong num2 = rankInfo.Points - rankInfo.RankStart;
				ulong num3 = rankInfo.NextRankStart - rankInfo.RankStart;
				ulong num4 = curve[rankInfo.RankId] - curve[rankInfo.RankId - 1];
				if (num3 == 0UL)
				{
					num3 = num4;
				}
				if (num2 > num3)
				{
					num2 = num3;
				}
				return curve[rankInfo.RankId - 1] + num4 * num2 / num3;
			}
			return curve[rankInfo.RankId - 1];
		}

		// Token: 0x06001F27 RID: 7975 RVA: 0x0007E9E7 File Offset: 0x0007CDE7
		public static ulong GetPoints(int rankId, List<ulong> curve)
		{
			if (rankId > curve.Count || rankId < 1)
			{
				throw new Exception(string.Format("Rank {0} is out of bounds [1, {1}]", rankId, curve.Count));
			}
			return curve[rankId - 1];
		}
	}
}

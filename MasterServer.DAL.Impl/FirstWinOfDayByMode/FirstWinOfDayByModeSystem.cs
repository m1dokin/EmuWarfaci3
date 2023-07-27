using System;
using MasterServer.DAL.FirstWinOfDayByMode;
using Util.Common;

namespace MasterServer.DAL.Impl.FirstWinOfDayByMode
{
	// Token: 0x02000013 RID: 19
	public class FirstWinOfDayByModeSystem : IFirstWinOfDayByModeSystem
	{
		// Token: 0x060000B1 RID: 177 RVA: 0x00007274 File Offset: 0x00005474
		public FirstWinOfDayByModeSystem(DAL dal)
		{
			this.m_dal = dal;
			this.m_pvpModeSerializer = new PvpModeWinNextOccurrenceSerializer();
		}

		// Token: 0x060000B2 RID: 178 RVA: 0x00007290 File Offset: 0x00005490
		public DALResult<bool> SetPvpModeFirstWin(ulong profileId, string mode, DateTime nextOccurrence)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT SetPvpModeFirstWin(?profileId, ?winMode, ?nextOccurrenceUT)", new object[]
			{
				"?profileId",
				profileId,
				"?winMode",
				mode,
				"?nextOccurrenceUT",
				TimeUtils.LocalTimeToUTCTimestamp(nextOccurrence)
			});
			string s = this.m_dal.CacheProxy.SetScalar(setOptions).ToString();
			bool val = int.Parse(s) > 0;
			return new DALResult<bool>(val, setOptions.stats);
		}

		// Token: 0x060000B3 RID: 179 RVA: 0x00007314 File Offset: 0x00005514
		public DALResultMulti<PvpModeWinNextOccurrence> GetPvpModesWinNextOccurrence(ulong profileId)
		{
			CacheProxy.Options<PvpModeWinNextOccurrence> options = new CacheProxy.Options<PvpModeWinNextOccurrence>
			{
				db_serializer = this.m_pvpModeSerializer
			};
			options.query("CALL GetPvpModesWinNextOccurrence(?profileId)", new object[]
			{
				"?profileId",
				profileId
			});
			return this.m_dal.CacheProxy.GetStream<PvpModeWinNextOccurrence>(options);
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00007368 File Offset: 0x00005568
		public DALResultVoid ResetPvpModesFirstWin(ulong profileId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL ResetPvpModesFirstWin(?profileId)", new object[]
			{
				"?profileId",
				profileId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x04000048 RID: 72
		private readonly DAL m_dal;

		// Token: 0x04000049 RID: 73
		private readonly PvpModeWinNextOccurrenceSerializer m_pvpModeSerializer;
	}
}

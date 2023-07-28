using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000088 RID: 136
	[Serializable]
	public struct SProfileInfo
	{
		// Token: 0x1700002D RID: 45
		// (get) Token: 0x06000195 RID: 405 RVA: 0x000050AF File Offset: 0x000034AF
		public DateTime LastRankedTime
		{
			get
			{
				return TimeUtils.UTCTimestampToLocalTime(this.LastRankedTimeUTC);
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x06000196 RID: 406 RVA: 0x000050BC File Offset: 0x000034BC
		public DateTime CreateTime
		{
			get
			{
				return TimeUtils.UTCTimestampToLocalTime(this.CreateTimeUTC);
			}
		}

		// Token: 0x1700002F RID: 47
		// (get) Token: 0x06000197 RID: 407 RVA: 0x000050C9 File Offset: 0x000034C9
		public DateTime BanTime
		{
			get
			{
				return TimeUtils.UTCTimestampToLocalTime(this.BanTimeUTC);
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x06000198 RID: 408 RVA: 0x000050D6 File Offset: 0x000034D6
		public DateTime MuteTime
		{
			get
			{
				return TimeUtils.UTCTimestampToLocalTime(this.MuteTimeUTC);
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x06000199 RID: 409 RVA: 0x000050E3 File Offset: 0x000034E3
		public DateTime LastSeenTime
		{
			get
			{
				return TimeUtils.UTCTimestampToLocalTime(this.LastSeenTimeUTC);
			}
		}

		// Token: 0x17000032 RID: 50
		// (get) Token: 0x0600019A RID: 410 RVA: 0x000050F0 File Offset: 0x000034F0
		// (set) Token: 0x0600019B RID: 411 RVA: 0x000050F8 File Offset: 0x000034F8
		public uint CurrentClass
		{
			get
			{
				return this.CurrClass;
			}
			set
			{
				this.CurrClass = ((value <= SProfileInfo.MAX_CLASS && value != SProfileInfo.HEAVY_CLASS_ID) ? value : SProfileInfo.DEFAULT_CLASS_ID);
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x0600019C RID: 412 RVA: 0x00005121 File Offset: 0x00003521
		public bool Empty
		{
			get
			{
				return this.Id == 0UL;
			}
		}

		// Token: 0x0600019D RID: 413 RVA: 0x00005130 File Offset: 0x00003530
		public override string ToString()
		{
			return string.Format("Id: {0}, UserId: {1}, Nick: {2}, Gender: {3}, Height: {4}, Fatness: {5}, {6}, Class: {7}, Head: {8}, MissionPassed: {9}, LastSeenTime: {10}, LastRankedTime: {11}, CreateTime: {12}, BanTime: {13}, MuteTime: {14}", new object[]
			{
				this.Id,
				this.UserID,
				this.Nickname,
				this.Gender,
				this.Height,
				this.Fatness,
				this.RankInfo,
				this.CurrentClass,
				this.Head,
				this.MissionPassed,
				this.LastSeenTime,
				this.LastRankedTime,
				this.CreateTime,
				this.BanTime,
				this.MuteTime
			});
		}

		// Token: 0x04000162 RID: 354
		public static uint MAX_CLASS = 4U;

		// Token: 0x04000163 RID: 355
		public static uint HEAVY_CLASS_ID = 1U;

		// Token: 0x04000164 RID: 356
		public static uint DEFAULT_CLASS_ID;

		// Token: 0x04000165 RID: 357
		public ulong Id;

		// Token: 0x04000166 RID: 358
		public ulong UserID;

		// Token: 0x04000167 RID: 359
		public string Nickname;

		// Token: 0x04000168 RID: 360
		public string Gender;

		// Token: 0x04000169 RID: 361
		public float Height;

		// Token: 0x0400016A RID: 362
		public float Fatness;

		// Token: 0x0400016B RID: 363
		public SRankInfo RankInfo;

		// Token: 0x0400016C RID: 364
		private uint CurrClass;

		// Token: 0x0400016D RID: 365
		public string Head;

		// Token: 0x0400016E RID: 366
		public byte MissionPassed;

		// Token: 0x0400016F RID: 367
		public SBannerInfo Banner;

		// Token: 0x04000170 RID: 368
		public ulong LastRankedTimeUTC;

		// Token: 0x04000171 RID: 369
		public ulong CreateTimeUTC;

		// Token: 0x04000172 RID: 370
		public ulong BanTimeUTC;

		// Token: 0x04000173 RID: 371
		public ulong MuteTimeUTC;

		// Token: 0x04000174 RID: 372
		public ulong LastSeenTimeUTC;
	}
}

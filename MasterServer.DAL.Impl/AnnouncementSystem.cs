using System;
using Util.Common;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000004 RID: 4
	internal class AnnouncementSystem : IAnnouncmentSystem
	{
		// Token: 0x06000013 RID: 19 RVA: 0x00002713 File Offset: 0x00000913
		public AnnouncementSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x06000014 RID: 20 RVA: 0x00002730 File Offset: 0x00000930
		public DALResult<ulong> Add(Announcement announcement)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("SELECT AddAnnouncements(?start_time, ?end_time, ?priority, ?repeat_times, ?target, ?channel, ?server, ?place, ?message)", new object[]
			{
				"?start_time",
				TimeUtils.LocalTimeToUTCTimestamp(announcement.StartTimeUTC),
				"?end_time",
				TimeUtils.LocalTimeToUTCTimestamp(announcement.EndTimeUTC),
				"?priority",
				(!announcement.IsSystem) ? 0 : 1,
				"?repeat_times",
				announcement.RepeatTimes,
				"?target",
				announcement.Target,
				"?channel",
				announcement.Channel,
				"?server",
				announcement.Server,
				"?place",
				(uint)announcement.Place,
				"?message",
				announcement.Message
			});
			setOptions.db_transaction = true;
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x06000015 RID: 21 RVA: 0x00002858 File Offset: 0x00000A58
		public DALResultVoid Modify(Announcement announcement)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL UpdateAnnouncements(?id, ?start_time, ?end_time, ?priority, ?repeat_times, ?target, ?channel, ?server, ?place, ?message)", new object[]
			{
				"?id",
				announcement.ID,
				"?start_time",
				TimeUtils.LocalTimeToUTCTimestamp(announcement.StartTimeUTC),
				"?end_time",
				TimeUtils.LocalTimeToUTCTimestamp(announcement.EndTimeUTC),
				"?priority",
				(!announcement.IsSystem) ? 0 : 1,
				"?repeat_times",
				announcement.RepeatTimes,
				"?target",
				announcement.Target,
				"?channel",
				announcement.Channel,
				"?server",
				announcement.Server,
				"?place",
				(uint)announcement.Place,
				"?message",
				announcement.Message
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000016 RID: 22 RVA: 0x0000297C File Offset: 0x00000B7C
		public DALResultVoid Remove(ulong id)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions();
			setOptions.query("CALL DeleteAnnouncements(?id)", new object[]
			{
				"?id",
				id
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000029C4 File Offset: 0x00000BC4
		public DALResult<Announcement> GetAnnouncementById(ulong id)
		{
			CacheProxy.Options<Announcement> options = new CacheProxy.Options<Announcement>
			{
				db_serializer = this.m_announcementSerializer
			};
			options.query("CALL GetAnnouncementById(?id)", new object[]
			{
				"?id",
				id
			});
			return this.m_dal.CacheProxy.Get<Announcement>(options);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x00002A18 File Offset: 0x00000C18
		public DALResultMulti<Announcement> GetAnnouncements()
		{
			CacheProxy.Options<Announcement> options = new CacheProxy.Options<Announcement>
			{
				db_serializer = this.m_announcementSerializer
			};
			options.query("CALL GetAnnouncements()", new object[0]);
			return this.m_dal.CacheProxy.GetStream<Announcement>(options);
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002A5C File Offset: 0x00000C5C
		public DALResultMulti<Announcement> GetActiveAnnouncements()
		{
			CacheProxy.Options<Announcement> options = new CacheProxy.Options<Announcement>
			{
				db_serializer = this.m_announcementSerializer
			};
			options.query("CALL GetActiveAnnouncements()", new object[0]);
			return this.m_dal.CacheProxy.GetStream<Announcement>(options);
		}

		// Token: 0x04000008 RID: 8
		private DAL m_dal;

		// Token: 0x04000009 RID: 9
		private AnnouncementSerializer m_announcementSerializer = new AnnouncementSerializer();
	}
}

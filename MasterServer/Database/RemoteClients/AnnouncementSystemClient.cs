using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F2 RID: 498
	internal class AnnouncementSystemClient : DALCacheProxy<IDALService>, IAnnouncementSystemClient
	{
		// Token: 0x060009BA RID: 2490 RVA: 0x000246D8 File Offset: 0x00022AD8
		internal void Reset(IAnnouncmentSystem system)
		{
			this.m_announcementSystem = system;
		}

		// Token: 0x060009BB RID: 2491 RVA: 0x000246E4 File Offset: 0x00022AE4
		public ulong Add(Announcement announcement)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.announcements,
				set_func = (() => this.m_announcementSystem.Add(announcement))
			};
			return base.SetAndClearScalar<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009BC RID: 2492 RVA: 0x00024738 File Offset: 0x00022B38
		public void Modify(Announcement announcement)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.announcements,
				set_func = (() => this.m_announcementSystem.Modify(announcement))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009BD RID: 2493 RVA: 0x0002478C File Offset: 0x00022B8C
		public void Remove(ulong id)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domain = cache_domains.announcements,
				set_func = (() => this.m_announcementSystem.Remove(id))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009BE RID: 2494 RVA: 0x000247E0 File Offset: 0x00022BE0
		public Announcement GetAnnouncementById(ulong id)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<Announcement> options = new DALCacheProxy<IDALService>.SetOptionsScalar<Announcement>
			{
				cache_domain = cache_domains.announcements,
				set_func = (() => this.m_announcementSystem.GetAnnouncementById(id))
			};
			return base.SetAndClearScalar<Announcement>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009BF RID: 2495 RVA: 0x00024834 File Offset: 0x00022C34
		public IEnumerable<Announcement> GetAnnouncements()
		{
			DALCacheProxy<IDALService>.Options<Announcement> options = new DALCacheProxy<IDALService>.Options<Announcement>
			{
				get_data_stream = (() => this.m_announcementSystem.GetAnnouncements())
			};
			return base.GetDataStream<Announcement>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x060009C0 RID: 2496 RVA: 0x00024868 File Offset: 0x00022C68
		public IEnumerable<Announcement> GetActiveAnnouncements()
		{
			DALCacheProxy<IDALService>.Options<Announcement> options = new DALCacheProxy<IDALService>.Options<Announcement>
			{
				cache_domain = cache_domains.announcements,
				get_data_stream = (() => this.m_announcementSystem.GetActiveAnnouncements())
			};
			return base.GetDataStream<Announcement>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400054E RID: 1358
		private IAnnouncmentSystem m_announcementSystem;
	}
}

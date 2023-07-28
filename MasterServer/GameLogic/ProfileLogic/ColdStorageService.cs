using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL;
using MasterServer.DAL.Utils;
using MasterServer.Database;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Telemetry;
using Util.Common;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000549 RID: 1353
	[Service]
	[Singleton]
	internal class ColdStorageService : ServiceModule, IColdStorageService
	{
		// Token: 0x06001D35 RID: 7477 RVA: 0x000760F4 File Offset: 0x000744F4
		public ColdStorageService(IProfileItems itemService, IDALService dalService, ITelemetryService telemetryService, IJobSchedulerService jobSchedulerService)
		{
			this.m_itemService = itemService;
			this.m_dalService = dalService;
			this.m_telemetryService = telemetryService;
			this.m_jobSchedulerService = jobSchedulerService;
		}

		// Token: 0x06001D36 RID: 7478 RVA: 0x00076140 File Offset: 0x00074540
		public override void Start()
		{
			if (Resources.DBUpdaterPermission)
			{
				this.m_jobSchedulerService.AddJob("archivation_profiles");
			}
		}

		// Token: 0x06001D37 RID: 7479 RVA: 0x0007615C File Offset: 0x0007455C
		public bool? IsProfileCold(ulong profile_id)
		{
			return this.m_dalService.ColdStorageSystem.IsProfileCold(profile_id);
		}

		// Token: 0x06001D38 RID: 7480 RVA: 0x00076170 File Offset: 0x00074570
		public TouchProfileResult TouchProfile(ulong profile_id, DBVersion current_version)
		{
			TouchProfileResult touchProfileResult = this.TouchProfileImpl(profile_id, current_version);
			Log.Verbose(Log.Group.ColdStorage, "Profile {0}: {1}", new object[]
			{
				profile_id,
				touchProfileResult.Status
			});
			return touchProfileResult;
		}

		// Token: 0x06001D39 RID: 7481 RVA: 0x000761B4 File Offset: 0x000745B4
		private TouchProfileResult TouchProfileImpl(ulong profile_id, DBVersion current_version)
		{
			TouchProfileResult touchProfileResult = this.m_dalService.ColdStorageSystem.TouchProfile(profile_id, current_version);
			if (touchProfileResult.Status == ETouchProfileResult.RestoredFromCold)
			{
				this.OnProfileRestoredFromCold(profile_id, touchProfileResult.DataVersion);
				this.ReportTelemetry("srv_cold_storage_restored", 1L);
			}
			return touchProfileResult;
		}

		// Token: 0x06001D3A RID: 7482 RVA: 0x000761FB File Offset: 0x000745FB
		public bool ArchiveProfile(ulong profile_id, TimeSpan threshold)
		{
			if (this.m_dalService.ColdStorageSystem.MoveProfileToCold(profile_id, threshold, Resources.LatestDbUpdateVersion))
			{
				this.ReportTelemetry("srv_cold_storage_archived", 1L);
				return true;
			}
			return false;
		}

		// Token: 0x06001D3B RID: 7483 RVA: 0x0007622C File Offset: 0x0007462C
		public void ArchiveProfiles(TimeSpan threshold, int batch_size)
		{
			int num = 0;
			try
			{
				Log.Verbose(Log.Group.ColdStorage, "Starting profiles archivation ...", new object[0]);
				List<ulong> list = this.m_dalService.ColdStorageSystem.GetUnusedProfiles(threshold, batch_size).ToList<ulong>();
				Log.Verbose(Log.Group.ColdStorage, "Got batch of {0} expired profiles", new object[]
				{
					list.Count
				});
				foreach (ulong profile_id in list)
				{
					bool flag = this.m_dalService.ColdStorageSystem.MoveProfileToCold(profile_id, threshold, Resources.LatestDbUpdateVersion);
					if (flag)
					{
						num++;
					}
				}
				Log.Verbose(Log.Group.ColdStorage, "Profile archivation has been finished: {0} profiles archived", new object[]
				{
					num
				});
			}
			finally
			{
				if (num != 0)
				{
					this.ReportTelemetry("srv_cold_storage_archived", (long)num);
				}
			}
		}

		// Token: 0x06001D3C RID: 7484 RVA: 0x00076338 File Offset: 0x00074738
		public void DebugRestoreAllProfiles(int limit)
		{
			Log.Verbose(Log.Group.ColdStorage, "Starting profiles restore from cold ...", new object[0]);
			List<ulong> list = this.m_dalService.ColdStorageSystem.GetColdProfiles(limit).ToList<ulong>();
			Log.Verbose(Log.Group.ColdStorage, "Got batch of {0} cold profiles", new object[]
			{
				list.Count
			});
			int num = 0;
			foreach (ulong profile_id in list)
			{
				if (this.TouchProfileImpl(profile_id, Resources.LatestDbUpdateVersion).Status == ETouchProfileResult.RestoredFromCold)
				{
					num++;
				}
			}
			Log.Verbose(Log.Group.ColdStorage, "Profile restore from cold has been finished: {0} profiles restored", new object[]
			{
				num
			});
		}

		// Token: 0x06001D3D RID: 7485 RVA: 0x00076414 File Offset: 0x00074814
		public DateTime DebugUpdateLastSeenDate(ulong profileId, DateTime lastSeenDate)
		{
			if (this.m_dalService.ProfileSystem.GetProfileInfo(profileId).Id == 0UL)
			{
				Log.Warning<ulong>("Can't find profile {0} to update last seen date", profileId);
				return DateTime.MinValue;
			}
			this.m_dalService.ProfileSystem.UpdateLastSeenDate(profileId, lastSeenDate);
			Log.Verbose("Profile {0}'s last seen date has been updated", new object[]
			{
				profileId
			});
			return TimeUtils.UTCTimestampToLocalTime(this.m_dalService.ProfileSystem.GetLastSeenDate(profileId));
		}

		// Token: 0x06001D3E RID: 7486 RVA: 0x00076494 File Offset: 0x00074894
		private void ReportTelemetry(string stat, long value)
		{
			string text = DateTime.Now.ToString("yyyy-MM-dd HH:00:00");
			this.m_telemetryService.AddMeasure(value, new object[]
			{
				"stat",
				stat,
				"server",
				Resources.ServerName,
				"host",
				Resources.Hostname,
				"date",
				text
			});
		}

		// Token: 0x06001D3F RID: 7487 RVA: 0x00076500 File Offset: 0x00074900
		private void OnProfileRestoredFromCold(ulong profileId, DBVersion dataVersion)
		{
			if (dataVersion < this.REMOVE_ATTACHMENTS_DB_VERSION)
			{
				this.ClearAttachments(profileId);
			}
			if (dataVersion < this.REMOVE_QUICKPLAY_PERSISTENT_SETTINGS_DB_VERSION)
			{
				this.m_dalService.ProfileSystem.ClearPersistentSettings(profileId, "quickplay");
			}
		}

		// Token: 0x06001D40 RID: 7488 RVA: 0x0007654C File Offset: 0x0007494C
		private void ClearAttachments(ulong profileId)
		{
			Dictionary<ulong, SProfileItem> profileItems = this.m_itemService.GetProfileItems(profileId, EquipOptions.All);
			foreach (SProfileItem sprofileItem in profileItems.Values)
			{
				if (sprofileItem.Status == EProfileItemStatus.REWARD && sprofileItem.GameItem.IsAttachmentItem)
				{
					this.m_itemService.DeleteProfileItem(profileId, sprofileItem.ProfileItemID);
				}
			}
		}

		// Token: 0x04000DF7 RID: 3575
		private readonly IProfileItems m_itemService;

		// Token: 0x04000DF8 RID: 3576
		private readonly IDALService m_dalService;

		// Token: 0x04000DF9 RID: 3577
		private readonly ITelemetryService m_telemetryService;

		// Token: 0x04000DFA RID: 3578
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x04000DFB RID: 3579
		private readonly DBVersion REMOVE_ATTACHMENTS_DB_VERSION = new DBVersion(2, 53);

		// Token: 0x04000DFC RID: 3580
		private readonly DBVersion REMOVE_QUICKPLAY_PERSISTENT_SETTINGS_DB_VERSION = new DBVersion(2, 70);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000208 RID: 520
	internal class PerformanceSystemClient : DALCacheProxy<IDALService>, IPerformanceSystemClient
	{
		// Token: 0x06000B03 RID: 2819 RVA: 0x00029338 File Offset: 0x00027738
		internal void Reset(IPerformanceSystem performanceSystem)
		{
			this.m_performanceSystem = performanceSystem;
		}

		// Token: 0x06000B04 RID: 2820 RVA: 0x00029344 File Offset: 0x00027744
		public MasterRecord GetPerformanceMasterRecord(string type, cache_domain domain)
		{
			DALCacheProxy<IDALService>.Options<MasterRecord> options = new DALCacheProxy<IDALService>.Options<MasterRecord>
			{
				cache_domain = domain,
				get_data = (() => this.m_performanceSystem.GetPerformanceMasterRecord(type))
			};
			return base.GetData<MasterRecord>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B05 RID: 2821 RVA: 0x00029394 File Offset: 0x00027794
		public ProfilePerformance GetProfilePerformance(ulong profile_id, List<Guid> currentMissions)
		{
			DALCacheProxy<IDALService>.Options<ProfilePerformance> options = new DALCacheProxy<IDALService>.Options<ProfilePerformance>
			{
				cache_domain = cache_domains.profile[profile_id].mission_performance,
				get_data = (() => this.m_performanceSystem.GetProfilePerformance(profile_id, currentMissions))
			};
			ProfilePerformance data = base.GetData<ProfilePerformance>(MethodBase.GetCurrentMethod(), options);
			if (data.Missions.Any((ProfilePerformance.MissionPerfInfo m) => !currentMissions.Contains(m.MissionID)))
			{
				DALCacheProxy<IDALService>.SetOptionsBase options2 = new DALCacheProxy<IDALService>.SetOptionsBase
				{
					cache_domain = options.cache_domain
				};
				base.ClearCache(MethodBase.GetCurrentMethod(), options2);
				return base.GetData<ProfilePerformance>(MethodBase.GetCurrentMethod(), options);
			}
			return data;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00029454 File Offset: 0x00027854
		public void UpdateMissionPerformance(PerformanceUpdate update, List<Guid> currentMissions)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_performanceSystem.UpdateMissionPerformance(update, currentMissions))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
			if (update.Status == MissionStatus.Finished)
			{
				DALCacheProxy<IDALService>.CASOptions<List<MissionPerformance>> options2 = new DALCacheProxy<IDALService>.CASOptions<List<MissionPerformance>>
				{
					cache_domain = cache_domains.mission_performance,
					cas_func = delegate(List<MissionPerformance> cachedPerformance, out List<MissionPerformance> newPerformance)
					{
						bool result = false;
						newPerformance = (cachedPerformance ?? new List<MissionPerformance>());
						newPerformance.RemoveAll((MissionPerformance x) => !currentMissions.Contains(x.MissionID));
						MissionPerformance item = newPerformance.Find((MissionPerformance x) => x.MissionID == update.MissionID);
						if (item.TopTeams == null)
						{
							item = default(MissionPerformance);
							item.MissionID = update.MissionID;
							item.TopTeams = new List<MissionPerformance.TopTeam>();
							newPerformance.Add(item);
						}
						using (List<PerformanceInfo>.Enumerator enumerator = update.Performances.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								PerformanceInfo perf = enumerator.Current;
								int num = item.TopTeams.FindIndex((MissionPerformance.TopTeam x) => x.Stat == perf.Stat);
								if (num == -1 || item.TopTeams[num].Performance < perf.Performance)
								{
									result = true;
									MissionPerformance.TopTeam topTeam = default(MissionPerformance.TopTeam);
									topTeam.Stat = perf.Stat;
									topTeam.Performance = perf.Performance;
									topTeam.ProfileIds = new List<ulong>(update.ProfilesIds);
									if (num == -1)
									{
										item.TopTeams.Add(topTeam);
									}
									else
									{
										item.TopTeams[num] = topTeam;
									}
								}
							}
						}
						return result;
					}
				};
				base.CheckAndStore<List<MissionPerformance>>(MethodBase.GetCurrentMethod(), options2);
			}
			DALCacheProxy<IDALService>.SetOptionsBase setOptionsBase = new DALCacheProxy<IDALService>.SetOptionsBase();
			setOptionsBase.cache_domains = from profileId in update.ProfilesIds
			select cache_domains.profile[profileId].mission_performance;
			DALCacheProxy<IDALService>.SetOptionsBase options3 = setOptionsBase;
			base.ClearCache(MethodBase.GetCurrentMethod(), options3);
		}

		// Token: 0x06000B07 RID: 2823 RVA: 0x00029534 File Offset: 0x00027934
		public void CleanupMissionPerformance()
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_performanceSystem.CleanupMissionPerformance())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B08 RID: 2824 RVA: 0x00029568 File Offset: 0x00027968
		public bool TryBeginUpdate(string onlineId, string lockString, TimeSpan updateFreq)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_performanceSystem.TryBeginUpdate(onlineId, lockString, updateFreq))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B09 RID: 2825 RVA: 0x000295C0 File Offset: 0x000279C0
		public void EndUpdate(List<MasterRecord.Record> missionRecords, string lockString, IEnumerable<cache_domain> domains)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = domains,
				set_func = (() => this.m_performanceSystem.EndUpdate(missionRecords, lockString))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000B0A RID: 2826 RVA: 0x00029618 File Offset: 0x00027A18
		public bool SetMissionProfileWin(Guid missionId, ulong profileId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_performanceSystem.SetMissionProfileWin(missionId, profileId))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x0400055A RID: 1370
		private IPerformanceSystem m_performanceSystem;
	}
}

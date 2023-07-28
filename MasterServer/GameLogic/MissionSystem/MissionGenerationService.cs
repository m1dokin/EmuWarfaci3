using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.MissionSystem.MissionGeneration;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003B4 RID: 948
	[Service]
	[Singleton]
	internal class MissionGenerationService : ServiceModule, IMissionGenerationService, IDebugMissionGenerationService
	{
		// Token: 0x060014FC RID: 5372 RVA: 0x00056360 File Offset: 0x00054760
		public MissionGenerationService(IJobSchedulerService jobSchedulerService, IDBUpdateService dbUpdateService, IMemcachedService memcachedService, IDALService dalService, IDailyMissionGeneratorFactory dailyMissionGeneratorFactory, IRealmsMissionGeneration realmsMissionGeneration, IMissionGenerationConfigReader missionGenerationConfigReader, List<IMissionGeneratorVisitor> visitors)
		{
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_dbUpdateService = dbUpdateService;
			this.m_memcachedService = memcachedService;
			this.m_dalService = dalService;
			this.m_dailyMissionGeneratorFactory = dailyMissionGeneratorFactory;
			this.m_realmsMissionGeneration = realmsMissionGeneration;
			this.m_missionGenerationConfigReader = missionGenerationConfigReader;
			this.m_visitors = visitors;
		}

		// Token: 0x14000045 RID: 69
		// (add) Token: 0x060014FD RID: 5373 RVA: 0x000563C8 File Offset: 0x000547C8
		// (remove) Token: 0x060014FE RID: 5374 RVA: 0x00056400 File Offset: 0x00054800
		public event Action MissionSetUpdated;

		// Token: 0x14000046 RID: 70
		// (add) Token: 0x060014FF RID: 5375 RVA: 0x00056438 File Offset: 0x00054838
		// (remove) Token: 0x06001500 RID: 5376 RVA: 0x00056470 File Offset: 0x00054870
		public event Action<Guid, MissionContext> MissionExpired;

		// Token: 0x170001F3 RID: 499
		// (get) Token: 0x06001501 RID: 5377 RVA: 0x000564A6 File Offset: 0x000548A6
		// (set) Token: 0x06001502 RID: 5378 RVA: 0x000564AE File Offset: 0x000548AE
		private IDailyMissionGenerator MissionGen { get; set; }

		// Token: 0x06001503 RID: 5379 RVA: 0x000564B7 File Offset: 0x000548B7
		public override void Init()
		{
			this.m_config = this.m_missionGenerationConfigReader.Read();
			this.m_realmsMissionGeneration.SetRealmGeneration(this.m_config);
		}

		// Token: 0x06001504 RID: 5380 RVA: 0x000564DB File Offset: 0x000548DB
		public override void Stop()
		{
			this.MissionSetUpdated = null;
			this.MissionExpired = null;
		}

		// Token: 0x06001505 RID: 5381 RVA: 0x000564EC File Offset: 0x000548EC
		public override void Start()
		{
			this.MissionGen = this.m_dailyMissionGeneratorFactory.Create(this.m_config);
			Dictionary<Guid, MissionContext> expired_missions;
			this.m_missionSet = this.LoadMissionSetFromDB(out expired_missions);
			bool flag = this.MissionGen.MissionSetValid(this.m_missionSet);
			if (this.m_realmsMissionGeneration.Enabled && this.m_realmsMissionGeneration.GenerationRole)
			{
				this.PostMissionSetUpdated(expired_missions);
				if (this.MissionGen.MissionSetValid(this.m_missionSet))
				{
					this.SaveMissionSetToRealm(this.m_missionSet);
				}
				else
				{
					this.RegenerateMissionSet();
				}
			}
			else if (this.m_realmsMissionGeneration.Enabled && this.m_realmsMissionGeneration.RealmSyncRole)
			{
				this.PostMissionSetUpdated(expired_missions);
				this.ReloadMissionSetFromRealm(!flag);
			}
			else if (!this.m_realmsMissionGeneration.Enabled && this.m_realmsMissionGeneration.GenerationRole && !flag)
			{
				this.PostMissionSetUpdated(expired_missions);
				this.RegenerateMissionSet();
			}
			else if (this.m_realmsMissionGeneration.SlaveRole)
			{
				this.SyncWithMaster();
			}
			if (this.m_realmsMissionGeneration.GenerationRole)
			{
				this.m_jobSchedulerService.AddJob("mission_regeneration");
			}
			else if (this.m_realmsMissionGeneration.RealmSyncRole)
			{
				this.m_jobSchedulerService.AddJob("mission_reloading_from_realm");
			}
			else
			{
				this.m_jobSchedulerService.AddJob("mission_reloading_from_db");
			}
		}

		// Token: 0x06001506 RID: 5382 RVA: 0x00056668 File Offset: 0x00054A68
		private void SyncWithMaster()
		{
			DateTime now = DateTime.Now;
			bool flag = true;
			MissionSet missionSet;
			bool flag2;
			do
			{
				if (!flag)
				{
					Thread.Sleep(MissionGenerationService.MASTER_SYNC_SLEEP_TIMEOUT);
				}
				Log.Info("Synchronizing mission set with master...");
				Dictionary<Guid, MissionContext> dictionary;
				missionSet = this.LoadMissionSetFromDB(out dictionary);
				flag2 = this.MissionGen.MissionSetValid(missionSet);
				flag = false;
			}
			while (!flag2 && DateTime.Now - now < MissionGenerationService.MASTER_SYNC_TIMEOUT);
			if (flag2)
			{
				this.m_missionSet = missionSet;
				this.PostMissionSetUpdated(null);
			}
			else
			{
				Log.Error("Failed to sync mission set with master, continuing with empty mission set");
				this.m_missionSet = new MissionSet();
			}
		}

		// Token: 0x06001507 RID: 5383 RVA: 0x00056704 File Offset: 0x00054B04
		public MissionHash GetMissionsHash()
		{
			MissionHash result = default(MissionHash);
			object @lock = this.m_lock;
			lock (@lock)
			{
				result.missionHash = this.m_missionSet.Hash;
			}
			result.ContentHash = this.m_visitors.Aggregate(0, (int current, IMissionGeneratorVisitor visitor) => current ^ visitor.Visit()).ToString(CultureInfo.InvariantCulture);
			return result;
		}

		// Token: 0x06001508 RID: 5384 RVA: 0x0005679C File Offset: 0x00054B9C
		public Dictionary<Guid, MissionContext> GetMissions()
		{
			object @lock = this.m_lock;
			Dictionary<Guid, MissionContext> result;
			lock (@lock)
			{
				result = new Dictionary<Guid, MissionContext>(this.m_missionSet.Missions);
			}
			return result;
		}

		// Token: 0x06001509 RID: 5385 RVA: 0x000567EC File Offset: 0x00054BEC
		public void RegenerateMissionSet()
		{
			if (!this.m_realmsMissionGeneration.GenerationRole)
			{
				Log.Warning("Unexpected: RegenerateMissionSet called with no permission for mission generation");
				return;
			}
			Dictionary<Guid, MissionContext> expired_missions;
			this.DoRegenerateMissionSet(out expired_missions);
			this.PostMissionSetUpdated(expired_missions);
		}

		// Token: 0x0600150A RID: 5386 RVA: 0x00056824 File Offset: 0x00054C24
		public void ReloadMissionSetFromRealm(bool force)
		{
			if (!this.m_realmsMissionGeneration.RealmSyncRole)
			{
				Log.Warning("Unexpected: SyncMissionSet called with no sync permission");
				return;
			}
			Dictionary<Guid, MissionContext> expired_missions;
			this.SyncMissionSetWithRealm(force, out expired_missions);
			this.PostMissionSetUpdated(expired_missions);
		}

		// Token: 0x0600150B RID: 5387 RVA: 0x0005685C File Offset: 0x00054C5C
		public void ReloadMissionSetFromDB(bool force)
		{
			Dictionary<Guid, MissionContext> expired_missions;
			this.SyncMissionSetFromDB(force, out expired_missions);
			this.PostMissionSetUpdated(expired_missions);
		}

		// Token: 0x0600150C RID: 5388 RVA: 0x0005687C File Offset: 0x00054C7C
		private void DoRegenerateMissionSet(out Dictionary<Guid, MissionContext> expired_missions)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				Log.Info("Generating new mission set");
				MissionSet missionSet = this.m_missionSet;
				do
				{
					missionSet = this.MissionGen.GenerateNewMissionSet(missionSet);
				}
				while (!this.MissionGen.MissionSetValid(missionSet));
				this.SaveMissionSetToDB(missionSet, out expired_missions);
				this.SaveMissionSetToRealm(missionSet);
				this.m_missionSet = missionSet;
				this.DebugDumpMissionSetContent();
				Log.Info("Mission generation done.");
			}
		}

		// Token: 0x0600150D RID: 5389 RVA: 0x00056910 File Offset: 0x00054D10
		private void SyncMissionSetWithRealm(bool force, out Dictionary<Guid, MissionContext> expired_missions)
		{
			expired_missions = new Dictionary<Guid, MissionContext>();
			object @lock = this.m_lock;
			lock (@lock)
			{
				Log.Verbose(Log.Group.Missions, "Syncing mission set with realm", new object[0]);
				RealmMissionLoadResult realmMissionLoadResult = this.m_realmsMissionGeneration.LoadRealmMissions(force);
				if (realmMissionLoadResult.Code == LoadResult.EQUAL)
				{
					Log.Verbose(Log.Group.Missions, "Realm missions are the same, nothing to sync", new object[0]);
				}
				else if (realmMissionLoadResult.Code == LoadResult.EMPTY)
				{
					Log.Info("Realm mission set is empty");
					if (this.m_realmsMissionGeneration.GenerationRole)
					{
						this.DoRegenerateMissionSet(out expired_missions);
					}
				}
				else if (realmMissionLoadResult.Code == LoadResult.OK && !this.MissionGen.MissionSetValid(realmMissionLoadResult.MissionSet))
				{
					Log.Info("Realm mission set is invalid");
					this.m_missionSet = realmMissionLoadResult.MissionSet;
					if (this.m_realmsMissionGeneration.GenerationRole)
					{
						this.DoRegenerateMissionSet(out expired_missions);
					}
				}
				else if (realmMissionLoadResult.Code == LoadResult.OK)
				{
					Log.Info("Applying realm mission set");
					this.ValidateRealmMissionSet(realmMissionLoadResult.MissionSet);
					this.m_missionSet = realmMissionLoadResult.MissionSet;
					this.SaveMissionSetToDB(this.m_missionSet, out expired_missions);
				}
			}
		}

		// Token: 0x0600150E RID: 5390 RVA: 0x00056A68 File Offset: 0x00054E68
		private void ValidateRealmMissionSet(MissionSet realm_mission_set)
		{
			if (realm_mission_set.Generation < this.m_missionSet.Generation)
			{
				throw new Exception(string.Format("Realm missions generation {0} is smaller than local generation {1} this can cause major synchronization issues of mission set", realm_mission_set.Generation, this.m_missionSet.Generation));
			}
			if (realm_mission_set.Generation == this.m_missionSet.Generation && realm_mission_set.Hash != this.m_missionSet.Hash)
			{
				throw new Exception(string.Format("Realm missions has same generation {0} but different hash: realm {1}, local {2} this can cause major synchronization issues of mission set, exiting.", realm_mission_set.Generation, realm_mission_set.Hash, this.m_missionSet.Hash));
			}
		}

		// Token: 0x0600150F RID: 5391 RVA: 0x00056B14 File Offset: 0x00054F14
		private void SyncMissionSetFromDB(bool force, out Dictionary<Guid, MissionContext> expired_missions)
		{
			expired_missions = new Dictionary<Guid, MissionContext>();
			object @lock = this.m_lock;
			lock (@lock)
			{
				string dataGroupHash = this.m_dbUpdateService.GetDataGroupHash(MissionGenerationService.DATA_HASH_GROUP, true);
				if (force || (!dataGroupHash.Equals(MissionGenerationService.DATA_HASH_GROUP_LOCK) && this.m_missionSet.Hash != dataGroupHash))
				{
					Log.Info("Syncing mission set from DB");
					this.m_missionSet = this.LoadMissionSetFromDB(out expired_missions);
				}
			}
		}

		// Token: 0x06001510 RID: 5392 RVA: 0x00056BB0 File Offset: 0x00054FB0
		private MissionSet LoadMissionSetFromDB(out Dictionary<Guid, MissionContext> expired_missions)
		{
			Log.Info("Loading missions from the DB");
			expired_missions = new Dictionary<Guid, MissionContext>();
			MissionSet missionSet = new MissionSet();
			List<MissionContext> list = new List<MissionContext>();
			if (!this.m_realmsMissionGeneration.SlaveRole)
			{
				bool flag = true;
				while (flag)
				{
					flag = false;
					foreach (SMission i in this.m_dalService.MissionSystem.GetMissions(this.MissionGen.GetTotalGenerationPeriod()))
					{
						try
						{
							MissionParser.Parse(i);
						}
						catch (MissionParseException)
						{
							Log.Info<Guid>("Remove broken mission {0} from DB", i.ID);
							this.m_dalService.MissionSystem.RemoveMission(i.ID);
							flag = true;
						}
					}
				}
			}
			foreach (SMission m2 in this.m_dalService.MissionSystem.GetMissions(this.MissionGen.GetTotalGenerationPeriod()))
			{
				try
				{
					MissionContext item = MissionParser.Parse(m2);
					list.Add(item);
				}
				catch (MissionParseException)
				{
					Log.Warning<Guid, int>("Failed to parse mission {0} generation {1}", m2.ID, m2.Generation);
				}
			}
			missionSet.Hash = this.m_dbUpdateService.GetDataGroupHash(MissionGenerationService.DATA_HASH_GROUP, true);
			missionSet.Generation = this.m_dalService.MissionSystem.GetGeneration();
			if (list.Any<MissionContext>())
			{
				missionSet.Generation = Math.Max(missionSet.Generation, list.Max((MissionContext m) => m.Generation));
			}
			Set<Guid> set = new Set<Guid>(this.m_dalService.MissionSystem.GetCurrentMissions());
			Dictionary<Guid, MissionContext> dictionary = new Dictionary<Guid, MissionContext>();
			foreach (MissionContext missionContext in list)
			{
				Guid guid = new Guid(missionContext.uid);
				if (set.Contains(guid))
				{
					dictionary.Add(guid, missionContext);
				}
			}
			if (!this.m_realmsMissionGeneration.SlaveRole)
			{
				Set<Guid> set2 = set.Subtract(new Set<Guid>(dictionary.Keys));
				foreach (Guid uid in ((IEnumerable<Guid>)set2))
				{
					this.m_dalService.MissionSystem.RemoveCurrentMission(uid);
				}
				list.Sort((MissionContext A, MissionContext B) => B.Generation - A.Generation);
				Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
				foreach (IGrouping<string, MissionContext> grouping in from m in dictionary.Values
				group m by m.missionType.Name)
				{
					dictionary2[grouping.Key] = grouping.Count<MissionContext>();
				}
				foreach (MissionContext missionContext2 in list)
				{
					Guid guid2 = new Guid(missionContext2.uid);
					if (!dictionary2.ContainsKey(missionContext2.missionType.Name))
					{
						dictionary2[missionContext2.missionType.Name] = 0;
					}
					if (this.MissionGen.IsMissionExpired(missionContext2, missionSet.Generation))
					{
						if (dictionary.ContainsKey(guid2))
						{
							Log.Info("Removing expired mission {0} {1} {2} generation {3} from current missions table", new object[]
							{
								guid2,
								missionContext2.missionType,
								missionContext2.name,
								missionContext2.Generation
							});
							Dictionary<string, int> dictionary3;
							string name;
							(dictionary3 = dictionary2)[name = missionContext2.missionType.Name] = dictionary3[name] - 1;
							dictionary.Remove(guid2);
							this.m_dalService.MissionSystem.RemoveCurrentMission(guid2);
							expired_missions.Add(guid2, missionContext2);
						}
					}
					else if (!dictionary.ContainsKey(guid2) && dictionary2[missionContext2.missionType.Name] < this.MissionGen.GetMissionCount(missionContext2.missionType.Name))
					{
						Log.Info("Adding mission {0} {1} {2} generation {3} to current set", new object[]
						{
							guid2,
							missionContext2.missionType,
							missionContext2.name,
							missionContext2.Generation
						});
						Dictionary<string, int> dictionary3;
						string name2;
						(dictionary3 = dictionary2)[name2 = missionContext2.missionType.Name] = dictionary3[name2] + 1;
						dictionary.Add(guid2, missionContext2);
						this.m_dalService.MissionSystem.AddCurrentMission(guid2);
					}
				}
			}
			missionSet.Missions = new Dictionary<Guid, MissionContext>();
			foreach (Guid key in dictionary.Keys)
			{
				if (this.MissionGen.ValidateMissionContext(dictionary[key]))
				{
					missionSet.Missions.Add(key, dictionary[key]);
				}
			}
			return missionSet;
		}

		// Token: 0x06001511 RID: 5393 RVA: 0x00057228 File Offset: 0x00055628
		private void SaveMissionSetToDB(MissionSet new_set, out Dictionary<Guid, MissionContext> expired_missions)
		{
			expired_missions = new Dictionary<Guid, MissionContext>();
			Set<Guid> set = new Set<Guid>(this.m_dalService.MissionSystem.GetCurrentMissions());
			Set<Guid> set2 = new Set<Guid>(new_set.Missions.Keys);
			this.m_dbUpdateService.SetDataGroupHash(MissionGenerationService.DATA_HASH_GROUP, MissionGenerationService.DATA_HASH_GROUP_LOCK);
			foreach (KeyValuePair<Guid, MissionContext> keyValuePair in new_set.Missions)
			{
				Guid key = keyValuePair.Key;
				MissionContext value = keyValuePair.Value;
				this.m_dalService.MissionSystem.SaveMission(key, value.name, value.gameMode, value.data, value.Generation);
			}
			this.m_dalService.MissionSystem.SaveGeneration(new_set.Generation);
			Set<Guid> set3 = set.Subtract(set2);
			Set<Guid> set4 = set2.Subtract(set);
			foreach (Guid guid in ((IEnumerable<Guid>)set3))
			{
				this.m_dalService.MissionSystem.RemoveCurrentMission(guid);
				MissionContext mission = this.GetMission(guid);
				if (mission != null)
				{
					expired_missions.Add(guid, mission);
				}
			}
			foreach (Guid uid in ((IEnumerable<Guid>)set4))
			{
				this.m_dalService.MissionSystem.AddCurrentMission(uid);
			}
			this.m_dbUpdateService.SetDataGroupHash(MissionGenerationService.DATA_HASH_GROUP, new_set.Hash);
		}

		// Token: 0x06001512 RID: 5394 RVA: 0x00057404 File Offset: 0x00055804
		private void SaveMissionSetToRealm(MissionSet new_set)
		{
			Log.Info("Saving mission set to realm storage");
			this.m_realmsMissionGeneration.SaveRealmMissions(new_set);
		}

		// Token: 0x06001513 RID: 5395 RVA: 0x00057420 File Offset: 0x00055820
		private void PostMissionSetUpdated(Dictionary<Guid, MissionContext> expired_missions)
		{
			this.DebugDumpMissionSet();
			if (!this.m_realmsMissionGeneration.SlaveRole)
			{
				foreach (KeyValuePair<Guid, MissionContext> keyValuePair in expired_missions)
				{
					this.NotifyOnMissionExpired(keyValuePair.Key, keyValuePair.Value);
				}
				this.m_dalService.PerformanceSystem.CleanupMissionPerformance();
				if (this.m_memcachedService != null && this.m_memcachedService.Connected)
				{
					this.m_memcachedService.Remove(cache_domains.mission_performance);
				}
			}
			else if (expired_missions != null && expired_missions.Count != 0)
			{
				Log.Error("Slave MasterServer somehow expired missions");
			}
			this.NotifyOnMissionUpdate();
		}

		// Token: 0x06001514 RID: 5396 RVA: 0x000574FC File Offset: 0x000558FC
		private void NotifyOnMissionExpired(Guid missionKey, MissionContext mission)
		{
			try
			{
				if (this.MissionExpired != null)
				{
					this.MissionExpired(missionKey, mission);
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06001515 RID: 5397 RVA: 0x00057544 File Offset: 0x00055944
		private void NotifyOnMissionUpdate()
		{
			try
			{
				if (this.MissionSetUpdated != null)
				{
					this.MissionSetUpdated();
				}
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x06001516 RID: 5398 RVA: 0x00057588 File Offset: 0x00055988
		public void DebugValidateMissionGraphs()
		{
			this.MissionGen.DebugValidateMissionGraphs();
		}

		// Token: 0x06001517 RID: 5399 RVA: 0x00057595 File Offset: 0x00055995
		public void DebugEmulateRotation(int elementsNum, int shufflesNum)
		{
			this.MissionGen.DebugEmulateRotation(elementsNum, shufflesNum);
		}

		// Token: 0x06001518 RID: 5400 RVA: 0x000575A4 File Offset: 0x000559A4
		public void DebugDumpMissionSet()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("Current generation: {0}\n", this.m_missionSet.Generation);
			stringBuilder.AppendFormat("Mission hash: {0}\n", this.m_missionSet.Hash);
			stringBuilder.Append("Mission set:\n");
			int num = 1;
			foreach (MissionContext missionContext in this.m_missionSet.Missions.Values)
			{
				stringBuilder.AppendFormat("{0}. {1} - {2} [{3}] generation {4}\n", new object[]
				{
					num++,
					missionContext.name,
					missionContext.missionType,
					missionContext.uid,
					missionContext.Generation
				});
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x06001519 RID: 5401 RVA: 0x000576A0 File Offset: 0x00055AA0
		public void DebugDumpMissionSetContent()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("Mission set:");
			foreach (MissionContext missionContext in this.m_missionSet.Missions.Values)
			{
				stringBuilder.AppendLine(missionContext.data);
			}
			Log.Info(stringBuilder.ToString());
		}

		// Token: 0x0600151A RID: 5402 RVA: 0x0005772C File Offset: 0x00055B2C
		private MissionContext GetMission(Guid uid)
		{
			SMission mission = this.m_dalService.MissionSystem.GetMission(uid);
			return string.IsNullOrEmpty(mission.Data) ? null : MissionParser.Parse(mission);
		}

		// Token: 0x040009D9 RID: 2521
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040009DA RID: 2522
		private readonly IDBUpdateService m_dbUpdateService;

		// Token: 0x040009DB RID: 2523
		private readonly IMemcachedService m_memcachedService;

		// Token: 0x040009DC RID: 2524
		private readonly IDALService m_dalService;

		// Token: 0x040009DD RID: 2525
		private readonly IDailyMissionGeneratorFactory m_dailyMissionGeneratorFactory;

		// Token: 0x040009DE RID: 2526
		private readonly IRealmsMissionGeneration m_realmsMissionGeneration;

		// Token: 0x040009DF RID: 2527
		private readonly IMissionGenerationConfigReader m_missionGenerationConfigReader;

		// Token: 0x040009E0 RID: 2528
		public static readonly string DATA_HASH_GROUP = "mission_set";

		// Token: 0x040009E1 RID: 2529
		public static readonly string DATA_HASH_GROUP_LOCK = "mission_set.lock";

		// Token: 0x040009E2 RID: 2530
		private static readonly TimeSpan MASTER_SYNC_TIMEOUT = new TimeSpan(0, 0, 30);

		// Token: 0x040009E3 RID: 2531
		private static readonly TimeSpan MASTER_SYNC_SLEEP_TIMEOUT = new TimeSpan(0, 0, 5);

		// Token: 0x040009E7 RID: 2535
		private object m_lock = new object();

		// Token: 0x040009E8 RID: 2536
		private Config m_config;

		// Token: 0x040009E9 RID: 2537
		private MissionSet m_missionSet = new MissionSet();

		// Token: 0x040009EA RID: 2538
		private readonly List<IMissionGeneratorVisitor> m_visitors;
	}
}

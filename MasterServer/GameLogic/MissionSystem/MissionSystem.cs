using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x02000791 RID: 1937
	[Service]
	[Singleton]
	internal class MissionSystem : ServiceModule, IMissionSystem
	{
		// Token: 0x0600281D RID: 10269 RVA: 0x000AC0C0 File Offset: 0x000AA4C0
		public MissionSystem(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x170003BA RID: 954
		// (get) Token: 0x0600281E RID: 10270 RVA: 0x000AC121 File Offset: 0x000AA521
		public MissionGenerator MissionGenerator
		{
			get
			{
				return this.m_missionGenerator;
			}
		}

		// Token: 0x170003BB RID: 955
		// (get) Token: 0x0600281F RID: 10271 RVA: 0x000AC129 File Offset: 0x000AA529
		public MissionGraphRepository MissionGraphRepository
		{
			get
			{
				return this.m_missionGraphRepository;
			}
		}

		// Token: 0x170003BC RID: 956
		// (get) Token: 0x06002820 RID: 10272 RVA: 0x000AC131 File Offset: 0x000AA531
		public SubMissionConfigRepository SubMissionConfigRepository
		{
			get
			{
				return this.m_submissionConfigRepository;
			}
		}

		// Token: 0x170003BD RID: 957
		// (get) Token: 0x06002821 RID: 10273 RVA: 0x000AC139 File Offset: 0x000AA539
		public ObjectivesRepository ObjectivesRepository
		{
			get
			{
				return this.m_objectivesRepository;
			}
		}

		// Token: 0x06002822 RID: 10274 RVA: 0x000AC144 File Offset: 0x000AA544
		public override void Init()
		{
			base.Init();
			this.m_missionGenerator = new MissionGenerator();
			this.m_missionGraphRepository = new MissionGraphRepository();
			this.m_submissionConfigRepository = new SubMissionConfigRepository();
			this.m_objectivesRepository = new ObjectivesRepository();
			this.m_missionGenerationService = ServicesManager.GetService<IMissionGenerationService>();
			this.m_missionGenerationService.MissionSetUpdated += this.OnMissionsUpdated;
			this.InitPackMissions();
			this.ReloadGeneratedMissions();
		}

		// Token: 0x06002823 RID: 10275 RVA: 0x000AC1B4 File Offset: 0x000AA5B4
		public override void Start()
		{
			base.Start();
			List<MissionContextBase> matchmakingMissions = this.GetMatchmakingMissions();
			Log.Info<string>("[MissionSystem] Matchmaking missions: {0}", string.Join("; ", (from m in matchmakingMissions
			select m.ToString()).ToArray<string>()));
		}

		// Token: 0x06002824 RID: 10276 RVA: 0x000AC20A File Offset: 0x000AA60A
		public override void Stop()
		{
			this.m_tempMissions.Dispose();
			this.m_missionGenerationService.MissionSetUpdated -= this.OnMissionsUpdated;
			base.Stop();
		}

		// Token: 0x06002825 RID: 10277 RVA: 0x000AC234 File Offset: 0x000AA634
		public void OnMissionsUpdated()
		{
			this.ReloadGeneratedMissions();
		}

		// Token: 0x06002826 RID: 10278 RVA: 0x000AC23C File Offset: 0x000AA63C
		private void ReloadGeneratedMissions()
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				Dictionary<Guid, MissionContext> missions = this.m_missionGenerationService.GetMissions();
				this.m_generatedMissions = missions.Values.ToDictionary((MissionContext e) => e.uid, (MissionContext e) => e);
				this.m_basicMissionsInfo = (from ctx in this.m_packMissions.Values
				where ctx.tutorialMission == 0 && ctx.releaseMission
				select ctx).Concat(from ctx in this.m_generatedMissions.Values
				where ctx.tutorialMission == 0 && ctx.releaseMission
				select ctx).ToList<MissionContextBase>();
			}
		}

		// Token: 0x06002827 RID: 10279 RVA: 0x000AC38C File Offset: 0x000AA78C
		private void InitPackMissions()
		{
			object @lock = this.m_lock;
			bool flag = false;
			try
			{
				Monitor.Enter(@lock, ref flag);
				bool flag2 = false;
				DirectoryInfo missionsDir = null;
				Utils.Retry(delegate
				{
					missionsDir = new DirectoryInfo(Resources.GetResourceFullPath(Resources.ResFiles.MISSIONS_DIR));
					return true;
				});
				string[] array = new string[8];
				foreach (FileInfo fileInfo in missionsDir.GetFiles())
				{
					string xml = File.ReadAllText(fileInfo.FullName);
					MissionContext missionContext = this.m_parser.ParseMissionData(xml);
					missionContext.missionName = fileInfo.Name.Split(new char[]
					{
						'.'
					})[0];
					bool flag3 = true;
					if (missionContext.tutorialMission != 0)
					{
						if (missionContext.tutorialMission < 9 && missionContext.tutorialMission > 0)
						{
							if (!string.IsNullOrEmpty(array[missionContext.tutorialMission - 1]))
							{
								Log.Warning<string, string, int>("Duplicate tutorials: {0} and {1} have same id: {2}", array[missionContext.tutorialMission], missionContext.name, missionContext.tutorialMission);
								flag3 = false;
							}
							else
							{
								array[missionContext.tutorialMission - 1] = missionContext.name;
							}
						}
						else
						{
							Log.Warning<string, int>("Tutorial mission {0} have wrong id {1} should be 1..8", missionContext.name, missionContext.tutorialMission);
							flag3 = false;
						}
					}
					if (this.m_packMissions.ContainsKey(missionContext.uid))
					{
						Log.Warning<string, string>("Key {1} already used, mission {0} will be ignored", missionContext.uid, missionContext.name);
						flag3 = false;
					}
					if (flag3)
					{
						if (!flag2 && missionContext.clanWarMission && missionContext.releaseMission)
						{
							flag2 = MissionSystem.MissionAvailable(missionContext);
						}
						this.m_packMissions.Add(missionContext.uid, missionContext);
					}
				}
				if (!flag2 && Resources.ChannelTypes.IsPvP(Resources.Channel))
				{
					throw new ApplicationException("At least 1 clan war mission available needed to start MS");
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(@lock);
				}
			}
		}

		// Token: 0x06002828 RID: 10280 RVA: 0x000AC598 File Offset: 0x000AA998
		public void AddMission(string key, string data)
		{
			if (!this.m_tempMissions.ContainsKey(key))
			{
				MissionContext data2 = this.m_parser.ParseMissionData(data);
				this.m_tempMissions.Add(key, data2);
			}
		}

		// Token: 0x06002829 RID: 10281 RVA: 0x000AC5D1 File Offset: 0x000AA9D1
		public void ResetUserMissions()
		{
			this.m_tempMissions.Clear();
		}

		// Token: 0x0600282A RID: 10282 RVA: 0x000AC5DE File Offset: 0x000AA9DE
		public bool IsUserMission(string key)
		{
			return this.m_tempMissions.ContainsKey(key);
		}

		// Token: 0x0600282B RID: 10283 RVA: 0x000AC5EC File Offset: 0x000AA9EC
		public MissionContext GetMission(string key)
		{
			if (this.m_tempMissions.ContainsKey(key))
			{
				return this.m_tempMissions.Get(key);
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				MissionContext result;
				if (this.m_packMissions.TryGetValue(key, out result))
				{
					return result;
				}
				if (this.m_generatedMissions.TryGetValue(key, out result))
				{
					return result;
				}
			}
			IEnumerable<SMission> missions = this.m_dalService.MissionSystem.GetMissions(100);
			foreach (SMission m in missions)
			{
				if (m.ID.ToString() == key)
				{
					return MissionParser.Parse(m);
				}
			}
			return null;
		}

		// Token: 0x0600282C RID: 10284 RVA: 0x000AC700 File Offset: 0x000AAB00
		public List<MissionContext> MatchMission(string key)
		{
			List<MissionContext> matches = new List<MissionContext>();
			CacheDictionary<string, MissionContext>.EnumerateDelegate enumerateDelegate = delegate(string k, MissionContext ctx)
			{
				if (ctx.uid.ToLower().StartsWith(key) || string.Equals(ctx.missionName, key, StringComparison.InvariantCulture))
				{
					matches.Add(ctx);
				}
				return true;
			};
			this.m_tempMissions.Enumerate(enumerateDelegate);
			object @lock = this.m_lock;
			lock (@lock)
			{
				foreach (MissionContext data in this.m_packMissions.Values)
				{
					enumerateDelegate(string.Empty, data);
				}
				foreach (MissionContext data2 in this.m_generatedMissions.Values)
				{
					enumerateDelegate(string.Empty, data2);
				}
			}
			return matches;
		}

		// Token: 0x0600282D RID: 10285 RVA: 0x000AC828 File Offset: 0x000AAC28
		public bool IsMissionExpired(string key)
		{
			if (this.m_tempMissions.ContainsKey(key))
			{
				return false;
			}
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_packMissions.ContainsKey(key) || this.m_generatedMissions.ContainsKey(key))
				{
					return false;
				}
			}
			return true;
		}

		// Token: 0x0600282E RID: 10286 RVA: 0x000AC8A4 File Offset: 0x000AACA4
		public List<MissionContextBase> GetMatchmakingMissions()
		{
			object @lock = this.m_lock;
			List<MissionContextBase> result;
			lock (@lock)
			{
				result = new List<MissionContextBase>(this.m_basicMissionsInfo);
			}
			return result;
		}

		// Token: 0x0600282F RID: 10287 RVA: 0x000AC8F0 File Offset: 0x000AACF0
		public void Dump()
		{
			StringBuilder sb = new StringBuilder();
			CacheDictionary<string, MissionContext>.EnumerateDelegate enumerateDelegate = delegate(string key, MissionContext ctx)
			{
				sb.AppendFormat("* \"{0}\" {1} {2} ({3}) \n", new object[]
				{
					ctx.name,
					ctx.gameMode,
					ctx.difficulty,
					ctx.uid
				});
				foreach (SubLevel subLevel in ctx.subLevels)
				{
					sb.AppendFormat("    {0}: {1} reward: ", subLevel.id, subLevel.name);
					sb.AppendLine(subLevel.pool.ToString());
				}
				return true;
			};
			sb.AppendLine("\n================ Added missions ================");
			this.m_tempMissions.Enumerate(enumerateDelegate);
			object @lock = this.m_lock;
			lock (@lock)
			{
				sb.AppendLine("================ Pack missions ================");
				foreach (MissionContext data in this.m_packMissions.Values)
				{
					enumerateDelegate(string.Empty, data);
				}
				sb.AppendLine("================ Generated missions ================");
				foreach (MissionContext data2 in this.m_generatedMissions.Values)
				{
					enumerateDelegate(string.Empty, data2);
				}
				sb.AppendLine("================ Matchmaking missions ================");
				foreach (MissionContextBase missionContextBase in this.m_basicMissionsInfo)
				{
					sb.AppendFormat("* \"{0}\" {1} {2} ({3}) \n", new object[]
					{
						missionContextBase.name,
						missionContextBase.gameMode,
						missionContextBase.difficulty,
						missionContextBase.uid
					});
				}
			}
			Log.Info(sb.ToString());
		}

		// Token: 0x06002830 RID: 10288 RVA: 0x000ACB14 File Offset: 0x000AAF14
		public void Dump(string key)
		{
			key = key.ToLower();
			List<MissionContext> matches = new List<MissionContext>();
			CacheDictionary<string, MissionContext>.EnumerateDelegate enumerateDelegate = delegate(string k, MissionContext ctx)
			{
				if (ctx.uid.ToLower().StartsWith(key))
				{
					matches.Add(ctx);
				}
				return true;
			};
			this.m_tempMissions.Enumerate(enumerateDelegate);
			object @lock = this.m_lock;
			lock (@lock)
			{
				foreach (MissionContext data in this.m_packMissions.Values)
				{
					enumerateDelegate(string.Empty, data);
				}
				foreach (MissionContext data2 in this.m_generatedMissions.Values)
				{
					enumerateDelegate(string.Empty, data2);
				}
			}
			foreach (MissionContext missionContext in matches)
			{
				Log.Info(missionContext.GetFormattedXml());
			}
		}

		// Token: 0x06002831 RID: 10289 RVA: 0x000ACC98 File Offset: 0x000AB098
		public static bool MissionAvailable(MissionContext ctx)
		{
			return ctx.channels.Contains(Resources.Channel);
		}

		// Token: 0x04001502 RID: 5378
		private const int CACHE_TIMEOUT_SEC = 10800;

		// Token: 0x04001503 RID: 5379
		private const int FALLBACK_GENERATION_PERIOD = 100;

		// Token: 0x04001504 RID: 5380
		private readonly object m_lock = new object();

		// Token: 0x04001505 RID: 5381
		private CacheDictionary<string, MissionContext> m_tempMissions = new CacheDictionary<string, MissionContext>(10800);

		// Token: 0x04001506 RID: 5382
		private Dictionary<string, MissionContext> m_packMissions = new Dictionary<string, MissionContext>();

		// Token: 0x04001507 RID: 5383
		private Dictionary<string, MissionContext> m_generatedMissions = new Dictionary<string, MissionContext>();

		// Token: 0x04001508 RID: 5384
		private List<MissionContextBase> m_basicMissionsInfo = new List<MissionContextBase>();

		// Token: 0x04001509 RID: 5385
		private MissionParser m_parser = new MissionParser();

		// Token: 0x0400150A RID: 5386
		private MissionGenerator m_missionGenerator;

		// Token: 0x0400150B RID: 5387
		private MissionGraphRepository m_missionGraphRepository;

		// Token: 0x0400150C RID: 5388
		private SubMissionConfigRepository m_submissionConfigRepository;

		// Token: 0x0400150D RID: 5389
		private ObjectivesRepository m_objectivesRepository;

		// Token: 0x0400150E RID: 5390
		private IMissionGenerationService m_missionGenerationService;

		// Token: 0x0400150F RID: 5391
		private readonly IDALService m_dalService;
	}
}

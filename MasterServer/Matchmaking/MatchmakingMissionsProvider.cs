using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000508 RID: 1288
	[Service]
	[Singleton]
	internal class MatchmakingMissionsProvider : ServiceModule, IMatchmakingMissionsProvider
	{
		// Token: 0x06001BD2 RID: 7122 RVA: 0x000709A4 File Offset: 0x0006EDA4
		public MatchmakingMissionsProvider(IMissionSystem missionSystem, IMatchmakingConfigProvider configProvider)
		{
			this.m_missionSystem = missionSystem;
			this.m_matchmakingConfigProvider = configProvider;
		}

		// Token: 0x170002E2 RID: 738
		// (get) Token: 0x06001BD3 RID: 7123 RVA: 0x000709F4 File Offset: 0x0006EDF4
		public IEnumerable<string> AutostartMissions
		{
			get
			{
				return (!this.m_matchmakingConfigProvider.Get().IsAutostartEnabled) ? Enumerable.Empty<string>() : this.m_autostartMissions;
			}
		}

		// Token: 0x170002E3 RID: 739
		// (get) Token: 0x06001BD4 RID: 7124 RVA: 0x00070A29 File Offset: 0x0006EE29
		public IEnumerable<string> PvpMissions
		{
			get
			{
				return this.m_pvpMissions;
			}
		}

		// Token: 0x170002E4 RID: 740
		// (get) Token: 0x06001BD5 RID: 7125 RVA: 0x00070A31 File Offset: 0x0006EE31
		public IEnumerable<string> RatingGameMissions
		{
			get
			{
				return this.m_ratingGameMissions;
			}
		}

		// Token: 0x06001BD6 RID: 7126 RVA: 0x00070A39 File Offset: 0x0006EE39
		public override void Start()
		{
			base.Start();
			this.m_matchmakingConfigProvider.OnConfigChanged += this.OnConfigChanged;
			this.ReloadMapLists();
			this.ValidateQuickplayChannelsConfiguration();
		}

		// Token: 0x06001BD7 RID: 7127 RVA: 0x00070A64 File Offset: 0x0006EE64
		public override void Stop()
		{
			this.m_matchmakingConfigProvider.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x06001BD8 RID: 7128 RVA: 0x00070A84 File Offset: 0x0006EE84
		public void ReloadMapLists()
		{
			if (!Resources.ChannelTypes.IsPvP(Resources.Channel))
			{
				return;
			}
			this.m_matchmakingMissions = this.m_missionSystem.GetMatchmakingMissions();
			this.FillPvpMissionsList();
			List<ConfigSection> source;
			if (!Resources.QuickplayConfig.TryGetSections("channel", out source))
			{
				if (!this.m_autostartMissions.Any<string>())
				{
					Log.Warning("[MatchmakingMissionsProvider] Config file can't be loaded, disabling Autostart");
				}
				return;
			}
			ConfigSection configSection = source.FirstOrDefault((ConfigSection sec) => string.Equals(sec.Get("type"), Resources.ChannelName, StringComparison.OrdinalIgnoreCase));
			if (configSection != null)
			{
				this.FillPvpAutostartMissionsList(configSection);
				this.FillRatingMissionsList(configSection);
			}
			else
			{
				Log.Warning<string>("[MatchmakingMissionsProvider] There is no map list for channel: {0} in quickplay_maps.xml", Resources.ChannelName);
				Interlocked.Exchange<IList<string>>(ref this.m_autostartMissions, new List<string>());
				Interlocked.Exchange<IList<string>>(ref this.m_ratingGameMissions, new List<string>());
			}
		}

		// Token: 0x06001BD9 RID: 7129 RVA: 0x00070B58 File Offset: 0x0006EF58
		private void FillPvpMissionsList()
		{
			List<string> value = (from m in this.m_matchmakingMissions
			where m.IsPvPMode()
			select m.uid).ToList<string>();
			Interlocked.Exchange<IList<string>>(ref this.m_pvpMissions, value);
		}

		// Token: 0x06001BDA RID: 7130 RVA: 0x00070BC4 File Offset: 0x0006EFC4
		private void FillPvpAutostartMissionsList(ConfigSection channelSec)
		{
			ConfigSection section = channelSec.GetSection("autostart_maps");
			if (section == null)
			{
				Log.Warning<string>("[MatchmakingMissionsProvider] There is no autostart_maps specified for channel: {0} in quickplay_maps.xml", Resources.ChannelName);
				return;
			}
			List<string> value = this.ReloadMapListInternal(section);
			Interlocked.Exchange<IList<string>>(ref this.m_autostartMissions, value);
		}

		// Token: 0x06001BDB RID: 7131 RVA: 0x00070C08 File Offset: 0x0006F008
		private void FillRatingMissionsList(ConfigSection channelSec)
		{
			ConfigSection section = channelSec.GetSection("rating_game_maps");
			if (section == null)
			{
				if (Resources.Channel != Resources.ChannelType.PVP_Newbie)
				{
					Log.Warning<string>("[MatchmakingMissionsProvider] There is no rating_missions specified for channel: {0} in quickplay_maps.xml", Resources.ChannelName);
				}
				return;
			}
			if (Resources.Channel == Resources.ChannelType.PVP_Newbie)
			{
				throw new ApplicationException("There is set up for rating games for the newbie channel in quickplay_maps.xml");
			}
			List<string> value = this.ReloadMapListInternal(section);
			Interlocked.Exchange<IList<string>>(ref this.m_ratingGameMissions, value);
		}

		// Token: 0x06001BDC RID: 7132 RVA: 0x00070C70 File Offset: 0x0006F070
		private List<string> ReloadMapListInternal(ConfigSection channelSec)
		{
			List<ConfigSection> list;
			if (!channelSec.TryGetSections("map", out list))
			{
				return new List<string>();
			}
			HashSet<string> hashSet = new HashSet<string>();
			List<string> list2 = new List<string>();
			foreach (ConfigSection configSection in list)
			{
				string mapName = configSection.Get("name");
				string gameMode = configSection.Get("mode");
				if (string.Equals(channelSec.Name, "rating_game_maps") && !MatchmakingMissionsProvider.m_ratingGamesAllowedGameModes.Contains(gameMode))
				{
					throw new ApplicationException(string.Format("Map: {0} with game mode: {1} couldn't be set up for rating game", mapName, gameMode));
				}
				MissionContextBase missionContextBase = this.m_matchmakingMissions.ToList<MissionContextBase>().Find((MissionContextBase ctx) => string.Equals(ctx.gameMode, gameMode, StringComparison.OrdinalIgnoreCase) && string.Equals(((MissionContext)ctx).missionName, mapName, StringComparison.OrdinalIgnoreCase));
				if (missionContextBase == null)
				{
					Log.Warning<string, string, string>("[MatchmakingMissionsProvider] Can't find mission {0},{1} in section {2} from quickplay_maps.xml in Matchmaking missions list", mapName, gameMode, channelSec.Name);
				}
				else if (!hashSet.Add(missionContextBase.uid))
				{
					Log.Warning<string, string, string>("[MatchmakingMissionsProvider] Duplicated entry detected for {0},{1} in section {2} from quickplay_maps.xml", mapName, gameMode, channelSec.Name);
				}
				else
				{
					list2.Add(missionContextBase.uid);
				}
			}
			return list2;
		}

		// Token: 0x06001BDD RID: 7133 RVA: 0x00070DF4 File Offset: 0x0006F1F4
		private void ValidateQuickplayChannelsConfiguration()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Quickplay.ChannelConfiguration");
			if (section == null)
			{
				throw new ApplicationException("Can't find 'ModuleSettings.Quickplay.ChannelConfiguration' section");
			}
			string[] source = new string[]
			{
				"browser_default",
				"quickplay_default",
				"quickplay_only"
			};
			if (!source.Contains(section.Get("Newbie")) || !source.Contains(section.Get("Skilled")) || !source.Contains(section.Get("Pro")))
			{
				throw new ApplicationException("Found unsupported values in 'ModuleSettings.Quickplay.ChannelConfiguration' section");
			}
		}

		// Token: 0x06001BDE RID: 7134 RVA: 0x00070E8E File Offset: 0x0006F28E
		private void OnConfigChanged(MatchmakingConfig config)
		{
			if (config.IsAutostartEnabled)
			{
				this.ReloadMapLists();
			}
		}

		// Token: 0x04000D50 RID: 3408
		private static readonly string[] m_ratingGamesAllowedGameModes = new string[]
		{
			"ptb",
			"ctf",
			"tdm",
			"tbs"
		};

		// Token: 0x04000D51 RID: 3409
		private readonly IMissionSystem m_missionSystem;

		// Token: 0x04000D52 RID: 3410
		private readonly IMatchmakingConfigProvider m_matchmakingConfigProvider;

		// Token: 0x04000D53 RID: 3411
		private IList<MissionContextBase> m_matchmakingMissions = new List<MissionContextBase>();

		// Token: 0x04000D54 RID: 3412
		private IList<string> m_autostartMissions = new List<string>();

		// Token: 0x04000D55 RID: 3413
		private IList<string> m_pvpMissions = new List<string>();

		// Token: 0x04000D56 RID: 3414
		private IList<string> m_ratingGameMissions = new List<string>();
	}
}

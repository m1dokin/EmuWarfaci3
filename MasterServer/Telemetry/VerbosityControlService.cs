using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameLogic.MissionSystem;

namespace MasterServer.Telemetry
{
	// Token: 0x02000737 RID: 1847
	[Service]
	[Singleton]
	public class VerbosityControlService : ServiceModule, IVerbosityControlService
	{
		// Token: 0x06002632 RID: 9778 RVA: 0x000A1BD0 File Offset: 0x0009FFD0
		public override void Init()
		{
			base.Init();
			ConfigSection section = Resources.ModuleSettings.GetSection("TelemetryVerbosityControl");
			section.OnConfigChanged += this.OnConfigChanged;
			this.m_defaultVebosityLevel = (VerbosityLevel)Enum.Parse(typeof(VerbosityLevel), section.Get("default_level"), true);
			ConfigSection section2 = section.GetSection("GameModeProbabilities");
			section2.OnConfigChanged += this.OnConfigChanged;
			foreach (KeyValuePair<string, ConfigValue> keyValuePair in section2.GetAllValues())
			{
				string text = keyValuePair.Key.ToLower();
				uint probability;
				if (!uint.TryParse(keyValuePair.Value.GetString(), out probability))
				{
					Log.Warning<string, string>("Incorrect log probability '{0}' for game mode '{1}'. Default log level will be used.", keyValuePair.Value.GetString(), text);
				}
				else
				{
					this.m_currentMissionCount.Add(text, new VerbosityControlService.Map(probability));
				}
			}
		}

		// Token: 0x06002633 RID: 9779 RVA: 0x000A1CE8 File Offset: 0x000A00E8
		public VerbosityLevel GetVerbosityLevel(string gameMode, string difficulty, string mapName)
		{
			IMissionSystem service = ServicesManager.GetService<IMissionSystem>();
			if (!service.IsUserMission(mapName))
			{
				string key = string.Format("{0}_{1}", gameMode, difficulty).ToLower();
				object sync = this.m_sync;
				lock (sync)
				{
					VerbosityControlService.Map map;
					if (this.m_currentMissionCount.TryGetValue(key, out map) && map.Probability > 0U)
					{
						string text = mapName.ToLower();
						if (!map.MapCount.ContainsKey(text))
						{
							map.MapCount.Add(mapName.ToLower(), 0U);
						}
						Dictionary<string, uint> mapCount;
						string key2;
						(mapCount = map.MapCount)[key2 = text] = mapCount[key2] + 1U;
						if (map.MapCount[text] % map.Probability == 0U)
						{
							return VerbosityLevel.Low;
						}
					}
				}
			}
			return this.m_defaultVebosityLevel;
		}

		// Token: 0x06002634 RID: 9780 RVA: 0x000A1DE4 File Offset: 0x000A01E4
		private void OnConfigChanged(ConfigEventArgs e)
		{
			object sync = this.m_sync;
			lock (sync)
			{
				if (string.Equals(e.Name, "default_level", StringComparison.CurrentCultureIgnoreCase))
				{
					this.m_defaultVebosityLevel = (VerbosityLevel)Enum.Parse(typeof(VerbosityLevel), e.sValue, true);
					Log.Info<VerbosityLevel>("Default verbosity level has been changed to '{0}'", this.m_defaultVebosityLevel);
				}
				else
				{
					string text = e.Name.ToLower();
					uint num;
					if (!uint.TryParse(e.sValue, out num))
					{
						Log.Warning<string, string>("Incorrect log probability '{0}' for game mode '{1}'. Default log level will be used.", e.sValue, text);
					}
					VerbosityControlService.Map map;
					if (this.m_currentMissionCount.TryGetValue(text, out map))
					{
						map.Probability = num;
					}
					else
					{
						this.m_currentMissionCount.Add(text, new VerbosityControlService.Map(num));
					}
					Log.Info<string, uint>("{0}'s log probability has been changed to '{1}'", text, num);
				}
			}
		}

		// Token: 0x040013A7 RID: 5031
		private VerbosityLevel m_defaultVebosityLevel;

		// Token: 0x040013A8 RID: 5032
		private readonly Dictionary<string, VerbosityControlService.Map> m_currentMissionCount = new Dictionary<string, VerbosityControlService.Map>();

		// Token: 0x040013A9 RID: 5033
		private readonly object m_sync = new object();

		// Token: 0x02000738 RID: 1848
		private class Map
		{
			// Token: 0x06002635 RID: 9781 RVA: 0x000A1EDC File Offset: 0x000A02DC
			public Map(uint probability)
			{
				this.Probability = probability;
			}

			// Token: 0x040013AA RID: 5034
			public uint Probability;

			// Token: 0x040013AB RID: 5035
			public readonly Dictionary<string, uint> MapCount = new Dictionary<string, uint>();
		}
	}
}

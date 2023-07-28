using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.GameRoomSystem;
using Util.Common;

namespace MasterServer.GameRoom.RoomExtensions.Reconnect
{
	// Token: 0x020004C9 RID: 1225
	[Service]
	[Singleton]
	internal class ReconnectConfigProvider : ServiceModule, IConfigProvider<ReconnectConfig>
	{
		// Token: 0x14000064 RID: 100
		// (add) Token: 0x06001A86 RID: 6790 RVA: 0x0006CE18 File Offset: 0x0006B218
		// (remove) Token: 0x06001A87 RID: 6791 RVA: 0x0006CE50 File Offset: 0x0006B250
		public event Action<ReconnectConfig> Changed;

		// Token: 0x06001A88 RID: 6792 RVA: 0x0006CE88 File Offset: 0x0006B288
		public override void Init()
		{
			ConfigSection reconnectConfig = this.GetReconnectConfig();
			reconnectConfig.OnConfigChanged += this.OnConfigChanged;
			this.m_config = this.ParseConfigData(reconnectConfig);
		}

		// Token: 0x06001A89 RID: 6793 RVA: 0x0006CEBC File Offset: 0x0006B2BC
		public override void Stop()
		{
			ConfigSection reconnectConfig = this.GetReconnectConfig();
			reconnectConfig.OnConfigChanged -= this.OnConfigChanged;
		}

		// Token: 0x06001A8A RID: 6794 RVA: 0x0006CEE2 File Offset: 0x0006B2E2
		public ReconnectConfig Get()
		{
			return this.m_config;
		}

		// Token: 0x06001A8B RID: 6795 RVA: 0x0006CEEC File Offset: 0x0006B2EC
		private ConfigSection GetReconnectConfig()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Reconnect");
			if (section == null)
			{
				throw new ReconnectConfigException("There is no reconnect section");
			}
			return section;
		}

		// Token: 0x06001A8C RID: 6796 RVA: 0x0006CF1C File Offset: 0x0006B31C
		private void OnConfigChanged(ConfigEventArgs args)
		{
			ConfigSection reconnectConfig = this.GetReconnectConfig();
			ReconnectConfig value = this.ParseConfigData(reconnectConfig);
			Interlocked.Exchange<ReconnectConfig>(ref this.m_config, value);
			this.Changed.SafeInvoke(this.m_config);
		}

		// Token: 0x06001A8D RID: 6797 RVA: 0x0006CF58 File Offset: 0x0006B358
		private ReconnectConfig ParseConfigData(ConfigSection reconnectConfig)
		{
			TimeSpan reconnectTimeout;
			try
			{
				if (!reconnectConfig.TryGet("timeout_sec", out reconnectTimeout, default(TimeSpan)) || reconnectTimeout.TotalSeconds < 0.0)
				{
					throw new ReconnectConfigException(string.Format("There is no reconnect timeout attribute: {0}", "timeout_sec"));
				}
			}
			catch (FormatException)
			{
				throw new ReconnectConfigException(string.Format("Invalid reconnect timeout attribute {0} value", "timeout_sec"));
			}
			string text = reconnectConfig.Get("room_types");
			IEnumerable<string> enumerable = from t in text.Split(new char[]
			{
				','
			})
			where !string.IsNullOrEmpty(t)
			select t;
			List<GameRoomType> list = new List<GameRoomType>();
			foreach (string text2 in enumerable)
			{
				GameRoomType item;
				if (!Enum.TryParse<GameRoomType>(text2, true, out item))
				{
					throw new ReconnectConfigException(string.Format("Invalid room type: {0}", text2));
				}
				list.Add(item);
			}
			return new ReconnectConfig(reconnectTimeout, list);
		}

		// Token: 0x04000CAA RID: 3242
		public const string ReconnectSection = "Reconnect";

		// Token: 0x04000CAB RID: 3243
		public const string TimeOutAttribute = "timeout_sec";

		// Token: 0x04000CAC RID: 3244
		public const string RoomTypesAttribute = "room_types";

		// Token: 0x04000CAD RID: 3245
		private ReconnectConfig m_config;
	}
}

using System;
using MasterServer.Core.Configuration;

namespace MasterServer.Database
{
	// Token: 0x020001B9 RID: 441
	public class ConfigurableConnectionPool : ConnectionPool
	{
		// Token: 0x06000853 RID: 2131 RVA: 0x000205EC File Offset: 0x0001E9EC
		protected void Init(ConnectionPool.Config cfg, ConfigSection poolingConfig)
		{
			base.Init(cfg);
			this.m_poolingConfig = poolingConfig;
			this.m_poolingConfig.OnConfigChanged += this.OnPoolingConfigChanged;
		}

		// Token: 0x06000854 RID: 2132 RVA: 0x00020613 File Offset: 0x0001EA13
		protected override void OnDisposing()
		{
			if (this.m_poolingConfig != null)
			{
				this.m_poolingConfig.OnConfigChanged -= this.OnPoolingConfigChanged;
			}
		}

		// Token: 0x06000855 RID: 2133 RVA: 0x00020638 File Offset: 0x0001EA38
		private void OnPoolingConfigChanged(ConfigEventArgs args)
		{
			switch ((ConfigurableConnectionPool.PoolingParameters)Enum.Parse(typeof(ConfigurableConnectionPool.PoolingParameters), args.Name, true))
			{
			case ConfigurableConnectionPool.PoolingParameters.MaxConnections:
				base.MaxConnections = args.iValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.MinConnections:
				base.MinConnections = args.iValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.InactivityTimeout:
				base.InactivityTimeout = args.iValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.AllocationTimeout:
				base.AllocationTimeout = args.iValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.MaxTimeInUse:
				base.MaxTimeInUse = args.iValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.ConnectionTimeout:
				base.ConnectionTimeout = args.uValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.CommandTimeout:
				base.CommandTimeout = args.iValue;
				break;
			case ConfigurableConnectionPool.PoolingParameters.IsolationLevel:
				base.IsolationLevel = args.sValue;
				break;
			}
		}

		// Token: 0x040004E0 RID: 1248
		private ConfigSection m_poolingConfig;

		// Token: 0x020001BA RID: 442
		private enum PoolingParameters
		{
			// Token: 0x040004E2 RID: 1250
			MaxConnections,
			// Token: 0x040004E3 RID: 1251
			MinConnections,
			// Token: 0x040004E4 RID: 1252
			InactivityTimeout,
			// Token: 0x040004E5 RID: 1253
			AllocationTimeout,
			// Token: 0x040004E6 RID: 1254
			MaxTimeInUse,
			// Token: 0x040004E7 RID: 1255
			ConnectionTimeout,
			// Token: 0x040004E8 RID: 1256
			CommandTimeout,
			// Token: 0x040004E9 RID: 1257
			IsolationLevel
		}
	}
}

using System;
using System.IO;
using System.Reflection;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.GameInterface
{
	// Token: 0x020002E9 RID: 745
	[OrphanService]
	[Singleton]
	internal class GameInterfaceRpc : ServiceModule
	{
		// Token: 0x06001165 RID: 4453 RVA: 0x00044E7C File Offset: 0x0004327C
		public override void Start()
		{
			if (!Resources.GIPort.IsValid())
			{
				return;
			}
			try
			{
				AppDomainSetup appDomainSetup = new AppDomainSetup();
				string directoryName = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
				appDomainSetup.ApplicationBase = Path.GetFullPath(directoryName);
				appDomainSetup.DisallowBindingRedirects = false;
				appDomainSetup.DisallowCodeDownload = true;
				appDomainSetup.ConfigurationFile = AppDomain.CurrentDomain.SetupInformation.ConfigurationFile;
				this.m_domain = AppDomain.CreateDomain("GIRpcDomain", null, appDomainSetup);
				this.m_callback = new GIRpcCallback();
				this.m_activator = (GIRpcActivator)this.m_domain.CreateInstanceAndUnwrap("MasterServer", "MasterServer.GameLogic.GameInterface.GIRpcActivator");
				this.m_activator.Start(Resources.GIPort.BindAddr, Resources.GIPort.Port, this.m_callback);
				Log.Info<Resources.PortData>("GIRpc server started at port: {0}", Resources.GIPort);
			}
			catch (Exception p)
			{
				Log.Error<Resources.PortData, Exception>("Failed to start GIRpc service on port {0}, {1}", Resources.GIPort, p);
			}
		}

		// Token: 0x06001166 RID: 4454 RVA: 0x00044F7C File Offset: 0x0004337C
		public override void Stop()
		{
			try
			{
				if (this.m_activator != null)
				{
					this.m_activator.Stop();
					this.m_activator = null;
				}
				if (this.m_domain != null)
				{
					AppDomain.Unload(this.m_domain);
					this.m_domain = null;
				}
				this.m_callback = null;
			}
			catch (Exception e)
			{
				Log.Error(e);
			}
		}

		// Token: 0x040007A9 RID: 1961
		private AppDomain m_domain;

		// Token: 0x040007AA RID: 1962
		private GIRpcActivator m_activator;

		// Token: 0x040007AB RID: 1963
		private GIRpcCallback m_callback;
	}
}

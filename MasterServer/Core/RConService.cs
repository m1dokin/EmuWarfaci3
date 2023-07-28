using System;
using System.IO;
using System.Reflection;
using HK2Net;
using MasterServer.Core.Services;

namespace MasterServer.Core
{
	// Token: 0x02000148 RID: 328
	[OrphanService]
	[Singleton]
	internal class RConService : ServiceModule
	{
		// Token: 0x060005B7 RID: 1463 RVA: 0x00016E8C File Offset: 0x0001528C
		public override void Start()
		{
			if (!Resources.RConPort.IsValid())
			{
				Log.Info("RCon server not started");
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
				this.m_domain = AppDomain.CreateDomain("RConDomain", null, appDomainSetup);
				this.m_activator = (RConActivator)this.m_domain.CreateInstanceAndUnwrap("MasterServer", "MasterServer.Core.RConActivator");
				this.m_callback = new RConCallback();
				this.m_activator.Start(Resources.RConPort.BindAddr, Resources.RConPort.Port, this.m_callback);
				Log.Info<Resources.PortData>("RCon server started at port: {0}", Resources.RConPort);
			}
			catch (Exception ex)
			{
				Log.Error<Resources.PortData, string, string>("Failed to start RCon service on port {0}, {1}:\n{1}", Resources.RConPort, ex.Message, ex.StackTrace);
			}
		}

		// Token: 0x060005B8 RID: 1464 RVA: 0x00016FA0 File Offset: 0x000153A0
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

		// Token: 0x040003B8 RID: 952
		private AppDomain m_domain;

		// Token: 0x040003B9 RID: 953
		private RConActivator m_activator;

		// Token: 0x040003BA RID: 954
		private RConCallback m_callback;
	}
}

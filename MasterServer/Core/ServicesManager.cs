using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core.Services;
using MasterServer.Database;
using Ninject;

namespace MasterServer.Core
{
	// Token: 0x0200014F RID: 335
	[OrphanService]
	[Singleton]
	public class ServicesManager
	{
		// Token: 0x060005C7 RID: 1479 RVA: 0x00017231 File Offset: 0x00015631
		public ServicesManager(IHabitat habitat)
		{
			ServicesManager.m_habitat = habitat;
			this.m_cmdManager = new ConsoleCmdManager(habitat);
			ServicesManager.m_ninjectKernel = ServicesManager.m_habitat.GetNinjectKernel();
			ServicesManager.m_ninjectKernel.Bind<IConsoleCmdManager>().ToConstant<IConsoleCmdManager>(this.m_cmdManager);
		}

		// Token: 0x1400001A RID: 26
		// (add) Token: 0x060005C8 RID: 1480 RVA: 0x00017270 File Offset: 0x00015670
		// (remove) Token: 0x060005C9 RID: 1481 RVA: 0x000172A4 File Offset: 0x000156A4
		public static event ServicesManager.ExecutionPhaseDeleg OnExecutionPhaseChanged;

		// Token: 0x170000A0 RID: 160
		// (get) Token: 0x060005CA RID: 1482 RVA: 0x000172D8 File Offset: 0x000156D8
		// (set) Token: 0x060005CB RID: 1483 RVA: 0x000172E4 File Offset: 0x000156E4
		public static ExecutionPhase ExecutionPhase
		{
			get
			{
				return ServicesManager.m_execPhase;
			}
			private set
			{
				if (ServicesManager.m_execPhase != value)
				{
					ServicesManager.m_execPhase = value;
					Log.Info<ExecutionPhase>("Entering '{0}' phase", ServicesManager.m_execPhase);
					if (ServicesManager.OnExecutionPhaseChanged != null)
					{
						ServicesManager.OnExecutionPhaseChanged(ServicesManager.m_execPhase);
					}
				}
			}
		}

		// Token: 0x060005CC RID: 1484 RVA: 0x00017334 File Offset: 0x00015734
		public void Init()
		{
			try
			{
				ServicesManager.ExecutionPhase = ExecutionPhase.Initializing;
				IEnumerable<IServiceModule> allServicesOrdered = ServicesManager.GetAllServicesOrdered();
				foreach (IServiceModule serviceModule in allServicesOrdered)
				{
					serviceModule.State = ServiceState.Stopped;
					if (ServicesManager.m_shutdownRequest)
					{
						break;
					}
					Log.Info<string>("Initializing module {0}", serviceModule.Name);
					serviceModule.Init();
					serviceModule.State = ServiceState.Initialized;
				}
				this.OnDalConnected();
			}
			catch (ServiceModuleException e)
			{
				Log.Error(e);
				ServicesManager.m_shutdownRequest = true;
			}
			if (ServicesManager.m_shutdownRequest)
			{
				ServicesManager.Stop();
			}
		}

		// Token: 0x060005CD RID: 1485 RVA: 0x00017400 File Offset: 0x00015800
		private void OnDalConnected()
		{
			if (!ServicesManager.m_shutdownRequest)
			{
				ServicesManager.ExecutionPhase = ExecutionPhase.PreUpdate;
			}
			if (!ServicesManager.m_shutdownRequest)
			{
				ServicesManager.ExecutionPhase = ExecutionPhase.Update;
			}
			if (!ServicesManager.m_shutdownRequest)
			{
				ServicesManager.ExecutionPhase = ExecutionPhase.PostUpdate;
			}
			if (!ServicesManager.m_shutdownRequest)
			{
				this.m_cmdManager.Init();
				this.Start();
				ServicesManager.GetService<IApplicationService>().Run();
			}
		}

		// Token: 0x060005CE RID: 1486 RVA: 0x0001746C File Offset: 0x0001586C
		private void Start()
		{
			ServicesManager.ExecutionPhase = ExecutionPhase.Starting;
			foreach (IServiceModule serviceModule in ServicesManager.GetAllServicesOrdered())
			{
				if (ServicesManager.m_shutdownRequest)
				{
					break;
				}
				Log.Info<string>("Starting service module {0}", serviceModule.Name);
				serviceModule.Start();
				serviceModule.State = ServiceState.Started;
			}
			if (!ServicesManager.m_shutdownRequest)
			{
				ServicesManager.ExecutionPhase = ExecutionPhase.Started;
			}
		}

		// Token: 0x060005CF RID: 1487 RVA: 0x00017504 File Offset: 0x00015904
		public static void Stop()
		{
			if (ServicesManager.ExecutionPhase != ExecutionPhase.Started && !ServicesManager.m_shutdownRequest)
			{
				ServicesManager.m_shutdownRequest = true;
				return;
			}
			ServicesManager.ExecutionPhase = ExecutionPhase.Stopping;
			foreach (IServiceModule serviceModule in ServicesManager.GetAllServicesOrdered().Reverse<IServiceModule>())
			{
				if (serviceModule.State != ServiceState.Stopped)
				{
					try
					{
						Log.Info<string>("Closing module {0}", serviceModule.Name);
						serviceModule.Stop();
						serviceModule.State = ServiceState.Stopped;
					}
					catch (ServiceModuleException e)
					{
						Log.Error(e);
					}
				}
			}
			ServicesManager.ExecutionPhase = ExecutionPhase.Stopped;
			ServicesManager.m_shutdownRequest = false;
		}

		// Token: 0x060005D0 RID: 1488 RVA: 0x000175D8 File Offset: 0x000159D8
		[Obsolete]
		public static T GetService<T>() where T : class
		{
			if (!Resources.DbgCacheServices)
			{
				return ServicesManager.GetByContractFromHabitat<T>();
			}
			object serviceCache = ServicesManager.m_serviceCache;
			T result;
			lock (serviceCache)
			{
				object obj;
				if (!ServicesManager.m_serviceCache.TryGetValue(typeof(T), out obj))
				{
					obj = ServicesManager.GetByContractFromHabitat<T>();
					ServicesManager.m_serviceCache.Add(typeof(T), obj);
				}
				result = (T)((object)obj);
			}
			return result;
		}

		// Token: 0x060005D1 RID: 1489 RVA: 0x00017668 File Offset: 0x00015A68
		private static T GetByContractFromHabitat<T>() where T : class
		{
			T result;
			try
			{
				result = ServicesManager.m_habitat.GetByContract<T>();
			}
			catch (ActivationException)
			{
				result = (T)((object)null);
			}
			return result;
		}

		// Token: 0x060005D2 RID: 1490 RVA: 0x000176A4 File Offset: 0x00015AA4
		public static List<IServiceModule> GetAllServices()
		{
			return ServicesManager.m_habitat.GetAllByContract<IServiceModule>().ToList<IServiceModule>();
		}

		// Token: 0x060005D3 RID: 1491 RVA: 0x000176B8 File Offset: 0x00015AB8
		private static IEnumerable<IServiceModule> GetAllServicesOrdered()
		{
			List<IServiceModule> list = ServicesManager.GetAllServices();
			if (Resources.IsDevMode)
			{
				list = new List<IServiceModule>(Utils.Shuffle<IServiceModule>(list));
			}
			if (Resources.DebugQueriesEnabled)
			{
				string path = Path.Combine(Resources.RootDir, "services_order.txt");
				if (File.Exists(path))
				{
					List<IServiceModule> list2 = new List<IServiceModule>();
					string[] array = File.ReadAllLines(path);
					for (int i = 0; i < array.Length; i++)
					{
						string srvName = array[i];
						IServiceModule serviceModule = list.FirstOrDefault((IServiceModule ent) => ent.Name == srvName);
						if (serviceModule != null)
						{
							list2.Add(serviceModule);
							list.Remove(serviceModule);
						}
					}
					list2.AddRange(list);
					list = list2;
				}
			}
			IServiceModule item = list.First((IServiceModule x) => x is IDALService);
			list.Remove(item);
			list.Insert(0, item);
			return list;
		}

		// Token: 0x040003C9 RID: 969
		private static volatile ExecutionPhase m_execPhase = ExecutionPhase.Stopped;

		// Token: 0x040003CA RID: 970
		private static volatile bool m_shutdownRequest = false;

		// Token: 0x040003CB RID: 971
		private static IHabitat m_habitat;

		// Token: 0x040003CC RID: 972
		private static IKernel m_ninjectKernel;

		// Token: 0x040003CD RID: 973
		private IConsoleCmdManager m_cmdManager;

		// Token: 0x040003CF RID: 975
		private static Dictionary<Type, object> m_serviceCache = new Dictionary<Type, object>();

		// Token: 0x02000150 RID: 336
		// (Invoke) Token: 0x060005D7 RID: 1495
		public delegate void ServiceStateDeleg(IServiceModule service, ServiceState state);

		// Token: 0x02000151 RID: 337
		// (Invoke) Token: 0x060005DB RID: 1499
		public delegate void ExecutionPhaseDeleg(ExecutionPhase execution_phase);
	}
}

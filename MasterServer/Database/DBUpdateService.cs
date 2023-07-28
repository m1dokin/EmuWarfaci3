using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.DAL.Utils;

namespace MasterServer.Database
{
	// Token: 0x020001D6 RID: 470
	[Service]
	[Singleton]
	internal class DBUpdateService : ServiceModule, IDBUpdateService, IDebugDbUpdateService
	{
		// Token: 0x060008E1 RID: 2273 RVA: 0x00021E19 File Offset: 0x00020219
		public DBUpdateService(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x060008E2 RID: 2274 RVA: 0x00021E4C File Offset: 0x0002024C
		public override void Init()
		{
			if (!this.m_dbHelper.CheckDatabase())
			{
				Log.Error<string, string, string>("Failed to check database {0} on server {1} for user {2}. Quit.", Resources.SqlDbName, Resources.SqlServerAddr, Resources.SqlLogin);
				ServicesManager.Stop();
				return;
			}
			ServicesManager.OnExecutionPhaseChanged += this.OnExecutionPhaseChanged;
		}

		// Token: 0x060008E3 RID: 2275 RVA: 0x00021E99 File Offset: 0x00020299
		public override void Stop()
		{
			this.m_gameVersionStamps = null;
			this.m_ecatVersionStamps = null;
			this.m_listeners.Clear();
			ServicesManager.OnExecutionPhaseChanged -= this.OnExecutionPhaseChanged;
		}

		// Token: 0x060008E4 RID: 2276 RVA: 0x00021EC5 File Offset: 0x000202C5
		public DBUpdateStage GetCurrentUpdateStage()
		{
			return this.m_currentStage;
		}

		// Token: 0x060008E5 RID: 2277 RVA: 0x00021ED0 File Offset: 0x000202D0
		private bool SetUpdateStage(DBUpdateStage stage)
		{
			this.m_currentStage = stage;
			if (!this.m_dbHelper.OnUpdateStage(stage, this))
			{
				return false;
			}
			IDBUpdateListener[] array = this.m_listeners.ToArray();
			foreach (IDBUpdateListener idbupdateListener in array)
			{
				try
				{
					if (!idbupdateListener.OnDBUpdateStage(this, stage))
					{
						return false;
					}
				}
				catch (Exception e)
				{
					Log.Error(e);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060008E6 RID: 2278 RVA: 0x00021F60 File Offset: 0x00020360
		private void OnExecutionPhaseChanged(ExecutionPhase execution_phase)
		{
			IDALService service = ServicesManager.GetService<IDALService>();
			if (execution_phase == ExecutionPhase.Stopping && Resources.DBUpdaterPermission)
			{
				service.CommonSystem.UnlockUpdaterPermission(Resources.Jid);
			}
			if (execution_phase != ExecutionPhase.Update)
			{
				return;
			}
			if (Resources.DBUpdaterPermissionReset)
			{
				Log.Warning("Master Server will reset db updater permission now");
				service.CommonSystem.ResetUpdaterPermission();
			}
			if (Resources.DBUpdaterPermission)
			{
				if (!service.CommonSystem.TryLockUpdaterPermission(Resources.Jid))
				{
					Log.Error("MasterServer failed to lock db updater permission");
					ServicesManager.Stop();
					return;
				}
				if (!this.RunUpdateChain())
				{
					ServicesManager.Stop();
					return;
				}
			}
			else if (!this.CheckVersion())
			{
				ServicesManager.Stop();
				return;
			}
			this.SyncWithSlaves();
		}

		// Token: 0x060008E7 RID: 2279 RVA: 0x00022018 File Offset: 0x00020418
		public bool RunUpdateChain()
		{
			bool result = this.SetUpdateStage(DBUpdateStage.PreUpdate) && this.SetUpdateStage(DBUpdateStage.Schema) && this.SetUpdateStage(DBUpdateStage.Procedures) && this.SetUpdateStage(DBUpdateStage.Data) && this.SetUpdateStage(DBUpdateStage.PostUpdate);
			this.m_currentStage = DBUpdateStage.None;
			return result;
		}

		// Token: 0x060008E8 RID: 2280 RVA: 0x00022068 File Offset: 0x00020468
		public bool CheckVersion()
		{
			if (Resources.SqlDbVersion != Resources.LatestDbUpdateVersion)
			{
				Log.Error<DBVersion, DBVersion>("Current DB version({0}) is different from latest update version ({1}) and MS does not have 'db_updater' rights, exiting", Resources.SqlDbVersion, Resources.LatestDbUpdateVersion);
				return false;
			}
			IDBUpdateListener[] array = this.m_listeners.ToArray();
			foreach (IDBUpdateListener idbupdateListener in array)
			{
				try
				{
					if (!idbupdateListener.OnDBUpdateStage(this, DBUpdateStage.CheckVersion))
					{
						return false;
					}
				}
				catch (Exception e)
				{
					Log.Error(e);
					return false;
				}
			}
			return true;
		}

		// Token: 0x060008E9 RID: 2281 RVA: 0x00022104 File Offset: 0x00020504
		public void SyncWithSlaves()
		{
			this.m_dbHelper.SyncVersionWithSlaves();
		}

		// Token: 0x060008EA RID: 2282 RVA: 0x00022114 File Offset: 0x00020514
		private string GetDataGroupHash(string group, bool refresh, ref Dictionary<string, string> dictionary, Func<IDALService, IEnumerable<SVersionStamp>> versionStampsGetter)
		{
			object @lock = this.m_lock;
			string result;
			lock (@lock)
			{
				if (refresh || dictionary == null)
				{
					dictionary = versionStampsGetter(ServicesManager.GetService<IDALService>()).ToDictionary((SVersionStamp entry) => entry.DataGroup, (SVersionStamp entry) => entry.Hash);
				}
				string text;
				result = ((!dictionary.TryGetValue(group, out text)) ? DBUpdateService.INVALID_HASH : text);
			}
			return result;
		}

		// Token: 0x060008EB RID: 2283 RVA: 0x000221C8 File Offset: 0x000205C8
		private void SetDataGroupHash(string group, string hash, Dictionary<string, string> dictionary, Func<string, string> groupHashGetter, Action<IDALService, string, string> groupHashSetter)
		{
			string text = groupHashGetter(group);
			if (text.Equals(hash, StringComparison.OrdinalIgnoreCase))
			{
				return;
			}
			dictionary[group] = hash;
			groupHashSetter(ServicesManager.GetService<IDALService>(), group, hash);
		}

		// Token: 0x060008EC RID: 2284 RVA: 0x00022202 File Offset: 0x00020602
		public string GetDataGroupHash(string group, bool refresh)
		{
			return this.GetDataGroupHash(group, refresh, ref this.m_gameVersionStamps, (IDALService dal) => dal.CommonSystem.GetDataVersionStamps());
		}

		// Token: 0x060008ED RID: 2285 RVA: 0x0002222F File Offset: 0x0002062F
		public string GetECatDataGroupHash(string group, bool refresh)
		{
			return this.GetDataGroupHash(group, refresh, ref this.m_ecatVersionStamps, (IDALService dal) => dal.ECatalog.GetDataVersionStamps());
		}

		// Token: 0x060008EE RID: 2286 RVA: 0x0002225C File Offset: 0x0002065C
		public string GetDataGroupHash(string group)
		{
			return this.GetDataGroupHash(group, false);
		}

		// Token: 0x060008EF RID: 2287 RVA: 0x00022266 File Offset: 0x00020666
		public string GetECatDataGroupHash(string group)
		{
			return this.GetECatDataGroupHash(group, false);
		}

		// Token: 0x060008F0 RID: 2288 RVA: 0x00022270 File Offset: 0x00020670
		public void SetDataGroupHash(string group, string hash)
		{
			this.SetDataGroupHash(group, hash, this.m_gameVersionStamps, new Func<string, string>(this.GetDataGroupHash), delegate(IDALService dal, string g, string h)
			{
				dal.CommonSystem.SetDataVersionStamp(g, h);
			});
		}

		// Token: 0x060008F1 RID: 2289 RVA: 0x000222A9 File Offset: 0x000206A9
		public void SetECatDataGroupHash(string group, string hash)
		{
			this.SetDataGroupHash(group, hash, this.m_ecatVersionStamps, new Func<string, string>(this.GetECatDataGroupHash), delegate(IDALService dal, string g, string h)
			{
				dal.ECatalog.SetDataVersionStamp(g, h);
			});
		}

		// Token: 0x060008F2 RID: 2290 RVA: 0x000222E2 File Offset: 0x000206E2
		public void RegisterListener(IDBUpdateListener listener)
		{
			if (!this.m_listeners.Contains(listener))
			{
				this.m_listeners.Add(listener);
			}
		}

		// Token: 0x060008F3 RID: 2291 RVA: 0x00022301 File Offset: 0x00020701
		public void UnregisterListener(IDBUpdateListener listener)
		{
			this.m_listeners.Remove(listener);
		}

		// Token: 0x060008F4 RID: 2292 RVA: 0x00022310 File Offset: 0x00020710
		public void DropProcedure(string procedureName)
		{
			this.m_dalService.CommonSystem.DebugDropProcedure(procedureName);
		}

		// Token: 0x060008F5 RID: 2293 RVA: 0x00022323 File Offset: 0x00020723
		public void RestoreProcedures()
		{
			this.m_dbHelper.UpdateProcedures();
		}

		// Token: 0x0400052B RID: 1323
		private readonly IDALService m_dalService;

		// Token: 0x0400052C RID: 1324
		public static readonly string INVALID_HASH = "INVALID_HASH";

		// Token: 0x0400052D RID: 1325
		private DBUpdateStage m_currentStage;

		// Token: 0x0400052E RID: 1326
		private DatabaseManagementHelper m_dbHelper = new DatabaseManagementHelper();

		// Token: 0x0400052F RID: 1327
		private List<IDBUpdateListener> m_listeners = new List<IDBUpdateListener>();

		// Token: 0x04000530 RID: 1328
		private Dictionary<string, string> m_gameVersionStamps;

		// Token: 0x04000531 RID: 1329
		private Dictionary<string, string> m_ecatVersionStamps;

		// Token: 0x04000532 RID: 1330
		private object m_lock = new object();
	}
}

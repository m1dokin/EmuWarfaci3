using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001F8 RID: 504
	internal class CommonSystemClient : DALCacheProxy<IDALService>, ICommonSystemClient
	{
		// Token: 0x06000A0C RID: 2572 RVA: 0x000259BD File Offset: 0x00023DBD
		internal void Reset(ICommonSystem commonSystem)
		{
			this.m_commonSystem = commonSystem;
		}

		// Token: 0x06000A0D RID: 2573 RVA: 0x000259C8 File Offset: 0x00023DC8
		public void UpdateServer(string msId, SServerEntity entity)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_commonSystem.UpdateServer(msId, entity))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A0E RID: 2574 RVA: 0x00025A18 File Offset: 0x00023E18
		public void DebugExecuteNoQuery(string sql)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_commonSystem.DebugExecuteNoQuery(sql))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A0F RID: 2575 RVA: 0x00025A60 File Offset: 0x00023E60
		public void DebugDropProcedure(string procedureName)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_commonSystem.DebugDropProcedure(procedureName))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A10 RID: 2576 RVA: 0x00025AA8 File Offset: 0x00023EA8
		public IEnumerable<SServerEntity> GetFreeServers(string msId)
		{
			DALCacheProxy<IDALService>.Options<SServerEntity> options = new DALCacheProxy<IDALService>.Options<SServerEntity>
			{
				get_data_stream = (() => this.m_commonSystem.GetFreeServers(msId))
			};
			return base.GetDataStream<SServerEntity>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A11 RID: 2577 RVA: 0x00025AF0 File Offset: 0x00023EF0
		public IEnumerable<SVersionStamp> GetDataVersionStamps()
		{
			DALCacheProxy<IDALService>.Options<SVersionStamp> options = new DALCacheProxy<IDALService>.Options<SVersionStamp>
			{
				get_data_stream = (() => this.m_commonSystem.GetDataVersionStamps())
			};
			return base.GetDataStream<SVersionStamp>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A12 RID: 2578 RVA: 0x00025B24 File Offset: 0x00023F24
		public void SetDataVersionStamp(string group, string hash)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_commonSystem.SetDataVersionStamp(group, hash))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A13 RID: 2579 RVA: 0x00025B74 File Offset: 0x00023F74
		public string GetTotalDataVersionStamp()
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<string> options = new DALCacheProxy<IDALService>.SetOptionsScalar<string>
			{
				set_func = (() => this.m_commonSystem.GetTotalDataVersionStamp())
			};
			return base.SetAndClearScalar<string>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A14 RID: 2580 RVA: 0x00025BA8 File Offset: 0x00023FA8
		public bool TryLockUpdaterPermission(string onlineId)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				set_func = (() => this.m_commonSystem.TryLockUpdaterPermission(onlineId))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A15 RID: 2581 RVA: 0x00025BF0 File Offset: 0x00023FF0
		public void UnlockUpdaterPermission(string onlineId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_commonSystem.UnlockUpdaterPermission(onlineId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A16 RID: 2582 RVA: 0x00025C38 File Offset: 0x00024038
		public void ResetUpdaterPermission()
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_commonSystem.ResetUpdaterPermission())
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000551 RID: 1361
		private ICommonSystem m_commonSystem;
	}
}

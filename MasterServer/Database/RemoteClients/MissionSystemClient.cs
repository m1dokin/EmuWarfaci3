using System;
using System.Collections.Generic;
using System.Reflection;
using MasterServer.DAL;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x02000204 RID: 516
	internal class MissionSystemClient : DALCacheProxy<IDALService>, IMissionSystemClient
	{
		// Token: 0x06000ADD RID: 2781 RVA: 0x00028BED File Offset: 0x00026FED
		internal void Reset(IMissionSystem missionSystem)
		{
			this.m_missionSystem = missionSystem;
		}

		// Token: 0x06000ADE RID: 2782 RVA: 0x00028BF8 File Offset: 0x00026FF8
		public void RemoveMission(Guid uid)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_missionSystem.RemoveMission(uid))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000ADF RID: 2783 RVA: 0x00028C40 File Offset: 0x00027040
		public SMission GetMission(Guid uid)
		{
			DALCacheProxy<IDALService>.Options<SMission> options = new DALCacheProxy<IDALService>.Options<SMission>
			{
				get_data = (() => this.m_missionSystem.GetMission(uid))
			};
			return base.GetData<SMission>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE0 RID: 2784 RVA: 0x00028C88 File Offset: 0x00027088
		public IEnumerable<SMission> GetMissions(int period)
		{
			DALCacheProxy<IDALService>.Options<SMission> options = new DALCacheProxy<IDALService>.Options<SMission>
			{
				get_data_stream = (() => this.m_missionSystem.GetMissions(period))
			};
			return base.GetDataStream<SMission>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE1 RID: 2785 RVA: 0x00028CD0 File Offset: 0x000270D0
		public IEnumerable<Guid> GetCurrentMissions()
		{
			DALCacheProxy<IDALService>.Options<Guid> options = new DALCacheProxy<IDALService>.Options<Guid>
			{
				get_data_stream = (() => this.m_missionSystem.GetCurrentMissions())
			};
			return base.GetDataStream<Guid>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE2 RID: 2786 RVA: 0x00028D04 File Offset: 0x00027104
		public int GetGeneration()
		{
			DALCacheProxy<IDALService>.Options<int> options = new DALCacheProxy<IDALService>.Options<int>
			{
				get_data = (() => this.m_missionSystem.GetGeneration())
			};
			return base.GetData<int>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE3 RID: 2787 RVA: 0x00028D38 File Offset: 0x00027138
		public void SaveGeneration(int generation)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_missionSystem.SaveGeneration(generation))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE4 RID: 2788 RVA: 0x00028D80 File Offset: 0x00027180
		public void SaveMission(Guid uid, string name, string gameMode, string data, int generation)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_missionSystem.SaveMission(uid, name, gameMode, data, generation))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE5 RID: 2789 RVA: 0x00028DE8 File Offset: 0x000271E8
		public void AddCurrentMission(Guid uid)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_missionSystem.AddCurrentMission(uid))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE6 RID: 2790 RVA: 0x00028E30 File Offset: 0x00027230
		public void RemoveCurrentMission(Guid uid)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_missionSystem.RemoveCurrentMission(uid))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE7 RID: 2791 RVA: 0x00028E78 File Offset: 0x00027278
		public IEnumerable<SoftShufflePoolData> GetSoftShufflePools()
		{
			DALCacheProxy<IDALService>.Options<SoftShufflePoolData> options = new DALCacheProxy<IDALService>.Options<SoftShufflePoolData>
			{
				get_data_stream = (() => this.m_missionSystem.GetSoftShufflePools())
			};
			return base.GetDataStream<SoftShufflePoolData>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000AE8 RID: 2792 RVA: 0x00028EAC File Offset: 0x000272AC
		public void SaveSoftShufflePool(SoftShufflePoolData pool)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				set_func = (() => this.m_missionSystem.SaveSoftShufflePool(pool))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x04000558 RID: 1368
		private IMissionSystem m_missionSystem;
	}
}

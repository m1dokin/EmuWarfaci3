using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Matchmaking.Data;

namespace MasterServer.Matchmaking
{
	// Token: 0x02000671 RID: 1649
	internal class MMEntityPool
	{
		// Token: 0x060022A7 RID: 8871 RVA: 0x00090C53 File Offset: 0x0008F053
		public MMEntityPool(IMatchmakingSystem matchmakingSystem)
		{
			this.m_matchmakingSystem = matchmakingSystem;
		}

		// Token: 0x060022A8 RID: 8872 RVA: 0x00090C78 File Offset: 0x0008F078
		public MMEntityPool Clone()
		{
			return new MMEntityPool(this.m_matchmakingSystem)
			{
				m_entityList = new Dictionary<string, MMEntityInfo>(this.m_entityList),
				m_missionList = new List<string>(this.m_missionList)
			};
		}

		// Token: 0x060022A9 RID: 8873 RVA: 0x00090CB4 File Offset: 0x0008F0B4
		public bool AddEntity(MMEntityInfo entity)
		{
			if (this.m_entityList.ContainsKey(entity.Id))
			{
				return false;
			}
			this.m_entityList.Add(entity.Id, entity);
			return true;
		}

		// Token: 0x060022AA RID: 8874 RVA: 0x00090CE4 File Offset: 0x0008F0E4
		public MMEntityInfo RemoveEntity(string entityId)
		{
			MMEntityInfo result;
			if (this.m_entityList.TryGetValue(entityId, out result))
			{
				this.m_entityList.Remove(entityId);
			}
			return result;
		}

		// Token: 0x060022AB RID: 8875 RVA: 0x00090D14 File Offset: 0x0008F114
		public MMEntityInfo GetEntityByProfileId(ulong profileId)
		{
			return this.m_entityList.Values.FirstOrDefault((MMEntityInfo e) => e.Players.Any((MMPlayerInfo p) => p.User.ProfileID == profileId));
		}

		// Token: 0x060022AC RID: 8876 RVA: 0x00090D4C File Offset: 0x0008F14C
		public MMEntityInfo GetEntityByInitiatorId(ulong profileId)
		{
			return this.m_entityList.Values.FirstOrDefault((MMEntityInfo e) => e.Initiator.ProfileID == profileId);
		}

		// Token: 0x060022AD RID: 8877 RVA: 0x00090D82 File Offset: 0x0008F182
		public IEnumerable<MMEntityInfo> GetEntities()
		{
			return new List<MMEntityInfo>(this.m_entityList.Values);
		}

		// Token: 0x060022AE RID: 8878 RVA: 0x00090D94 File Offset: 0x0008F194
		public IEnumerable<MMPlayerInfo> GetPlayers()
		{
			return this.m_entityList.SelectMany((KeyValuePair<string, MMEntityInfo> e) => e.Value.Players).ToList<MMPlayerInfo>();
		}

		// Token: 0x060022AF RID: 8879 RVA: 0x00090DC3 File Offset: 0x0008F1C3
		public void SetMissionList(List<string> missionList)
		{
			this.m_missionList = missionList;
		}

		// Token: 0x04001163 RID: 4451
		private static readonly Random Random = new Random((int)DateTime.Now.Ticks);

		// Token: 0x04001164 RID: 4452
		private readonly IMatchmakingSystem m_matchmakingSystem;

		// Token: 0x04001165 RID: 4453
		private Dictionary<string, MMEntityInfo> m_entityList = new Dictionary<string, MMEntityInfo>();

		// Token: 0x04001166 RID: 4454
		private List<string> m_missionList = new List<string>();
	}
}

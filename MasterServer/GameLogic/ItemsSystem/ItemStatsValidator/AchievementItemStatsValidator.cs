using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.Achievements;
using MasterServer.GameLogic.StatsTracking;

namespace MasterServer.GameLogic.ItemsSystem.ItemStatsValidator
{
	// Token: 0x02000327 RID: 807
	[Service]
	[Singleton]
	internal class AchievementItemStatsValidator : IItemStatsValidator
	{
		// Token: 0x06001247 RID: 4679 RVA: 0x00048EE9 File Offset: 0x000472E9
		public AchievementItemStatsValidator(IItemStats itemStats, IAchievementSystem achievementSystem)
		{
			this.m_itemStats = itemStats;
			this.m_achievementSystem = achievementSystem;
		}

		// Token: 0x06001248 RID: 4680 RVA: 0x00048F00 File Offset: 0x00047300
		public void Validate(IEnumerable<StoreOffer> offers)
		{
			foreach (MetaGameDesc metaGame in this.m_itemStats.GetMetaGameDescs())
			{
				this.CheckAchievementData(metaGame);
			}
		}

		// Token: 0x06001249 RID: 4681 RVA: 0x00048F60 File Offset: 0x00047360
		private void CheckAchievementData(MetaGameDesc metaGame)
		{
			List<string> list = metaGame.Get("on_activate.unlock_achievement").ToList<string>();
			foreach (string text in list)
			{
				uint id;
				if (!uint.TryParse(text, out id))
				{
					throw new AchievementItemValidationException("Item {0} contains invalid achievement id = {1}", new object[]
					{
						metaGame.Name,
						text
					});
				}
				AchievementDescription achievementDesc = this.m_achievementSystem.GetAchievementDesc(id);
				if (achievementDesc == null)
				{
					throw new AchievementItemValidationException("Item {0} contains unexisting achievement with id = {1}", new object[]
					{
						metaGame.Name,
						text
					});
				}
				if (achievementDesc.Kind != EStatsEvent.HIDDEN)
				{
					throw new AchievementItemValidationException("Item {0} contains not hidden achievement with id = {1}", new object[]
					{
						metaGame.Name,
						text
					});
				}
			}
		}

		// Token: 0x0400086A RID: 2154
		private readonly IItemStats m_itemStats;

		// Token: 0x0400086B RID: 2155
		private readonly IAchievementSystem m_achievementSystem;
	}
}

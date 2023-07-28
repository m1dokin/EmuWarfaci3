using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.RewardSystem.RewardCalculators.ClanPointCalculator;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B4 RID: 1460
	[Service]
	[Singleton]
	internal class RewardCalculationChainFactory : IRewardCalculationChainFactory
	{
		// Token: 0x06001F55 RID: 8021 RVA: 0x0007F2B0 File Offset: 0x0007D6B0
		public RewardCalculationChainFactory(IEnumerable<IRewardCalculatorElement> elements)
		{
			this.m_rewardCalculator = elements.ToDictionary((IRewardCalculatorElement x) => x.GetType(), (IRewardCalculatorElement x) => x);
		}

		// Token: 0x06001F56 RID: 8022 RVA: 0x0007F30C File Offset: 0x0007D70C
		public List<IRewardCalculatorElement> CreateRewardCalculationChain(GameRoomType roomType, MissionType missionType)
		{
			List<IRewardCalculatorElement> result;
			if (roomType == GameRoomType.PvP_AutoStart)
			{
				result = this.CreatePvpAutostartChain();
			}
			else if (roomType == GameRoomType.PvP_Rating)
			{
				result = this.CreatePvpRatingChain();
			}
			else if (GameRoomType.PvP.HasFlag(roomType))
			{
				result = this.CreatePvpChain();
			}
			else
			{
				if (!GameRoomType.PvE.HasFlag(roomType))
				{
					throw new RewardCalculationChainFactoryException(string.Format("Can't create reward chain for room type {0} and mission type {1}", roomType, missionType));
				}
				result = ((!missionType.IsSurvival()) ? this.CreatePvEChain() : this.CreateSurvivalChain());
			}
			return result;
		}

		// Token: 0x06001F57 RID: 8023 RVA: 0x0007F3B4 File Offset: 0x0007D7B4
		private List<IRewardCalculatorElement> CreatePvpAutostartChain()
		{
			return new List<IRewardCalculatorElement>
			{
				this.m_rewardCalculator[typeof(BaseRewardCalculator)],
				this.m_rewardCalculator[typeof(TimeFixRewardCalculator)],
				this.m_rewardCalculator[typeof(KillFixRewardCalculator)],
				this.m_rewardCalculator[typeof(PlayerCountFixRewardCalculator)],
				this.m_rewardCalculator[typeof(MinRewardFixCalculator)],
				this.m_rewardCalculator[typeof(PvPFirstWinOfDayRewardCalculator)],
				this.m_rewardCalculator[typeof(ApplyMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ClanPointCalculator)],
				this.m_rewardCalculator[typeof(ApplyDynamicMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ApplyBoostersCalculator)],
				this.m_rewardCalculator[typeof(PvpSkillRewardCalculator)]
			};
		}

		// Token: 0x06001F58 RID: 8024 RVA: 0x0007F4F4 File Offset: 0x0007D8F4
		private List<IRewardCalculatorElement> CreatePvpRatingChain()
		{
			return new List<IRewardCalculatorElement>
			{
				this.m_rewardCalculator[typeof(BaseRewardCalculator)],
				this.m_rewardCalculator[typeof(TimeFixRewardCalculator)],
				this.m_rewardCalculator[typeof(KillFixRewardCalculator)],
				this.m_rewardCalculator[typeof(PlayerCountFixRewardCalculator)],
				this.m_rewardCalculator[typeof(MinRewardFixCalculator)],
				this.m_rewardCalculator[typeof(ApplyMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ClanPointCalculator)],
				this.m_rewardCalculator[typeof(ApplyDynamicMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ApplyBoostersCalculator)],
				this.m_rewardCalculator[typeof(RatingRewardCalculator)]
			};
		}

		// Token: 0x06001F59 RID: 8025 RVA: 0x0007F618 File Offset: 0x0007DA18
		private List<IRewardCalculatorElement> CreatePvpChain()
		{
			return new List<IRewardCalculatorElement>
			{
				this.m_rewardCalculator[typeof(BaseRewardCalculator)],
				this.m_rewardCalculator[typeof(TimeFixRewardCalculator)],
				this.m_rewardCalculator[typeof(KillFixRewardCalculator)],
				this.m_rewardCalculator[typeof(PlayerCountFixRewardCalculator)],
				this.m_rewardCalculator[typeof(RoundLimitFixRewardCalculator)],
				this.m_rewardCalculator[typeof(MinRewardFixCalculator)],
				this.m_rewardCalculator[typeof(ApplyMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ClanPointCalculator)],
				this.m_rewardCalculator[typeof(ApplyDynamicMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ApplyBoostersCalculator)]
			};
		}

		// Token: 0x06001F5A RID: 8026 RVA: 0x0007F73C File Offset: 0x0007DB3C
		private List<IRewardCalculatorElement> CreatePvEChain()
		{
			return new List<IRewardCalculatorElement>
			{
				this.m_rewardCalculator[typeof(BaseRewardCalculator)],
				this.m_rewardCalculator[typeof(MinRewardFixCalculator)],
				this.m_rewardCalculator[typeof(RewardForSecondaryObjectivesCalculator)],
				this.m_rewardCalculator[typeof(PvEFirstWinOfDayRewardCalculator)],
				this.m_rewardCalculator[typeof(ApplyMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ClanPointCalculator)],
				this.m_rewardCalculator[typeof(CrownRewardCalculator)],
				this.m_rewardCalculator[typeof(ApplyDynamicMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ApplyDynamicCrownMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ApplyBoostersCalculator)]
			};
		}

		// Token: 0x06001F5B RID: 8027 RVA: 0x0007F860 File Offset: 0x0007DC60
		private List<IRewardCalculatorElement> CreateSurvivalChain()
		{
			return new List<IRewardCalculatorElement>
			{
				this.m_rewardCalculator[typeof(SurvivalRewardCalculator)],
				this.m_rewardCalculator[typeof(MinRewardFixCalculator)],
				this.m_rewardCalculator[typeof(ApplyMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ClanPointCalculator)],
				this.m_rewardCalculator[typeof(ApplyDynamicMultipliersCalculator)],
				this.m_rewardCalculator[typeof(ApplyBoostersCalculator)],
				this.m_rewardCalculator[typeof(CrownRewardCalculator)]
			};
		}

		// Token: 0x04000F3D RID: 3901
		private readonly Dictionary<Type, IRewardCalculatorElement> m_rewardCalculator;
	}
}

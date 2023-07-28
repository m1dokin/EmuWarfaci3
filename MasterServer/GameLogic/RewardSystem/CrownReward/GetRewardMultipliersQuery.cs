using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.RewardSystem.CrownReward
{
	// Token: 0x020000CE RID: 206
	[QueryAttributes(TagName = "get_reward_multipliers")]
	internal class GetRewardMultipliersQuery : BaseQuery
	{
		// Token: 0x06000353 RID: 851 RVA: 0x0000F3C0 File Offset: 0x0000D7C0
		public GetRewardMultipliersQuery(IRewardMultiplierService rewardMultiplierService)
		{
			this.m_rewardMultiplierService = rewardMultiplierService;
		}

		// Token: 0x06000354 RID: 852 RVA: 0x0000F3D0 File Offset: 0x0000D7D0
		public override async Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			int result;
			if (!base.GetClientInfo(query.online_id, out user))
			{
				result = -3;
			}
			else
			{
				SRewardMultiplier multipliers = await this.m_rewardMultiplierService.GetResultMultiplier(user.ProfileID);
				response.SetAttribute("money_multiplier", multipliers.MoneyMultiplier.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("exp_multiplier", multipliers.ExperienceMultiplier.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("sp_multiplier", multipliers.SponsorPointsMultiplier.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("crown_multiplier", multipliers.CrownMultiplier.ToString(CultureInfo.InvariantCulture));
				result = 0;
			}
			return result;
		}

		// Token: 0x04000168 RID: 360
		private readonly IRewardMultiplierService m_rewardMultiplierService;
	}
}

using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem.WinModels
{
	// Token: 0x02000334 RID: 820
	internal abstract class WinModelBase : IWinModel, IDisposable
	{
		// Token: 0x0600127E RID: 4734 RVA: 0x00049F1C File Offset: 0x0004831C
		protected WinModelBase(IUserRepository userRepository, ILogService logService)
		{
			this.m_userRepository = userRepository;
			this.m_logService = logService;
		}

		// Token: 0x170001AF RID: 431
		// (get) Token: 0x0600127F RID: 4735
		public abstract TopPrizeWinModel WinModel { get; }

		// Token: 0x06001280 RID: 4736
		public abstract int AddPrizeToken(ulong userId, string tokenName);

		// Token: 0x06001281 RID: 4737
		public abstract void ResetPrizeTokensCount(ulong userId, ulong profileId, string tokenName);

		// Token: 0x06001282 RID: 4738
		public abstract Dictionary<string, ulong> GetCollectedPrizeTokensCount(ulong userId);

		// Token: 0x06001283 RID: 4739 RVA: 0x00049F32 File Offset: 0x00048332
		public virtual void Init()
		{
		}

		// Token: 0x06001284 RID: 4740 RVA: 0x00049F34 File Offset: 0x00048334
		public virtual void Dispose()
		{
		}

		// Token: 0x06001285 RID: 4741 RVA: 0x00049F38 File Offset: 0x00048338
		protected void LogTopPrizeTokensResetForUser(ulong userId, string tokenName, ulong tokenCount)
		{
			UserInfo.User userByUserId = this.m_userRepository.GetUserByUserId(userId);
			if (userByUserId == null)
			{
				Log.Warning<ulong>("Unable to log top prize tokens reset for user {0} since there's no corresponding record in user repository.", userId);
				return;
			}
			this.m_logService.Event.TopPrizeTokensReset(userByUserId.ProfileID, tokenName, tokenCount);
		}

		// Token: 0x06001286 RID: 4742 RVA: 0x00049F7C File Offset: 0x0004837C
		protected void LogTopPrizeTokensReset(ulong profileId, string tokenName, ulong tokenCount)
		{
			this.m_logService.Event.TopPrizeTokensReset(profileId, tokenName, tokenCount);
		}

		// Token: 0x0400088A RID: 2186
		protected readonly IUserRepository m_userRepository;

		// Token: 0x0400088B RID: 2187
		private readonly ILogService m_logService;
	}
}

using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.SpecialProfileRewards.Actions
{
	// Token: 0x020005C4 RID: 1476
	[SpecialRewardAction("money")]
	internal class GiveMoneyAction : SpecialRewardAction
	{
		// Token: 0x06001F9B RID: 8091 RVA: 0x00081080 File Offset: 0x0007F480
		public GiveMoneyAction(ConfigSection config, IDALService dalService, ICatalogService catalogService, ISessionStorage sessionStorage) : base(config)
		{
			this.m_dalService = dalService;
			this.m_catalogService = catalogService;
			this.m_sessionStorage = sessionStorage;
			this.m_amount = uint.Parse(config.Get("amount"));
			this.m_currency = GiveMoneyAction.StrToCurrency(config.Get("currency"));
			if (this.m_currency == Currency.CryMoney)
			{
				throw new ApplicationException("Special profile rewards do not support 'cry_money' currency");
			}
		}

		// Token: 0x17000342 RID: 834
		// (get) Token: 0x06001F9C RID: 8092 RVA: 0x000810ED File Offset: 0x0007F4ED
		public override string PrizeName
		{
			get
			{
				return string.Format("{0}:{1}", this.m_currency, this.m_amount);
			}
		}

		// Token: 0x06001F9D RID: 8093 RVA: 0x00081110 File Offset: 0x0007F510
		public override SNotification Activate(ulong profileId, ILogGroup logGroup, XmlElement userData)
		{
			ulong userID = this.m_dalService.ProfileSystem.GetProfileInfo(profileId).UserID;
			string session_id = string.Format("tutorial_{0}", profileId);
			SRewardMultiplier data = this.m_sessionStorage.GetData<SRewardMultiplier>(session_id, ESessionData.RewardMultiplier);
			float num = 1f;
			if (data.ProviderID != null)
			{
				Currency currency = this.m_currency;
				if (currency != Currency.GameMoney)
				{
					if (currency == Currency.CrownMoney)
					{
						num = data.CrownMultiplier;
					}
				}
				else
				{
					num = data.MoneyMultiplier;
				}
			}
			uint num2 = (uint)(0.5 + (double)(this.m_amount * num));
			this.m_catalogService.AddMoney(userID, this.m_currency, (ulong)num2, string.Empty);
			logGroup.ShopMoneyChangedLog(userID, profileId, (long)((ulong)((this.m_currency != Currency.GameMoney) ? 0U : num2)), (long)((ulong)((this.m_currency != Currency.CryMoney) ? 0U : num2)), (long)((ulong)((this.m_currency != Currency.CrownMoney) ? 0U : num2)), LogGroup.ProduceType.Reward, TransactionStatus.OK, string.Empty, string.Empty);
			return base.CreateNotification<string>(ENotificationType.MoneyGiven, new SMoneyRewardNotification
			{
				user_data = userData,
				curr = this.m_currency,
				amount = (ulong)num2,
				notify = true
			}.ToXml().OuterXml, TimeSpan.FromDays(1.0), EConfirmationType.Confirmation);
		}

		// Token: 0x06001F9E RID: 8094 RVA: 0x00081288 File Offset: 0x0007F688
		public static Currency StrToCurrency(string curr)
		{
			if (curr != null)
			{
				if (curr == "game_money")
				{
					return Currency.GameMoney;
				}
				if (curr == "cry_money")
				{
					return Currency.CryMoney;
				}
				if (curr == "crown_money")
				{
					return Currency.CrownMoney;
				}
			}
			throw new ArgumentException(string.Format("Wrong currency name {0}: ", curr));
		}

		// Token: 0x06001F9F RID: 8095 RVA: 0x000812E6 File Offset: 0x0007F6E6
		public static string CurrencyToStr(Currency curr)
		{
			switch (curr)
			{
			case Currency.GameMoney:
				return "game_money";
			case Currency.CryMoney:
				return "cry_money";
			case Currency.CrownMoney:
				return "crown_money";
			default:
				throw new ArgumentException(string.Format("Wrong currency type {0}: ", curr));
			}
		}

		// Token: 0x06001FA0 RID: 8096 RVA: 0x00081326 File Offset: 0x0007F726
		public override string ToString()
		{
			return string.Format("money currency:{0} amount:{1}", GiveMoneyAction.CurrencyToStr(this.m_currency), this.m_amount);
		}

		// Token: 0x04000F72 RID: 3954
		private readonly Currency m_currency;

		// Token: 0x04000F73 RID: 3955
		private readonly uint m_amount;

		// Token: 0x04000F74 RID: 3956
		private readonly IDALService m_dalService;

		// Token: 0x04000F75 RID: 3957
		private readonly ICatalogService m_catalogService;

		// Token: 0x04000F76 RID: 3958
		private readonly ISessionStorage m_sessionStorage;
	}
}

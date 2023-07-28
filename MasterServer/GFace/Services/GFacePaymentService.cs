using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.DAL;
using MasterServer.GFaceAPI;
using MasterServer.GFaceAPI.Responses;
using MasterServer.Platform.Payment;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Telemetry.Metrics;
using MasterServer.Users;

namespace MasterServer.GFace.Services
{
	// Token: 0x0200052F RID: 1327
	[Service]
	[Singleton]
	internal class GFacePaymentService : PaymentService
	{
		// Token: 0x06001CE2 RID: 7394 RVA: 0x00074C29 File Offset: 0x00073029
		public GFacePaymentService(IGFaceAPIService gfaceApiService, IPaymentMetricsTracker paymentMetricsTracker, ILogService logService, IUserRepository userRepo) : base(paymentMetricsTracker, logService)
		{
			this.m_gfaceAPI = gfaceApiService;
			this.m_userRepo = userRepo;
		}

		// Token: 0x06001CE3 RID: 7395 RVA: 0x00074C42 File Offset: 0x00073042
		protected override ConfigSection GetConfig()
		{
			return Resources.ModuleSettings.GetSection("Payment");
		}

		// Token: 0x06001CE4 RID: 7396 RVA: 0x00074C54 File Offset: 0x00073054
		protected override Task<ulong> GetMoneyImpl(ulong userId)
		{
			userId = this.m_userRepo.UnmangleUserId(userId);
			Task<SingleWallet> task = this.m_gfaceAPI.RequestAsync<SingleWallet>(CallOptions.Reliable, GFaceAPIs.wallet_get, new object[]
			{
				"token",
				ServerTokenPlaceHolder.Instance,
				"userid",
				userId
			});
			return task.ContinueWith<ulong>(delegate(Task<SingleWallet> t)
			{
				if (!t.IsFaulted)
				{
					SingleWallet result = t.Result;
					if (result.wallet.creditbalances != null)
					{
						foreach (WalletContentDTO walletContentDTO in result.wallet.creditbalances.Value.credits)
						{
							int num;
							if (GFaceCurrencyHelper.LookupCurrencyId(out num, walletContentDTO.currency) && num == 1)
							{
								return (ulong)((walletContentDTO.amount >= 0L) ? walletContentDTO.amount : 0L);
							}
						}
					}
					return 0UL;
				}
				GFaceUserStateException ex = t.Exception.Flatten().InnerExceptions.OfType<GFaceUserStateException>().FirstOrDefault<GFaceUserStateException>();
				if (ex != null && ex.ErrorCode == GErrorCode.NoWallet)
				{
					Log.Warning(ex);
					return 0UL;
				}
				throw new PaymentServiceInternalErrorException(t.Exception);
			});
		}

		// Token: 0x06001CE5 RID: 7397 RVA: 0x00074CD0 File Offset: 0x000730D0
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, StoreOffer offer)
		{
			userId = this.m_userRepo.UnmangleUserId(userId);
			return this.SpendMoneyFunc(userId, GFacePaymentService.GeneratePurchaseRecord(userId, offer, new object[0]));
		}

		// Token: 0x06001CE6 RID: 7398 RVA: 0x00074CF4 File Offset: 0x000730F4
		protected override Task<PaymentResult> SpendMoneyImpl(ulong userId, ulong amount, SpendMoneyReason spendMoneyReason)
		{
			userId = this.m_userRepo.UnmangleUserId(userId);
			return this.SpendMoneyFunc(userId, GFacePaymentService.GeneratePurchaseRecord(userId, amount, spendMoneyReason));
		}

		// Token: 0x06001CE7 RID: 7399 RVA: 0x00074D14 File Offset: 0x00073114
		private Task<PaymentResult> SpendMoneyFunc(ulong userId, string recordStr)
		{
			Task task = this.m_gfaceAPI.RequestAsync(CallOptions.Reliable, GFaceAPIs.purchase_ext_log, new object[]
			{
				"token",
				ServerTokenPlaceHolder.Instance,
				"entry",
				recordStr
			});
			return task.ContinueWith<PaymentResult>(delegate(Task t)
			{
				if (t.IsFaulted)
				{
					GFaceException ex = t.Exception.Flatten().InnerExceptions.OfType<GFaceException>().FirstOrDefault<GFaceException>();
					if (ex != null)
					{
						GFaceError errorInfo = ex.ErrorInfo;
						if (errorInfo.ErrorCode == GErrorCode.NotEnoughCredits || errorInfo.ErrorCode == GErrorCode.NotEnoughPoints)
						{
							return PaymentResult.NotEnoughMoney;
						}
					}
					throw new PaymentServiceInternalErrorException(t.Exception);
				}
				return PaymentResult.Ok;
			});
		}

		// Token: 0x06001CE8 RID: 7400 RVA: 0x00074D7D File Offset: 0x0007317D
		private static string GeneratePurchaseRecord(ulong customerId, ulong amount, SpendMoneyReason reason)
		{
			return GFacePaymentService.GeneratePurchaseRecord(customerId, reason.ToString(), (long)amount, 1UL, new object[0]);
		}

		// Token: 0x06001CE9 RID: 7401 RVA: 0x00074D9C File Offset: 0x0007319C
		private static string GeneratePurchaseRecord(ulong customerId, string productCode, long price, ulong quantity, params object[] remarks)
		{
			Currency currencyId = Currency.CryMoney;
			string remark;
			if (remarks.Length == 1 && remarks[0] is string)
			{
				remark = (remarks[0] as string);
			}
			else
			{
				if (remarks.Length % 2 != 0)
				{
					throw new ArgumentOutOfRangeException("remarks", "Remarks must be organized in pairs.");
				}
				StringBuilder stringBuilder = new StringBuilder();
				uint num = 0U;
				while ((ulong)num < (ulong)((long)remarks.Length))
				{
					stringBuilder.AppendFormat("[{0}]=({1}), ", remarks[(int)((UIntPtr)num)], remarks[(int)((UIntPtr)(num + 1U))]);
					num += 2U;
				}
				remark = ((stringBuilder.Length != 0) ? stringBuilder.ToString() : null);
			}
			return GFaceCurrencyHelper.GeneratePurchaseRecord(customerId, productCode, (int)currencyId, price, new ulong?(quantity), remark);
		}

		// Token: 0x06001CEA RID: 7402 RVA: 0x00074E4C File Offset: 0x0007324C
		private static string GeneratePurchaseRecord(ulong customerId, StoreOffer offer, params object[] remarks)
		{
			PriceTag? priceTag = null;
			foreach (PriceTag value in offer.Prices)
			{
				if (value.Price != 0UL)
				{
					if (priceTag != null)
					{
						throw new ApplicationException("Only one currency per offer is allowed! OfferId: " + offer.StoreID);
					}
					priceTag = new PriceTag?(value);
				}
			}
			if (priceTag == null || priceTag.Value.Currency != Currency.CryMoney)
			{
				throw new ApplicationException("No CryMoney price for this offer! OfferId: " + offer.StoreID);
			}
			return GFacePaymentService.GeneratePurchaseRecord(customerId, offer.Content.Item.Name, (long)priceTag.Value.Price, offer.Content.Quantity, remarks);
		}

		// Token: 0x04000DC6 RID: 3526
		private readonly IGFaceAPIService m_gfaceAPI;

		// Token: 0x04000DC7 RID: 3527
		private readonly IUserRepository m_userRepo;
	}
}

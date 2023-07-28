using System;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.GFaceAPI;
using MasterServer.GFaceAPI.Responses;
using MasterServer.Platform.Payment;
using MasterServer.Platform.Payment.Exceptions;
using MasterServer.Users;

namespace MasterServer.GFace.Services
{
	// Token: 0x02000531 RID: 1329
	[Service]
	[Singleton]
	internal class GFaceProxyService : ServiceModule, IDebugPaymentService
	{
		// Token: 0x06001CED RID: 7405 RVA: 0x000750E1 File Offset: 0x000734E1
		public GFaceProxyService(IGFaceAPIService gface, IPaymentService payment, IUserRepository users)
		{
			this.m_gface = gface;
			this.m_payment = payment;
			this.m_users = users;
		}

		// Token: 0x06001CEE RID: 7406 RVA: 0x00075100 File Offset: 0x00073500
		public void SetMoney(ulong userId, ulong amount)
		{
			try
			{
				this.m_gface.Request(CallOptions.Reliable, GFaceProxyAPIs.commerce_wallet_set, new object[]
				{
					"userid",
					this.m_users.UnmangleUserId(userId),
					"amount",
					amount
				});
			}
			catch (AggregateException inner)
			{
				throw new PaymentServiceException(inner);
			}
		}

		// Token: 0x06001CEF RID: 7407 RVA: 0x00075170 File Offset: 0x00073570
		public ulong AddMoney(ulong userId, ulong amount)
		{
			ulong amount2;
			try
			{
				SingleWallet singleWallet = this.m_gface.Request<SingleWallet>(CallOptions.Reliable, GFaceProxyAPIs.commerce_wallet_add, new object[]
				{
					"userid",
					this.m_users.UnmangleUserId(userId),
					"amount",
					amount
				});
				amount2 = (ulong)singleWallet.wallet.creditbalances.Value.credits[0].amount;
			}
			catch (AggregateException inner)
			{
				throw new PaymentServiceException(inner);
			}
			return amount2;
		}

		// Token: 0x06001CF0 RID: 7408 RVA: 0x00075204 File Offset: 0x00073604
		public PaymentResult SpendMoney(ulong userId, ulong amount)
		{
			return this.m_payment.SpendMoney(userId, amount, SpendMoneyReason.Debug);
		}

		// Token: 0x06001CF1 RID: 7409 RVA: 0x00075214 File Offset: 0x00073614
		public void SetRequestFailedState(bool state)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001CF2 RID: 7410 RVA: 0x0007521B File Offset: 0x0007361B
		public bool GetRequestFailedState()
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001CF3 RID: 7411 RVA: 0x00075222 File Offset: 0x00073622
		public void SetRequestTimeout(TimeSpan timeout)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06001CF4 RID: 7412 RVA: 0x00075229 File Offset: 0x00073629
		public TimeSpan GetRequestTimeout()
		{
			throw new NotImplementedException();
		}

		// Token: 0x04000DCB RID: 3531
		private readonly IGFaceAPIService m_gface;

		// Token: 0x04000DCC RID: 3532
		private readonly IPaymentService m_payment;

		// Token: 0x04000DCD RID: 3533
		private readonly IUserRepository m_users;
	}
}

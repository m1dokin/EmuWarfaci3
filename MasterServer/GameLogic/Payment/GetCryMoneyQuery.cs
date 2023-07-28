using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Platform.Payment;
using MasterServer.Users;
using Ninject;
using Util.Common;

namespace MasterServer.GameLogic.Payment
{
	// Token: 0x020003E5 RID: 997
	[QueryAttributes(TagName = "get_cry_money")]
	internal class GetCryMoneyQuery : BaseQuery
	{
		// Token: 0x060015AF RID: 5551 RVA: 0x0005A67F File Offset: 0x00058A7F
		public GetCryMoneyQuery([Optional] IPaymentService paymentService)
		{
			this.m_paymentService = paymentService;
		}

		// Token: 0x060015B0 RID: 5552 RVA: 0x0005A690 File Offset: 0x00058A90
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			Task<int> result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetMoneyQuery"))
			{
				UserInfo.User user;
				if (this.m_paymentService == null)
				{
					result = TaskHelpers.Completed<int>(-1);
				}
				else if (!base.GetClientInfo(query.online_id, out user))
				{
					result = TaskHelpers.Completed<int>(-3);
				}
				else
				{
					Task<ulong> moneyAsync = this.m_paymentService.GetMoneyAsync(user.UserID);
					result = moneyAsync.ContinueWith<int>(delegate(Task<ulong> t)
					{
						response.SetAttribute("cry_money", t.Result.ToString(CultureInfo.InvariantCulture));
						return 0;
					});
				}
			}
			return result;
		}

		// Token: 0x04000A56 RID: 2646
		public const string Name = "get_cry_money";

		// Token: 0x04000A57 RID: 2647
		private readonly IPaymentService m_paymentService;
	}
}

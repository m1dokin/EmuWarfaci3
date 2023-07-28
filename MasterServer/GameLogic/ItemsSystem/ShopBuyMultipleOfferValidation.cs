using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000339 RID: 825
	[Service]
	[Singleton]
	internal class ShopBuyMultipleOfferValidation : ServiceModule, IShopBuyMultipleOfferValidation
	{
		// Token: 0x06001291 RID: 4753 RVA: 0x0004A5A9 File Offset: 0x000489A9
		public ShopBuyMultipleOfferValidation(IEnumerable<IMultipleOfferValidationProvider> validators)
		{
			this.m_validators = validators.ToList<IMultipleOfferValidationProvider>();
		}

		// Token: 0x06001292 RID: 4754 RVA: 0x0004A5C0 File Offset: 0x000489C0
		public override void Init()
		{
			base.Init();
			foreach (IMultipleOfferValidationProvider multipleOfferValidationProvider in this.m_validators)
			{
				multipleOfferValidationProvider.Initialize();
			}
		}

		// Token: 0x06001293 RID: 4755 RVA: 0x0004A624 File Offset: 0x00048A24
		public override void Stop()
		{
			base.Stop();
			foreach (IMultipleOfferValidationProvider multipleOfferValidationProvider in this.m_validators)
			{
				multipleOfferValidationProvider.Dispose();
			}
		}

		// Token: 0x06001294 RID: 4756 RVA: 0x0004A688 File Offset: 0x00048A88
		public IEnumerable<ulong> Validate(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			return this.m_validators.Aggregate(offerIds, (IEnumerable<ulong> a, IMultipleOfferValidationProvider kv) => kv.Validate(user, supplierId, a));
		}

		// Token: 0x06001295 RID: 4757 RVA: 0x0004A6C4 File Offset: 0x00048AC4
		public void Confirm(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			this.m_validators.ForEach(delegate(IMultipleOfferValidationProvider v)
			{
				v.Confirm(user, supplierId, offerIds);
			});
		}

		// Token: 0x0400088F RID: 2191
		private readonly List<IMultipleOfferValidationProvider> m_validators;
	}
}

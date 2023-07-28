using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using MasterServer.DAL;
using MasterServer.DAL.VoucherSystem;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.VoucherSystem.Exceptions;
using MasterServer.GameLogic.VoucherSystem.VoucherProviders;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.VoucherSystem
{
	// Token: 0x0200047A RID: 1146
	[Service]
	[Singleton]
	internal class VoucherService : ServiceModule, IVoucherService, IDebugVoucherService
	{
		// Token: 0x0600181B RID: 6171 RVA: 0x00063738 File Offset: 0x00061B38
		public VoucherService(IVoucherValidator voucherValidator, IItemService itemService, ILogService logService, IJobSchedulerService jobSchedulerService, IEnumerable<IVoucherProvider> providers)
		{
			this.m_providers = providers;
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_logService = logService;
			this.m_voucherValidator = voucherValidator;
			this.m_itemService = itemService;
		}

		// Token: 0x0600181C RID: 6172 RVA: 0x00063765 File Offset: 0x00061B65
		public override void Start()
		{
			base.Start();
			if (Resources.RealmDBUpdaterPermission)
			{
				this.m_jobSchedulerService.AddJob("voucher_synchronization");
			}
		}

		// Token: 0x0600181D RID: 6173 RVA: 0x00063788 File Offset: 0x00061B88
		public Task<IEnumerable<GiveItemResponse>> ProccessVoucher(UserInfo.User user)
		{
			ILogGroup logGroup = this.m_logService.CreateGroup();
			List<Task<IEnumerable<GiveItemResponse>>> tasks = (from provider in this.m_providers
			select this.ProccessVouchersFromProvider(user, provider, logGroup)).ToList<Task<IEnumerable<GiveItemResponse>>>();
			return Task.WhenAll<IEnumerable<GiveItemResponse>>(tasks).ContinueWith<IEnumerable<GiveItemResponse>>(delegate(Task<IEnumerable<GiveItemResponse>[]> t)
			{
				logGroup.Dispose();
				List<GiveItemResponse> list = new List<GiveItemResponse>();
				foreach (Task<IEnumerable<GiveItemResponse>> task in tasks)
				{
					if (task.IsFaulted)
					{
						Log.Error(task.Exception);
					}
					else
					{
						list.AddRange(task.Result);
					}
				}
				return list;
			});
		}

		// Token: 0x0600181E RID: 6174 RVA: 0x000637FC File Offset: 0x00061BFC
		public IEnumerable<Voucher> GetAllVouchers(ulong userId)
		{
			List<Voucher> list = new List<Voucher>();
			foreach (IVoucherProvider voucherProvider in this.m_providers)
			{
				list.AddRange(voucherProvider.GetAllVouchersForUser(userId));
			}
			return list;
		}

		// Token: 0x0600181F RID: 6175 RVA: 0x00063864 File Offset: 0x00061C64
		private Task<IEnumerable<GiveItemResponse>> ProccessVouchersFromProvider(UserInfo.User user, IVoucherProvider provider, ILogGroup logGroup)
		{
			VoucherService.<ProccessVouchersFromProvider>c__AnonStorey1 <ProccessVouchersFromProvider>c__AnonStorey = new VoucherService.<ProccessVouchersFromProvider>c__AnonStorey1();
			<ProccessVouchersFromProvider>c__AnonStorey.user = user;
			<ProccessVouchersFromProvider>c__AnonStorey.provider = provider;
			<ProccessVouchersFromProvider>c__AnonStorey.logGroup = logGroup;
			<ProccessVouchersFromProvider>c__AnonStorey.$this = this;
			return <ProccessVouchersFromProvider>c__AnonStorey.provider.GetNewVouchers(<ProccessVouchersFromProvider>c__AnonStorey.user.UserID).ContinueWith<IEnumerable<GiveItemResponse>>(delegate(Task<IEnumerable<Voucher>> t)
			{
				List<GiveItemResponse> items = new List<GiveItemResponse>();
				if (t.IsFaulted)
				{
					Log.Error(t.Exception);
					return items;
				}
				IEnumerable<Voucher> result = t.Result;
				IEnumerable<Voucher> source = result;
				Action<Voucher> step = delegate(Voucher voucher)
				{
					IEnumerable<GiveItemResponse> collection = <ProccessVouchersFromProvider>c__AnonStorey.ProcessVoucher(<ProccessVouchersFromProvider>c__AnonStorey.user, voucher, <ProccessVouchersFromProvider>c__AnonStorey.provider, <ProccessVouchersFromProvider>c__AnonStorey.logGroup);
					items.AddRange(collection);
				};
				if (VoucherService.<>f__mg$cache0 == null)
				{
					VoucherService.<>f__mg$cache0 = new Action<Exception>(Log.Error);
				}
				source.SafeForEachEx(step, VoucherService.<>f__mg$cache0);
				return items;
			});
		}

		// Token: 0x06001820 RID: 6176 RVA: 0x000638BC File Offset: 0x00061CBC
		private IEnumerable<GiveItemResponse> ProcessVoucher(UserInfo.User user, Voucher voucher, IVoucherProvider provider, ILogGroup logGroup)
		{
			if (voucher.Status != VoucherStatus.New)
			{
				throw new VoucherException(string.Format("Voucher {0} from provider: {1} has status '{2}' expected '{3}'", new object[]
				{
					voucher.Id,
					provider.GetType().Name,
					voucher.Status,
					VoucherStatus.New
				}));
			}
			IEnumerable<GiveItemResponse> result = Enumerable.Empty<GiveItemResponse>();
			voucher.Status = VoucherStatus.Corrupted;
			if (this.m_voucherValidator.IsValid(voucher))
			{
				try
				{
					IEnumerable<GiveItemResponse> source = this.GiveVoucherContent(user, voucher, logGroup);
					if (source.All((GiveItemResponse item) => item.OperationStatus == TransactionStatus.OK || item.OperationStatus == TransactionStatus.LIMIT_REACHED))
					{
						voucher.Status = VoucherStatus.Successful;
						result = from v in source
						where v.OperationStatus == TransactionStatus.OK
						select v;
					}
				}
				catch (ItemServiceException e)
				{
					Log.Error(e);
				}
				catch (TransactionError e2)
				{
					Log.Error(e2);
				}
			}
			try
			{
				provider.ReportVoucher(voucher);
			}
			catch (VoucherProviderInternalErrorException e3)
			{
				Log.Error(e3);
			}
			return result;
		}

		// Token: 0x06001821 RID: 6177 RVA: 0x000639FC File Offset: 0x00061DFC
		private IEnumerable<GiveItemResponse> GiveVoucherContent(UserInfo.User user, Voucher voucher, ILogGroup group)
		{
			List<GiveItemResponse> list = new List<GiveItemResponse>();
			foreach (VoucherItem voucherItem in voucher)
			{
				GiveItemResponse giveItemResponse = null;
				switch (voucherItem.Type)
				{
				case OfferType.Expiration:
					giveItemResponse = this.m_itemService.GiveExpirableItem(voucher.UserId, voucherItem.ItemName, voucherItem.ExpirationTime, LogGroup.ProduceType.Voucher, group, "-");
					break;
				case OfferType.Permanent:
					giveItemResponse = this.m_itemService.GivePermanentItem(voucher.UserId, voucherItem.ItemName, LogGroup.ProduceType.Voucher, group, "-");
					break;
				case OfferType.Consumable:
					giveItemResponse = this.m_itemService.GiveConsumableItem(voucher.UserId, voucherItem.ItemName, voucherItem.Quantity, LogGroup.ProduceType.Voucher, group, 0, "-");
					break;
				case OfferType.Regular:
				{
					RandomBoxPurchaseHandler purchaseListener = RandomBoxPurchaseHandler.Create(user.ProfileID, null);
					giveItemResponse = this.m_itemService.GiveRegularItem(voucher.UserId, voucherItem.ItemName, LogGroup.ProduceType.Voucher, purchaseListener, group, "-", false);
					giveItemResponse.PurchaseListener = purchaseListener;
					break;
				}
				}
				if (giveItemResponse != null)
				{
					giveItemResponse.Message = voucher.Message;
					list.Add(giveItemResponse);
				}
			}
			return list;
		}

		// Token: 0x04000B96 RID: 2966
		private readonly IItemService m_itemService;

		// Token: 0x04000B97 RID: 2967
		private readonly ILogService m_logService;

		// Token: 0x04000B98 RID: 2968
		private readonly IVoucherValidator m_voucherValidator;

		// Token: 0x04000B99 RID: 2969
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x04000B9A RID: 2970
		private readonly IEnumerable<IVoucherProvider> m_providers;

		// Token: 0x04000B9B RID: 2971
		[CompilerGenerated]
		private static Action<Exception> <>f__mg$cache0;
	}
}

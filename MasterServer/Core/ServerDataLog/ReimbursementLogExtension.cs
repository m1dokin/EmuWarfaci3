using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x0200012F RID: 303
	[Service]
	[Singleton]
	internal class ReimbursementLogExtension : AbstractServerDataLogExtension
	{
		// Token: 0x060004FC RID: 1276 RVA: 0x00015658 File Offset: 0x00013A58
		public ReimbursementLogExtension(IItemsReimbursement itemsReimbursement, ILogService logService, bool isEnabled) : base(logService, isEnabled)
		{
			this.m_itemsReimbursement = itemsReimbursement;
		}

		// Token: 0x060004FD RID: 1277 RVA: 0x00015669 File Offset: 0x00013A69
		public override void Start()
		{
			base.Start();
			this.m_itemsReimbursement.ReimbursementItemsUpdated += base.OnDataUpdated;
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00015688 File Offset: 0x00013A88
		public override void Dispose()
		{
			this.m_itemsReimbursement.ReimbursementItemsUpdated -= base.OnDataUpdated;
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x000156A4 File Offset: 0x00013AA4
		protected override void LogData()
		{
			using (ILogGroup logGroup = this.LogService.CreateGroup())
			{
				foreach (ItemToReimburse itemToReimburse in this.m_itemsReimbursement.GetItemsToReimburse())
				{
					logGroup.ItemReimbursementLog(itemToReimburse.name, (itemToReimburse.currency != Currency.GameMoney) ? 0UL : itemToReimburse.moneyAmount, (itemToReimburse.currency != Currency.CryMoney) ? 0UL : itemToReimburse.moneyAmount, (itemToReimburse.currency != Currency.CrownMoney) ? 0UL : itemToReimburse.moneyAmount);
				}
			}
		}

		// Token: 0x04000212 RID: 530
		private readonly IItemsReimbursement m_itemsReimbursement;
	}
}

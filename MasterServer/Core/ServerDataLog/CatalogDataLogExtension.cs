using System;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;

namespace MasterServer.Core.ServerDataLog
{
	// Token: 0x0200012A RID: 298
	[Service]
	[Singleton]
	internal class CatalogDataLogExtension : AbstractServerDataLogExtension
	{
		// Token: 0x060004E4 RID: 1252 RVA: 0x00015047 File Offset: 0x00013447
		public CatalogDataLogExtension(ICatalogService catalogService, ILogService logService, bool isEnabled) : base(logService, isEnabled)
		{
			this.m_catalogService = catalogService;
		}

		// Token: 0x060004E5 RID: 1253 RVA: 0x00015058 File Offset: 0x00013458
		public override void Start()
		{
			base.Start();
			this.m_catalogService.CatalogItemsUpdated += base.OnDataUpdated;
		}

		// Token: 0x060004E6 RID: 1254 RVA: 0x00015077 File Offset: 0x00013477
		public override void Dispose()
		{
			this.m_catalogService.CatalogItemsUpdated -= base.OnDataUpdated;
		}

		// Token: 0x060004E7 RID: 1255 RVA: 0x00015090 File Offset: 0x00013490
		protected override void LogData()
		{
			using (ILogGroup logGroup = this.LogService.CreateGroup())
			{
				foreach (CatalogItem catalogItem in this.m_catalogService.GetCatalogItems().Values)
				{
					logGroup.CatalogItemsLog(catalogItem.ID, catalogItem.Name, catalogItem.MaxAmount, catalogItem.Type);
				}
			}
		}

		// Token: 0x0400020B RID: 523
		private readonly ICatalogService m_catalogService;
	}
}

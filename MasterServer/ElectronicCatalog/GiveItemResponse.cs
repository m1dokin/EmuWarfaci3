using System;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000233 RID: 563
	internal class GiveItemResponse
	{
		// Token: 0x06000BFE RID: 3070 RVA: 0x0002E19C File Offset: 0x0002C59C
		internal GiveItemResponse(OfferItem item, ulong profileId, ulong userId, TransactionStatus status, LogGroup.ProduceType produceType, OfferType offerType, ILogGroup logGroup, string reason)
		{
			this.ItemGiven = item;
			this.ProfileId = profileId;
			this.UserId = userId;
			this.OperationStatus = status;
			this.LogProduceType = produceType;
			this.ItemOfferType = offerType;
			this.LoggingGroup = logGroup;
			this.Reason = reason;
		}

		// Token: 0x06000BFF RID: 3071 RVA: 0x0002E1EC File Offset: 0x0002C5EC
		internal GiveItemResponse(TransactionStatus status)
		{
			this.OperationStatus = status;
		}

		// Token: 0x1700015C RID: 348
		// (get) Token: 0x06000C00 RID: 3072 RVA: 0x0002E1FB File Offset: 0x0002C5FB
		// (set) Token: 0x06000C01 RID: 3073 RVA: 0x0002E203 File Offset: 0x0002C603
		internal string Message { get; set; }

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x06000C02 RID: 3074 RVA: 0x0002E20C File Offset: 0x0002C60C
		// (set) Token: 0x06000C03 RID: 3075 RVA: 0x0002E214 File Offset: 0x0002C614
		internal OfferItem ItemGiven { get; private set; }

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x06000C04 RID: 3076 RVA: 0x0002E21D File Offset: 0x0002C61D
		// (set) Token: 0x06000C05 RID: 3077 RVA: 0x0002E225 File Offset: 0x0002C625
		internal ulong ProfileId { get; private set; }

		// Token: 0x1700015F RID: 351
		// (get) Token: 0x06000C06 RID: 3078 RVA: 0x0002E22E File Offset: 0x0002C62E
		// (set) Token: 0x06000C07 RID: 3079 RVA: 0x0002E236 File Offset: 0x0002C636
		internal ulong UserId { get; private set; }

		// Token: 0x17000160 RID: 352
		// (get) Token: 0x06000C08 RID: 3080 RVA: 0x0002E23F File Offset: 0x0002C63F
		// (set) Token: 0x06000C09 RID: 3081 RVA: 0x0002E247 File Offset: 0x0002C647
		internal TransactionStatus OperationStatus { get; private set; }

		// Token: 0x17000161 RID: 353
		// (get) Token: 0x06000C0A RID: 3082 RVA: 0x0002E250 File Offset: 0x0002C650
		// (set) Token: 0x06000C0B RID: 3083 RVA: 0x0002E258 File Offset: 0x0002C658
		internal LogGroup.ProduceType LogProduceType { get; private set; }

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x06000C0C RID: 3084 RVA: 0x0002E261 File Offset: 0x0002C661
		// (set) Token: 0x06000C0D RID: 3085 RVA: 0x0002E269 File Offset: 0x0002C669
		internal OfferType ItemOfferType { get; private set; }

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x06000C0E RID: 3086 RVA: 0x0002E272 File Offset: 0x0002C672
		// (set) Token: 0x06000C0F RID: 3087 RVA: 0x0002E27A File Offset: 0x0002C67A
		internal ILogGroup LoggingGroup { get; private set; }

		// Token: 0x17000164 RID: 356
		// (get) Token: 0x06000C10 RID: 3088 RVA: 0x0002E283 File Offset: 0x0002C683
		// (set) Token: 0x06000C11 RID: 3089 RVA: 0x0002E28B File Offset: 0x0002C68B
		internal string Reason { get; private set; }

		// Token: 0x17000165 RID: 357
		// (get) Token: 0x06000C12 RID: 3090 RVA: 0x0002E294 File Offset: 0x0002C694
		// (set) Token: 0x06000C13 RID: 3091 RVA: 0x0002E29C File Offset: 0x0002C69C
		internal IPurchaseListener PurchaseListener { get; set; }
	}
}

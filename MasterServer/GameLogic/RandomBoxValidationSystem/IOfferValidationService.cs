using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.DAL;

namespace MasterServer.GameLogic.RandomBoxValidationSystem
{
	// Token: 0x020000BC RID: 188
	[Contract]
	internal interface IOfferValidationService
	{
		// Token: 0x060002FB RID: 763
		void Validate(IEnumerable<StoreOffer> newOffers);

		// Token: 0x060002FC RID: 764
		void Validate(IEnumerable<StoreOffer> currentOffers, IEnumerable<StoreOffer> newOffers);
	}
}

using System;
using System.Collections.Generic;
using MasterServer.DAL;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x02000057 RID: 87
	public class StoreOfferComparer : EqualityComparer<StoreOffer>
	{
		// Token: 0x06000151 RID: 337 RVA: 0x00009CA4 File Offset: 0x000080A4
		public override bool Equals(StoreOffer s1, StoreOffer s2)
		{
			return s1.Category.Equals(s2.Category) && s1.Status.Equals(s2.Status) && s1.Type.Equals(s2.Type) && s1.Content.Item.Name.Equals(s2.Content.Item.Name) && s1.Content.ExpirationTime.Equals(s2.Content.ExpirationTime) && s1.Content.DurabilityPoints.Equals(s2.Content.DurabilityPoints) && s1.Content.Quantity.Equals(s2.Content.Quantity) && s1.GetOriginalPriceByCurrency(Currency.GameMoney).Equals(s2.GetOriginalPriceByCurrency(Currency.GameMoney)) && s1.GetOriginalPriceByCurrency(Currency.CryMoney).Equals(s2.GetOriginalPriceByCurrency(Currency.CryMoney)) && s1.GetOriginalPriceByCurrency(Currency.CrownMoney).Equals(s2.GetOriginalPriceByCurrency(Currency.CrownMoney));
		}

		// Token: 0x06000152 RID: 338 RVA: 0x00009DD7 File Offset: 0x000081D7
		public override int GetHashCode(StoreOffer s)
		{
			return s.GetHashCode() ^ s.StoreID.GetHashCode() ^ s.SupplierID.GetHashCode();
		}
	}
}

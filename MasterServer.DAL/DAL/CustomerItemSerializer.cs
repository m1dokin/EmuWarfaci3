using System;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x0200005D RID: 93
	public class CustomerItemSerializer : IDBSerializer<CustomerItem>
	{
		// Token: 0x060000BB RID: 187 RVA: 0x00003E5C File Offset: 0x0000225C
		public void Deserialize(IDataReaderEx reader, out CustomerItem ret)
		{
			ret = new CustomerItem();
			ret.CatalogItem.ID = ulong.Parse(reader["catalog_id"].ToString());
			ret.CatalogItem.Name = reader["item_name"].ToString();
			ret.CatalogItem.Type = reader["type"].ToString();
			ret.CatalogItem.Active = ParseUtils.ParseBool(reader["active"].ToString());
			ret.CatalogItem.Stackable = ParseUtils.ParseBool(reader["stackable"].ToString());
			ret.InstanceID = ulong.Parse(reader["catalog_instance_id"].ToString());
			ret.OfferType = (OfferType)Enum.Parse(typeof(OfferType), reader["offer_type"].ToString());
			ret.ExpirationTimeUTC = ulong.Parse(reader["expiration_time"].ToString());
			ret.TotalDurabilityPoints = int.Parse(reader["total_durability_points"].ToString());
			ret.DurabilityPoints = int.Parse(reader["durability_points"].ToString());
			ret.Quantity = ulong.Parse(reader["current_quantity"].ToString());
			ret.BuyTimeUTC = ulong.Parse(reader["catalog_buy_time"].ToString());
		}
	}
}

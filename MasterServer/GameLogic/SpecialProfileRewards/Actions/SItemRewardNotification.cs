using System;
using System.Xml;
using MasterServer.DAL;

namespace MasterServer.GameLogic.SpecialProfileRewards.Actions
{
	// Token: 0x020005C1 RID: 1473
	public struct SItemRewardNotification
	{
		// Token: 0x06001F94 RID: 8084 RVA: 0x00080B18 File Offset: 0x0007EF18
		public XmlElement ToXml()
		{
			XmlElement xmlElement = new XmlDocument().CreateElement("give_item");
			xmlElement.SetAttribute("name", this.name);
			xmlElement.SetAttribute("offer_type", this.offer_type.ToString());
			xmlElement.SetAttribute("notify", (!this.notify) ? "0" : "1");
			if (this.offer_type == OfferType.Expiration)
			{
				xmlElement.SetAttribute("extended_time", this.extended_time);
			}
			if (this.offer_type == OfferType.Consumable)
			{
				xmlElement.SetAttribute("consumables_count", this.consumables_count.ToString());
			}
			if (!string.IsNullOrEmpty(this.correlationId))
			{
				xmlElement.SetAttribute("correlation_id", this.correlationId);
			}
			if (this.user_data != null)
			{
				XmlNode newChild = xmlElement.OwnerDocument.ImportNode(this.user_data, true);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x04000F5F RID: 3935
		public string name;

		// Token: 0x04000F60 RID: 3936
		public string extended_time;

		// Token: 0x04000F61 RID: 3937
		public ulong consumables_count;

		// Token: 0x04000F62 RID: 3938
		public OfferType offer_type;

		// Token: 0x04000F63 RID: 3939
		public XmlElement user_data;

		// Token: 0x04000F64 RID: 3940
		public string correlationId;

		// Token: 0x04000F65 RID: 3941
		public bool notify;
	}
}

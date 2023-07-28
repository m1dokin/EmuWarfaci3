using System;
using System.Globalization;
using System.Xml;
using MasterServer.Common;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog.ShopSupplier.Serializers;
using MasterServer.GameLogic.VoucherSystem.Exceptions;
using Util.Common;

namespace MasterServer.GameLogic.VoucherSystem.Serializers
{
	// Token: 0x0200047F RID: 1151
	internal class VoucherItemXmlSerializer : IXmlSerializer<VoucherItem>
	{
		// Token: 0x06001842 RID: 6210 RVA: 0x000643A2 File Offset: 0x000627A2
		public VoucherItemXmlSerializer(XmlDocument xmlDocument)
		{
			this.m_xmlDocument = xmlDocument;
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x000643B4 File Offset: 0x000627B4
		public VoucherItem Deserialize(XmlElement element)
		{
			VoucherItem result = default(VoucherItem);
			try
			{
				result.ItemName = element.GetAttribute("name");
				result.Type = Utils.ParseEnum<OfferType>(element.GetAttribute("type"));
				result.Quantity = ((!element.HasAttribute("quantity")) ? 0 : ushort.Parse(element.GetAttribute("quantity")));
				result.Durability = ((!element.HasAttribute("durability")) ? 0 : int.Parse(element.GetAttribute("durability")));
				result.ExpirationTime = ((!element.HasAttribute("expiration")) ? TimeSpan.Zero : TimeUtils.GetExpireTime(element.GetAttribute("expiration")));
			}
			catch (Exception innerException)
			{
				string message = string.Format("Failed to deserialize voucher item: {0}", element.OuterXml);
				throw new VoucherItemDeserializationException(message, innerException);
			}
			return result;
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x000644B0 File Offset: 0x000628B0
		public XmlElement Serialize(VoucherItem item)
		{
			XmlElement xmlElement = this.m_xmlDocument.CreateElement("item");
			xmlElement.SetAttribute("name", item.ItemName);
			xmlElement.SetAttribute("type", item.Type.ToString());
			xmlElement.SetAttribute("quantity", item.Quantity.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("durability", item.Durability.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("expiration", (!(item.ExpirationTime == TimeSpan.Zero)) ? TimeUtils.GetExpireTime(item.ExpirationTime) : string.Empty);
			return xmlElement;
		}

		// Token: 0x04000BAB RID: 2987
		private readonly XmlDocument m_xmlDocument;
	}
}

using System;
using System.Xml;

namespace MasterServer.ElectronicCatalog.ShopSupplier.Serializers
{
	// Token: 0x0200024E RID: 590
	public interface IXmlSerializer<T>
	{
		// Token: 0x06000D02 RID: 3330
		XmlElement Serialize(T obj);

		// Token: 0x06000D03 RID: 3331
		T Deserialize(XmlElement element);
	}
}

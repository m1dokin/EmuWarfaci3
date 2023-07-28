using System;
using System.Runtime.InteropServices;
using System.Xml;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000373 RID: 883
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct SItemUnequippedNotification
	{
		// Token: 0x170001E2 RID: 482
		// (get) Token: 0x06001420 RID: 5152 RVA: 0x00051AAF File Offset: 0x0004FEAF
		// (set) Token: 0x06001421 RID: 5153 RVA: 0x00051AB7 File Offset: 0x0004FEB7
		public string ItemName { get; set; }

		// Token: 0x06001422 RID: 5154 RVA: 0x00051AC0 File Offset: 0x0004FEC0
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("unequipped_item");
			xmlElement.SetAttribute("item_name", this.ItemName);
			return xmlElement;
		}
	}
}

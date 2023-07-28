using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x02000305 RID: 773
	internal class RoomTypeRestriction
	{
		// Token: 0x060011DE RID: 4574 RVA: 0x00046A97 File Offset: 0x00044E97
		public RoomTypeRestriction(XmlNode node)
		{
			this.ReadData(node);
		}

		// Token: 0x17000199 RID: 409
		public string this[ERoomRestriction index]
		{
			get
			{
				string text;
				return (!this.m_restrictions.TryGetValue(index, out text)) ? null : text;
			}
		}

		// Token: 0x060011E0 RID: 4576 RVA: 0x00046AD0 File Offset: 0x00044ED0
		private void ReadData(XmlNode attrNode)
		{
			XmlNodeList xmlNodeList = attrNode.SelectNodes("restrictions/restriction");
			XmlNodeList nodeList = xmlNodeList;
			if (RoomTypeRestriction.<>f__mg$cache0 == null)
			{
				RoomTypeRestriction.<>f__mg$cache0 = new Func<XmlNode, ERoomRestriction>(CommonMethods.GetKindValue<ERoomRestriction>);
			}
			this.m_restrictions = nodeList.ReadToDictionary(RoomTypeRestriction.<>f__mg$cache0, (XmlNode n) => SettingHelper.ExtractAttributeValue(n, "option"));
		}

		// Token: 0x040007FC RID: 2044
		private IDictionary<ERoomRestriction, string> m_restrictions;

		// Token: 0x040007FD RID: 2045
		[CompilerGenerated]
		private static Func<XmlNode, ERoomRestriction> <>f__mg$cache0;
	}
}

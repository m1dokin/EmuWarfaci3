using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x02000306 RID: 774
	internal class RoomTypeSettings
	{
		// Token: 0x060011E2 RID: 4578 RVA: 0x00046B3C File Offset: 0x00044F3C
		public RoomTypeSettings(XmlNode node)
		{
			this.ReadData(node);
		}

		// Token: 0x1700019A RID: 410
		public string this[ERoomSetting index]
		{
			get
			{
				string text;
				return (!this.m_settings.TryGetValue(index, out text)) ? string.Empty : text;
			}
		}

		// Token: 0x060011E4 RID: 4580 RVA: 0x00046B77 File Offset: 0x00044F77
		public bool TryGetValue(ERoomSetting setting, out string value)
		{
			return this.m_settings.TryGetValue(setting, out value);
		}

		// Token: 0x060011E5 RID: 4581 RVA: 0x00046B88 File Offset: 0x00044F88
		public void Merge(RoomTypeSettings other)
		{
			foreach (KeyValuePair<ERoomSetting, string> keyValuePair in other.m_settings)
			{
				if (!this.m_settings.ContainsKey(keyValuePair.Key))
				{
					this.m_settings.Add(keyValuePair.Key, keyValuePair.Value);
				}
			}
		}

		// Token: 0x060011E6 RID: 4582 RVA: 0x00046C0C File Offset: 0x0004500C
		private void ReadData(XmlNode attrNode)
		{
			XmlNodeList xmlNodeList = attrNode.SelectNodes("settings/setting");
			XmlNodeList nodeList = xmlNodeList;
			if (RoomTypeSettings.<>f__mg$cache0 == null)
			{
				RoomTypeSettings.<>f__mg$cache0 = new Func<XmlNode, ERoomSetting>(CommonMethods.GetKindValue<ERoomSetting>);
			}
			this.m_settings = nodeList.ReadToDictionary(RoomTypeSettings.<>f__mg$cache0, (XmlNode n) => SettingHelper.ExtractAttributeValue(n, "value"));
		}

		// Token: 0x040007FF RID: 2047
		private IDictionary<ERoomSetting, string> m_settings;

		// Token: 0x04000800 RID: 2048
		[CompilerGenerated]
		private static Func<XmlNode, ERoomSetting> <>f__mg$cache0;
	}
}

using System;
using System.Collections.Generic;
using System.Xml;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C6 RID: 966
	internal class Objective
	{
		// Token: 0x0600154D RID: 5453 RVA: 0x0005946A File Offset: 0x0005786A
		public Objective(string type)
		{
			this.m_type = type;
		}

		// Token: 0x0600154E RID: 5454 RVA: 0x00059484 File Offset: 0x00057884
		public void AddValue(string key, string val)
		{
			this.m_Values.Add(key, val);
		}

		// Token: 0x0600154F RID: 5455 RVA: 0x00059494 File Offset: 0x00057894
		public XmlElement GetXml(XmlDocument doc)
		{
			XmlElement xmlElement = doc.CreateElement("Objective");
			xmlElement.SetAttribute("type", this.m_type);
			foreach (KeyValuePair<string, string> keyValuePair in this.m_Values)
			{
				xmlElement.SetAttribute(keyValuePair.Key, keyValuePair.Value);
			}
			return xmlElement;
		}

		// Token: 0x170001F6 RID: 502
		// (get) Token: 0x06001551 RID: 5457 RVA: 0x00059525 File Offset: 0x00057925
		// (set) Token: 0x06001550 RID: 5456 RVA: 0x0005951C File Offset: 0x0005791C
		public string ObjectiveType
		{
			get
			{
				return this.m_type;
			}
			set
			{
				this.m_type = value;
			}
		}

		// Token: 0x04000A29 RID: 2601
		private Dictionary<string, string> m_Values = new Dictionary<string, string>();

		// Token: 0x04000A2A RID: 2602
		private string m_type;
	}
}

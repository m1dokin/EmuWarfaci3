using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x02000302 RID: 770
	internal class RoomRestrictionConfig
	{
		// Token: 0x060011CE RID: 4558 RVA: 0x000464EC File Offset: 0x000448EC
		public void ReadData(XmlDocument doc)
		{
			this.m_restrictionDesc.Clear();
			XmlNodeList elementsByTagName = doc.GetElementsByTagName("restriction");
			IEnumerator enumerator = elementsByTagName.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (!(xmlNode.ParentNode.Name != "restriction_options"))
					{
						if (xmlNode.NodeType != XmlNodeType.Comment)
						{
							XmlAttribute xmlAttribute = xmlNode.Attributes["is_global"];
							bool isGlobal = xmlAttribute != null && int.Parse(xmlAttribute.Value) > 0;
							string value = xmlNode.Attributes["kind"].Value;
							xmlAttribute = xmlNode.Attributes["channels"];
							string channels = (xmlAttribute == null) ? string.Empty : xmlAttribute.Value;
							RoomRestrictionDesc value2 = new RoomRestrictionDesc(isGlobal, channels);
							XmlNodeList xmlNodeList = xmlNode.SelectNodes("option");
							IEnumerator enumerator2 = xmlNodeList.GetEnumerator();
							try
							{
								while (enumerator2.MoveNext())
								{
									object obj2 = enumerator2.Current;
									XmlNode node = (XmlNode)obj2;
									this.ReadOptions(node, ref value2);
								}
							}
							finally
							{
								IDisposable disposable;
								if ((disposable = (enumerator2 as IDisposable)) != null)
								{
									disposable.Dispose();
								}
							}
							this.m_restrictionDesc.Add(RestrictionHelper.Parse(value), value2);
						}
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}

		// Token: 0x060011CF RID: 4559 RVA: 0x0004669C File Offset: 0x00044A9C
		private void ReadOptions(XmlNode node, ref RoomRestrictionDesc desc)
		{
			string value = node.Attributes["id"].Value;
			XmlAttribute xmlAttribute = node.Attributes["default"];
			string defaultValue = (xmlAttribute == null) ? null : xmlAttribute.Value;
			RoomRestrictionOption roomRestrictionOption = new RoomRestrictionOption();
			IEnumerator enumerator = node.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType != XmlNodeType.Comment)
					{
						roomRestrictionOption.values.Add(xmlNode.Attributes["value"].Value);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			desc.Add(value, defaultValue, roomRestrictionOption);
		}

		// Token: 0x060011D0 RID: 4560 RVA: 0x00046780 File Offset: 0x00044B80
		public RoomRestrictionDesc GetRestrictionDesc(ERoomRestriction type)
		{
			if (this.m_restrictionDesc.ContainsKey(type))
			{
				return this.m_restrictionDesc[type];
			}
			return null;
		}

		// Token: 0x060011D1 RID: 4561 RVA: 0x000467A4 File Offset: 0x00044BA4
		public IEnumerable<KeyValuePair<ERoomRestriction, RoomRestrictionDesc>> GetGlobalRestricions()
		{
			return from restr in this.m_restrictionDesc
			where restr.Value.isGlobal
			select restr;
		}

		// Token: 0x060011D2 RID: 4562 RVA: 0x000467DC File Offset: 0x00044BDC
		public bool IsGlobal(ERoomRestriction kind)
		{
			RoomRestrictionDesc roomRestrictionDesc;
			this.m_restrictionDesc.TryGetValue(kind, out roomRestrictionDesc);
			return roomRestrictionDesc != null && roomRestrictionDesc.isGlobal;
		}

		// Token: 0x040007E8 RID: 2024
		private readonly Dictionary<ERoomRestriction, RoomRestrictionDesc> m_restrictionDesc = new Dictionary<ERoomRestriction, RoomRestrictionDesc>();
	}
}

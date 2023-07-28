using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.PersistentSettingsSystem
{
	// Token: 0x020003F5 RID: 1013
	internal class PersistentSettings
	{
		// Token: 0x060015EE RID: 5614 RVA: 0x0005B7E0 File Offset: 0x00059BE0
		public PersistentSettings()
		{
		}

		// Token: 0x060015EF RID: 5615 RVA: 0x0005B7F4 File Offset: 0x00059BF4
		public PersistentSettings(ulong profileId, IDALService dalService)
		{
			XmlDocument xmlDocument = new XmlDocument();
			foreach (SPersistentSettings spersistentSettings in dalService.ProfileSystem.GetPersistentSettings(profileId))
			{
				xmlDocument.LoadXml(spersistentSettings.Settings);
				IEnumerator enumerator2 = xmlDocument.DocumentElement.Attributes.GetEnumerator();
				try
				{
					while (enumerator2.MoveNext())
					{
						object obj = enumerator2.Current;
						XmlAttribute xmlAttribute = (XmlAttribute)obj;
						string key = string.Format("{0}.{1}", spersistentSettings.Group, xmlAttribute.Name);
						this.m_settings.Add(key, xmlAttribute.Value);
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
			}
		}

		// Token: 0x060015F0 RID: 5616 RVA: 0x0005B8F4 File Offset: 0x00059CF4
		public string GetValue(string setting)
		{
			return (!this.m_settings.ContainsKey(setting)) ? null : this.m_settings[setting];
		}

		// Token: 0x060015F1 RID: 5617 RVA: 0x0005B91C File Offset: 0x00059D1C
		public bool IsTrue(string setting)
		{
			string value = this.GetValue(setting);
			return value == null || value == "1";
		}

		// Token: 0x060015F2 RID: 5618 RVA: 0x0005B945 File Offset: 0x00059D45
		public bool IsFalse(string setting)
		{
			return !this.IsTrue(setting);
		}

		// Token: 0x04000A83 RID: 2691
		private readonly Dictionary<string, string> m_settings = new Dictionary<string, string>();
	}
}

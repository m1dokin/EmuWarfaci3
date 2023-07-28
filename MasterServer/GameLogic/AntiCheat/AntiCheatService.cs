using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameLogic.AntiCheat
{
	// Token: 0x02000260 RID: 608
	[Service]
	[Singleton]
	internal class AntiCheatService : ServiceModule, IAntiCheatService
	{
		// Token: 0x06000D59 RID: 3417 RVA: 0x00034D2C File Offset: 0x0003312C
		public AntiCheatService()
		{
			foreach (List<ConfigSection> list in Resources.AntiCheatConfig.GetAllSections().Values)
			{
				foreach (ConfigSection configSection in list)
				{
					foreach (List<ConfigSection> list2 in configSection.GetAllSections().Values)
					{
						foreach (ConfigSection configSection2 in list2)
						{
							configSection2.OnConfigChanged += this.OnConfigChanged;
						}
					}
				}
			}
			this.BuildConfigXml();
		}

		// Token: 0x06000D5A RID: 3418 RVA: 0x00034E80 File Offset: 0x00033280
		private void OnConfigChanged(ConfigEventArgs e)
		{
			this.BuildConfigXml();
		}

		// Token: 0x06000D5B RID: 3419 RVA: 0x00034E88 File Offset: 0x00033288
		private void BuildConfigXml()
		{
			object configLock = this.m_configLock;
			lock (configLock)
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlNode xmlNode = xmlDocument.CreateElement("anticheat_configuration");
				xmlDocument.AppendChild(xmlNode);
				foreach (List<ConfigSection> list in Resources.AntiCheatConfig.GetAllSections().Values)
				{
					foreach (ConfigSection configSection in list)
					{
						XmlElement newChild = xmlNode.OwnerDocument.CreateElement(configSection.Name);
						XmlNode xmlNode2 = xmlNode.AppendChild(newChild);
						foreach (List<ConfigSection> list2 in configSection.GetAllSections().Values)
						{
							foreach (ConfigSection configSection2 in list2)
							{
								XmlElement xmlElement = xmlNode.OwnerDocument.CreateElement(configSection2.Name);
								foreach (KeyValuePair<string, ConfigValue> keyValuePair in configSection2.GetAllValues())
								{
									xmlElement.SetAttribute(keyValuePair.Key, keyValuePair.Value.GetString());
								}
								xmlNode2.AppendChild(xmlElement);
							}
						}
					}
				}
				this.m_config = xmlDocument.DocumentElement;
			}
		}

		// Token: 0x06000D5C RID: 3420 RVA: 0x000350F0 File Offset: 0x000334F0
		public void WriteConfig(XmlElement request)
		{
			object configLock = this.m_configLock;
			lock (configLock)
			{
				XmlNode newChild = request.OwnerDocument.ImportNode(this.m_config, true);
				request.AppendChild(newChild);
			}
		}

		// Token: 0x06000D5D RID: 3421 RVA: 0x00035148 File Offset: 0x00033548
		public void ProcessSessionReport(XmlElement report)
		{
			ILogService service = ServicesManager.GetService<ILogService>();
			IDALService service2 = ServicesManager.GetService<IDALService>();
			string attribute = report.GetAttribute("session_id");
			using (ILogGroup logGroup = service.CreateGroup())
			{
				IEnumerator enumerator = report.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement = (XmlElement)obj;
						string attribute2 = xmlElement.GetAttribute("profile_id");
						string attribute3 = xmlElement.GetAttribute("type");
						string attribute4 = xmlElement.GetAttribute("score");
						string attribute5 = xmlElement.GetAttribute("calls");
						string text = xmlElement.GetAttribute("description");
						byte[] bytes = Convert.FromBase64String(text);
						text = Encoding.UTF8.GetString(bytes);
						text = text.Replace(LogGroup.RECORD_SEPARATOR, "|");
						ulong num;
						if (ulong.TryParse(attribute2, out num))
						{
							IAbuseReportService service3 = ServicesManager.GetService<IAbuseReportService>();
							SProfileInfo profileInfo = service2.ProfileSystem.GetProfileInfo(num);
							string[] param = new string[]
							{
								attribute,
								profileInfo.Nickname,
								profileInfo.RankInfo.RankId.ToString(),
								service3.GetTotalOnlineTime(num).ToString(),
								attribute4,
								attribute5,
								text
							};
							int num2 = 0;
							int.TryParse(attribute5, out num2);
							if (num2 > 0)
							{
								service3.StoreReportToDB(0UL, num, attribute3, EAbuseReportSource.eARS_Anticheat, param);
							}
							if (num2 > 0)
							{
								logGroup.AntiCheatReport(profileInfo.UserID, num, attribute, attribute3, attribute4, attribute5, text);
							}
							else
							{
								logGroup.AntiCheatImmediateReport(profileInfo.UserID, num, attribute, attribute3, attribute4, text);
							}
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
			}
		}

		// Token: 0x04000625 RID: 1573
		private XmlElement m_config;

		// Token: 0x04000626 RID: 1574
		private object m_configLock = new object();
	}
}

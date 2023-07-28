using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using MasterServer.DAL.Utils;
using OLAPHypervisor;

namespace MasterServer.DAL.PlayerStats
{
	// Token: 0x0200003C RID: 60
	public class PlayerStatisticsDataSerializer : IDataSerializer<PlayerStatistics>
	{
		// Token: 0x06000096 RID: 150 RVA: 0x00003668 File Offset: 0x00001A68
		public void Serialize(PlayerStatistics data, TextWriter wr)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("player_statistics");
			xmlElement.SetAttribute("version", data.Version.ToString());
			xmlDocument.AppendChild(xmlElement);
			foreach (Measure measure in data.Measures)
			{
				XmlElement xmlElement2 = xmlDocument.CreateElement("stat");
				xmlElement2.SetAttribute("row_count", measure.RowCount.ToString(CultureInfo.InvariantCulture));
				xmlElement2.SetAttribute("value", measure.Value.ToString(CultureInfo.InvariantCulture));
				foreach (KeyValuePair<string, string> keyValuePair in measure.Dimensions)
				{
					XmlElement xmlElement3 = xmlDocument.CreateElement("dimension");
					xmlElement3.SetAttribute("key", keyValuePair.Key);
					xmlElement3.SetAttribute("value", keyValuePair.Value);
					xmlElement2.AppendChild(xmlElement3);
				}
				xmlElement.AppendChild(xmlElement2);
			}
			wr.Write(xmlElement.OuterXml);
		}

		// Token: 0x06000097 RID: 151 RVA: 0x000037D4 File Offset: 0x00001BD4
		public PlayerStatistics Deserialize(TextReader rd, DBVersion version)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(rd);
			XmlElement documentElement = xmlDocument.DocumentElement;
			PlayerStatistics playerStatistics = new PlayerStatistics
			{
				Measures = new List<Measure>(),
				Version = DBVersion.Parse(documentElement.GetAttribute("version"))
			};
			IEnumerator enumerator = documentElement.SelectNodes("stat").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element)
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						long rowCount;
						long value;
						try
						{
							rowCount = long.Parse(xmlElement.GetAttribute("row_count"));
							value = long.Parse(xmlElement.GetAttribute("value"));
						}
						catch (Exception)
						{
							Log.Error("[PlayerStatistics] Query processing error: attrbutes row_count and value must be long. payload: {0}", new object[]
							{
								xmlDocument.OuterXml
							});
							throw;
						}
						Measure measure = default(Measure);
						Measure measure2 = measure;
						measure2.RowCount = rowCount;
						measure2.Value = value;
						measure2.Dimensions = new SortedList<string, string>();
						measure = measure2;
						IEnumerator enumerator2 = xmlElement.SelectNodes("dimension").GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlNode xmlNode2 = (XmlNode)obj2;
								if (xmlNode2.NodeType == XmlNodeType.Element)
								{
									XmlElement xmlElement2 = xmlNode2 as XmlElement;
									measure.Dimensions.Add(xmlElement2.GetAttribute("key"), xmlElement2.GetAttribute("value"));
								}
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
						playerStatistics.Measures.Add(measure);
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
			return playerStatistics;
		}
	}
}

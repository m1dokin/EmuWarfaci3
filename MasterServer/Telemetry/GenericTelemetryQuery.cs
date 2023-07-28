using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using OLAPHypervisor;

namespace MasterServer.Telemetry
{
	// Token: 0x020006EB RID: 1771
	[QueryAttributes(TagName = "generic_telemetry", QoSClass = "generic_telemetry")]
	internal class GenericTelemetryQuery : BaseQuery
	{
		// Token: 0x06002538 RID: 9528 RVA: 0x0009B8E0 File Offset: 0x00099CE0
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "SetDedicatedStatsQuery"))
			{
				ITelemetryService service = ServicesManager.GetService<ITelemetryService>();
				if (!service.CheckMode(TelemetryMode.Generic))
				{
					result = 0;
				}
				else
				{
					List<Measure> list = new List<Measure>();
					IEnumerator enumerator = request.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlNode xmlNode = (XmlNode)obj;
							if (xmlNode.Name.Equals("dimensions"))
							{
								SortedList<string, string> sortedList = new SortedList<string, string>();
								IEnumerator enumerator2 = xmlNode.Attributes.GetEnumerator();
								try
								{
									while (enumerator2.MoveNext())
									{
										object obj2 = enumerator2.Current;
										XmlAttribute xmlAttribute = (XmlAttribute)obj2;
										sortedList[xmlAttribute.Name] = xmlAttribute.Value;
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
								IEnumerator enumerator3 = xmlNode.ChildNodes.GetEnumerator();
								try
								{
									while (enumerator3.MoveNext())
									{
										object obj3 = enumerator3.Current;
										XmlNode xmlNode2 = (XmlNode)obj3;
										this.ParseMeasure(xmlNode2 as XmlElement, fromJid, list, sortedList);
									}
								}
								finally
								{
									IDisposable disposable2;
									if ((disposable2 = (enumerator3 as IDisposable)) != null)
									{
										disposable2.Dispose();
									}
								}
							}
							else
							{
								this.ParseMeasure(xmlNode as XmlElement, fromJid, list, null);
							}
						}
					}
					finally
					{
						IDisposable disposable3;
						if ((disposable3 = (enumerator as IDisposable)) != null)
						{
							disposable3.Dispose();
						}
					}
					if (request.HasAttribute("immediate") && request.GetAttribute("immediate") == "1")
					{
						string value = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
						foreach (Measure measure in list)
						{
							measure.Dimensions["date"] = value;
						}
						service.AddMeasure(list);
					}
					else
					{
						string schedule = Resources.ModuleSettings.Get("MonitoringSchedule");
						TimeSpan jitter = TimeSpan.Parse(Resources.ModuleSettings.Get("MonitoringScheduleJitter"));
						service.DeferredStream.AddMeasure(list, schedule, jitter);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06002539 RID: 9529 RVA: 0x0009BBA8 File Offset: 0x00099FA8
		private bool ParseMeasure(XmlElement elem, string fromJid, List<Measure> measures, SortedList<string, string> dimensions)
		{
			if (elem == null)
			{
				return false;
			}
			Measure item = default(Measure);
			if (dimensions != null)
			{
				item.Dimensions = new SortedList<string, string>(dimensions);
			}
			else
			{
				item.Dimensions = new SortedList<string, string>();
			}
			item.RowCount = 1L;
			IEnumerator enumerator = elem.Attributes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlAttribute xmlAttribute = (XmlAttribute)obj;
					if (string.Equals(xmlAttribute.Name, "value"))
					{
						item.Value = long.Parse(xmlAttribute.Value);
					}
					else
					{
						item.Dimensions[xmlAttribute.Name] = xmlAttribute.Value;
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
			item.Dimensions["stat"] = elem.Name;
			if (!elem.Name.StartsWith("client_") && !elem.Name.StartsWith("hrdw_"))
			{
				item.Dimensions["server"] = fromJid;
			}
			item.Dimensions["host"] = Resources.Hostname;
			measures.Add(item);
			return true;
		}
	}
}

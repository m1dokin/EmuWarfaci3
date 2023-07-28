using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using HK2Net;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;

namespace MasterServer.Core
{
	// Token: 0x02000158 RID: 344
	[Service]
	[Singleton]
	internal class OnlineVariables : ServiceModule, IOnlineVariables
	{
		// Token: 0x06000608 RID: 1544 RVA: 0x00017FC4 File Offset: 0x000163C4
		public override void Init()
		{
			this.m_random = new Random((int)DateTime.Now.Ticks);
			this.FillOnlineVariables();
			Dictionary<string, List<ConfigSection>> allSections = Resources.OnlineVariablesSettings.GetAllSections();
			foreach (KeyValuePair<string, List<ConfigSection>> keyValuePair in allSections)
			{
				foreach (ConfigSection configSection in keyValuePair.Value)
				{
					configSection.OnConfigChanged += delegate(ConfigEventArgs arg)
					{
						this.FillOnlineVariables();
					};
				}
			}
		}

		// Token: 0x06000609 RID: 1545 RVA: 0x00018098 File Offset: 0x00016498
		private void FillOnlineVariables()
		{
			Dictionary<string, List<OnlineVariable>> dictionary = new Dictionary<string, List<OnlineVariable>>();
			List<ConfigSection> items;
			if (Resources.OnlineVariablesSettings.TryGetSections("Variable", out items))
			{
				this.LoadVariables(items, dictionary);
			}
			Interlocked.Exchange<Dictionary<string, List<OnlineVariable>>>(ref this.m_vars, dictionary);
		}

		// Token: 0x0600060A RID: 1546 RVA: 0x000180D8 File Offset: 0x000164D8
		public string Get(string key, OnlineVariableDestination dest)
		{
			List<OnlineVariable> list = this.m_vars[key];
			foreach (OnlineVariable onlineVariable in list)
			{
				if (onlineVariable.Destination == dest)
				{
					return onlineVariable.Value;
				}
			}
			return string.Empty;
		}

		// Token: 0x0600060B RID: 1547 RVA: 0x00018154 File Offset: 0x00016554
		public void WriteVariables(XmlElement rootNode, OnlineVariableDestination destination)
		{
			XmlElement xmlElement = rootNode.OwnerDocument.CreateElement("variables");
			foreach (List<OnlineVariable> list in this.m_vars.Values)
			{
				foreach (OnlineVariable onlineVariable in list)
				{
					if (onlineVariable.Destination.HasFlag(destination) || destination.HasFlag(onlineVariable.Destination))
					{
						object random = this.m_random;
						int num;
						lock (random)
						{
							num = this.m_random.Next(0, 100);
						}
						if (onlineVariable.Probability > num)
						{
							XmlElement xmlElement2 = rootNode.OwnerDocument.CreateElement("item");
							xmlElement2.SetAttribute("key", onlineVariable.Name);
							xmlElement2.SetAttribute("value", onlineVariable.Value);
							xmlElement.AppendChild(xmlElement2);
						}
					}
				}
			}
			rootNode.AppendChild(xmlElement);
		}

		// Token: 0x0600060C RID: 1548 RVA: 0x00018300 File Offset: 0x00016700
		public void Dump()
		{
			foreach (List<OnlineVariable> list in this.m_vars.Values)
			{
				foreach (OnlineVariable onlineVariable in list)
				{
					Log.Info(onlineVariable.ToString());
				}
			}
		}

		// Token: 0x0600060D RID: 1549 RVA: 0x000183A4 File Offset: 0x000167A4
		private void LoadVariables(IEnumerable<ConfigSection> items, Dictionary<string, List<OnlineVariable>> onlineVariables)
		{
			foreach (ConfigSection configSection in items)
			{
				string value = configSection.Get("value");
				string text = configSection.Get("name");
				string text2 = configSection.Get("destination");
				string text3 = configSection.Get("probability");
				OnlineVariableDestination destination = (OnlineVariableDestination)Enum.Parse(typeof(OnlineVariableDestination), text2, true);
				int num = (!string.IsNullOrEmpty(text3)) ? int.Parse(text3) : 100;
				if (num < 0 || num > 100)
				{
					throw new OnlineVariablesException(string.Format("Incorect value of probability {0} for {1}, must be between 0 and 100", num, configSection.Name));
				}
				List<OnlineVariable> list;
				if (!onlineVariables.TryGetValue(text, out list))
				{
					list = new List<OnlineVariable>();
					onlineVariables.Add(text, list);
				}
				else if (list.Any((OnlineVariable x) => x.Destination == destination))
				{
					throw new OnlineVariablesException(string.Format("Duplicate online variable found! Name: {0}, Destination: {1}", text, text2));
				}
				list.Add(new OnlineVariable(text, value, destination, num));
			}
		}

		// Token: 0x040003DD RID: 989
		private Dictionary<string, List<OnlineVariable>> m_vars = new Dictionary<string, List<OnlineVariable>>();

		// Token: 0x040003DE RID: 990
		private Random m_random;
	}
}

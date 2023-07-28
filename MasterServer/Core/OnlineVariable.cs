using System;
using System.Collections.Generic;
using System.Linq;
using MasterServer.Core.Configuration;

namespace MasterServer.Core
{
	// Token: 0x02000157 RID: 343
	internal class OnlineVariable
	{
		// Token: 0x060005FC RID: 1532 RVA: 0x00017D7E File Offset: 0x0001617E
		public OnlineVariable(string name, string value, OnlineVariableDestination destination, int probability)
		{
			this.Name = name;
			this.m_value = value;
			this.Destination = destination;
			this.Probability = probability;
			if (OnlineVariable.IsLinkedVariable(this.m_value))
			{
				this.CreateConfigLink();
			}
		}

		// Token: 0x170000A4 RID: 164
		// (get) Token: 0x060005FD RID: 1533 RVA: 0x00017DB9 File Offset: 0x000161B9
		// (set) Token: 0x060005FE RID: 1534 RVA: 0x00017DC1 File Offset: 0x000161C1
		public string Name { get; private set; }

		// Token: 0x170000A5 RID: 165
		// (get) Token: 0x060005FF RID: 1535 RVA: 0x00017DCA File Offset: 0x000161CA
		public string Value
		{
			get
			{
				if (this.m_config != null)
				{
					return this.m_config.Get(this.m_configSection);
				}
				return this.m_value;
			}
		}

		// Token: 0x170000A6 RID: 166
		// (get) Token: 0x06000600 RID: 1536 RVA: 0x00017DEF File Offset: 0x000161EF
		// (set) Token: 0x06000601 RID: 1537 RVA: 0x00017DF7 File Offset: 0x000161F7
		public OnlineVariableDestination Destination { get; private set; }

		// Token: 0x170000A7 RID: 167
		// (get) Token: 0x06000602 RID: 1538 RVA: 0x00017E00 File Offset: 0x00016200
		// (set) Token: 0x06000603 RID: 1539 RVA: 0x00017E08 File Offset: 0x00016208
		public int Probability { get; private set; }

		// Token: 0x06000604 RID: 1540 RVA: 0x00017E11 File Offset: 0x00016211
		public override string ToString()
		{
			return string.Format("Name: {0}, Value: {1}, Destination: {2}, Probability: {3}%", new object[]
			{
				this.Name,
				this.Value,
				this.Destination,
				this.Probability
			});
		}

		// Token: 0x06000605 RID: 1541 RVA: 0x00017E51 File Offset: 0x00016251
		public static bool IsLinkedVariable(string value)
		{
			return !string.IsNullOrEmpty(value) && value[0] == '@';
		}

		// Token: 0x06000606 RID: 1542 RVA: 0x00017E6C File Offset: 0x0001626C
		private void CreateConfigLink()
		{
			string text = this.m_value.Substring(1);
			string[] array = text.Split(new char[]
			{
				'.'
			});
			if (array.Length < 2)
			{
				throw new OnlineVariablesException(string.Format("Reference variable {0} has invalid format", this.Name));
			}
			string text2 = array[0];
			string configSection = array.Last<string>();
			IEnumerable<string> enumerable = array.Skip(1).Take(array.Length - 2);
			if (text2 != null)
			{
				Config config;
				if (!(text2 == "ModuleSettings"))
				{
					if (!(text2 == "LBSettings"))
					{
						goto IL_9F;
					}
					config = Resources.LBSettings;
				}
				else
				{
					config = Resources.ModuleSettings;
				}
				ConfigSection configSection2 = config;
				foreach (string text3 in enumerable)
				{
					configSection2 = configSection2.GetSection(text3);
					if (configSection2 == null)
					{
						throw new OnlineVariablesException(string.Format("Missing section {0} in path {1}", text3, this.Name));
					}
				}
				this.m_config = configSection2;
				this.m_configSection = configSection;
				return;
			}
			IL_9F:
			throw new OnlineVariablesException(string.Format("{0} unsupported config name", text2));
		}

		// Token: 0x040003DA RID: 986
		private readonly string m_value;

		// Token: 0x040003DB RID: 987
		private ConfigSection m_config;

		// Token: 0x040003DC RID: 988
		private string m_configSection;
	}
}

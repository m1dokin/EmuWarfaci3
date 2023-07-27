using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using OLAPHypervisor;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000023 RID: 35
	public class StatsMappings
	{
		// Token: 0x0600017E RID: 382 RVA: 0x0000E600 File Offset: 0x0000C800
		public StatsMappings(string telemetryStatsMap, Hypervisor hv)
		{
			this.StatMap = new Dictionary<string, StatsMappings.StatParams>();
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(telemetryStatsMap);
			string attribute = xmlDocument.DocumentElement.GetAttribute("default_agg");
			this.DefaultAggOp = this.ParseAggOp(attribute);
			IEnumerator enumerator = xmlDocument.DocumentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != "stat"))
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						string attribute2 = xmlElement.GetAttribute("name");
						string cube = xmlElement.GetAttribute("cube");
						EAggOperation op = (!xmlElement.HasAttribute("agg")) ? this.DefaultAggOp : this.ParseAggOp(xmlElement.GetAttribute("agg"));
						Cube c = hv.DBSchema.Cubes.Find((Cube C) => C.Name == cube);
						this.StatMap.Add(attribute2, new StatsMappings.StatParams(c, op));
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

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600017F RID: 383 RVA: 0x0000E75C File Offset: 0x0000C95C
		// (set) Token: 0x06000180 RID: 384 RVA: 0x0000E764 File Offset: 0x0000C964
		public EAggOperation DefaultAggOp { get; private set; }

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x06000181 RID: 385 RVA: 0x0000E76D File Offset: 0x0000C96D
		// (set) Token: 0x06000182 RID: 386 RVA: 0x0000E775 File Offset: 0x0000C975
		public Dictionary<string, StatsMappings.StatParams> StatMap { get; private set; }

		// Token: 0x06000183 RID: 387 RVA: 0x0000E780 File Offset: 0x0000C980
		private EAggOperation ParseAggOp(string s)
		{
			if (s != null)
			{
				if (s == "sum")
				{
					return 0;
				}
				if (s == "min")
				{
					return 1;
				}
				if (s == "max")
				{
					return 2;
				}
				if (s == "avg")
				{
					return 3;
				}
				if (s == "override")
				{
					return 4;
				}
				if (s == "discard")
				{
					return 5;
				}
			}
			throw new FormatException("Unrecognized aggregation operation: " + s);
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0000E814 File Offset: 0x0000CA14
		public bool GetStatParams(string stat, out Cube cube, out EAggOperation aggOp)
		{
			cube = null;
			aggOp = this.DefaultAggOp;
			StatsMappings.StatParams statParams;
			if (this.StatMap.TryGetValue(stat, out statParams))
			{
				cube = statParams.Cube;
				aggOp = statParams.AggOp;
				return true;
			}
			Log.Warning("Writing stats '{0}' which does not have mapping to cube", new object[]
			{
				stat
			});
			return false;
		}

		// Token: 0x06000185 RID: 389 RVA: 0x0000E868 File Offset: 0x0000CA68
		public bool SetStatParams(string stat, Cube cube, EAggOperation aggOp)
		{
			if (this.StatMap.ContainsKey(stat))
			{
				this.StatMap[stat] = new StatsMappings.StatParams(cube, aggOp);
				return true;
			}
			Log.Warning("No params for stat '{0}' found", new object[]
			{
				stat
			});
			return false;
		}

		// Token: 0x02000024 RID: 36
		public struct StatParams
		{
			// Token: 0x06000186 RID: 390 RVA: 0x0000E8A5 File Offset: 0x0000CAA5
			public StatParams(Cube c, EAggOperation op)
			{
				this.Cube = c;
				this.AggOp = op;
			}

			// Token: 0x04000078 RID: 120
			public Cube Cube;

			// Token: 0x04000079 RID: 121
			public EAggOperation AggOp;
		}
	}
}

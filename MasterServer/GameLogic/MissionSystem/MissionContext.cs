using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x0200078F RID: 1935
	internal class MissionContext : MissionContextBase
	{
		// Token: 0x0600280D RID: 10253 RVA: 0x000ABFCE File Offset: 0x000AA3CE
		public override string ToString()
		{
			return string.Format("MissionContext {0} {1} {2}", this.uid, this.name, this.difficulty);
		}

		// Token: 0x0600280E RID: 10254 RVA: 0x000ABFEC File Offset: 0x000AA3EC
		public override void Dump(bool isFull)
		{
			base.Dump(isFull);
			if (isFull)
			{
				this.baseLevel.Dump();
				Log.Info<int>("Sub levels amount: {0}", this.subLevels.Count);
				foreach (SubLevel subLevel in this.subLevels)
				{
					subLevel.Dump();
				}
			}
		}

		// Token: 0x0600280F RID: 10255 RVA: 0x000AC074 File Offset: 0x000AA474
		public string GetFormattedXml()
		{
			XmlDocument xmlDocument = new XmlDocument();
			StringWriter stringWriter = new StringWriter(new StringBuilder());
			XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter);
			xmlTextWriter.Formatting = Formatting.Indented;
			xmlDocument.LoadXml(this.data);
			xmlDocument.WriteTo(xmlTextWriter);
			return stringWriter.GetStringBuilder().ToString();
		}

		// Token: 0x040014F6 RID: 5366
		public int Version;

		// Token: 0x040014F7 RID: 5367
		public int Generation;

		// Token: 0x040014F8 RID: 5368
		public MissionUIInfo UIInfo;

		// Token: 0x040014F9 RID: 5369
		public string missionName = string.Empty;

		// Token: 0x040014FA RID: 5370
		public string data;

		// Token: 0x040014FB RID: 5371
		public string levelGraph;

		// Token: 0x040014FC RID: 5372
		public SubLevel baseLevel;

		// Token: 0x040014FD RID: 5373
		public List<SubLevel> subLevels = new List<SubLevel>();

		// Token: 0x040014FE RID: 5374
		public List<MissionObjective> objectives = new List<MissionObjective>();

		// Token: 0x040014FF RID: 5375
		public DependencyInfo timeDependencyInfo = default(DependencyInfo);

		// Token: 0x04001500 RID: 5376
		public DependencyInfo killDependencyInfo = default(DependencyInfo);

		// Token: 0x04001501 RID: 5377
		public bool noTeamsMode;
	}
}

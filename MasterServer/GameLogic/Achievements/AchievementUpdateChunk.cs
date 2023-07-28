using System;
using System.Xml;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x0200025B RID: 603
	[Serializable]
	public struct AchievementUpdateChunk
	{
		// Token: 0x06000D42 RID: 3394 RVA: 0x00034388 File Offset: 0x00032788
		public AchievementUpdateChunk(uint aid, int pr, ulong completionTime)
		{
			this.achievementId = aid;
			this.progress = pr;
			this.completionTime = completionTime;
		}

		// Token: 0x06000D43 RID: 3395 RVA: 0x000343A0 File Offset: 0x000327A0
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("achievement");
			xmlElement.SetAttribute("achievement_id", this.achievementId.ToString());
			xmlElement.SetAttribute("progress", this.progress.ToString());
			xmlElement.SetAttribute("completion_time", this.completionTime.ToString());
			return xmlElement;
		}

		// Token: 0x06000D44 RID: 3396 RVA: 0x0003440E File Offset: 0x0003280E
		public override string ToString()
		{
			return string.Format("Id: {0}, Progress: {1}, Complete Time: {2}", this.achievementId, this.progress, this.completionTime);
		}

		// Token: 0x04000618 RID: 1560
		public uint achievementId;

		// Token: 0x04000619 RID: 1561
		public int progress;

		// Token: 0x0400061A RID: 1562
		public ulong completionTime;
	}
}

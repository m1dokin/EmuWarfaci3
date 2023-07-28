using System;
using System.Xml;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C5 RID: 965
	internal class SecondaryObjective
	{
		// Token: 0x06001548 RID: 5448 RVA: 0x00059414 File Offset: 0x00057814
		public SecondaryObjective(int id, XmlElement difficultiesElem, string difficulty)
		{
			this.Id = id;
			this.Difficulty = difficulty;
			XmlElement xmlElement = difficultiesElem.GetElementsByTagName(difficulty)[0] as XmlElement;
		}

		// Token: 0x170001F4 RID: 500
		// (get) Token: 0x0600154A RID: 5450 RVA: 0x00059451 File Offset: 0x00057851
		// (set) Token: 0x06001549 RID: 5449 RVA: 0x00059448 File Offset: 0x00057848
		public int Id
		{
			get
			{
				return this.m_id;
			}
			set
			{
				this.m_id = value;
			}
		}

		// Token: 0x170001F5 RID: 501
		// (get) Token: 0x0600154C RID: 5452 RVA: 0x00059462 File Offset: 0x00057862
		// (set) Token: 0x0600154B RID: 5451 RVA: 0x00059459 File Offset: 0x00057859
		public string Difficulty
		{
			get
			{
				return this.m_difficulty;
			}
			set
			{
				this.m_difficulty = value;
			}
		}

		// Token: 0x04000A27 RID: 2599
		private int m_id;

		// Token: 0x04000A28 RID: 2600
		private string m_difficulty;
	}
}

using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.DAL;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003ED RID: 1005
	internal class ProfilePerformanceInfo
	{
		// Token: 0x060015DE RID: 5598 RVA: 0x0005B2B3 File Offset: 0x000596B3
		public ProfilePerformanceInfo(ulong profileID)
		{
			this.ProfileID = profileID;
		}

		// Token: 0x04000A6F RID: 2671
		public ulong ProfileID;

		// Token: 0x04000A70 RID: 2672
		public Dictionary<string, ProfilePerformanceInfo.MissionPerformance> MissionPerformances = new Dictionary<string, ProfilePerformanceInfo.MissionPerformance>();

		// Token: 0x020003EE RID: 1006
		public struct Performer
		{
			// Token: 0x060015DF RID: 5599 RVA: 0x0005B2CD File Offset: 0x000596CD
			public Performer(ulong pid, string nck, string clan, ulong exp)
			{
				this.ProfileID = pid;
				this.Nickname = nck;
				this.ClanName = clan;
				this.Experience = exp;
			}

			// Token: 0x04000A71 RID: 2673
			public ulong ProfileID;

			// Token: 0x04000A72 RID: 2674
			public string Nickname;

			// Token: 0x04000A73 RID: 2675
			public string ClanName;

			// Token: 0x04000A74 RID: 2676
			public ulong Experience;
		}

		// Token: 0x020003EF RID: 1007
		public class StatPerformance
		{
			// Token: 0x04000A75 RID: 2677
			public uint Stat;

			// Token: 0x04000A76 RID: 2678
			public uint ProfilePerformance;

			// Token: 0x04000A77 RID: 2679
			public uint TopPerformance;

			// Token: 0x04000A78 RID: 2680
			public int PredictedPlace;

			// Token: 0x04000A79 RID: 2681
			public int League;

			// Token: 0x04000A7A RID: 2682
			public List<ProfilePerformanceInfo.Performer> TopPerformers = new List<ProfilePerformanceInfo.Performer>();
		}

		// Token: 0x020003F0 RID: 1008
		public class MissionPerformance
		{
			// Token: 0x060015E2 RID: 5602 RVA: 0x0005B314 File Offset: 0x00059714
			public XmlElement WriteToXml(XmlDocument ownerDocument)
			{
				XmlElement xmlElement = ownerDocument.CreateElement("performance");
				int num = (this.Status != MissionStatus.Finished) ? ((this.Status != MissionStatus.Failed) ? -1 : 0) : 1;
				xmlElement.SetAttribute("mission_id", this.MissionID.ToString("D"));
				xmlElement.SetAttribute("success", num.ToString());
				foreach (ProfilePerformanceInfo.StatPerformance statPerformance in this.StatPerformances.Values)
				{
					XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("leaderboard");
					xmlElement.AppendChild(xmlElement2);
					xmlElement2.SetAttribute("stat", statPerformance.Stat.ToString());
					xmlElement2.SetAttribute("profile_performance", statPerformance.ProfilePerformance.ToString());
					xmlElement2.SetAttribute("max_performance", statPerformance.TopPerformance.ToString());
					xmlElement2.SetAttribute("position", statPerformance.PredictedPlace.ToString());
					xmlElement2.SetAttribute("league", statPerformance.League.ToString());
					foreach (ProfilePerformanceInfo.Performer performer in statPerformance.TopPerformers)
					{
						XmlElement xmlElement3 = xmlElement.OwnerDocument.CreateElement("player");
						xmlElement2.AppendChild(xmlElement3);
						xmlElement3.SetAttribute("nickname", performer.Nickname);
						xmlElement3.SetAttribute("experience", performer.Experience.ToString());
						xmlElement3.SetAttribute("clan", performer.ClanName);
					}
				}
				return xmlElement;
			}

			// Token: 0x04000A7B RID: 2683
			public Guid MissionID;

			// Token: 0x04000A7C RID: 2684
			public MissionStatus Status;

			// Token: 0x04000A7D RID: 2685
			public Dictionary<uint, ProfilePerformanceInfo.StatPerformance> StatPerformances = new Dictionary<uint, ProfilePerformanceInfo.StatPerformance>();
		}
	}
}

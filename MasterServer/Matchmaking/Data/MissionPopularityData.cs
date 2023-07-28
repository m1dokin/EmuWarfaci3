using System;

namespace MasterServer.Matchmaking.Data
{
	// Token: 0x02000594 RID: 1428
	internal class MissionPopularityData : IComparable<MissionPopularityData>
	{
		// Token: 0x06001EBA RID: 7866 RVA: 0x0007D14A File Offset: 0x0007B54A
		public MissionPopularityData(string missionId, int popularity)
		{
			this.MissionId = missionId;
			this.Popularity = popularity;
		}

		// Token: 0x17000322 RID: 802
		// (get) Token: 0x06001EBB RID: 7867 RVA: 0x0007D160 File Offset: 0x0007B560
		// (set) Token: 0x06001EBC RID: 7868 RVA: 0x0007D168 File Offset: 0x0007B568
		public string MissionId { get; set; }

		// Token: 0x17000323 RID: 803
		// (get) Token: 0x06001EBD RID: 7869 RVA: 0x0007D171 File Offset: 0x0007B571
		// (set) Token: 0x06001EBE RID: 7870 RVA: 0x0007D179 File Offset: 0x0007B579
		public int Popularity { get; set; }

		// Token: 0x06001EBF RID: 7871 RVA: 0x0007D184 File Offset: 0x0007B584
		public int CompareTo(MissionPopularityData other)
		{
			return other.Popularity.CompareTo(this.Popularity);
		}
	}
}

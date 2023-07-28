using System;

namespace MasterServer.DAL
{
	// Token: 0x0200007F RID: 127
	public class ProfileProgressionSerializer : IDBSerializer<SProfileProgression>
	{
		// Token: 0x06000187 RID: 391 RVA: 0x00004D5C File Offset: 0x0000315C
		public void Deserialize(IDataReaderEx reader, out SProfileProgression ret)
		{
			ret = default(SProfileProgression);
			ret.ProfileId = ulong.Parse(reader["profile_id"].ToString());
			ret.MissionPassCounter = int.Parse(reader["mission_pass_counter"].ToString());
			ret.ZombieMissionPassCounter = int.Parse(reader["zombie_mission_pass_counter"].ToString());
			ret.CampaignPassCounter = int.Parse(reader["campaign_pass_counter"].ToString());
			ret.VolcanoCampaignPasCounter = int.Parse(reader["volcano_campaign_pass_counter"].ToString());
			ret.AnubisCampaignPassCounter = int.Parse(reader["anubis_campaign_pass_counter"].ToString());
			ret.ZombieTowerCampaignPassCounter = int.Parse(reader["zombietower_campaign_pass_counter"].ToString());
			ret.IceBreakerCampaignPassCounter = int.Parse(reader["icebreaker_campaign_pass_counter"].ToString());
			ret.MissionUnlocked = int.Parse(reader["mission_unlocked"].ToString());
			ret.TutorialUnlocked = int.Parse(reader["tutorial_unlocked"].ToString());
			ret.TutorialPassed = int.Parse(reader["tutorial_passed"].ToString());
			ret.ClassUnlocked = int.Parse(reader["class_unlocked"].ToString());
		}
	}
}

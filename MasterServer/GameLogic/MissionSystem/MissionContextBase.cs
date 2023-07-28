using System;
using System.Collections.Generic;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x0200078C RID: 1932
	internal abstract class MissionContextBase
	{
		// Token: 0x06002806 RID: 10246 RVA: 0x000ABE53 File Offset: 0x000AA253
		public bool IsPveMode()
		{
			return this.gameMode.ToLower() == "pve";
		}

		// Token: 0x06002807 RID: 10247 RVA: 0x000ABE6A File Offset: 0x000AA26A
		public bool IsPvPMode()
		{
			return !this.IsPveMode();
		}

		// Token: 0x06002808 RID: 10248 RVA: 0x000ABE75 File Offset: 0x000AA275
		public bool IsAvailableForRatingGame()
		{
			return this.ratingMission;
		}

		// Token: 0x06002809 RID: 10249 RVA: 0x000ABE7D File Offset: 0x000AA27D
		public GameRoomType GetRoomType()
		{
			return (!this.IsPveMode()) ? GameRoomType.PvP : GameRoomType.PvE;
		}

		// Token: 0x0600280A RID: 10250 RVA: 0x000ABE94 File Offset: 0x000AA294
		public virtual void Dump(bool isFull)
		{
			Log.Info<string>("Mission: {0}", this.name);
			Log.Info<string>("Setting: {0}", this.setting);
			Log.Info<string>("Game mode: {0}", this.gameMode);
			Log.Info<MissionType>("Type: {0}", this.missionType);
			Log.Info<string>("Difficulty: {0}", this.difficulty);
			Log.Info<bool>("Release mission: {0}", this.releaseMission);
			Log.Info<bool>("ClanWar mission: {0}", this.clanWarMission);
			Log.Info<bool>("Only ClanWar mission: {0}", this.onlyClanWarMission);
			Log.Info<int>("Tutorial mission: {0}", this.tutorialMission);
			Log.Info<string>("Time of day: {0}", this.timeOfDay);
			Log.Info<string>("GUID: {0}", this.uid);
		}

		// Token: 0x040014E3 RID: 5347
		public string name;

		// Token: 0x040014E4 RID: 5348
		public string setting;

		// Token: 0x040014E5 RID: 5349
		public string uid;

		// Token: 0x040014E6 RID: 5350
		public string gameMode;

		// Token: 0x040014E7 RID: 5351
		public MissionType missionType;

		// Token: 0x040014E8 RID: 5352
		public string difficulty;

		// Token: 0x040014E9 RID: 5353
		public bool releaseMission;

		// Token: 0x040014EA RID: 5354
		public bool clanWarMission;

		// Token: 0x040014EB RID: 5355
		public bool onlyClanWarMission;

		// Token: 0x040014EC RID: 5356
		public HashSet<Resources.ChannelType> channels;

		// Token: 0x040014ED RID: 5357
		public int tutorialMission;

		// Token: 0x040014EE RID: 5358
		public string timeOfDay;

		// Token: 0x040014EF RID: 5359
		public bool ratingMission;
	}
}

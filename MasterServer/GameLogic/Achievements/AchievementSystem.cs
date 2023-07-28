using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.StatsTracking;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x0200025C RID: 604
	[Service]
	[Singleton]
	internal class AchievementSystem : ServiceModule, IAchievementSystem, IDebugAchievementSystem
	{
		// Token: 0x06000D45 RID: 3397 RVA: 0x0003443B File Offset: 0x0003283B
		public AchievementSystem(IAchievementConfigProvider configProvider, IDALService dalService, IUserRepository userRepository, IGameRoomManager gameRoomManager, ILogService logService)
		{
			this.m_achievementConfig = configProvider.GetConfing();
			this.m_dalService = dalService;
			this.m_userRepository = userRepository;
			this.m_gameRoomManager = gameRoomManager;
			this.m_logService = logService;
		}

		// Token: 0x06000D46 RID: 3398 RVA: 0x00034478 File Offset: 0x00032878
		public override void Init()
		{
			string filename = Path.Combine(Resources.GetResourcesDirectory(), "achievements_data.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(filename);
			this.m_achievementConfig.ReadData(xmlDocument);
		}

		// Token: 0x06000D47 RID: 3399 RVA: 0x000344AE File Offset: 0x000328AE
		public override void Start()
		{
			base.Start();
			this.m_achievementTracker.Init();
		}

		// Token: 0x06000D48 RID: 3400 RVA: 0x000344C4 File Offset: 0x000328C4
		public Dictionary<uint, AchievementUpdateChunk> GetCurrentProfileAchievements(ulong profileId)
		{
			Dictionary<uint, AchievementUpdateChunk> dictionary = new Dictionary<uint, AchievementUpdateChunk>();
			IEnumerable<AchievementInfo> profileAchievements = this.m_dalService.AchievementSystem.GetProfileAchievements(profileId);
			foreach (AchievementInfo achievementInfo in profileAchievements)
			{
				dictionary.Add((uint)achievementInfo.ID, new AchievementUpdateChunk((uint)achievementInfo.ID, achievementInfo.Progress, achievementInfo.CompletionTime));
			}
			return dictionary;
		}

		// Token: 0x06000D49 RID: 3401 RVA: 0x00034554 File Offset: 0x00032954
		public bool SetAchievementProgress(ulong profileId, AchievementDescription ach, ref AchievementUpdateChunk new_state)
		{
			SAchievementUpdate updateRes = this.m_dalService.AchievementSystem.SetAchievementProgress(profileId, ach.Id, new_state.progress, ach.Amount, new_state.completionTime);
			return this.ProcessAchievementChange(profileId, ref new_state, updateRes);
		}

		// Token: 0x06000D4A RID: 3402 RVA: 0x00034594 File Offset: 0x00032994
		public bool UpdateAchievementProgress(ulong profileId, AchievementDescription ach, ref AchievementUpdateChunk new_state)
		{
			SAchievementUpdate updateRes = this.m_dalService.AchievementSystem.UpdateAchievementProgress(profileId, ach.Id, new_state.progress, ach.Amount);
			return this.ProcessAchievementChange(profileId, ref new_state, updateRes);
		}

		// Token: 0x06000D4B RID: 3403 RVA: 0x000345D0 File Offset: 0x000329D0
		public AchievementLockStatus DeleteProfileHiddenAchievement(ulong profileId, uint achievement_id)
		{
			AchievementDescription achievementDesc = this.GetAchievementDesc(achievement_id);
			if (achievementDesc == null)
			{
				return AchievementLockStatus.NotExists;
			}
			if (achievementDesc.Kind != EStatsEvent.HIDDEN)
			{
				return AchievementLockStatus.NotHidden;
			}
			this.m_dalService.AchievementSystem.DeleteProfileAchievement(profileId, achievement_id);
			return AchievementLockStatus.Ok;
		}

		// Token: 0x06000D4C RID: 3404 RVA: 0x0003460F File Offset: 0x00032A0F
		public AchievementDescription GetAchievementDesc(uint id)
		{
			return this.m_achievementConfig.GetAchievementDesc(id);
		}

		// Token: 0x06000D4D RID: 3405 RVA: 0x0003461D File Offset: 0x00032A1D
		public Dictionary<uint, AchievementDescription> GetAllAchievementDescs()
		{
			return this.m_achievementConfig.GetAllAchievementDescs();
		}

		// Token: 0x06000D4E RID: 3406 RVA: 0x0003462A File Offset: 0x00032A2A
		public void DeleteProfileAchievements(ulong profileId)
		{
			this.m_dalService.AchievementSystem.DeleteProfileAchievements(profileId);
		}

		// Token: 0x06000D4F RID: 3407 RVA: 0x00034640 File Offset: 0x00032A40
		private bool ProcessAchievementChange(ulong profileId, ref AchievementUpdateChunk newState, SAchievementUpdate updateRes)
		{
			newState.achievementId = (uint)updateRes.Info.ID;
			newState.progress = updateRes.Info.Progress;
			newState.completionTime = updateRes.Info.CompletionTime;
			this.LogCharacterAchieved(profileId, newState, updateRes.Status);
			return updateRes.Status != EAchevementStatus.AlreadyCompleted;
		}

		// Token: 0x06000D50 RID: 3408 RVA: 0x000346A4 File Offset: 0x00032AA4
		private void LogCharacterAchieved(ulong profileId, AchievementUpdateChunk newState, EAchevementStatus status)
		{
			SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
			UserInfo.User user = this.m_userRepository.GetUser(profileId);
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			GameRoomType roomType = GameRoomType.All;
			string missionName = string.Empty;
			string missionSetting = string.Empty;
			if (roomByPlayer != null)
			{
				roomType = roomByPlayer.Type;
				roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					MissionState state = r.GetState<MissionState>(AccessMode.ReadOnly);
					missionName = state.Mission.name;
					missionSetting = state.Mission.setting;
				});
			}
			this.m_logService.Event.CharacterAchievedLog(profileInfo.UserID, (user == null) ? "0.0.0.0" : user.IP, profileId, profileInfo.Nickname, profileInfo.RankInfo.RankId, roomType, missionName, missionSetting, newState.achievementId, newState.progress, status);
		}

		// Token: 0x0400061B RID: 1563
		private readonly AchievementConfig m_achievementConfig;

		// Token: 0x0400061C RID: 1564
		private readonly AchievementTracker m_achievementTracker = new AchievementTracker();

		// Token: 0x0400061D RID: 1565
		private readonly IDALService m_dalService;

		// Token: 0x0400061E RID: 1566
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400061F RID: 1567
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000620 RID: 1568
		private readonly ILogService m_logService;
	}
}

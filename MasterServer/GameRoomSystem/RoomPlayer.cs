using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.GameLogic.SkillSystem;
using MasterServer.Users;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000774 RID: 1908
	internal class RoomPlayer : ICloneable
	{
		// Token: 0x06002797 RID: 10135 RVA: 0x00064C7C File Offset: 0x0006307C
		public RoomPlayer()
		{
		}

		// Token: 0x06002798 RID: 10136 RVA: 0x00064CB8 File Offset: 0x000630B8
		public RoomPlayer(UserInfo.User user, IClanService clanService)
		{
			this.ProfileID = user.ProfileID;
			this.UserID = user.UserID;
			this.OnlineID = user.OnlineID;
			this.Nickname = user.Nickname;
			this.UserStatus = (UserStatus.Online | UserStatus.InGameRoom);
			this.Rank = user.Rank;
			this.Experience = user.Experience;
			this.Banner = user.Banner;
			this.SessionsPlayedInRoom = 0;
			this.JoinTime = DateTime.UtcNow;
			this.GameWaitStartTime = DateTime.MinValue;
			this.LastSessionEndTime = DateTime.MinValue;
			this.QuickPlaySearchTime = TimeSpan.Zero;
			this.ProfileProgression = user.ProfileProgression;
			this.BuildType = user.BuildType;
			this.RegionId = user.RegionId;
			this.UpdateClanInfo(clanService);
		}

		// Token: 0x170003B0 RID: 944
		// (get) Token: 0x06002799 RID: 10137 RVA: 0x00064DB8 File Offset: 0x000631B8
		public bool HasGroup
		{
			get
			{
				return this.GroupID != RoomPlayer.DefaultGroup;
			}
		}

		// Token: 0x0600279A RID: 10138 RVA: 0x00064DCA File Offset: 0x000631CA
		public void UpdateClanInfo(IClanService clanService)
		{
			this.m_clanInfo = clanService.GetClanInfoByPid(this.ProfileID);
		}

		// Token: 0x0600279B RID: 10139 RVA: 0x00064DDE File Offset: 0x000631DE
		public virtual bool CanJoinMission(IMissionAccessLimitationService limitationService, string missionName)
		{
			return limitationService.CanJoinMission(this.ProfileID, missionName);
		}

		// Token: 0x0600279C RID: 10140 RVA: 0x00064DED File Offset: 0x000631ED
		public virtual string GetClanName()
		{
			return (this.m_clanInfo != null) ? this.m_clanInfo.Name : string.Empty;
		}

		// Token: 0x0600279D RID: 10141 RVA: 0x00064E10 File Offset: 0x00063210
		public bool IsInClan()
		{
			string clanName = this.GetClanName();
			return !string.IsNullOrEmpty(clanName);
		}

		// Token: 0x0600279E RID: 10142 RVA: 0x00064E2D File Offset: 0x0006322D
		public bool IsInClan(string clanName)
		{
			return this.IsInClan() && clanName == this.GetClanName();
		}

		// Token: 0x0600279F RID: 10143 RVA: 0x00064E4C File Offset: 0x0006324C
		public virtual void Serialize(XmlElement el, RoomUpdate.Target target)
		{
			el.SetAttribute("profile_id", this.ProfileID.ToString());
			el.SetAttribute("team_id", this.TeamID.ToString());
			string name = "status";
			int roomStatus = (int)this.RoomStatus;
			el.SetAttribute(name, roomStatus.ToString());
			el.SetAttribute("observer", (!this.Observer) ? "0" : "1");
			el.SetAttribute("skill", this.Skill.Value.ToString("0.000"));
			if (target != RoomUpdate.Target.Server)
			{
				el.SetAttribute("nickname", this.Nickname);
				el.SetAttribute("clanName", this.GetClanName());
				el.SetAttribute("class_id", this.ClassID.ToString());
				el.SetAttribute("online_id", this.OnlineID);
				el.SetAttribute("group_id", this.GroupID);
				string name2 = "presence";
				int userStatus = (int)this.UserStatus;
				el.SetAttribute(name2, userStatus.ToString());
				el.SetAttribute("experience", this.Experience.ToString());
				el.SetAttribute("rank", this.Rank.ToString());
				el.SetAttribute("banner_badge", this.Banner.Badge.ToString());
				el.SetAttribute("banner_mark", this.Banner.Mark.ToString());
				el.SetAttribute("banner_stripe", this.Banner.Stripe.ToString());
				el.SetAttribute("region_id", this.RegionId);
			}
		}

		// Token: 0x060027A0 RID: 10144 RVA: 0x0006502C File Offset: 0x0006342C
		public object Clone()
		{
			RoomPlayer roomPlayer = (RoomPlayer)base.MemberwiseClone();
			UserInfo.User user = ServicesManager.GetService<IUserRepository>().GetUser(this.ProfileID);
			if (user != null)
			{
				roomPlayer.Rank = user.Rank;
				roomPlayer.Experience = user.Experience;
				roomPlayer.Banner = user.Banner;
				roomPlayer.ProfileProgression = user.ProfileProgression;
			}
			return roomPlayer;
		}

		// Token: 0x060027A1 RID: 10145 RVA: 0x00065090 File Offset: 0x00063490
		public override bool Equals(object obj)
		{
			RoomPlayer roomPlayer = obj as RoomPlayer;
			return roomPlayer != null && this.Equals(roomPlayer);
		}

		// Token: 0x060027A2 RID: 10146 RVA: 0x000650B4 File Offset: 0x000634B4
		public bool Equals(RoomPlayer p)
		{
			return p.ProfileID == this.ProfileID && p.UserID == this.UserID && p.OnlineID == this.OnlineID && p.Nickname == this.Nickname && p.GetClanName() == this.GetClanName() && p.TeamID == this.TeamID && p.ClassID == this.ClassID && p.RoomStatus == this.RoomStatus && p.UserStatus == this.UserStatus && p.Rank == this.Rank && p.Experience == this.Experience && p.Banner.Equals(this.Banner) && p.ProfileProgression.Equals(this.ProfileProgression) && p.Observer == this.Observer && p.GroupID == this.GroupID && p.RegionId == this.RegionId;
		}

		// Token: 0x060027A3 RID: 10147 RVA: 0x000651F4 File Offset: 0x000635F4
		public override int GetHashCode()
		{
			return this.ProfileID.GetHashCode() ^ this.UserID.GetHashCode() ^ this.OnlineID.GetHashCode() ^ this.Nickname.GetHashCode() ^ this.GetClanName().GetHashCode() ^ this.GroupID.GetHashCode() ^ this.TeamID ^ this.ClassID ^ (int)this.RoomStatus ^ (int)this.UserStatus ^ this.Rank ^ this.Experience.GetHashCode() ^ this.Banner.GetHashCode() ^ this.ProfileProgression.GetHashCode() ^ this.Observer.GetHashCode() ^ this.RegionId.GetHashCode();
		}

		// Token: 0x060027A4 RID: 10148 RVA: 0x000652C8 File Offset: 0x000636C8
		public override string ToString()
		{
			return string.Format("ProfileId {0}, Nickname {1}, Team {2}, Group {3}, RegionId {4}, Skill {5}", new object[]
			{
				this.ProfileID,
				this.Nickname,
				this.TeamID,
				this.GroupID,
				this.RegionId,
				this.Skill.Value
			});
		}

		// Token: 0x04001495 RID: 5269
		public static readonly string DefaultGroup = string.Empty;

		// Token: 0x04001496 RID: 5270
		public ulong ProfileID;

		// Token: 0x04001497 RID: 5271
		public ulong UserID;

		// Token: 0x04001498 RID: 5272
		public string OnlineID = string.Empty;

		// Token: 0x04001499 RID: 5273
		public string Nickname = string.Empty;

		// Token: 0x0400149A RID: 5274
		public readonly string BuildType = string.Empty;

		// Token: 0x0400149B RID: 5275
		public int TeamID;

		// Token: 0x0400149C RID: 5276
		public int ClassID;

		// Token: 0x0400149D RID: 5277
		public string GroupID = RoomPlayer.DefaultGroup;

		// Token: 0x0400149E RID: 5278
		public RoomPlayer.EStatus RoomStatus;

		// Token: 0x0400149F RID: 5279
		public UserStatus UserStatus;

		// Token: 0x040014A0 RID: 5280
		public int Rank;

		// Token: 0x040014A1 RID: 5281
		public ulong Experience;

		// Token: 0x040014A2 RID: 5282
		public Skill Skill;

		// Token: 0x040014A3 RID: 5283
		public SBannerInfo Banner;

		// Token: 0x040014A4 RID: 5284
		public ProfileProgressionInfo ProfileProgression;

		// Token: 0x040014A5 RID: 5285
		public bool Observer;

		// Token: 0x040014A6 RID: 5286
		public string RegionId;

		// Token: 0x040014A7 RID: 5287
		public int SessionsPlayedInRoom;

		// Token: 0x040014A8 RID: 5288
		public DateTime JoinTime;

		// Token: 0x040014A9 RID: 5289
		public DateTime LastSessionEndTime;

		// Token: 0x040014AA RID: 5290
		public TimeSpan QuickPlaySearchTime;

		// Token: 0x040014AB RID: 5291
		public DateTime GameWaitStartTime;

		// Token: 0x040014AC RID: 5292
		public bool WasMaster;

		// Token: 0x040014AD RID: 5293
		public int Revision = 1;

		// Token: 0x040014AE RID: 5294
		private ClanInfo m_clanInfo;

		// Token: 0x02000775 RID: 1909
		public enum EStatus
		{
			// Token: 0x040014B0 RID: 5296
			NotReady,
			// Token: 0x040014B1 RID: 5297
			Ready,
			// Token: 0x040014B2 RID: 5298
			CantBeReady
		}
	}
}

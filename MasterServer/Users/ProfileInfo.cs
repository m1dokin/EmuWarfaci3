using System;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.Users
{
	// Token: 0x020007ED RID: 2029
	public struct ProfileInfo
	{
		// Token: 0x0600299A RID: 10650 RVA: 0x000B3968 File Offset: 0x000B1D68
		public ProfileInfo(NeutralProfileRef neutralRef)
		{
			this.ProfileID = neutralRef.ProfileId;
			this.Nickname = neutralRef.Nickname;
			this.UserID = 0UL;
			this.OnlineID = string.Empty;
			this.Status = UserStatus.Offline;
			this.Location = string.Empty;
			this.RankId = 0;
			this.IPAddress = string.Empty;
			this.LoginTime = default(DateTime);
		}

		// Token: 0x0600299B RID: 10651 RVA: 0x000B39D3 File Offset: 0x000B1DD3
		internal ProfileInfo(UserInfo.User user)
		{
			this = new ProfileInfo(new NeutralProfileRef(0UL));
			this.Complete(user);
		}

		// Token: 0x0600299C RID: 10652 RVA: 0x000B39E9 File Offset: 0x000B1DE9
		public bool IsPartial()
		{
			return (string.IsNullOrEmpty(this.Nickname) || this.ProfileID == 0UL) && this.Status == UserStatus.Offline;
		}

		// Token: 0x0600299D RID: 10653 RVA: 0x000B3A14 File Offset: 0x000B1E14
		public void Complete(SProfileInfo dbProfileInfo)
		{
			this.Nickname = dbProfileInfo.Nickname;
			this.ProfileID = dbProfileInfo.Id;
			this.UserID = dbProfileInfo.UserID;
			this.RankId = dbProfileInfo.RankInfo.RankId;
		}

		// Token: 0x0600299E RID: 10654 RVA: 0x000B3A50 File Offset: 0x000B1E50
		internal void Complete(UserInfo.User user)
		{
			this.Nickname = user.Nickname;
			this.ProfileID = user.ProfileID;
			this.UserID = user.UserID;
			this.OnlineID = user.OnlineID;
			this.Status = UserStatus.Online;
			this.Location = string.Empty;
			this.RankId = user.Rank;
			this.IPAddress = user.IP;
			this.LoginTime = user.LoginTime;
		}

		// Token: 0x0600299F RID: 10655 RVA: 0x000B3AC4 File Offset: 0x000B1EC4
		public void Dump()
		{
			Log.Info<string>("Nickname: {0}", this.Nickname);
			Log.Info<string>("OnlineID: {0}", this.OnlineID);
			Log.Info<string>("Status: {0}", this.Status.ToString());
			Log.Info<string>("Location: {0}", this.Location);
			Log.Info<string>("Pid: {0}", this.ProfileID.ToString());
			Log.Info<string>("RankId: {0}", this.RankId.ToString());
			Log.Info<string>("User: {0}", this.UserID.ToString());
			Log.Info<string>("IP: {0}", this.IPAddress);
			Log.Info<string>("LoginTime: {0}", this.LoginTime.ToString());
		}

		// Token: 0x04001615 RID: 5653
		public string Nickname;

		// Token: 0x04001616 RID: 5654
		public string OnlineID;

		// Token: 0x04001617 RID: 5655
		public UserStatus Status;

		// Token: 0x04001618 RID: 5656
		public string Location;

		// Token: 0x04001619 RID: 5657
		public ulong ProfileID;

		// Token: 0x0400161A RID: 5658
		public int RankId;

		// Token: 0x0400161B RID: 5659
		public ulong UserID;

		// Token: 0x0400161C RID: 5660
		public string IPAddress;

		// Token: 0x0400161D RID: 5661
		public DateTime LoginTime;
	}
}

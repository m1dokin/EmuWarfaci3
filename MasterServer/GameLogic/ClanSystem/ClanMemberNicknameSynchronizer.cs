using System;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.Platform.Nickname;
using Util.Common;

namespace MasterServer.GameLogic.ClanSystem
{
	// Token: 0x02000003 RID: 3
	[Service]
	[Singleton]
	internal class ClanMemberNicknameSynchronizer : ServiceModule, IClanMemberNicknameSynchronizer
	{
		// Token: 0x06000003 RID: 3 RVA: 0x0000401B File Offset: 0x0000241B
		public ClanMemberNicknameSynchronizer(IClanService clanService, IExternalNicknameSyncService externalNicknameSyncService)
		{
			this.m_clanService = clanService;
			this.m_externalNicknameSyncService = externalNicknameSyncService;
		}

		// Token: 0x14000002 RID: 2
		// (add) Token: 0x06000004 RID: 4 RVA: 0x00004034 File Offset: 0x00002434
		// (remove) Token: 0x06000005 RID: 5 RVA: 0x0000406C File Offset: 0x0000246C
		public event Action<ClanMember> ClanMemberRenamed;

		// Token: 0x06000006 RID: 6 RVA: 0x000040A2 File Offset: 0x000024A2
		public override void Start()
		{
			base.Start();
			this.m_externalNicknameSyncService.ProfileRenamed += this.OnProfileRenamed;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000040C1 File Offset: 0x000024C1
		public override void Stop()
		{
			this.m_externalNicknameSyncService.ProfileRenamed -= this.OnProfileRenamed;
			base.Stop();
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000040E0 File Offset: 0x000024E0
		private void OnProfileRenamed(ulong profileId)
		{
			ClanMember memberInfo = this.m_clanService.GetMemberInfo(profileId);
			if (memberInfo == null)
			{
				return;
			}
			this.ClanMemberRenamed.SafeInvoke(memberInfo);
		}

		// Token: 0x04000001 RID: 1
		private readonly IClanService m_clanService;

		// Token: 0x04000002 RID: 2
		private readonly IExternalNicknameSyncService m_externalNicknameSyncService;
	}
}

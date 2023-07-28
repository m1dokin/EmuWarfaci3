using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.ClanSystem;
using MasterServer.GameLogic.InvitationSystem;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.ProfileLogic;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.GameLogic.InvitationService
{
	// Token: 0x0200030E RID: 782
	[QueryAttributes(TagName = "send_invitation")]
	internal class InvitationQuery : BaseQuery
	{
		// Token: 0x060011FD RID: 4605 RVA: 0x00047267 File Offset: 0x00045667
		public InvitationQuery(IFriendsService friendsService, IClanService clanService, IDALService dalService)
		{
			this.m_friendsService = friendsService;
			this.m_clanService = clanService;
			this.m_dalService = dalService;
		}

		// Token: 0x060011FE RID: 4606 RVA: 0x00047284 File Offset: 0x00045684
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(query.online_id, out user))
			{
				return TaskHelpers.Completed<int>(-3);
			}
			string target = request.GetAttribute("target");
			ulong targetId;
			if (!string.IsNullOrEmpty(target))
			{
				targetId = this.m_dalService.ProfileSystem.GetProfileIDByNickname(target);
			}
			else
			{
				ulong profileId = ulong.Parse(request.GetAttribute("target_id"));
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(profileId);
				targetId = profileInfo.Id;
				target = profileInfo.Nickname;
			}
			ENotificationType type = (ENotificationType)uint.Parse(request.GetAttribute("type"));
			Task<EInviteStatus> task;
			if (type != ENotificationType.ClanInvite)
			{
				if (type != ENotificationType.FriendInvite)
				{
					task = TaskHelpers.Completed<EInviteStatus>(EInviteStatus.ServiceError);
				}
				else
				{
					task = this.m_friendsService.Invite(user, targetId, target);
				}
			}
			else
			{
				task = this.m_clanService.Invite(user, targetId, target);
			}
			return task.ContinueWith<int>(delegate(Task<EInviteStatus> t)
			{
				EInviteStatus result = t.Result;
				if (result != EInviteStatus.Pending)
				{
					return (int)result;
				}
				XmlElement response2 = response;
				string name = "type";
				uint type = (uint)type;
				response2.SetAttribute(name, type.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("target", target);
				return 0;
			});
		}

		// Token: 0x0400080C RID: 2060
		private readonly IFriendsService m_friendsService;

		// Token: 0x0400080D RID: 2061
		private readonly IClanService m_clanService;

		// Token: 0x0400080E RID: 2062
		private readonly IDALService m_dalService;
	}
}

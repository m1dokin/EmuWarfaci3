using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.Users
{
	// Token: 0x020007DC RID: 2012
	[QueryAttributes(TagName = "invitation_send")]
	internal class InvitationSendQuery : BaseQuery
	{
		// Token: 0x06002917 RID: 10519 RVA: 0x000B22A5 File Offset: 0x000B06A5
		public InvitationSendQuery(IUserProxyRepository userProxyRepository, IUserInvitation invitationService, IProfileValidationService profileValidation)
		{
			this.m_userProxyRepository = userProxyRepository;
			this.m_invitationService = invitationService;
			this.m_profileValidation = profileValidation;
		}

		// Token: 0x06002918 RID: 10520 RVA: 0x000B22C4 File Offset: 0x000B06C4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "InvitationSendQuery"))
			{
				UserInfo.User fromUser;
				if (!base.GetClientInfo(fromJid, out fromUser))
				{
					result = -3;
				}
				else
				{
					string attribute = request.GetAttribute("is_follow");
					string attribute2 = request.GetAttribute("group_id");
					UserInfo.User user;
					if (request.HasAttribute("user_id"))
					{
						ulong userId = ulong.Parse(request.GetAttribute("user_id"));
						user = this.m_userProxyRepository.GetUserOrProxyByUserId(userId);
					}
					else
					{
						string attribute3 = request.GetAttribute("nickname");
						NameValidationResult nameValidationResult = this.m_profileValidation.ValidateNickname(attribute3);
						if (nameValidationResult != NameValidationResult.NoError)
						{
							Log.Warning<string, string>("Received incorrect nickname from '{0}'. Validation error: {1}", fromJid, nameValidationResult.ToString());
							return -1;
						}
						user = this.m_userProxyRepository.GetUserOrProxyByNickname(attribute3);
					}
					EInvitationStatus einvitationStatus = this.m_invitationService.SendInvitation(fromUser, user, attribute2, attribute);
					if (einvitationStatus != EInvitationStatus.Pending && einvitationStatus != EInvitationStatus.UserOffline)
					{
						result = (int)einvitationStatus;
					}
					else
					{
						response.SetAttribute("nickname", user.Nickname);
						result = 0;
					}
				}
			}
			return result;
		}

		// Token: 0x040015E9 RID: 5609
		private const string QueryName = "invitation_send";

		// Token: 0x040015EA RID: 5610
		private readonly IUserProxyRepository m_userProxyRepository;

		// Token: 0x040015EB RID: 5611
		private readonly IUserInvitation m_invitationService;

		// Token: 0x040015EC RID: 5612
		private readonly IProfileValidationService m_profileValidation;
	}
}

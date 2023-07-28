using System;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.Users;
using Util.Common;

namespace MasterServer.MySqlQueries
{
	// Token: 0x02000680 RID: 1664
	[QueryAttributes(TagName = "get_account_profiles", QoSClass = "pre_login")]
	internal class GetAccountProfilesQuery : BaseQuery
	{
		// Token: 0x0600230B RID: 8971 RVA: 0x00093258 File Offset: 0x00091658
		public GetAccountProfilesQuery(IClientVersionsManagementService clientVersionsManagementService, IDALService dalService, IRankSystem rankSystem, IUserIdValidator userIdValidator)
		{
			this.m_clientVersionsManagementService = clientVersionsManagementService;
			this.m_dalService = dalService;
			this.m_rankSystem = rankSystem;
			this.m_userIdValidator = userIdValidator;
		}

		// Token: 0x0600230C RID: 8972 RVA: 0x00093280 File Offset: 0x00091680
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			Task<int> result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetAccountProfilesQuery"))
			{
				ulong num;
				if (!request.HasAttribute("user_id") || !request.HasAttribute("version"))
				{
					Log.Error("Get account profiles, invalid query parameters");
					result = TaskHelpers.Completed<int>(-1);
				}
				else if (!ulong.TryParse(request.GetAttribute("user_id"), out num))
				{
					Log.Error<string>("Get account profiles, parameter user_id = {0} is invalid", request.GetAttribute("user_id"));
					result = TaskHelpers.Completed<int>(-1);
				}
				else if (!this.m_userIdValidator.ValidateAgainstJid(query.online_id, num))
				{
					Log.Error<ulong, string>("Get account profiles user authentication failed for user {0} from {1}", num, query.online_id);
					result = TaskHelpers.Completed<int>(-1);
				}
				else
				{
					ClientVersion clientVersion;
					ClientVersion.TryParse(request.GetAttribute("version"), out clientVersion);
					if (!this.m_clientVersionsManagementService.Validate(clientVersion))
					{
						Log.Warning<ulong, ClientVersion>("User {0} has unsupported client version {1}", num, clientVersion);
						result = TaskHelpers.Completed<int>(1);
					}
					else
					{
						foreach (SAuthProfile sauthProfile in this.m_dalService.ProfileSystem.GetUserProfiles(num))
						{
							XmlElement xmlElement = response.OwnerDocument.CreateElement("profile");
							xmlElement.SetAttribute("id", sauthProfile.ProfileID.ToString());
							xmlElement.SetAttribute("nickname", sauthProfile.Nickname);
							response.AppendChild(xmlElement);
						}
						if (!response.HasChildNodes && !this.m_rankSystem.CanCreateProfileOnChannel())
						{
							Log.Warning<ulong, string>("User {0} cann't create profile on channel {1}", num, Resources.ChannelName);
							result = TaskHelpers.Completed<int>(2);
						}
						else
						{
							result = TaskHelpers.Completed<int>(0);
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040011A9 RID: 4521
		private const int E_VERSION_MISSMATCH = 1;

		// Token: 0x040011AA RID: 4522
		private const int E_INVALID_RANK = 2;

		// Token: 0x040011AB RID: 4523
		private readonly IClientVersionsManagementService m_clientVersionsManagementService;

		// Token: 0x040011AC RID: 4524
		private readonly IDALService m_dalService;

		// Token: 0x040011AD RID: 4525
		private readonly IRankSystem m_rankSystem;

		// Token: 0x040011AE RID: 4526
		private readonly IUserIdValidator m_userIdValidator;
	}
}

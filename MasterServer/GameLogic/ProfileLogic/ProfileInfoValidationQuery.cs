using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000557 RID: 1367
	[QueryAttributes(TagName = "validate_player_info")]
	internal class ProfileInfoValidationQuery : BaseQuery
	{
		// Token: 0x06001D77 RID: 7543 RVA: 0x00077878 File Offset: 0x00075C78
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user_info;
			if (!base.GetClientInfo(fromJid, out user_info))
			{
				return -3;
			}
			IProfileValidationService service = ServicesManager.GetService<IProfileValidationService>();
			service.Validate(user_info, request);
			return 0;
		}
	}
}

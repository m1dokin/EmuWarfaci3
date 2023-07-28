using System;
using System.Xml;
using HK2Net;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000009 RID: 9
	[Contract]
	internal interface IProfileValidationService
	{
		// Token: 0x06000015 RID: 21
		bool Validate(UserInfo.User user_info, XmlElement profile_info);

		// Token: 0x06000016 RID: 22
		bool ValidateProfileClass(UserInfo.User user_info, uint curr_class);

		// Token: 0x06000017 RID: 23
		NameValidationResult ValidateNickname(string nickname);

		// Token: 0x06000018 RID: 24
		bool IsHeadValid(string head);
	}
}

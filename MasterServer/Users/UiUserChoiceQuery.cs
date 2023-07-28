using System;
using System.Collections;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x020007E1 RID: 2017
	[QueryAttributes(TagName = "ui_user_choice", QoSClass = "ui_statistic")]
	internal class UiUserChoiceQuery : BaseQuery
	{
		// Token: 0x0600294F RID: 10575 RVA: 0x000B355A File Offset: 0x000B195A
		public UiUserChoiceQuery(ILogService logService)
		{
			this.m_logService = logService;
		}

		// Token: 0x06002950 RID: 10576 RVA: 0x000B356C File Offset: 0x000B196C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(fromJid, out user))
			{
				return -3;
			}
			using (ILogGroup logGroup = this.m_logService.CreateGroup())
			{
				IEnumerator enumerator = request.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement = (XmlElement)obj;
						string attribute = xmlElement.GetAttribute("choice_from");
						string attribute2 = xmlElement.GetAttribute("choice_id");
						int choiceResult = int.Parse(xmlElement.GetAttribute("choice_result"));
						logGroup.UiUserChoice(user.UserID, user.ProfileID, attribute, attribute2, choiceResult);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
			}
			return 0;
		}

		// Token: 0x040015FE RID: 5630
		private readonly ILogService m_logService;
	}
}

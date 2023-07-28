using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x0200053C RID: 1340
	[QueryAttributes(TagName = "abuse_report")]
	internal class AbuseReportQuery : BaseQuery
	{
		// Token: 0x06001D0E RID: 7438 RVA: 0x00075558 File Offset: 0x00073958
		public AbuseReportQuery(IDALService dal, IAbuseReportService abuseReport, IProfileValidationService profileValidation)
		{
			this.m_dal = dal;
			this.m_abuseReport = abuseReport;
			this.m_profileValidation = profileValidation;
		}

		// Token: 0x06001D0F RID: 7439 RVA: 0x00075578 File Offset: 0x00073978
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (!base.GetClientInfo(query.online_id, out user))
			{
				return Task.FromResult<int>(-3);
			}
			string attribute = request.GetAttribute("target");
			string attribute2 = request.GetAttribute("type");
			string comment = "no comment";
			if (request.HasAttribute("comment"))
			{
				byte[] bytes = Convert.FromBase64String(request.GetAttribute("comment"));
				comment = Encoding.UTF8.GetString(bytes);
			}
			NameValidationResult nameValidationResult = this.m_profileValidation.ValidateNickname(attribute);
			if (nameValidationResult != NameValidationResult.NoError)
			{
				Log.Warning<string, string>("Received incorrect nickname from '{0}'. Validation error: {1}", query.online_id, nameValidationResult.ToString());
				return Task.FromResult<int>(-1);
			}
			ulong profileIDByNickname = this.m_dal.ProfileSystem.GetProfileIDByNickname(attribute);
			if (profileIDByNickname == 0UL || user.ProfileID == profileIDByNickname)
			{
				return Task.FromResult<int>(-1);
			}
			SProfileInfo profileInfo = this.m_dal.ProfileSystem.GetProfileInfo(profileIDByNickname);
			if (profileInfo.UserID == 0UL)
			{
				return Task.FromResult<int>(-1);
			}
			return this.m_abuseReport.ProcessReport(user, profileInfo.UserID, profileIDByNickname, attribute, attribute2, comment).ContinueWith<int>(delegate(Task<EAbuseReportingResult> t)
			{
				EAbuseReportingResult result = t.Result;
				XmlElement response2 = response;
				string name = "result";
				uint num = (uint)result;
				response2.SetAttribute(name, num.ToString(CultureInfo.InvariantCulture));
				return 0;
			});
		}

		// Token: 0x04000DD8 RID: 3544
		private readonly IDALService m_dal;

		// Token: 0x04000DD9 RID: 3545
		private readonly IAbuseReportService m_abuseReport;

		// Token: 0x04000DDA RID: 3546
		private readonly IProfileValidationService m_profileValidation;
	}
}

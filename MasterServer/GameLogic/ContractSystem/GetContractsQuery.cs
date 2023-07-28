using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ContractSystem
{
	// Token: 0x0200029A RID: 666
	[QueryAttributes(TagName = "get_contracts")]
	internal class GetContractsQuery : BaseQuery
	{
		// Token: 0x06000E67 RID: 3687 RVA: 0x0003A0E7 File Offset: 0x000384E7
		public GetContractsQuery(IContractService contractService, IItemStats itemStats)
		{
			this.m_contractService = contractService;
			this.m_itemStats = itemStats;
		}

		// Token: 0x06000E68 RID: 3688 RVA: 0x0003A100 File Offset: 0x00038500
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			UserInfo.User user;
			if (request.HasAttribute("profile_id"))
			{
				ulong profileId = ulong.Parse(request.GetAttribute("profile_id"));
				user = base.UserRepository.GetUser(profileId);
			}
			else
			{
				base.GetClientInfo(fromJid, out user);
			}
			if (user == null)
			{
				return -3;
			}
			ProfileContract profileContract = this.m_contractService.RotateContract(user.ProfileID);
			if (profileContract != null)
			{
				XmlElement xmlElement = profileContract.ToXml(response.OwnerDocument);
				xmlElement.SetAttribute("is_available", (!this.m_itemStats.IsItemAvailableForUser(profileContract.ContractName, user)) ? "0" : "1");
				response.AppendChild(xmlElement);
			}
			return 0;
		}

		// Token: 0x040006A2 RID: 1698
		private readonly IContractService m_contractService;

		// Token: 0x040006A3 RID: 1699
		private readonly IItemStats m_itemStats;
	}
}

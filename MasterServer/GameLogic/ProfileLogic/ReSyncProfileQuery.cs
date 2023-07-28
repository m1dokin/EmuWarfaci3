using System;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000572 RID: 1394
	[QueryAttributes(TagName = "resync_profile")]
	internal class ReSyncProfileQuery : BaseQuery
	{
		// Token: 0x06001E0B RID: 7691 RVA: 0x00079C3A File Offset: 0x0007803A
		public ReSyncProfileQuery(IItemsPurchase itemsPurchase)
		{
			this.m_itemsPurchase = itemsPurchase;
		}

		// Token: 0x06001E0C RID: 7692 RVA: 0x00079C4C File Offset: 0x0007804C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			ulong profileId;
			if (!base.GetClientProfileId(fromJid, out profileId))
			{
				return -3;
			}
			ProfileProxy profile = new ProfileProxy(profileId);
			ProfileReader profileReader = new ProfileReader(profile);
			this.m_itemsPurchase.SyncProfileItemsWithCatalog(profile);
			profileReader.ReadProfileItems(response);
			profileReader.ReadUnlockedItems(response);
			XmlElement xmlElement = response.OwnerDocument.CreateElement("money");
			response.AppendChild(xmlElement);
			profileReader.ReadProfileMoney(xmlElement);
			XmlElement xmlElement2 = response.OwnerDocument.CreateElement("character");
			profileReader.FillCharacterInfo(xmlElement2);
			response.AppendChild(xmlElement2);
			XmlElement xmlElement3 = response.OwnerDocument.CreateElement("progression");
			profileReader.ReadProgression(xmlElement3);
			response.AppendChild(xmlElement3);
			return 0;
		}

		// Token: 0x04000E86 RID: 3718
		private readonly IItemsPurchase m_itemsPurchase;
	}
}

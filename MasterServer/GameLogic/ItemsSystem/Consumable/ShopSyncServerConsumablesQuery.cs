using System;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameLogic.ItemsSystem.Consumable
{
	// Token: 0x0200033F RID: 831
	[QueryAttributes(TagName = "shop_sync_consumables")]
	internal class ShopSyncServerConsumablesQuery : BaseQuery
	{
		// Token: 0x060012A8 RID: 4776 RVA: 0x0004AEEC File Offset: 0x000492EC
		public override void SendRequest(string onlineId, XmlElement request, params object[] queryParams)
		{
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ShopSyncServerConsumablesQuery"))
			{
				string value = (string)queryParams[0];
				ulong profileID = (ulong)queryParams[1];
				ulong num = (ulong)queryParams[2];
				ulong num2 = (ulong)queryParams[3];
				request.SetAttribute("session_id", value);
				IProfileItems service = ServicesManager.GetService<IProfileItems>();
				SProfileItem profileItem = service.GetProfileItem(profileID, num);
				if (profileItem != null)
				{
					XmlElement xmlElement = request.OwnerDocument.CreateElement("profile_items");
					xmlElement.SetAttribute("profile_id", profileID.ToString(CultureInfo.InvariantCulture));
					XmlElement xml = ServerItem.GetXml(profileItem, request.OwnerDocument, "item");
					xml.SetAttribute("added_quantity", num2.ToString(CultureInfo.InvariantCulture));
					xmlElement.AppendChild(xml);
					request.AppendChild(xmlElement);
				}
				else
				{
					Log.Warning<ulong>("Can't find item {0} for quantity update", num);
				}
			}
		}
	}
}

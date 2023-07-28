using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.ClanSystem;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005EF RID: 1519
	[QueryAttributes(TagName = "player_clan_info_updated")]
	internal class PlayerClanInfoUpdatedQuery : BaseQuery
	{
		// Token: 0x0600204A RID: 8266 RVA: 0x00082A24 File Offset: 0x00080E24
		public override void SendRequest(string online_id, XmlElement request, params object[] queryParams)
		{
			IEnumerable<ulong> enumerable = queryParams[0] as IEnumerable<ulong>;
			string text = string.Empty;
			foreach (ulong num in enumerable)
			{
				if (text.Length != 0)
				{
					text += ',';
				}
				text += num.ToString();
			}
			request.SetAttribute("pids", text);
			IOnlineClient service = ServicesManager.GetService<IOnlineClient>();
			string text2 = string.Empty;
			foreach (SOnlineServer sonlineServer in service.OnlineServers)
			{
				if (text2.Length != 0)
				{
					text2 += ',';
				}
				text2 += service.GetJid("masterserver", sonlineServer.Resource);
			}
			request.SetAttribute("bcast_receivers", text2);
		}

		// Token: 0x0600204B RID: 8267 RVA: 0x00082B54 File Offset: 0x00080F54
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "PlayerClanInfoUpdatedQuery"))
			{
				string attribute = request.GetAttribute("pids");
				string[] array = attribute.Split(new char[]
				{
					','
				});
				Log.Info<int>("Receiving clan update info for {0} players", array.Length);
				foreach (string s in array)
				{
					ulong profileId = ulong.Parse(s);
					IClanService service = ServicesManager.GetService<IClanService>();
					service.RemoteClanInfoUpdate(profileId);
				}
				result = 0;
			}
			return result;
		}
	}
}

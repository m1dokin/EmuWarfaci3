using System;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.ElectronicCatalog.Exceptions;
using MasterServer.GameRoomSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.ItemsSystem.Consumable
{
	// Token: 0x0200033D RID: 829
	[QueryAttributes(TagName = "consume_item")]
	internal class ConsumeItemQuery : BaseQuery
	{
		// Token: 0x060012A2 RID: 4770 RVA: 0x0004AC7C File Offset: 0x0004907C
		public ConsumeItemQuery(IUserRepository userRepository, IItemService itemService, ISessionStorage sessionStorage)
		{
			this.m_userRepository = userRepository;
			this.m_itemService = itemService;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x060012A3 RID: 4771 RVA: 0x0004AC9C File Offset: 0x0004909C
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "ConsumeItemQuery"))
			{
				string attribute = request.GetAttribute("session_id");
				if (!this.m_sessionStorage.ValidateSession(fromJid, attribute))
				{
					Log.Warning<string, string>("Ignoring consume item request from server {0} which has incorrect session id {1}", fromJid, attribute);
					return -1;
				}
				ulong num = ulong.Parse(request.GetAttribute("profile_id"));
				ulong num2 = ulong.Parse(request.GetAttribute("item_profile_id"));
				uint checkpoint = uint.Parse(request.GetAttribute("current_checkpoint"));
				UserInfo.User user = this.m_userRepository.GetUser(num);
				if (user == null)
				{
					Log.Warning<ulong, ulong>("Non-existing player {0} is trying to consume item {1}", num, num2);
					return -1;
				}
				ConsumeItemResponse consumeItemResponse;
				try
				{
					consumeItemResponse = this.m_itemService.ConsumeItem(user, fromJid, attribute, checkpoint, num2, 1);
				}
				catch (ItemServiceDalException e)
				{
					Log.Error(e);
					return 1;
				}
				catch (ItemServiceException e2)
				{
					Log.Error(e2);
					return -1;
				}
				response.SetAttribute("profile_id", num.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("item_profile_id", num2.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("items_consumed", 1.ToString(CultureInfo.InvariantCulture));
				response.SetAttribute("items_left", consumeItemResponse.ItemsLeft.ToString(CultureInfo.InvariantCulture));
			}
			return 0;
		}

		// Token: 0x04000897 RID: 2199
		private const int CONSUME_DB_FAIL = 1;

		// Token: 0x04000898 RID: 2200
		private readonly IItemService m_itemService;

		// Token: 0x04000899 RID: 2201
		private readonly IUserRepository m_userRepository;

		// Token: 0x0400089A RID: 2202
		private readonly ISessionStorage m_sessionStorage;
	}
}

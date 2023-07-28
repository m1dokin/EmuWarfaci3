using System;
using System.Collections.Generic;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001B5 RID: 437
	[Service]
	[Singleton]
	internal class SequenceQueryCache : ServiceModule, ISequenceQueryCache
	{
		// Token: 0x06000824 RID: 2084 RVA: 0x0001F4C7 File Offset: 0x0001D8C7
		public SequenceQueryCache(IItemCache itemCache, IGameRoomManager roomManager, IShopService shopService)
		{
			this.m_itemCache = itemCache;
			this.m_roomManager = roomManager;
			this.m_shopService = shopService;
			this.m_dtCache.Init();
		}

		// Token: 0x06000825 RID: 2085 RVA: 0x0001F4FC File Offset: 0x0001D8FC
		public override void Start()
		{
			this.m_shopService.OffersUpdated += this.OnOffersUpdated;
			this.m_itemCache.ItemsCacheUpdated += this.OnItemsCacheUpdated;
			this.m_roomManager.RoomOpened += this.OnRoomsChanged;
			this.m_roomManager.RoomClosed += this.OnRoomsChanged;
		}

		// Token: 0x06000826 RID: 2086 RVA: 0x0001F565 File Offset: 0x0001D965
		public int SaveData(List<XmlElement> data, string token, CacheType type)
		{
			return this.m_dtCache.SaveToken(data, token, type);
		}

		// Token: 0x06000827 RID: 2087 RVA: 0x0001F575 File Offset: 0x0001D975
		public int GetData(int tokenId, string token, out List<XmlElement> data)
		{
			return this.m_dtCache.GetToken(tokenId, token, out data);
		}

		// Token: 0x06000828 RID: 2088 RVA: 0x0001F585 File Offset: 0x0001D985
		public void FreeData(int tokenId, string token)
		{
			this.m_dtCache.FreeToken(tokenId, token);
		}

		// Token: 0x06000829 RID: 2089 RVA: 0x0001F594 File Offset: 0x0001D994
		private void OnOffersUpdated(IEnumerable<StoreOffer> offers)
		{
			this.m_dtCache.InvalidateCache(CacheType.Permanent, null);
		}

		// Token: 0x0600082A RID: 2090 RVA: 0x0001F5A3 File Offset: 0x0001D9A3
		private void OnItemsCacheUpdated(IEnumerable<SItem> sItems)
		{
			this.m_dtCache.InvalidateCache(CacheType.Permanent, null);
		}

		// Token: 0x0600082B RID: 2091 RVA: 0x0001F5B2 File Offset: 0x0001D9B2
		private void OnRoomsChanged(IGameRoom room)
		{
			this.m_dtCache.InvalidateCache(CacheType.Regular, "GetGameRooms");
		}

		// Token: 0x040004C2 RID: 1218
		private DataCache<List<XmlElement>> m_dtCache = new DataCache<List<XmlElement>>();

		// Token: 0x040004C3 RID: 1219
		private readonly IShopService m_shopService;

		// Token: 0x040004C4 RID: 1220
		private readonly IItemCache m_itemCache;

		// Token: 0x040004C5 RID: 1221
		private readonly IGameRoomManager m_roomManager;
	}
}

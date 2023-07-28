using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoom.RoomExtensions;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200081D RID: 2077
	[QueryAttributes(TagName = "gameroom_get")]
	internal class GetRoomsQuery : SequenceQuery
	{
		// Token: 0x06002ACB RID: 10955 RVA: 0x000B8BA8 File Offset: 0x000B6FA8
		public GetRoomsQuery(ISequenceQueryCache queryCacheService, IGameRoomManager gameRoomManager) : base(queryCacheService)
		{
			this.m_gameRoomManager = gameRoomManager;
		}

		// Token: 0x06002ACC RID: 10956 RVA: 0x000B8BB8 File Offset: 0x000B6FB8
		protected override int CreateSequenceData(XmlElement parentNode, out List<XmlElement> sequenceData, string fromJid)
		{
			List<XmlElement> nodes = new List<XmlElement>();
			GameRoomType roomType = (GameRoomType)int.Parse(parentNode.GetAttribute("room_type"));
			List<IGameRoom> rooms = this.m_gameRoomManager.GetRooms((IGameRoom R) => (R.Type & roomType) != (GameRoomType)0);
			foreach (IGameRoom gameRoom in rooms)
			{
				try
				{
					gameRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
					{
						if (!r.Type.IsPvpAutoStartMode() && r.PlayerCount != 0)
						{
							XmlElement xmlElement = GetRoomsQuery.SerializeRoomDesc(r, parentNode.OwnerDocument);
							if (xmlElement != null)
							{
								nodes.Add(xmlElement);
							}
						}
					});
				}
				catch (RoomClosedException)
				{
					Log.Info<ulong>("Room {0} is already closed and wasn't added to response", gameRoom.ID);
				}
			}
			sequenceData = nodes;
			return this.m_queryCacheService.SaveData(sequenceData, this.GetTokenName(fromJid), CacheType.Regular);
		}

		// Token: 0x06002ACD RID: 10957 RVA: 0x000B8CB0 File Offset: 0x000B70B0
		public static XmlElement SerializeRoomDesc(IGameRoom room, XmlDocument factory)
		{
			CoreState state = room.GetState<CoreState>(AccessMode.ReadOnly);
			MissionState state2 = room.GetState<MissionState>(AccessMode.ReadOnly);
			SessionState state3 = room.GetState<SessionState>(AccessMode.ReadOnly);
			CustomParams state4 = room.GetState<CustomParams>(AccessMode.ReadOnly);
			RegionState state5 = room.GetState<RegionState>(AccessMode.ReadOnly);
			ClanWar clanWar = room.TryGetState<ClanWar>(AccessMode.ReadOnly);
			RoomMasterState roomMasterState = room.TryGetState<RoomMasterState>(AccessMode.ReadOnly);
			if (state.Private || string.IsNullOrEmpty(state2.Mission.uid))
			{
				return null;
			}
			XmlElement xmlElement = room.CreateRoomElement(factory);
			XmlElement xmlElement2 = xmlElement.OwnerDocument.CreateElement("core");
			CoreStateExtension.SerializeRoomCore(xmlElement2, state, RoomUpdate.Target.Client);
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xmlElement.OwnerDocument.CreateElement("mission");
			MissionExtension.SerializeMission(xmlElement3, state2.Mission, RoomUpdate.Target.Client, RoomUpdate.InformationType.UiBaseInfo);
			xmlElement.AppendChild(xmlElement3);
			XmlElement xmlElement4 = xmlElement.OwnerDocument.CreateElement("session");
			SessionExtension.SerializeSessionState(xmlElement4, state3, false);
			xmlElement.AppendChild(xmlElement4);
			XmlElement xmlElement5 = xmlElement.OwnerDocument.CreateElement("custom_params");
			CustomParamsExtension.SerializeCustomParams(xmlElement5, state4);
			xmlElement.AppendChild(xmlElement5);
			XmlElement xmlElement6 = xmlElement.OwnerDocument.CreateElement("regions");
			RegionRoomMasterExtension.SerializeTo(xmlElement6, state5);
			xmlElement.AppendChild(xmlElement6);
			if (roomMasterState != null)
			{
				XmlElement xmlElement7 = xmlElement.OwnerDocument.CreateElement("room_master");
				RoomMasterExtension.SerializeRoomMaster(xmlElement7, roomMasterState);
				xmlElement.AppendChild(xmlElement7);
			}
			if (clanWar != null)
			{
				XmlElement xmlElement8 = xmlElement.OwnerDocument.CreateElement("clan_war");
				ClanWarExtension.SerializeClanWar(xmlElement8, clanWar);
				xmlElement.AppendChild(xmlElement8);
			}
			IEnumerator enumerator = xmlElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement9 = (XmlElement)obj;
					xmlElement9.SetAttribute("revision", "1");
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
			return xmlElement;
		}

		// Token: 0x06002ACE RID: 10958 RVA: 0x000B8EAC File Offset: 0x000B72AC
		protected override string GetTokenName(string fromJid)
		{
			return "GetGameRooms";
		}

		// Token: 0x040016D2 RID: 5842
		private readonly IGameRoomManager m_gameRoomManager;
	}
}

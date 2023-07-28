using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.Commands
{
	// Token: 0x0200046A RID: 1130
	[ConsoleCmdAttributes(CmdName = "room_info", Help = "Gets room info.")]
	internal class GameRoomInfoCmd : ConsoleCommand<GameRoomInfoCmdParams>
	{
		// Token: 0x060017E1 RID: 6113 RVA: 0x00062F3A File Offset: 0x0006133A
		public GameRoomInfoCmd(IGameRoomManager roomManager, IProfileItems profileItems)
		{
			this.m_roomManager = roomManager;
			this.m_profileItems = profileItems;
		}

		// Token: 0x060017E2 RID: 6114 RVA: 0x00062F50 File Offset: 0x00061350
		protected override void Execute(GameRoomInfoCmdParams param)
		{
			IGameRoom room = this.m_roomManager.GetRoom(param.RoomId);
			if (room != null)
			{
				XmlDocument xmlDocument = new XmlDocument();
				XmlElement xmlElement = room.FullStateSnapshot(RoomUpdate.Target.Client, xmlDocument);
				if (param.DumpEquipment)
				{
					this.AppendEquipmentToRoomPlayers(xmlElement, xmlDocument);
				}
				StringWriter stringWriter = new StringWriter(new StringBuilder());
				XmlTextWriter w = new XmlTextWriter(stringWriter)
				{
					Formatting = Formatting.Indented
				};
				xmlElement.WriteTo(w);
				Log.Info(stringWriter.GetStringBuilder().ToString());
			}
			else
			{
				Log.Info("No such room");
			}
		}

		// Token: 0x060017E3 RID: 6115 RVA: 0x00062FE0 File Offset: 0x000613E0
		private void AppendEquipmentToRoomPlayers(XmlElement roomInfoXml, XmlDocument documentContext)
		{
			XmlNodeList xmlNodeList = roomInfoXml.SelectNodes("/core/players/player");
			if (xmlNodeList != null)
			{
				IEnumerator enumerator = xmlNodeList.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement = (XmlElement)obj;
						ulong profileId = ulong.Parse(xmlElement.GetAttribute("profile_id"));
						XmlElement profileEquipmentXml = this.GetProfileEquipmentXml(profileId, documentContext);
						xmlElement.AppendChild(profileEquipmentXml);
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
		}

		// Token: 0x060017E4 RID: 6116 RVA: 0x00063074 File Offset: 0x00061474
		private XmlElement GetProfileEquipmentXml(ulong profileId, XmlDocument documentContext)
		{
			Dictionary<ulong, SProfileItem> profileItems = this.m_profileItems.GetProfileItems(profileId);
			XmlElement xmlElement = documentContext.CreateElement("equipment");
			foreach (SProfileItem sprofileItem in profileItems.Values)
			{
				if (sprofileItem.IsEquipped)
				{
					XmlElement xmlElement2 = documentContext.CreateElement("item");
					xmlElement2.SetAttribute("name", sprofileItem.GameItem.Name);
					xmlElement2.SetAttribute("type", sprofileItem.GameItem.Type);
					xmlElement.AppendChild(xmlElement2);
				}
			}
			return xmlElement;
		}

		// Token: 0x04000B87 RID: 2951
		private readonly IGameRoomManager m_roomManager;

		// Token: 0x04000B88 RID: 2952
		private readonly IProfileItems m_profileItems;
	}
}

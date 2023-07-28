using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002FD RID: 765
	internal class GameModesConfig
	{
		// Token: 0x060011BA RID: 4538 RVA: 0x000461D0 File Offset: 0x000445D0
		public GameModesConfig(XmlDocument doc)
		{
			this.m_roomRestriction.ReadData(doc);
			SettingHelper.ReadDebugData(doc);
			XmlNodeList elementsByTagName = doc.GetElementsByTagName("for");
			List<KeyValuePair<ERoomRestriction, RoomRestrictionDesc>> globalRest = this.m_roomRestriction.GetGlobalRestricions().ToList<KeyValuePair<ERoomRestriction, RoomRestrictionDesc>>();
			IDictionary<GameRoomType, RoomTypeSettings> globalRoomSettings = SettingHelper.CreateGlobalSettings(doc);
			IEnumerator enumerator = elementsByTagName.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.Attributes != null)
					{
						string value = xmlNode.Attributes["mode"].Value;
						GameModeRestriction gameModeRestriction = new GameModeRestriction();
						gameModeRestriction.ReadData(globalRest, xmlNode);
						GameModeSetting gameModeSetting = new GameModeSetting();
						gameModeSetting.ReadData(xmlNode, globalRoomSettings);
						GameMode key = new GameMode(value);
						this.m_gameModeRestrictions.Add(key, gameModeRestriction);
						this.m_gameModeSettings.Add(key, gameModeSetting);
					}
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

		// Token: 0x060011BB RID: 4539 RVA: 0x000462FC File Offset: 0x000446FC
		public IEnumerable<GameMode> GetModes()
		{
			return this.m_gameModeSettings.Keys;
		}

		// Token: 0x060011BC RID: 4540 RVA: 0x00046309 File Offset: 0x00044709
		public RoomRestrictionDesc GetRestrictionDesc(ERoomRestriction type)
		{
			return this.m_roomRestriction.GetRestrictionDesc(type);
		}

		// Token: 0x060011BD RID: 4541 RVA: 0x00046318 File Offset: 0x00044718
		public GameModeRestriction GetGameModeRestriction(GameMode mode)
		{
			GameModeRestriction result;
			this.m_gameModeRestrictions.TryGetValue(mode, out result);
			return result;
		}

		// Token: 0x060011BE RID: 4542 RVA: 0x00046338 File Offset: 0x00044738
		public GameModeSetting GetGameModeSetting(GameMode mode)
		{
			GameModeSetting result;
			this.m_gameModeSettings.TryGetValue(mode, out result);
			return result;
		}

		// Token: 0x040007CB RID: 1995
		private readonly RoomRestrictionConfig m_roomRestriction = new RoomRestrictionConfig();

		// Token: 0x040007CC RID: 1996
		private readonly Dictionary<GameMode, GameModeRestriction> m_gameModeRestrictions = new Dictionary<GameMode, GameModeRestriction>();

		// Token: 0x040007CD RID: 1997
		private readonly Dictionary<GameMode, GameModeSetting> m_gameModeSettings = new Dictionary<GameMode, GameModeSetting>();
	}
}

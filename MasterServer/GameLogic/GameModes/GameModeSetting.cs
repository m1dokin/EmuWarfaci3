using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002FA RID: 762
	internal class GameModeSetting
	{
		// Token: 0x06001193 RID: 4499 RVA: 0x00045918 File Offset: 0x00043D18
		public void SetDebugSetting(ERoomSetting kind, int value)
		{
			this.SetDebugSetting(kind, value.ToString());
		}

		// Token: 0x06001194 RID: 4500 RVA: 0x0004592E File Offset: 0x00043D2E
		public void SetDebugSetting(ERoomSetting kind, string value)
		{
			if (this.m_debugSettings != null)
			{
				this.m_debugSettings[kind] = value;
			}
		}

		// Token: 0x06001195 RID: 4501 RVA: 0x00045948 File Offset: 0x00043D48
		public bool GetSetting(ERoomSetting kind, out string value)
		{
			return this.GetSetting(GameRoomType.All, kind, out value);
		}

		// Token: 0x06001196 RID: 4502 RVA: 0x00045954 File Offset: 0x00043D54
		public bool GetSetting(ERoomSetting kind, out int value)
		{
			return this.GetSetting(GameRoomType.All, kind, out value);
		}

		// Token: 0x06001197 RID: 4503 RVA: 0x00045960 File Offset: 0x00043D60
		public bool GetSetting(ERoomSetting kind, out bool value)
		{
			return this.GetSetting(GameRoomType.All, kind, out value);
		}

		// Token: 0x06001198 RID: 4504 RVA: 0x0004596C File Offset: 0x00043D6C
		public bool GetSetting(GameRoomType roomType, ERoomSetting kind, out string value)
		{
			if (Resources.DebugGameModeSettingsEnabled && (this.GetDebugSetting(kind, out value) || SettingHelper.GetGlobalDebugSetting(roomType, kind, out value)))
			{
				return true;
			}
			string text = null;
			RoomTypeSettings roomTypeSettings;
			if (this.m_roomTypeSettings.TryGetValue(roomType, out roomTypeSettings))
			{
				text = roomTypeSettings[kind];
			}
			if (string.IsNullOrEmpty(text))
			{
				this.m_settings.TryGetValue(kind, out text);
			}
			value = text;
			return !string.IsNullOrEmpty(value);
		}

		// Token: 0x06001199 RID: 4505 RVA: 0x000459E4 File Offset: 0x00043DE4
		public bool GetSetting(GameRoomType roomType, ERoomSetting kind, out int value)
		{
			string s;
			if (this.GetSetting(roomType, kind, out s))
			{
				return int.TryParse(s, out value);
			}
			value = 0;
			return false;
		}

		// Token: 0x0600119A RID: 4506 RVA: 0x00045A0C File Offset: 0x00043E0C
		public bool GetSetting(GameRoomType roomType, ERoomSetting kind, out bool value)
		{
			int num;
			bool setting = this.GetSetting(roomType, kind, out num);
			value = (num == 1);
			return setting;
		}

		// Token: 0x0600119B RID: 4507 RVA: 0x00045A2C File Offset: 0x00043E2C
		public void ReadData(XmlNode attrNode, IEnumerable<KeyValuePair<GameRoomType, RoomTypeSettings>> globalRoomSettings)
		{
			XmlNodeList xmlNodeList = attrNode.SelectNodes("settings/setting");
			XmlNodeList nodeList = xmlNodeList;
			if (GameModeSetting.<>f__mg$cache0 == null)
			{
				GameModeSetting.<>f__mg$cache0 = new Func<XmlNode, ERoomSetting>(CommonMethods.GetKindValue<ERoomSetting>);
			}
			this.m_settings = nodeList.ReadToDictionary(GameModeSetting.<>f__mg$cache0, (XmlNode n) => SettingHelper.ExtractAttributeValue(n, "value"));
			if (Resources.DebugGameModeSettingsEnabled)
			{
				XmlNodeList xmlNodeList2 = attrNode.SelectNodes("debug_settings/setting");
				XmlNodeList nodeList2 = xmlNodeList2;
				if (GameModeSetting.<>f__mg$cache1 == null)
				{
					GameModeSetting.<>f__mg$cache1 = new Func<XmlNode, ERoomSetting>(CommonMethods.GetKindValue<ERoomSetting>);
				}
				this.m_debugSettings = nodeList2.ReadToDictionary(GameModeSetting.<>f__mg$cache1, (XmlNode n) => SettingHelper.ExtractAttributeValue(n, "value"));
			}
			XmlNodeList xmlNodeList3 = attrNode.SelectNodes("room");
			XmlNodeList nodeList3 = xmlNodeList3;
			if (GameModeSetting.<>f__mg$cache2 == null)
			{
				GameModeSetting.<>f__mg$cache2 = new Func<XmlNode, GameRoomType>(CommonMethods.GetRoomType);
			}
			this.m_roomTypeSettings = nodeList3.ReadToDictionary(GameModeSetting.<>f__mg$cache2, (XmlNode n) => new RoomTypeSettings(n));
			this.MergeWithGlobalSettings(globalRoomSettings);
		}

		// Token: 0x0600119C RID: 4508 RVA: 0x00045B40 File Offset: 0x00043F40
		private void MergeWithGlobalSettings(IEnumerable<KeyValuePair<GameRoomType, RoomTypeSettings>> globalRoomSettings)
		{
			foreach (KeyValuePair<GameRoomType, RoomTypeSettings> keyValuePair in globalRoomSettings)
			{
				GameRoomType key = keyValuePair.Key;
				RoomTypeSettings roomTypeSettings;
				if (!this.m_roomTypeSettings.TryGetValue(key, out roomTypeSettings))
				{
					this.m_roomTypeSettings.Add(key, keyValuePair.Value);
				}
				else
				{
					roomTypeSettings.Merge(keyValuePair.Value);
				}
			}
		}

		// Token: 0x0600119D RID: 4509 RVA: 0x00045BD0 File Offset: 0x00043FD0
		private bool GetDebugSetting(ERoomSetting kind, out string value)
		{
			if (this.m_debugSettings != null)
			{
				return this.m_debugSettings.TryGetValue(kind, out value);
			}
			value = null;
			return false;
		}

		// Token: 0x040007C0 RID: 1984
		private IDictionary<ERoomSetting, string> m_settings;

		// Token: 0x040007C1 RID: 1985
		private IDictionary<GameRoomType, RoomTypeSettings> m_roomTypeSettings;

		// Token: 0x040007C2 RID: 1986
		private IDictionary<ERoomSetting, string> m_debugSettings;

		// Token: 0x040007C3 RID: 1987
		[CompilerGenerated]
		private static Func<XmlNode, ERoomSetting> <>f__mg$cache0;

		// Token: 0x040007C4 RID: 1988
		[CompilerGenerated]
		private static Func<XmlNode, ERoomSetting> <>f__mg$cache1;

		// Token: 0x040007C5 RID: 1989
		[CompilerGenerated]
		private static Func<XmlNode, GameRoomType> <>f__mg$cache2;
	}
}

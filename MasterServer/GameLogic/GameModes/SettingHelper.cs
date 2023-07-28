using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x02000304 RID: 772
	internal static class SettingHelper
	{
		// Token: 0x060011D4 RID: 4564 RVA: 0x00046818 File Offset: 0x00044C18
		public static void ReadDebugData(XmlDocument doc)
		{
			if (!Resources.DebugGameModeSettingsEnabled)
			{
				return;
			}
			XmlNode xmlNode = doc.SelectSingleNode("game_modes_config");
			XmlNodeList xmlNodeList = xmlNode.SelectNodes("debug_settings/room");
			XmlNodeList nodeList = xmlNodeList;
			if (SettingHelper.<>f__mg$cache0 == null)
			{
				SettingHelper.<>f__mg$cache0 = new Func<XmlNode, GameRoomType>(CommonMethods.GetRoomType);
			}
			SettingHelper.m_globalDebugSettings = nodeList.ReadToDictionary(SettingHelper.<>f__mg$cache0, (XmlNode n) => new RoomTypeSettings(n));
		}

		// Token: 0x060011D5 RID: 4565 RVA: 0x00046890 File Offset: 0x00044C90
		public static IDictionary<GameRoomType, RoomTypeSettings> CreateGlobalSettings(XmlDocument doc)
		{
			XmlNodeList xmlNodeList = doc.SelectNodes("game_modes_config/global_settings/room");
			XmlNodeList nodeList = xmlNodeList;
			if (SettingHelper.<>f__mg$cache1 == null)
			{
				SettingHelper.<>f__mg$cache1 = new Func<XmlNode, GameRoomType>(CommonMethods.GetRoomType);
			}
			return nodeList.ReadToDictionary(SettingHelper.<>f__mg$cache1, (XmlNode n) => new RoomTypeSettings(n));
		}

		// Token: 0x060011D6 RID: 4566 RVA: 0x000468E9 File Offset: 0x00044CE9
		public static void ClearDebugData()
		{
			SettingHelper.m_globalDebugSettings = null;
		}

		// Token: 0x060011D7 RID: 4567 RVA: 0x000468F4 File Offset: 0x00044CF4
		public static bool GetGlobalDebugSetting(GameRoomType roomType, ERoomSetting kind, out string value)
		{
			value = null;
			RoomTypeSettings roomTypeSettings;
			return SettingHelper.m_globalDebugSettings != null && SettingHelper.m_globalDebugSettings.TryGetValue(roomType, out roomTypeSettings) && roomTypeSettings.TryGetValue(kind, out value);
		}

		// Token: 0x060011D8 RID: 4568 RVA: 0x00046930 File Offset: 0x00044D30
		public static bool GetGlobalDebugSetting(GameRoomType roomType, ERoomSetting kind, out int value)
		{
			string s;
			if (SettingHelper.GetGlobalDebugSetting(roomType, kind, out s))
			{
				return int.TryParse(s, out value);
			}
			value = 0;
			return false;
		}

		// Token: 0x060011D9 RID: 4569 RVA: 0x00046958 File Offset: 0x00044D58
		public static IDictionary<TKey, TValue> ReadToDictionary<TKey, TValue>(this XmlNodeList nodeList, Func<XmlNode, TKey> keyExtractor, Func<XmlNode, TValue> valueExtractor)
		{
			Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();
			if (nodeList == null)
			{
				return dictionary;
			}
			IEnumerator enumerator = nodeList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode arg = (XmlNode)obj;
					TKey tkey = keyExtractor(arg);
					TValue value = valueExtractor(arg);
					if (!object.Equals(tkey, default(TKey)))
					{
						dictionary[tkey] = value;
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
			return dictionary;
		}

		// Token: 0x060011DA RID: 4570 RVA: 0x00046A00 File Offset: 0x00044E00
		public static TValue ExtractAttributeValue<TValue>(XmlNode node, string name, Func<string, TValue> converter)
		{
			return (node.Attributes == null || node.Attributes.Count <= 0) ? default(TValue) : converter(node.Attributes[name].Value);
		}

		// Token: 0x060011DB RID: 4571 RVA: 0x00046A4E File Offset: 0x00044E4E
		public static string ExtractAttributeValue(XmlNode node, string name)
		{
			return (node.Attributes == null || node.Attributes.Count <= 0) ? string.Empty : node.Attributes[name].Value;
		}

		// Token: 0x040007F7 RID: 2039
		private static IDictionary<GameRoomType, RoomTypeSettings> m_globalDebugSettings;

		// Token: 0x040007F8 RID: 2040
		[CompilerGenerated]
		private static Func<XmlNode, GameRoomType> <>f__mg$cache0;

		// Token: 0x040007FA RID: 2042
		[CompilerGenerated]
		private static Func<XmlNode, GameRoomType> <>f__mg$cache1;
	}
}

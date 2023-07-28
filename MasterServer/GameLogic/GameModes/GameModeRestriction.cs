using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F2 RID: 754
	internal class GameModeRestriction
	{
		// Token: 0x17000195 RID: 405
		public string this[GameRoomType roomType, ERoomRestriction index]
		{
			get
			{
				string text = null;
				RoomTypeRestriction roomTypeRestriction;
				if (this.m_roomTypesRestrictions.TryGetValue(roomType, out roomTypeRestriction))
				{
					text = roomTypeRestriction[index];
				}
				if (string.IsNullOrEmpty(text) && this.m_restrictions.TryGetValue(index, out text))
				{
					return text;
				}
				return text;
			}
		}

		// Token: 0x0600117F RID: 4479 RVA: 0x000452CC File Offset: 0x000436CC
		public void ReadData(IEnumerable<KeyValuePair<ERoomRestriction, RoomRestrictionDesc>> globalRest, XmlNode attrNode)
		{
			XmlNodeList xmlNodeList = attrNode.SelectNodes("restrictions/restriction");
			XmlNodeList nodeList = xmlNodeList;
			if (GameModeRestriction.<>f__mg$cache0 == null)
			{
				GameModeRestriction.<>f__mg$cache0 = new Func<XmlNode, ERoomRestriction>(CommonMethods.GetKindValue<ERoomRestriction>);
			}
			this.m_restrictions = nodeList.ReadToDictionary(GameModeRestriction.<>f__mg$cache0, (XmlNode n) => SettingHelper.ExtractAttributeValue(n, "option"));
			foreach (KeyValuePair<ERoomRestriction, RoomRestrictionDesc> keyValuePair in globalRest)
			{
				this.m_restrictions[keyValuePair.Key] = keyValuePair.Value.FirstOption;
			}
			XmlNodeList xmlNodeList2 = attrNode.SelectNodes("room");
			XmlNodeList nodeList2 = xmlNodeList2;
			if (GameModeRestriction.<>f__mg$cache1 == null)
			{
				GameModeRestriction.<>f__mg$cache1 = new Func<XmlNode, GameRoomType>(CommonMethods.GetRoomType);
			}
			this.m_roomTypesRestrictions = nodeList2.ReadToDictionary(GameModeRestriction.<>f__mg$cache1, (XmlNode n) => new RoomTypeRestriction(n));
		}

		// Token: 0x040007B2 RID: 1970
		private IDictionary<ERoomRestriction, string> m_restrictions;

		// Token: 0x040007B3 RID: 1971
		private IDictionary<GameRoomType, RoomTypeRestriction> m_roomTypesRestrictions;

		// Token: 0x040007B4 RID: 1972
		[CompilerGenerated]
		private static Func<XmlNode, ERoomRestriction> <>f__mg$cache0;

		// Token: 0x040007B5 RID: 1973
		[CompilerGenerated]
		private static Func<XmlNode, GameRoomType> <>f__mg$cache1;
	}
}

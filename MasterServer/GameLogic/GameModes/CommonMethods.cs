using System;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002F0 RID: 752
	internal static class CommonMethods
	{
		// Token: 0x06001175 RID: 4469 RVA: 0x000451D1 File Offset: 0x000435D1
		public static T GetKindValue<T>(XmlNode n)
		{
			return SettingHelper.ExtractAttributeValue<T>(n, "kind", new Func<string, T>(Utils.ParseEnum<T>));
		}

		// Token: 0x06001176 RID: 4470 RVA: 0x000451EA File Offset: 0x000435EA
		public static GameRoomType GetRoomType(XmlNode n)
		{
			string name = "type";
			if (CommonMethods.<>f__mg$cache0 == null)
			{
				CommonMethods.<>f__mg$cache0 = new Func<string, GameRoomType>(Utils.ParseEnum<GameRoomType>);
			}
			return SettingHelper.ExtractAttributeValue<GameRoomType>(n, name, CommonMethods.<>f__mg$cache0);
		}

		// Token: 0x040007B0 RID: 1968
		[CompilerGenerated]
		private static Func<string, GameRoomType> <>f__mg$cache0;
	}
}

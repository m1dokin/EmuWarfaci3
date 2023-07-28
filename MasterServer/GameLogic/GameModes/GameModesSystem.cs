using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.GameModes
{
	// Token: 0x020002FC RID: 764
	[Service]
	[Singleton]
	internal class GameModesSystem : ServiceModule, IGameModesSystem
	{
		// Token: 0x060011AB RID: 4523 RVA: 0x00045C14 File Offset: 0x00044014
		public GameModesSystem()
		{
			string resourceFullPath = Resources.GetResourceFullPath("game_modes_config.xml");
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(resourceFullPath);
			this.m_gameModesConfig = new GameModesConfig(xmlDocument);
			if (!Resources.DebugGameModeSettingsEnabled)
			{
				this.ValidateConfigForRatingGames();
			}
			this.ValidateMinPlayersSetup();
		}

		// Token: 0x060011AC RID: 4524 RVA: 0x00045C61 File Offset: 0x00044061
		public GameModeRestriction GetGameModeRestriction(MissionContextBase ctx)
		{
			return this.GetGameModeRestriction((!ctx.IsPveMode()) ? ctx.gameMode : ctx.missionType.Name);
		}

		// Token: 0x060011AD RID: 4525 RVA: 0x00045C8C File Offset: 0x0004408C
		public GameModeRestriction GetGameModeRestriction(string mode)
		{
			GameMode mode2 = new GameMode(mode);
			return this.m_gameModesConfig.GetGameModeRestriction(mode2);
		}

		// Token: 0x060011AE RID: 4526 RVA: 0x00045CAC File Offset: 0x000440AC
		public GameModeSetting GetGameModeSetting(MissionContextBase ctx)
		{
			return this.GetGameModeSetting((!ctx.IsPveMode()) ? ctx.gameMode : ctx.missionType.Name);
		}

		// Token: 0x060011AF RID: 4527 RVA: 0x00045CD8 File Offset: 0x000440D8
		public GameModeSetting GetGameModeSetting(string mode)
		{
			GameMode mode2 = new GameMode(mode);
			return this.m_gameModesConfig.GetGameModeSetting(mode2);
		}

		// Token: 0x060011B0 RID: 4528 RVA: 0x00045CF8 File Offset: 0x000440F8
		public RoomRestrictionDesc GetRestrictionDesc(ERoomRestriction type)
		{
			return this.m_gameModesConfig.GetRestrictionDesc(type);
		}

		// Token: 0x060011B1 RID: 4529 RVA: 0x00045D08 File Offset: 0x00044108
		public bool IsGlobalRestriction(string kind)
		{
			ERoomRestriction type = RestrictionHelper.Parse(kind);
			RoomRestrictionDesc restrictionDesc = this.GetRestrictionDesc(type);
			if (restrictionDesc.isGlobal)
			{
				Log.Warning<string>("[GameModesSystem]: Restriction {0} can't be changed, it's global", kind);
				return true;
			}
			return false;
		}

		// Token: 0x060011B2 RID: 4530 RVA: 0x00045D40 File Offset: 0x00044140
		public bool IsRestrictionLocked(string kind)
		{
			ERoomRestriction type = RestrictionHelper.Parse(kind);
			RoomRestrictionDesc restrictionDesc = this.GetRestrictionDesc(type);
			return !string.IsNullOrEmpty(restrictionDesc.Channels) && restrictionDesc.Channels.IndexOf(Resources.ChannelName, StringComparison.OrdinalIgnoreCase) == -1;
		}

		// Token: 0x060011B3 RID: 4531 RVA: 0x00045D84 File Offset: 0x00044184
		public bool ValidateRestriction<T>(MissionContext mission, GameRoomType type, string kind, T changedValue, ref T oldValue)
		{
			bool flag = changedValue is bool;
			string text;
			if (flag)
			{
				text = ((!(bool)((object)changedValue)) ? "0" : "1");
			}
			else
			{
				text = changedValue.ToString();
			}
			bool flag2 = this.ValidateRestrictionInternal(mission, type, kind, ref text, true);
			if (flag2)
			{
				oldValue = (T)((object)Convert.ChangeType((!flag) ? text : ((!(text == "1")) ? "false" : "true"), typeof(T)));
			}
			return flag2;
		}

		// Token: 0x060011B4 RID: 4532 RVA: 0x00045E38 File Offset: 0x00044238
		public void GetDefaultValue<T>(MissionContext mission, GameRoomType type, ERoomRestriction kind, ref T value)
		{
			bool flag = value is bool;
			GameModeRestriction gameModeRestriction = this.GetGameModeRestriction((!type.IsPveMode()) ? mission.gameMode : mission.missionType.Name);
			RoomRestrictionDesc restrictionDesc = this.GetRestrictionDesc(kind);
			if (gameModeRestriction == null || restrictionDesc == null)
			{
				Log.Warning<string, string>("[GameModesSystem]: Cannot create GameModeDesc for {0}, RoomRestrictionDesc for {1}", mission.gameMode, kind.ToString());
				return;
			}
			string text = gameModeRestriction[type, kind];
			if (string.IsNullOrEmpty(text) || !restrictionDesc.HasValues(text))
			{
				return;
			}
			string @default = restrictionDesc.GetDefault(text);
			if (string.IsNullOrEmpty(@default))
			{
				Log.Warning<ERoomRestriction>("[GameModesSystem]: Restriction kind {0} doesn't have default value", kind);
				return;
			}
			value = (T)((object)Convert.ChangeType((!flag) ? @default : ((!(@default == "1")) ? "false" : "true"), typeof(T)));
		}

		// Token: 0x060011B5 RID: 4533 RVA: 0x00045F42 File Offset: 0x00044342
		public IEnumerable<string> GetSupportedModes()
		{
			return from x in this.m_gameModesConfig.GetModes()
			select x.Mode;
		}

		// Token: 0x060011B6 RID: 4534 RVA: 0x00045F74 File Offset: 0x00044374
		private void ValidateConfigForRatingGames()
		{
			foreach (string mode in this.GetSupportedModes())
			{
				GameModeSetting gameModeSetting = this.GetGameModeSetting(mode);
				int num;
				gameModeSetting.GetSetting(GameRoomType.PvP_Rating, ERoomSetting.MIN_PLAYERS_READY, out num);
				int num2;
				gameModeSetting.GetSetting(GameRoomType.PvP_Rating, ERoomSetting.MIN_PLAYERS_FOR_ROOM_CREATION, out num2);
				if (num != num2)
				{
					throw new ApplicationException("[GameModesSystem] min_players_ready not equal min_players_for_room_creation in game_modes_config for rating games settings");
				}
			}
		}

		// Token: 0x060011B7 RID: 4535 RVA: 0x00045FFC File Offset: 0x000443FC
		private bool ValidateRestrictionInternal(MissionContext mission, GameRoomType type, string kind, ref string changedValue, bool writeLogs)
		{
			ERoomRestriction eroomRestriction = RestrictionHelper.Parse(kind);
			GameModeRestriction gameModeRestriction = this.GetGameModeRestriction(mission.gameMode);
			RoomRestrictionDesc restrictionDesc = this.GetRestrictionDesc(eroomRestriction);
			if (gameModeRestriction == null || restrictionDesc == null)
			{
				if (writeLogs)
				{
					Log.Warning<string, string>("[GameModesSystem]: Cannot create GameModeDesc for {0}, RoomRestrictionDesc for {1}", mission.gameMode, kind);
				}
				return false;
			}
			string text = gameModeRestriction[type, eroomRestriction];
			if (string.IsNullOrEmpty(text))
			{
				return false;
			}
			if (!restrictionDesc.HasValues(text))
			{
				return false;
			}
			string @default = restrictionDesc.GetDefault(text);
			if (this.IsRestrictionLocked(kind) && changedValue != @default)
			{
				Log.Error("[GameModesSystem.ValidateRestrictionInternal] Locked restrictions cannot be set");
				return false;
			}
			if (!restrictionDesc.ContainsValue(text, changedValue))
			{
				if (writeLogs)
				{
					Log.Warning<string, string, string>("[GameModesSystem]: Validation Restriction change invalid value {0} to default {1} for kind {2}", changedValue, @default, kind);
				}
				changedValue = @default;
			}
			return true;
		}

		// Token: 0x060011B8 RID: 4536 RVA: 0x000460D0 File Offset: 0x000444D0
		private void ValidateMinPlayersSetup()
		{
			GameRoomType[] array = new GameRoomType[]
			{
				GameRoomType.PvE_AutoStart,
				GameRoomType.PvP_AutoStart,
				GameRoomType.PvP_Rating
			};
			foreach (string text in this.GetSupportedModes())
			{
				foreach (GameRoomType gameRoomType in array)
				{
					GameModeSetting gameModeSetting = this.GetGameModeSetting(text);
					int num;
					gameModeSetting.GetSetting(gameRoomType, ERoomSetting.MIN_PLAYERS_FOR_ROOM_CREATION, out num);
					int num2;
					gameModeSetting.GetSetting(gameRoomType, ERoomSetting.MIN_PLAYERS_FOR_ROOM_JOINING, out num2);
					if (num < 1)
					{
						throw new ApplicationException(string.Format("[GameModesSystem] Setting min_players_for_room_creation should be greater then 0, got value {0} for mode {1}, room type {2}", num, text, gameRoomType));
					}
					if (num2 < 2)
					{
						throw new ApplicationException(string.Format("[GameModesSystem] Setting min_players_for_room_joining should be greater then 1, got value {0} for mode {1}, room type {2}", num2, text, gameRoomType));
					}
				}
			}
		}

		// Token: 0x040007C9 RID: 1993
		private readonly GameModesConfig m_gameModesConfig;
	}
}

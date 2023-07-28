using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameLogic.GameModes;
using MasterServer.GameLogic.MissionAccessLimitation;
using MasterServer.GameLogic.MissionSystem;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F9 RID: 1529
	[RoomExtension]
	internal class CustomParamsExtension : RoomExtensionBase
	{
		// Token: 0x0600207E RID: 8318 RVA: 0x0008429C File Offset: 0x0008269C
		public CustomParamsExtension(IMissionAccessLimitationService limitationService, IGameModesSystem gameModesSystem, ISessionStorage sessionStorage)
		{
			this.m_limitationService = limitationService;
			this.m_gameModesSystem = gameModesSystem;
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x14000080 RID: 128
		// (add) Token: 0x0600207F RID: 8319 RVA: 0x00084304 File Offset: 0x00082704
		// (remove) Token: 0x06002080 RID: 8320 RVA: 0x0008433C File Offset: 0x0008273C
		internal event CustomParamsExtension.CustomParamsChanged customParamsChanged;

		// Token: 0x06002081 RID: 8321 RVA: 0x00084374 File Offset: 0x00082774
		public override void Init(IGameRoom room)
		{
			base.Init(room);
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_started += this.OnSessionStarted;
			extension.tr_session_start_failed += this.OnSessionStartFailed;
			extension.tr_session_ended += this.OnSessionEnded;
			base.Room.tr_player_added += this.OnPlayerAdded;
			base.Room.tr_player_add_check += this.OnPlayerAddCheck;
		}

		// Token: 0x06002082 RID: 8322 RVA: 0x000843F8 File Offset: 0x000827F8
		protected override void OnDisposing()
		{
			SessionExtension extension = base.Room.GetExtension<SessionExtension>();
			extension.tr_session_started -= this.OnSessionStarted;
			extension.tr_session_start_failed -= this.OnSessionStartFailed;
			extension.tr_session_ended -= this.OnSessionEnded;
			base.Room.tr_player_added -= this.OnPlayerAdded;
			base.Room.tr_player_add_check -= this.OnPlayerAddCheck;
			base.OnDisposing();
		}

		// Token: 0x06002083 RID: 8323 RVA: 0x0008447C File Offset: 0x0008287C
		public override XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			CustomParams prms = (CustomParams)ctx.new_state;
			XmlElement xmlElement = ctx.factory.CreateElement("custom_params");
			CustomParamsExtension.SerializeCustomParams(xmlElement, prms);
			return xmlElement;
		}

		// Token: 0x06002084 RID: 8324 RVA: 0x000844B0 File Offset: 0x000828B0
		public void SetDefaultValues()
		{
			MissionContext mission = base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission;
			if (mission == null)
			{
				throw new ApplicationException("Room has no mission set");
			}
			CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadWrite);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.FRIENDLY_FIRE, ref state.FriendlyFire);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.ENEMY_OUTLINES, ref state.EnemyOutlines);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.AUTO_TEAM_BALANCE, ref state.Autobalance);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.JOIN_IN_THE_PROCESS, ref state.JoinInTheProcess);
			this.m_gameModesSystem.GetDefaultValue<int>(mission, base.Room.Type, ERoomRestriction.MAX_PLAYERS, ref state.MaxPlayers);
			this.m_gameModesSystem.GetDefaultValue<int>(mission, base.Room.Type, ERoomRestriction.ROUND_LIMIT, ref state.RoundLimit);
			this.m_gameModesSystem.GetDefaultValue<int>(mission, base.Room.Type, ERoomRestriction.PREROUND_TIME, ref state.PreRoundTime);
			this.m_gameModesSystem.GetDefaultValue<int>(mission, base.Room.Type, ERoomRestriction.INVENTORY_SLOT, ref state.InventorySlots);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.DEAD_CAN_CHAT, ref state.DeadCanChat);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.LOCKED_SPECTATOR_CAMERA, ref state.LockedSpectatorCamera);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.HIGH_LATENCY_AUTOKICK, ref state.HighLatencyAutoKick);
			this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, ERoomRestriction.OVERTIME_MODE, ref state.OvertimeMode);
			foreach (KeyValuePair<ProfileProgressionInfo.PlayerClass, string> keyValuePair in this.m_classRestrictions)
			{
				bool enabled = true;
				this.m_gameModesSystem.GetDefaultValue<bool>(mission, base.Room.Type, RestrictionHelper.Parse(keyValuePair.Value), ref enabled);
				state.SetClassRestriction(keyValuePair.Key, enabled);
			}
			state.SetClassRestriction(ProfileProgressionInfo.PlayerClass.Heavy, false);
		}

		// Token: 0x06002085 RID: 8325 RVA: 0x000846E4 File Offset: 0x00082AE4
		public static void SerializeCustomParams(XmlElement el, CustomParams prms)
		{
			el.SetAttribute("friendly_fire", ((!prms.FriendlyFire) ? 0 : 1).ToString());
			el.SetAttribute("enemy_outlines", ((!prms.EnemyOutlines) ? 0 : 1).ToString());
			el.SetAttribute("auto_team_balance", ((!prms.Autobalance) ? 0 : 1).ToString());
			el.SetAttribute("dead_can_chat", ((!prms.DeadCanChat) ? 0 : 1).ToString());
			el.SetAttribute("join_in_the_process", ((!prms.JoinInTheProcess) ? 0 : 1).ToString());
			el.SetAttribute("max_players", prms.MaxPlayers.ToString());
			el.SetAttribute("round_limit", prms.RoundLimit.ToString());
			el.SetAttribute("preround_time", prms.PreRoundTime.ToString());
			el.SetAttribute("class_restriction", prms.ClassRestriction.ToString());
			el.SetAttribute("inventory_slot", prms.InventorySlots.ToString());
			el.SetAttribute("locked_spectator_camera", ((!prms.LockedSpectatorCamera) ? 0 : 1).ToString());
			el.SetAttribute("high_latency_autokick", ((!prms.HighLatencyAutoKick) ? 0 : 1).ToString());
			el.SetAttribute("overtime_mode", ((!prms.OvertimeMode) ? 0 : 1).ToString());
		}

		// Token: 0x06002086 RID: 8326 RVA: 0x000848DC File Offset: 0x00082CDC
		public void CheckAndSetRestrictions(XmlElement node)
		{
			Dictionary<int, bool> dictionary = new Dictionary<int, bool>();
			string[] array = new string[]
			{
				"friendly_fire",
				"enemy_outlines",
				"auto_team_balance",
				"dead_can_chat",
				"join_in_the_process",
				"locked_spectator_camera",
				"high_latency_autokick",
				"overtime_mode"
			};
			string[] array2 = new string[]
			{
				"max_players",
				"round_limit",
				"inventory_slot"
			};
			CustomParams oldState = (CustomParams)base.Room.GetState<CustomParams>(AccessMode.ReadOnly).Clone();
			IEnumerator enumerator = node.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					int num = int.Parse(xmlNode.Attributes["class_id"].Value);
					bool value = int.Parse(xmlNode.Attributes["enabled"].Value) == 1;
					if (dictionary.ContainsKey(num))
					{
						string message = string.Format("Duplicated restriction for class {0}", num);
						throw new RoomGenericException(base.Room.ID, message);
					}
					dictionary.Add(num, value);
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
			foreach (string text in array)
			{
				if (node.HasAttribute(text))
				{
					this.CheckAndSetRestriction(text, int.Parse(node.GetAttribute(text)) == 1, true);
				}
			}
			foreach (string text2 in array2)
			{
				if (node.HasAttribute(text2))
				{
					this.CheckAndSetRestriction(text2, int.Parse(node.GetAttribute(text2)), true);
				}
			}
			this.CheckAndSetClassRestriction(this.m_classRestrictions, dictionary);
			if (this.customParamsChanged != null)
			{
				CustomParams newState = (CustomParams)base.Room.GetState<CustomParams>(AccessMode.ReadOnly).Clone();
				this.customParamsChanged(oldState, newState);
			}
		}

		// Token: 0x06002087 RID: 8327 RVA: 0x00084B10 File Offset: 0x00082F10
		private bool CheckAndSetRestriction(string restriction, bool value, bool roomCreation)
		{
			MissionContext mission = base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission;
			if (mission == null)
			{
				throw new ApplicationException("Room has no mission set");
			}
			CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadWrite);
			if (this.m_gameModesSystem.IsGlobalRestriction(restriction) && !roomCreation)
			{
				return false;
			}
			bool result = false;
			bool flag = false;
			bool flag2 = false;
			switch (restriction)
			{
			case "friendly_fire":
				flag = state.FriendlyFire;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.FriendlyFire);
				flag2 = state.FriendlyFire;
				break;
			case "enemy_outlines":
				flag = state.EnemyOutlines;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.EnemyOutlines);
				flag2 = state.EnemyOutlines;
				break;
			case "auto_team_balance":
				flag = state.Autobalance;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.Autobalance);
				flag2 = state.Autobalance;
				break;
			case "dead_can_chat":
				flag = state.DeadCanChat;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.DeadCanChat);
				flag2 = state.DeadCanChat;
				break;
			case "join_in_the_process":
				flag = state.JoinInTheProcess;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.JoinInTheProcess);
				flag2 = state.JoinInTheProcess;
				break;
			case "locked_spectator_camera":
				flag = state.LockedSpectatorCamera;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.LockedSpectatorCamera);
				flag2 = state.LockedSpectatorCamera;
				break;
			case "high_latency_autokick":
				flag = state.HighLatencyAutoKick;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.HighLatencyAutoKick);
				flag2 = state.HighLatencyAutoKick;
				break;
			case "overtime_mode":
				flag = state.OvertimeMode;
				result = this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, restriction, value, ref state.OvertimeMode);
				flag2 = state.OvertimeMode;
				break;
			}
			if (flag != flag2)
			{
				Dictionary<ulong, RoomPlayer> players = base.Room.GetState<CoreState>(AccessMode.ReadWrite).Players;
				foreach (RoomPlayer player in players.Values)
				{
					this.ResetRoomPlayerStatus(player);
				}
				base.Room.SignalPlayersChanged();
			}
			return result;
		}

		// Token: 0x06002088 RID: 8328 RVA: 0x00084E4C File Offset: 0x0008324C
		private bool CheckAndSetRestriction(string restriction, int value, bool roomCreation)
		{
			MissionContext mission = base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission;
			if (mission == null)
			{
				throw new ApplicationException("Room has no mission set");
			}
			CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadWrite);
			if (this.m_gameModesSystem.IsGlobalRestriction(restriction) && !roomCreation)
			{
				return false;
			}
			bool result = false;
			int num = 0;
			int num2 = 0;
			if (restriction != null)
			{
				if (!(restriction == "max_players"))
				{
					if (!(restriction == "round_limit"))
					{
						if (!(restriction == "preround_time"))
						{
							if (restriction == "inventory_slot")
							{
								num = state.InventorySlots;
								result = this.m_gameModesSystem.ValidateRestriction<int>(mission, base.Room.Type, restriction, value, ref state.InventorySlots);
								num2 = state.InventorySlots;
							}
						}
						else
						{
							num = state.PreRoundTime;
							result = this.m_gameModesSystem.ValidateRestriction<int>(mission, base.Room.Type, restriction, value, ref state.RoundLimit);
							num2 = state.PreRoundTime;
						}
					}
					else
					{
						num = state.RoundLimit;
						result = this.m_gameModesSystem.ValidateRestriction<int>(mission, base.Room.Type, restriction, value, ref state.RoundLimit);
						num2 = state.RoundLimit;
					}
				}
				else
				{
					num = state.MaxPlayers;
					result = this.m_gameModesSystem.ValidateRestriction<int>(mission, base.Room.Type, restriction, value, ref state.MaxPlayers);
					num2 = state.MaxPlayers;
				}
			}
			if (num != num2)
			{
				Dictionary<ulong, RoomPlayer> players = base.Room.GetState<CoreState>(AccessMode.ReadWrite).Players;
				foreach (RoomPlayer player in players.Values)
				{
					this.ResetRoomPlayerStatus(player);
				}
				base.Room.SignalPlayersChanged();
			}
			return result;
		}

		// Token: 0x06002089 RID: 8329 RVA: 0x00085040 File Offset: 0x00083440
		private void CheckAndSetClassRestriction(Dictionary<ProfileProgressionInfo.PlayerClass, string> classNames, Dictionary<int, bool> classRestrictions)
		{
			MissionContext mission = base.Room.GetState<MissionState>(AccessMode.ReadOnly).Mission;
			if (mission == null)
			{
				throw new ApplicationException("Room has no mission set");
			}
			CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadWrite);
			bool flag = false;
			foreach (KeyValuePair<int, bool> keyValuePair in classRestrictions)
			{
				bool flag2 = true;
				ProfileProgressionInfo.PlayerClass playerClass = Utils.ConvertToEnumFlag<ProfileProgressionInfo.PlayerClass>(keyValuePair.Key);
				string kind;
				if (!classNames.TryGetValue(playerClass, out kind))
				{
					string message = string.Format("Invalid profile class {0}({1}) in class restriction", playerClass, keyValuePair.Key);
					throw new RoomGenericException(base.Room.ID, message);
				}
				this.m_gameModesSystem.ValidateRestriction<bool>(mission, base.Room.Type, kind, keyValuePair.Value, ref flag2);
				flag |= (state.IsClassSuported(keyValuePair.Key) != flag2);
				state.SetClassRestriction(playerClass, flag2);
			}
			Dictionary<ulong, RoomPlayer> players = base.Room.GetState<CoreState>(AccessMode.ReadWrite).Players;
			foreach (RoomPlayer player in players.Values)
			{
				this.ValidatePlayerClass(player);
				if (flag)
				{
					this.ResetRoomPlayerStatus(player);
				}
			}
			if (flag)
			{
				base.Room.SignalPlayersChanged();
			}
		}

		// Token: 0x0600208A RID: 8330 RVA: 0x000851DC File Offset: 0x000835DC
		private bool ValidatePlayerClass(RoomPlayer player)
		{
			CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadOnly);
			if (state.IsClassSuported(player.ClassID))
			{
				if (player.RoomStatus == RoomPlayer.EStatus.CantBeReady)
				{
					player.RoomStatus = RoomPlayer.EStatus.NotReady;
				}
				return true;
			}
			if ((player.ProfileProgression.ClassUnlocked & (ProfileProgressionInfo.PlayerClass)state.ClassRestriction) == ProfileProgressionInfo.PlayerClass.None)
			{
				player.RoomStatus = RoomPlayer.EStatus.CantBeReady;
				return false;
			}
			for (int i = 0; i < 5; i++)
			{
				if (state.IsClassSuported(i))
				{
					player.ClassID = i;
					if (player.RoomStatus == RoomPlayer.EStatus.CantBeReady)
					{
						player.RoomStatus = RoomPlayer.EStatus.NotReady;
					}
					break;
				}
			}
			return false;
		}

		// Token: 0x0600208B RID: 8331 RVA: 0x0008527C File Offset: 0x0008367C
		private void OnPlayerAdded(ulong profileId, GameRoomPlayerAddReason reason)
		{
			RoomPlayer player = base.Room.GetPlayer(profileId);
			bool flag = !this.ValidatePlayerClass(player);
			if (!player.CanJoinMission(this.m_limitationService, base.Room.MissionType.Name))
			{
				flag = true;
				player.RoomStatus = RoomPlayer.EStatus.CantBeReady;
			}
			if (flag)
			{
				base.Room.SignalPlayersChanged();
			}
		}

		// Token: 0x0600208C RID: 8332 RVA: 0x000852DC File Offset: 0x000836DC
		private GameRoomRetCode OnPlayerAddCheck(RoomPlayer player)
		{
			GameRoomRetCode result = GameRoomRetCode.OK;
			if (base.Room.IsPvpMode())
			{
				CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadOnly);
				if (!state.JoinInTheProcess && base.Room.Locked)
				{
					result = GameRoomRetCode.PRIVATE;
				}
				if (((byte)player.ProfileProgression.ClassUnlocked & state.ClassRestriction) == 0)
				{
					result = GameRoomRetCode.CLASS_RESTRICTED;
				}
			}
			return result;
		}

		// Token: 0x0600208D RID: 8333 RVA: 0x00085344 File Offset: 0x00083744
		private void OnSessionStartFailed()
		{
			if (base.Room.IsPvpMode())
			{
				CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadOnly);
				if (!state.JoinInTheProcess)
				{
					base.Room.Locked = false;
				}
			}
		}

		// Token: 0x0600208E RID: 8334 RVA: 0x00085388 File Offset: 0x00083788
		private void OnSessionEnded(string session_id, bool abnormal)
		{
			if (base.Room.IsPvpMode())
			{
				CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadOnly);
				if (!state.JoinInTheProcess)
				{
					base.Room.Locked = false;
				}
			}
		}

		// Token: 0x0600208F RID: 8335 RVA: 0x000853CC File Offset: 0x000837CC
		private void OnSessionStarted(string session_id)
		{
			if (base.Room.IsPvpMode())
			{
				CustomParams state = base.Room.GetState<CustomParams>(AccessMode.ReadOnly);
				if (!state.JoinInTheProcess)
				{
					base.Room.Locked = true;
				}
				this.m_sessionStorage.AddData(session_id, ESessionData.Restrictions, state);
			}
		}

		// Token: 0x06002090 RID: 8336 RVA: 0x0008541B File Offset: 0x0008381B
		private void ResetRoomPlayerStatus(RoomPlayer player)
		{
			player.RoomStatus = ((player.RoomStatus == RoomPlayer.EStatus.CantBeReady) ? RoomPlayer.EStatus.CantBeReady : RoomPlayer.EStatus.NotReady);
		}

		// Token: 0x04000FFA RID: 4090
		private readonly IMissionAccessLimitationService m_limitationService;

		// Token: 0x04000FFB RID: 4091
		private readonly IGameModesSystem m_gameModesSystem;

		// Token: 0x04000FFC RID: 4092
		private readonly ISessionStorage m_sessionStorage;

		// Token: 0x04000FFD RID: 4093
		private readonly Dictionary<ProfileProgressionInfo.PlayerClass, string> m_classRestrictions = new Dictionary<ProfileProgressionInfo.PlayerClass, string>
		{
			{
				ProfileProgressionInfo.PlayerClass.Rifleman,
				"class_rifleman"
			},
			{
				ProfileProgressionInfo.PlayerClass.Sniper,
				"class_sniper"
			},
			{
				ProfileProgressionInfo.PlayerClass.Medic,
				"class_medic"
			},
			{
				ProfileProgressionInfo.PlayerClass.Engineer,
				"class_engineer"
			}
		};

		// Token: 0x020005FA RID: 1530
		// (Invoke) Token: 0x06002092 RID: 8338
		internal delegate void CustomParamsChanged(CustomParams oldState, CustomParams newState);
	}
}

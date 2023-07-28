using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.PunishmentSystem
{
	// Token: 0x0200058E RID: 1422
	[QueryAttributes(TagName = "punish_mode")]
	internal class PunishModeQuery : SessionQuery
	{
		// Token: 0x06001EA3 RID: 7843 RVA: 0x0007C5D0 File Offset: 0x0007A9D0
		public PunishModeQuery(ISessionStorage sessionStorage, IPunishmentService punishmentService) : base(sessionStorage)
		{
			this.m_punishmentService = punishmentService;
		}

		// Token: 0x06001EA4 RID: 7844 RVA: 0x0007C5E0 File Offset: 0x0007A9E0
		protected override int QueryGetResponseImpl(string fromJid, XmlElement request, XmlElement response)
		{
			ulong num = ulong.Parse(request.GetAttribute("profile_id"));
			string attribute = request.GetAttribute("session_id");
			string attribute2 = request.GetAttribute("punish_mode");
			Log.Info("Executing punish_mode command '{0} {1} {2}' sent by {3}", new object[]
			{
				num,
				attribute,
				attribute2,
				fromJid
			});
			if (attribute2 != null)
			{
				if (PunishModeQuery.<>f__switch$map1 == null)
				{
					PunishModeQuery.<>f__switch$map1 = new Dictionary<string, int>(7)
					{
						{
							"kick",
							0
						},
						{
							"kick_anticheat",
							1
						},
						{
							"kick_eac_other",
							2
						},
						{
							"kick_eac_auth",
							3
						},
						{
							"kick_eac_banned",
							4
						},
						{
							"kick_eac_violation",
							5
						},
						{
							"kick_high_latency",
							6
						}
					};
				}
				int num2;
				if (PunishModeQuery.<>f__switch$map1.TryGetValue(attribute2, out num2))
				{
					switch (num2)
					{
					case 0:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickAdmin);
						break;
					case 1:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickAntiCheat);
						break;
					case 2:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickEACOther);
						break;
					case 3:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickEACAuthenticationFailed);
						break;
					case 4:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickEACBanned);
						break;
					case 5:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickEACViolation);
						break;
					case 6:
						this.m_punishmentService.KickPlayerLocal(num, GameRoomPlayerRemoveReason.KickHighLatency);
						break;
					case 7:
						goto IL_187;
					default:
						goto IL_187;
					}
					return 0;
				}
			}
			IL_187:
			throw new NotImplementedException(string.Format("Punishment Mode {0} not implemented", attribute2));
		}

		// Token: 0x04000EE9 RID: 3817
		private readonly IPunishmentService m_punishmentService;
	}
}

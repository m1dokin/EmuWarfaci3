using System;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Database;
using MasterServer.GameRoomSystem;
using MasterServer.GameRoomSystem.RoomExtensions;
using MasterServer.Users;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000546 RID: 1350
	[Service]
	[Singleton]
	internal class ClassChangingService : ServiceModule, IClassChangingService
	{
		// Token: 0x06001D2A RID: 7466 RVA: 0x00075E95 File Offset: 0x00074295
		public ClassChangingService(IProfileValidationService profileValidation, IGameRoomManager gameRoomManager, IDALService dal)
		{
			this.m_profileValidation = profileValidation;
			this.m_gameRoomManager = gameRoomManager;
			this.m_dal = dal;
		}

		// Token: 0x06001D2B RID: 7467 RVA: 0x00075EB2 File Offset: 0x000742B2
		private ClassChangeStatus LogAndReturn(ClassChangeStatus err, ulong profileId)
		{
			if (err != ClassChangeStatus.Ok)
			{
				Log.Warning<string, ulong>("[ClassChangingService.ChangePlayersClass] {0} while processing request from {1}", err.ToString(), profileId);
			}
			return err;
		}

		// Token: 0x06001D2C RID: 7468 RVA: 0x00075ED4 File Offset: 0x000742D4
		public ClassChangeStatus ChangePlayersClass(UserInfo.User user, uint classId)
		{
			if (!this.m_profileValidation.ValidateProfileClass(user, classId))
			{
				return this.LogAndReturn(ClassChangeStatus.ClassValidationFail, user.ProfileID);
			}
			if (!user.ProfileProgression.IsClassUnlocked((int)classId))
			{
				return this.LogAndReturn(ClassChangeStatus.ProgressionRestriction, user.ProfileID);
			}
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(user.ProfileID);
			if (roomByPlayer != null && !roomByPlayer.IsPveMode())
			{
				bool success = false;
				roomByPlayer.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
				{
					CustomParams state = r.GetState<CustomParams>(AccessMode.ReadOnly);
					if (state.IsClassSuported((int)classId))
					{
						success = true;
					}
				});
				if (!success)
				{
					return this.LogAndReturn(ClassChangeStatus.RoomRestriction, user.ProfileID);
				}
			}
			this.m_dal.ProfileSystem.SetProfileCurClass(user.ProfileID, classId);
			return ClassChangeStatus.Ok;
		}

		// Token: 0x04000DF3 RID: 3571
		private readonly IProfileValidationService m_profileValidation;

		// Token: 0x04000DF4 RID: 3572
		private readonly IGameRoomManager m_gameRoomManager;

		// Token: 0x04000DF5 RID: 3573
		private readonly IDALService m_dal;
	}
}

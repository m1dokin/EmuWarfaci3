using System;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.ProfileLogic;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x020005F8 RID: 1528
	[RoomState(new Type[]
	{
		typeof(CustomParamsExtension)
	})]
	internal class CustomParams : RoomStateBase
	{
		// Token: 0x0600207B RID: 8315 RVA: 0x0008420C File Offset: 0x0008260C
		public void SetClassRestriction(ProfileProgressionInfo.PlayerClass pClass, bool enabled)
		{
			if (enabled)
			{
				this.ClassRestriction |= (byte)pClass;
			}
			else
			{
				this.ClassRestriction &= (byte)(~(byte)pClass);
			}
		}

		// Token: 0x0600207C RID: 8316 RVA: 0x0008423C File Offset: 0x0008263C
		public bool IsClassSuported(int class_id)
		{
			byte b = (byte)(1 << class_id);
			return (b & this.ClassRestriction) > 0;
		}

		// Token: 0x0600207D RID: 8317 RVA: 0x0008425C File Offset: 0x0008265C
		public bool IsItemAllowed(SProfileItem profileItem, int classId)
		{
			int num = classId * 5;
			ulong num2 = 31UL << num;
			int num3 = (int)((profileItem.SlotIDs & num2) >> num);
			int num4 = 1 << num3;
			return (this.InventorySlots & num4) != 0;
		}

		// Token: 0x04000FEC RID: 4076
		public bool FriendlyFire;

		// Token: 0x04000FED RID: 4077
		public bool EnemyOutlines = true;

		// Token: 0x04000FEE RID: 4078
		public bool Autobalance;

		// Token: 0x04000FEF RID: 4079
		public bool DeadCanChat;

		// Token: 0x04000FF0 RID: 4080
		public bool JoinInTheProcess;

		// Token: 0x04000FF1 RID: 4081
		public int MaxPlayers = 16;

		// Token: 0x04000FF2 RID: 4082
		public int RoundLimit;

		// Token: 0x04000FF3 RID: 4083
		public int PreRoundTime = -1;

		// Token: 0x04000FF4 RID: 4084
		public byte ClassRestriction = byte.MaxValue;

		// Token: 0x04000FF5 RID: 4085
		public int InventorySlots = int.MaxValue;

		// Token: 0x04000FF6 RID: 4086
		public bool LockedSpectatorCamera;

		// Token: 0x04000FF7 RID: 4087
		public bool HighLatencyAutoKick = true;

		// Token: 0x04000FF8 RID: 4088
		public bool OvertimeMode;
	}
}

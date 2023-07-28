using System;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200052C RID: 1324
	public class RoomReference
	{
		// Token: 0x06001CBF RID: 7359 RVA: 0x0007395F File Offset: 0x00071D5F
		public RoomReference(string roomName)
		{
			this.Reference = roomName;
		}

		// Token: 0x17000312 RID: 786
		// (get) Token: 0x06001CC0 RID: 7360 RVA: 0x0007396E File Offset: 0x00071D6E
		// (set) Token: 0x06001CC1 RID: 7361 RVA: 0x00073976 File Offset: 0x00071D76
		public string Reference { get; private set; }

		// Token: 0x06001CC2 RID: 7362 RVA: 0x00073980 File Offset: 0x00071D80
		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(obj, this))
			{
				return true;
			}
			RoomReference roomReference = obj as RoomReference;
			return roomReference != null && this.Reference.Equals(roomReference.Reference);
		}

		// Token: 0x06001CC3 RID: 7363 RVA: 0x000739BC File Offset: 0x00071DBC
		public override int GetHashCode()
		{
			return this.Reference.GetHashCode();
		}

		// Token: 0x06001CC4 RID: 7364 RVA: 0x000739C9 File Offset: 0x00071DC9
		public override string ToString()
		{
			return this.Reference;
		}

		// Token: 0x04000DB3 RID: 3507
		public static readonly RoomReference EmptyReference = new RoomReference(string.Empty);
	}
}

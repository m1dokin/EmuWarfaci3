using System;
using System.Xml;
using MasterServer.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005E3 RID: 1507
	internal class RoomExtensionBase : IRoomExtension, IDisposable
	{
		// Token: 0x1700034A RID: 842
		// (get) Token: 0x06002000 RID: 8192 RVA: 0x0006735A File Offset: 0x0006575A
		// (set) Token: 0x06002001 RID: 8193 RVA: 0x00067362 File Offset: 0x00065762
		private protected IGameRoom Room { protected get; private set; }

		// Token: 0x06002002 RID: 8194 RVA: 0x0006736B File Offset: 0x0006576B
		public virtual void Init(IGameRoom room)
		{
			this.Room = room;
		}

		// Token: 0x06002003 RID: 8195 RVA: 0x00067374 File Offset: 0x00065774
		public virtual void Close()
		{
		}

		// Token: 0x06002004 RID: 8196 RVA: 0x00067376 File Offset: 0x00065776
		public void Dispose()
		{
			if (this.m_disposed)
			{
				throw new ObjectDisposedException(string.Format("Object already disposed {0}", base.GetType().Name));
			}
			this.OnDisposing();
			this.m_disposed = true;
		}

		// Token: 0x06002005 RID: 8197 RVA: 0x000673AB File Offset: 0x000657AB
		public virtual void GetStateUpdateRecepients(RoomUpdate.Context ctx, Set<string> recepients)
		{
		}

		// Token: 0x06002006 RID: 8198 RVA: 0x000673AD File Offset: 0x000657AD
		public virtual XmlElement SerializeStateChanges(RoomUpdate.Context ctx)
		{
			return null;
		}

		// Token: 0x06002007 RID: 8199 RVA: 0x000673B0 File Offset: 0x000657B0
		public virtual void PostStateChanged(IRoomState new_state, IRoomState old_state)
		{
		}

		// Token: 0x06002008 RID: 8200 RVA: 0x000673B2 File Offset: 0x000657B2
		protected virtual void OnDisposing()
		{
		}

		// Token: 0x04000FA9 RID: 4009
		private bool m_disposed;
	}
}

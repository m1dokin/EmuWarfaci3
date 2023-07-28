using System;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Timers;
using MasterServer.CryOnlineNET;

namespace MasterServer.GameRoomSystem.RoomExtensions
{
	// Token: 0x0200061A RID: 1562
	[RoomExtension]
	internal class StateSyncExtension : RoomExtensionBase
	{
		// Token: 0x06002191 RID: 8593 RVA: 0x00089A64 File Offset: 0x00087E64
		public StateSyncExtension(IOnlineClient onlineClient)
		{
			this.m_onlineClient = onlineClient;
			int milliseconds = this.DEFAULT_ROOM_SYNC_TIMEOUT;
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			if (section.HasValue("room_sync_timeout"))
			{
				milliseconds = int.Parse(section.Get("room_sync_timeout"));
			}
			this.m_client_sync_timeout = new TimeSpan(0, 0, 0, 0, milliseconds);
			section.OnConfigChanged += this.OnConfigChanged;
		}

		// Token: 0x06002192 RID: 8594 RVA: 0x00089AF0 File Offset: 0x00087EF0
		protected override void OnDisposing()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("GameRoom");
			section.OnConfigChanged -= this.OnConfigChanged;
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.StopTimer();
			}
			base.OnDisposing();
		}

		// Token: 0x06002193 RID: 8595 RVA: 0x00089B5C File Offset: 0x00087F5C
		private void StartTimer(TimeSpan timeout)
		{
			if (this.m_deliver_timer == null)
			{
				this.m_deliver_timer = new SafeTimer(new TimerCallback(this.OnDeliveryTimerTick), null, timeout);
			}
		}

		// Token: 0x06002194 RID: 8596 RVA: 0x00089B82 File Offset: 0x00087F82
		private void StopTimer()
		{
			if (this.m_deliver_timer != null)
			{
				this.m_deliver_timer.Dispose();
				this.m_deliver_timer = null;
			}
		}

		// Token: 0x06002195 RID: 8597 RVA: 0x00089BA4 File Offset: 0x00087FA4
		private void MergePendingUpdates(Set<string> recepients, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			if (this.m_pending_update == null)
			{
				this.m_pending_update = snapshot;
				this.m_pending_recepients = recepients;
				return;
			}
			this.m_pending_recepients.Add(recepients);
			using (List<DoubleBuffer<Type, IRoomState>.Flip>.Enumerator enumerator = snapshot.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DoubleBuffer<Type, IRoomState>.Flip new_flip = enumerator.Current;
					if (new_flip.new_value != null)
					{
						int num = this.m_pending_update.FindIndex((DoubleBuffer<Type, IRoomState>.Flip F) => F.key == new_flip.key);
						if (num < 0)
						{
							this.m_pending_update.Add(new_flip);
						}
						else
						{
							DoubleBuffer<Type, IRoomState>.Flip old_flip = this.m_pending_update[num];
							if (old_flip.new_value == null || new_flip.new_value.Revision > old_flip.new_value.Revision)
							{
								this.m_pending_update[num] = this.MergeFlips(old_flip, new_flip);
							}
						}
					}
				}
			}
		}

		// Token: 0x06002196 RID: 8598 RVA: 0x00089CC4 File Offset: 0x000880C4
		private DoubleBuffer<Type, IRoomState>.Flip MergeFlips(DoubleBuffer<Type, IRoomState>.Flip old_flip, DoubleBuffer<Type, IRoomState>.Flip new_flip)
		{
			return new DoubleBuffer<Type, IRoomState>.Flip
			{
				key = old_flip.key,
				old_value = old_flip.old_value,
				new_value = new_flip.new_value
			};
		}

		// Token: 0x06002197 RID: 8599 RVA: 0x00089D04 File Offset: 0x00088104
		public void SyncStateToClients(Set<string> recepients, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				this.MergePendingUpdates(recepients, snapshot);
				TimeSpan timeSpan = DateTime.Now - this.m_last_sync_time;
				if (this.IsImportantUpdate(snapshot) || timeSpan >= this.m_client_sync_timeout)
				{
					this.StopTimer();
					this.FlushClientSync();
				}
				else
				{
					Log.Verbose(Log.Group.GameRoom, "Room {0} delays state sync to clients", new object[]
					{
						base.Room.ID
					});
					this.StartTimer(this.m_client_sync_timeout - timeSpan);
				}
			}
		}

		// Token: 0x06002198 RID: 8600 RVA: 0x00089DC0 File Offset: 0x000881C0
		public void SyncStateToServer(string server_jid, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			Log.Verbose(Log.Group.GameRoom, "Room {0} sends incremental update to server", new object[]
			{
				base.Room.ID
			});
			XmlDocument factory = new XmlDocument();
			XmlElement xmlElement = base.Room.SerializeStateChanges(RoomUpdate.Target.Server, RoomUpdate.Kind.Incremental, snapshot, factory);
			if (xmlElement != null)
			{
				QueryManager.RequestSt("mission_update", server_jid, new object[]
				{
					xmlElement
				});
			}
		}

		// Token: 0x06002199 RID: 8601 RVA: 0x00089E24 File Offset: 0x00088224
		private void OnDeliveryTimerTick(object dummy)
		{
			object @lock = this.m_lock;
			lock (@lock)
			{
				if (this.m_deliver_timer != null)
				{
					this.FlushClientSync();
					this.StopTimer();
				}
			}
		}

		// Token: 0x0600219A RID: 8602 RVA: 0x00089E80 File Offset: 0x00088280
		private void FlushClientSync()
		{
			this.m_last_sync_time = DateTime.Now;
			Log.Verbose(Log.Group.GameRoom, "Room {0} broadcasts incremental updates to clients", new object[]
			{
				base.Room.ID
			});
			XmlDocument factory = new XmlDocument();
			XmlElement xmlElement = base.Room.SerializeStateChanges(RoomUpdate.Target.Client, RoomUpdate.Kind.Incremental, this.m_pending_update, factory);
			if (xmlElement != null)
			{
				QueryManager.RequestSt("gameroom_sync", "k01." + this.m_onlineClient.XmppHost, new object[]
				{
					this.m_pending_recepients,
					xmlElement
				});
			}
			this.m_pending_update = null;
			this.m_pending_recepients = null;
		}

		// Token: 0x0600219B RID: 8603 RVA: 0x00089F20 File Offset: 0x00088320
		private bool IsImportantUpdate(DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			int num = snapshot.FindIndex((DoubleBuffer<Type, IRoomState>.Flip F) => F.key == typeof(CoreState));
			if (num < 0)
			{
				return false;
			}
			CoreState coreState = (CoreState)snapshot[num].old_value;
			CoreState coreState2 = (CoreState)snapshot[num].new_value;
			return coreState != null && coreState2 != null && coreState.Players.Count != coreState2.Players.Count;
		}

		// Token: 0x0600219C RID: 8604 RVA: 0x00089FB0 File Offset: 0x000883B0
		private void OnConfigChanged(ConfigEventArgs args)
		{
			if (string.Compare(args.Name, "room_sync_timeout", true) == 0)
			{
				object @lock = this.m_lock;
				lock (@lock)
				{
					this.m_client_sync_timeout = new TimeSpan(0, 0, 0, 0, args.iValue);
				}
			}
		}

		// Token: 0x04001040 RID: 4160
		private int DEFAULT_ROOM_SYNC_TIMEOUT = 1000;

		// Token: 0x04001041 RID: 4161
		private DoubleBuffer<Type, IRoomState>.Snapshot m_pending_update;

		// Token: 0x04001042 RID: 4162
		private Set<string> m_pending_recepients;

		// Token: 0x04001043 RID: 4163
		private SafeTimer m_deliver_timer;

		// Token: 0x04001044 RID: 4164
		private DateTime m_last_sync_time;

		// Token: 0x04001045 RID: 4165
		private TimeSpan m_client_sync_timeout;

		// Token: 0x04001046 RID: 4166
		private readonly object m_lock = new object();

		// Token: 0x04001047 RID: 4167
		private readonly IOnlineClient m_onlineClient;
	}
}

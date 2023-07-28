using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.GameRoomSystem.RoomExtensions;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x020005EA RID: 1514
	internal class GameRoomBase : IDisposable
	{
		// Token: 0x06002013 RID: 8211 RVA: 0x00069225 File Offset: 0x00067625
		protected GameRoomBase(GameRoomType type, RoomExtensionsData extensions)
		{
			this.Type = type;
			this.m_buffer = new DoubleBuffer<Type, IRoomState>(GameRoomBase.LoadStates(extensions));
			this.LoadExtensions(extensions);
		}

		// Token: 0x06002014 RID: 8212 RVA: 0x0006924C File Offset: 0x0006764C
		public bool IsPveMode()
		{
			return this.Type.IsPveMode();
		}

		// Token: 0x06002015 RID: 8213 RVA: 0x00069259 File Offset: 0x00067659
		public bool IsPvpMode()
		{
			return this.Type.IsPvpMode();
		}

		// Token: 0x06002016 RID: 8214 RVA: 0x00069266 File Offset: 0x00067666
		public bool IsClanWarMode()
		{
			return this.Type.IsClanWarMode();
		}

		// Token: 0x06002017 RID: 8215 RVA: 0x00069273 File Offset: 0x00067673
		public bool IsAutoStartMode()
		{
			return this.Type.IsAutoStartMode();
		}

		// Token: 0x06002018 RID: 8216 RVA: 0x00069280 File Offset: 0x00067680
		public bool IsPublicPvPMode()
		{
			return this.Type.IsPublicPvPMode();
		}

		// Token: 0x06002019 RID: 8217 RVA: 0x0006928D File Offset: 0x0006768D
		public bool IsPveAutoStartMode()
		{
			return this.Type.IsPveAutoStartMode();
		}

		// Token: 0x0600201A RID: 8218 RVA: 0x0006929A File Offset: 0x0006769A
		public bool IsPvpAutoStartMode()
		{
			return this.Type.IsPvpAutoStartMode();
		}

		// Token: 0x0600201B RID: 8219 RVA: 0x000692A7 File Offset: 0x000676A7
		public bool IsPvpRatingMode()
		{
			return this.Type.IsPvpRatingMode();
		}

		// Token: 0x1700034B RID: 843
		// (get) Token: 0x0600201C RID: 8220 RVA: 0x000692B4 File Offset: 0x000676B4
		// (set) Token: 0x0600201D RID: 8221 RVA: 0x000692BC File Offset: 0x000676BC
		public ulong ID { get; protected set; }

		// Token: 0x1700034C RID: 844
		// (get) Token: 0x0600201E RID: 8222 RVA: 0x000692C5 File Offset: 0x000676C5
		// (set) Token: 0x0600201F RID: 8223 RVA: 0x000692CD File Offset: 0x000676CD
		public GameRoomType Type { get; private set; }

		// Token: 0x1700034D RID: 845
		// (get) Token: 0x06002020 RID: 8224 RVA: 0x000692D6 File Offset: 0x000676D6
		protected bool Closed
		{
			get
			{
				return this.m_closed;
			}
		}

		// Token: 0x06002021 RID: 8225 RVA: 0x000692E0 File Offset: 0x000676E0
		public virtual void Close()
		{
			this.CheckAccessMode(AccessMode.ReadWrite);
			foreach (IRoomExtension roomExtension in this.m_extensions.Values)
			{
				roomExtension.Close();
			}
			this.m_closed = true;
		}

		// Token: 0x06002022 RID: 8226 RVA: 0x00069350 File Offset: 0x00067750
		public virtual void Dispose()
		{
			this.m_buffer.enter(AccessMode.ReadWrite);
			try
			{
				this.m_disposed = true;
				foreach (IRoomExtension roomExtension in this.m_extensions.Values)
				{
					roomExtension.Dispose();
				}
			}
			finally
			{
				this.m_buffer.discard();
				this.m_buffer.exit();
			}
		}

		// Token: 0x06002023 RID: 8227 RVA: 0x000693EC File Offset: 0x000677EC
		private static Dictionary<Type, IRoomState> LoadStates(RoomExtensionsData extension_data)
		{
			return extension_data.SelectMany((KeyValuePair<IRoomExtension, List<IRoomState>> kv) => kv.Value).ToDictionary((IRoomState state) => state.GetType());
		}

		// Token: 0x06002024 RID: 8228 RVA: 0x00069440 File Offset: 0x00067840
		private void LoadExtensions(RoomExtensionsData extension_data)
		{
			this.m_extensions = new Dictionary<Type, IRoomExtension>();
			this.m_state_to_extension = new Dictionary<Type, IRoomExtension>();
			foreach (KeyValuePair<IRoomExtension, List<IRoomState>> keyValuePair in extension_data)
			{
				this.m_extensions.Add(keyValuePair.Key.GetType(), keyValuePair.Key);
				foreach (IRoomState roomState in keyValuePair.Value)
				{
					this.m_state_to_extension.Add(roomState.GetType(), keyValuePair.Key);
				}
			}
		}

		// Token: 0x1700034E RID: 846
		// (get) Token: 0x06002025 RID: 8229 RVA: 0x00069524 File Offset: 0x00067924
		public AccessMode CurrentAccessMode
		{
			get
			{
				return this.m_buffer.mode;
			}
		}

		// Token: 0x06002026 RID: 8230 RVA: 0x00069531 File Offset: 0x00067931
		public void CheckAccessMode(AccessMode atleast)
		{
			if (!this.m_buffer.check_access(atleast))
			{
				throw new ApplicationException("Transaction access mode violation");
			}
		}

		// Token: 0x06002027 RID: 8231 RVA: 0x0006954F File Offset: 0x0006794F
		public T GetState<T>(AccessMode acc) where T : IRoomState
		{
			this.CheckAccessMode(acc);
			return (T)((object)this.m_buffer.get(typeof(T), acc));
		}

		// Token: 0x06002028 RID: 8232 RVA: 0x00069574 File Offset: 0x00067974
		public T TryGetState<T>(AccessMode acc) where T : IRoomState
		{
			if (this.m_state_to_extension.ContainsKey(typeof(T)))
			{
				return this.GetState<T>(acc);
			}
			return default(T);
		}

		// Token: 0x06002029 RID: 8233 RVA: 0x000695AC File Offset: 0x000679AC
		public void transaction(AccessMode acc, Action<IGameRoom> action)
		{
			this.transaction(acc, action, null);
		}

		// Token: 0x0600202A RID: 8234 RVA: 0x000695B7 File Offset: 0x000679B7
		public void transaction(AccessMode acc, Action<IGameRoom> action, Action<IGameRoom> post_commit)
		{
			this.transaction(null, null, acc, action, post_commit);
		}

		// Token: 0x0600202B RID: 8235 RVA: 0x000695C5 File Offset: 0x000679C5
		public XmlElement transaction(string online_id, XmlDocument factory, AccessMode acc, Action<IGameRoom> action)
		{
			return this.transaction(online_id, factory, acc, action, null);
		}

		// Token: 0x0600202C RID: 8236 RVA: 0x000695D3 File Offset: 0x000679D3
		private XmlElement transaction(string online_id, XmlDocument factory, AccessMode acc, Action<IGameRoom> action, Action<IGameRoom> post_commit)
		{
			if (acc == AccessMode.ReadOnly)
			{
				this.transaction_ro(action);
				return null;
			}
			if (acc != AccessMode.ReadWrite)
			{
				throw new InvalidOperationException("Invalid access mode");
			}
			return this.transaction_rw(online_id, factory, action, post_commit);
		}

		// Token: 0x0600202D RID: 8237 RVA: 0x0006960C File Offset: 0x00067A0C
		private void transaction_ro(Action<IGameRoom> action)
		{
			GameRoom gameRoom = (GameRoom)this;
			Log.Verbose(Log.Group.GameRoom, "Starting room {0} transaction: ReadOnly", new object[]
			{
				gameRoom.ID
			});
			this.m_buffer.enter(AccessMode.ReadOnly);
			try
			{
				if (this.m_disposed)
				{
					throw new RoomClosedException(gameRoom.ID);
				}
				action(gameRoom);
			}
			finally
			{
				this.m_buffer.exit();
			}
			Log.Verbose(Log.Group.GameRoom, "Committing room {0} transaction: ReadOnly", new object[]
			{
				gameRoom.ID
			});
		}

		// Token: 0x0600202E RID: 8238 RVA: 0x000696AC File Offset: 0x00067AAC
		private XmlElement transaction_rw(string online_id, XmlDocument factory, Action<IGameRoom> action, Action<IGameRoom> post_commit)
		{
			GameRoom gameRoom = (GameRoom)this;
			Log.Verbose(Log.Group.GameRoom, "Starting room {0} transaction: ReadWrite", new object[]
			{
				gameRoom.ID
			});
			this.m_buffer.enter(AccessMode.ReadWrite);
			DoubleBuffer<Type, IRoomState>.Snapshot snapshot;
			try
			{
				if (this.m_disposed || this.m_closed)
				{
					throw new RoomClosedException(gameRoom.ID);
				}
				action(gameRoom);
				snapshot = this.m_buffer.commit();
			}
			catch
			{
				Log.Info<ulong, GameRoomType>("Reverting room {0} {1} transaction: ReadWrite", gameRoom.ID, gameRoom.Type);
				this.m_buffer.discard();
				this.m_buffer.exit();
				throw;
			}
			XmlElement result;
			try
			{
				Log.Verbose(Log.Group.GameRoom, "Committing room {0} transaction: ReadWrite", new object[]
				{
					gameRoom.ID
				});
				if (post_commit != null)
				{
					post_commit(gameRoom);
				}
				result = ((snapshot == null) ? null : this.PostCommitTransaction(online_id, factory, snapshot));
			}
			finally
			{
				this.m_buffer.exit();
			}
			return result;
		}

		// Token: 0x0600202F RID: 8239 RVA: 0x000697C8 File Offset: 0x00067BC8
		private XmlElement PostCommitTransaction(string initiator_online_id, XmlDocument factory, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			bool flag = false;
			foreach (DoubleBuffer<Type, IRoomState>.Flip flip in snapshot)
			{
				if (flip.new_value != null)
				{
					if (flip.key == typeof(CoreState))
					{
						this.PostStateChanged(flip.new_value, flip.old_value);
						CoreState coreState = (CoreState)flip.new_value;
						CoreState coreState2 = (CoreState)flip.old_value;
						flag |= ((coreState2.Players.Any<KeyValuePair<ulong, RoomPlayer>>() || coreState2.ReservedPlayers.Any<KeyValuePair<ulong, RoomPlayer>>()) && (!coreState.Players.Any<KeyValuePair<ulong, RoomPlayer>>() && !coreState.ReservedPlayers.Any<KeyValuePair<ulong, RoomPlayer>>()));
					}
					else
					{
						IRoomExtension roomExtension = this.m_state_to_extension[flip.key];
						roomExtension.PostStateChanged(flip.new_value, flip.old_value);
					}
				}
			}
			this.BroadcastStateUpdate(initiator_online_id, snapshot);
			if (flag)
			{
				try
				{
					this.transaction(AccessMode.ReadWrite, delegate(IGameRoom r)
					{
						r.Close();
					});
				}
				catch (RoomClosedException)
				{
				}
			}
			if (!string.IsNullOrEmpty(initiator_online_id))
			{
				return this.SerializeStateChanges(RoomUpdate.Target.Client, RoomUpdate.Kind.Incremental, snapshot, factory);
			}
			return null;
		}

		// Token: 0x06002030 RID: 8240 RVA: 0x0006994C File Offset: 0x00067D4C
		protected virtual void BroadcastStateUpdate(string initiator_online_id, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
			Set<string> set = new Set<string>();
			Set<string> set2 = new Set<string>();
			foreach (DoubleBuffer<Type, IRoomState>.Flip flip in snapshot)
			{
				this.CollectRecepients(RoomUpdate.Target.Client, flip, set);
				this.CollectRecepients(RoomUpdate.Target.Server, flip, set2);
			}
			set.Remove(initiator_online_id);
			if (set.Count != 0)
			{
				this.SendBroadcastUpdate(RoomUpdate.Target.Client, set, snapshot);
			}
			if (set2.Count == 1)
			{
				this.SendBroadcastUpdate(RoomUpdate.Target.Server, set2, snapshot);
			}
		}

		// Token: 0x06002031 RID: 8241 RVA: 0x000699EC File Offset: 0x00067DEC
		protected virtual void SendBroadcastUpdate(RoomUpdate.Target target, Set<string> recepients, DoubleBuffer<Type, IRoomState>.Snapshot snapshot)
		{
		}

		// Token: 0x06002032 RID: 8242 RVA: 0x000699F0 File Offset: 0x00067DF0
		private void CollectRecepients(RoomUpdate.Target target, DoubleBuffer<Type, IRoomState>.Flip flip, Set<string> recepients)
		{
			RoomUpdate.Context ctx = new RoomUpdate.Context
			{
				target = target,
				kind = RoomUpdate.Kind.Incremental,
				new_state = (flip.new_value ?? flip.old_value),
				old_state = flip.old_value,
				factory = null
			};
			IRoomExtension roomExtension = this.m_state_to_extension[flip.key];
			roomExtension.GetStateUpdateRecepients(ctx, recepients);
		}

		// Token: 0x06002033 RID: 8243 RVA: 0x00069A60 File Offset: 0x00067E60
		public XmlElement SerializeStateChanges(RoomUpdate.Target target, RoomUpdate.Kind kind, DoubleBuffer<Type, IRoomState>.Snapshot snapshot, XmlDocument factory)
		{
			XmlElement xmlElement = null;
			foreach (DoubleBuffer<Type, IRoomState>.Flip flip in snapshot)
			{
				if (flip.new_value != null)
				{
					RoomUpdate.Context ctx = new RoomUpdate.Context
					{
						target = target,
						kind = kind,
						new_state = flip.new_value,
						old_state = flip.old_value,
						factory = factory
					};
					IRoomExtension roomExtension = this.m_state_to_extension[flip.key];
					XmlElement xmlElement2 = roomExtension.SerializeStateChanges(ctx);
					if (xmlElement2 != null)
					{
						if (xmlElement == null)
						{
							xmlElement = this.CreateRoomElement(factory);
						}
						xmlElement2.SetAttribute("revision", flip.new_value.Revision.ToString());
						xmlElement.AppendChild(xmlElement2);
					}
				}
			}
			return xmlElement;
		}

		// Token: 0x06002034 RID: 8244 RVA: 0x00069B68 File Offset: 0x00067F68
		public virtual XmlElement CreateRoomElement(XmlDocument factory)
		{
			return factory.CreateElement("game_room");
		}

		// Token: 0x06002035 RID: 8245 RVA: 0x00069B75 File Offset: 0x00067F75
		protected virtual void PostStateChanged(IRoomState new_state, IRoomState old_state)
		{
		}

		// Token: 0x06002036 RID: 8246 RVA: 0x00069B77 File Offset: 0x00067F77
		public XmlElement FullStateSnapshot(RoomUpdate.Target target, XmlDocument factory)
		{
			return this.FullStateSnapshot(target, factory, 0UL);
		}

		// Token: 0x06002037 RID: 8247 RVA: 0x00069B84 File Offset: 0x00067F84
		public XmlElement FullStateSnapshot(RoomUpdate.Target target, XmlDocument factory, ulong profile_id)
		{
			XmlElement ret = null;
			this.transaction(AccessMode.ReadOnly, delegate(IGameRoom x)
			{
				DoubleBuffer<Type, IRoomState>.Snapshot snapshot = new DoubleBuffer<Type, IRoomState>.Snapshot();
				foreach (KeyValuePair<Type, IRoomState> keyValuePair in this.m_buffer.items)
				{
					DoubleBuffer<Type, IRoomState>.Flip item = new DoubleBuffer<Type, IRoomState>.Flip
					{
						key = keyValuePair.Key,
						new_value = keyValuePair.Value,
						old_value = null
					};
					snapshot.Add(item);
				}
				ret = this.SerializeStateChanges(target, RoomUpdate.Kind.Full, snapshot, factory);
			});
			return ret;
		}

		// Token: 0x06002038 RID: 8248 RVA: 0x00069BCC File Offset: 0x00067FCC
		public T GetExtension<T>() where T : IRoomExtension
		{
			Type typeFromHandle = typeof(T);
			IRoomExtension roomExtension;
			if (!this.m_extensions.TryGetValue(typeFromHandle, out roomExtension))
			{
				throw new RoomUnsupportedExtensionException(this.ID, this.Type, typeFromHandle, this.m_extensions.Keys);
			}
			return (T)((object)roomExtension);
		}

		// Token: 0x06002039 RID: 8249 RVA: 0x00069C1C File Offset: 0x0006801C
		public bool TryGetExtension<T>(out T ext) where T : IRoomExtension
		{
			ext = default(T);
			Type typeFromHandle = typeof(T);
			IRoomExtension roomExtension;
			if (!this.m_extensions.TryGetValue(typeFromHandle, out roomExtension))
			{
				return false;
			}
			ext = (T)((object)roomExtension);
			return true;
		}

		// Token: 0x04000FBB RID: 4027
		private bool m_closed;

		// Token: 0x04000FBC RID: 4028
		private bool m_disposed;

		// Token: 0x04000FBD RID: 4029
		private DoubleBuffer<Type, IRoomState> m_buffer;

		// Token: 0x04000FBE RID: 4030
		private Dictionary<Type, IRoomExtension> m_state_to_extension;

		// Token: 0x04000FBF RID: 4031
		protected Dictionary<Type, IRoomExtension> m_extensions;
	}
}

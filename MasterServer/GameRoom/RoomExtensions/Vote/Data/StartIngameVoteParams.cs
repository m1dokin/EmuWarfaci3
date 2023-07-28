using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Common;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameRoom.RoomExtensions.Vote.Data
{
	// Token: 0x020004CF RID: 1231
	internal class StartIngameVoteParams
	{
		// Token: 0x06001AA7 RID: 6823 RVA: 0x0006D580 File Offset: 0x0006B980
		internal StartIngameVoteParams()
		{
		}

		// Token: 0x06001AA8 RID: 6824 RVA: 0x0006D588 File Offset: 0x0006B988
		internal StartIngameVoteParams(IGameRoomManager manager, XmlElement xml, VoteType type)
		{
			this.m_gameRoomManager = manager;
			this.ParseInitiatorInfo(xml);
			this.ParseVotersInfo(xml);
			if (type == VoteType.KickVote)
			{
				this.ParseTargetInfo(xml);
			}
		}

		// Token: 0x170002AA RID: 682
		// (get) Token: 0x06001AA9 RID: 6825 RVA: 0x0006D5B2 File Offset: 0x0006B9B2
		// (set) Token: 0x06001AAA RID: 6826 RVA: 0x0006D5BA File Offset: 0x0006B9BA
		internal IGameRoom InitiatorRoom { get; private set; }

		// Token: 0x170002AB RID: 683
		// (get) Token: 0x06001AAB RID: 6827 RVA: 0x0006D5C3 File Offset: 0x0006B9C3
		// (set) Token: 0x06001AAC RID: 6828 RVA: 0x0006D5CB File Offset: 0x0006B9CB
		internal RoomPlayer Initiator { get; private set; }

		// Token: 0x170002AC RID: 684
		// (get) Token: 0x06001AAD RID: 6829 RVA: 0x0006D5D4 File Offset: 0x0006B9D4
		// (set) Token: 0x06001AAE RID: 6830 RVA: 0x0006D5DC File Offset: 0x0006B9DC
		internal int InitiatorTeamId { get; private set; }

		// Token: 0x170002AD RID: 685
		// (get) Token: 0x06001AAF RID: 6831 RVA: 0x0006D5E5 File Offset: 0x0006B9E5
		// (set) Token: 0x06001AB0 RID: 6832 RVA: 0x0006D5ED File Offset: 0x0006B9ED
		internal RoomPlayer Target { get; private set; }

		// Token: 0x170002AE RID: 686
		// (get) Token: 0x06001AB1 RID: 6833 RVA: 0x0006D5F6 File Offset: 0x0006B9F6
		// (set) Token: 0x06001AB2 RID: 6834 RVA: 0x0006D5FE File Offset: 0x0006B9FE
		internal IList<ulong> VotersList { get; private set; }

		// Token: 0x06001AB3 RID: 6835 RVA: 0x0006D608 File Offset: 0x0006BA08
		private void ParseInitiatorInfo(XmlElement xml)
		{
			ulong initiatorProfileId = ulong.Parse(xml.GetAttribute("initiator_profile_id"));
			this.InitiatorTeamId = int.Parse(xml.GetAttribute("initiator_team_id"));
			this.InitiatorRoom = this.GetRoomByProfileId(initiatorProfileId);
			this.InitiatorRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				this.Initiator = r.GetPlayer(initiatorProfileId, AccessMode.ReadOnly);
			});
		}

		// Token: 0x06001AB4 RID: 6836 RVA: 0x0006D678 File Offset: 0x0006BA78
		private void ParseTargetInfo(XmlElement xml)
		{
			ulong targetProfileId = ulong.Parse(xml.GetAttribute("target_profile_id"));
			this.InitiatorRoom.transaction(AccessMode.ReadOnly, delegate(IGameRoom r)
			{
				this.Target = r.GetPlayer(targetProfileId, AccessMode.ReadOnly);
			});
		}

		// Token: 0x06001AB5 RID: 6837 RVA: 0x0006D6C0 File Offset: 0x0006BAC0
		private void ParseVotersInfo(XmlElement xml)
		{
			this.VotersList = new List<ulong>();
			IEnumerator enumerator = xml.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					IEnumerator enumerator2 = xmlNode.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator2.MoveNext())
						{
							object obj2 = enumerator2.Current;
							XmlElement xmlElement = (XmlElement)obj2;
							ulong item = ulong.Parse(xmlElement.GetAttribute("profile_id"));
							this.VotersList.Add(item);
						}
					}
					finally
					{
						IDisposable disposable;
						if ((disposable = (enumerator2 as IDisposable)) != null)
						{
							disposable.Dispose();
						}
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
		}

		// Token: 0x06001AB6 RID: 6838 RVA: 0x0006D79C File Offset: 0x0006BB9C
		private IGameRoom GetRoomByProfileId(ulong profileId)
		{
			IGameRoom roomByPlayer = this.m_gameRoomManager.GetRoomByPlayer(profileId);
			if (roomByPlayer == null)
			{
				throw new RoomNotFoundException(string.Format("Can't find room with player with profileId: {0}", profileId));
			}
			return roomByPlayer;
		}

		// Token: 0x04000CBE RID: 3262
		private readonly IGameRoomManager m_gameRoomManager;
	}
}

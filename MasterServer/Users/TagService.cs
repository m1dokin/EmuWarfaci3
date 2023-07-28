using System;
using System.Collections.Generic;
using HK2Net;
using MasterServer.Core.Services;
using MasterServer.CryOnlineNET;

namespace MasterServer.Users
{
	// Token: 0x020006E6 RID: 1766
	[Service]
	[Singleton]
	internal class TagService : ServiceModule, ITagService
	{
		// Token: 0x0600250F RID: 9487 RVA: 0x0009AA28 File Offset: 0x00098E28
		public TagService(IOnlineClient onlineClient, IQueryManager queryManager, IUserRepository userRepository, ITagStorage tagStorage)
		{
			this.m_onlineClient = onlineClient;
			this.m_queryManager = queryManager;
			this.m_userRepository = userRepository;
			this.m_tagStorage = tagStorage;
			this.m_tagsLock = new object();
			this.m_userTags = new Dictionary<ulong, string>();
		}

		// Token: 0x06002510 RID: 9488 RVA: 0x0009AA63 File Offset: 0x00098E63
		public override void Start()
		{
			base.Start();
			this.m_userRepository.UserLoggedOut += this.ClearUserTagsCache;
		}

		// Token: 0x06002511 RID: 9489 RVA: 0x0009AA82 File Offset: 0x00098E82
		public override void Stop()
		{
			this.m_userRepository.UserLoggedOut -= this.ClearUserTagsCache;
			base.Stop();
		}

		// Token: 0x06002512 RID: 9490 RVA: 0x0009AAA4 File Offset: 0x00098EA4
		public UserTags GetUserTags(ulong userId)
		{
			object tagsLock = this.m_tagsLock;
			string persistentUserTags;
			lock (tagsLock)
			{
				if (!this.m_userTags.TryGetValue(userId, out persistentUserTags))
				{
					persistentUserTags = this.m_tagStorage.GetPersistentUserTags(userId);
					this.SetUserTagsLocaly(userId, persistentUserTags);
				}
			}
			return new UserTags(persistentUserTags);
		}

		// Token: 0x06002513 RID: 9491 RVA: 0x0009AB10 File Offset: 0x00098F10
		public void AddUserTags(ulong userId, UserTags tags)
		{
			UserTags userTags = this.GetUserTags(userId);
			userTags.Add(tags);
			this.SetUserTags(userId, userTags);
		}

		// Token: 0x06002514 RID: 9492 RVA: 0x0009AB34 File Offset: 0x00098F34
		public void SetUserTags(ulong userId, UserTags tags)
		{
			this.m_tagStorage.SetPersistentUserTags(userId, tags.ToString());
			this.UpdateAndSyncUserTags(userId, tags);
		}

		// Token: 0x06002515 RID: 9493 RVA: 0x0009AB50 File Offset: 0x00098F50
		public void RemoveUserTags(ulong userId, UserTags tags)
		{
			UserTags userTags = this.GetUserTags(userId);
			userTags.Remove(tags);
			this.SetUserTags(userId, userTags);
		}

		// Token: 0x06002516 RID: 9494 RVA: 0x0009AB74 File Offset: 0x00098F74
		public void RemoveUserTags(ulong userId)
		{
			this.m_tagStorage.RemovePersistentUserTags(userId);
			this.UpdateAndSyncUserTags(userId, new UserTags(null));
		}

		// Token: 0x06002517 RID: 9495 RVA: 0x0009AB90 File Offset: 0x00098F90
		public void SyncUserTags(ulong userId)
		{
			string persistentUserTags = this.m_tagStorage.GetPersistentUserTags(userId);
			this.SetUserTagsLocaly(userId, persistentUserTags);
		}

		// Token: 0x06002518 RID: 9496 RVA: 0x0009ABB2 File Offset: 0x00098FB2
		private void UpdateAndSyncUserTags(ulong userId, UserTags tags)
		{
			this.SetUserTagsLocaly(userId, tags.ToString());
			this.m_queryManager.Request("set_user_tags", this.m_onlineClient.TargetRoute, new object[]
			{
				userId,
				tags
			});
		}

		// Token: 0x06002519 RID: 9497 RVA: 0x0009ABF0 File Offset: 0x00098FF0
		private void SetUserTagsLocaly(ulong userId, string tags)
		{
			if (this.m_userRepository.GetUserByUserId(userId) == null)
			{
				return;
			}
			object tagsLock = this.m_tagsLock;
			lock (tagsLock)
			{
				this.m_userTags[userId] = tags;
			}
		}

		// Token: 0x0600251A RID: 9498 RVA: 0x0009AC4C File Offset: 0x0009904C
		private void ClearUserTagsCache(UserInfo.User user, ELogoutType type)
		{
			object tagsLock = this.m_tagsLock;
			lock (tagsLock)
			{
				this.m_userTags.Remove(user.UserID);
			}
		}

		// Token: 0x040012B3 RID: 4787
		public static readonly UserTags BlockPurchaseTag = new UserTags("block_purchase_tag");

		// Token: 0x040012B4 RID: 4788
		private readonly IOnlineClient m_onlineClient;

		// Token: 0x040012B5 RID: 4789
		private readonly IQueryManager m_queryManager;

		// Token: 0x040012B6 RID: 4790
		private readonly IUserRepository m_userRepository;

		// Token: 0x040012B7 RID: 4791
		private readonly ITagStorage m_tagStorage;

		// Token: 0x040012B8 RID: 4792
		private readonly object m_tagsLock;

		// Token: 0x040012B9 RID: 4793
		private readonly Dictionary<ulong, string> m_userTags;
	}
}

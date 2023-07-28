using System;
using System.Threading.Tasks;
using System.Xml;
using MasterServer.Core;
using MasterServer.Users;
using Ninject;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000170 RID: 368
	internal class BaseQuery : QueryHandler
	{
		// Token: 0x06000689 RID: 1673 RVA: 0x000042F4 File Offset: 0x000026F4
		public override Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request)
		{
			XmlElement response = request.OwnerDocument.CreateElement(base.Tag);
			if (base.IsBlocked)
			{
				TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
				taskCompletionSource.SetResult(-5);
				return taskCompletionSource.Task;
			}
			Task<int> task = this.HandleRequestAsync(query, request, response);
			return task.ContinueWith<int>(delegate(Task<int> t)
			{
				if (t.Result == 0)
				{
					this.Manager.Response(this, query, response);
				}
				return t.Result;
			}, TaskContinuationOptions.ExecuteSynchronously);
		}

		// Token: 0x0600068A RID: 1674 RVA: 0x00004377 File Offset: 0x00002777
		public virtual int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			return this.QueryGetResponse(query.online_id, request, response);
		}

		// Token: 0x0600068B RID: 1675 RVA: 0x00004388 File Offset: 0x00002788
		public virtual Task<int> HandleRequestAsync(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			int result = this.HandleRequest(query, request, response);
			TaskCompletionSource<int> taskCompletionSource = new TaskCompletionSource<int>();
			taskCompletionSource.SetResult(result);
			return taskCompletionSource.Task;
		}

		// Token: 0x0600068C RID: 1676 RVA: 0x000043B2 File Offset: 0x000027B2
		public virtual int QueryGetResponse(string from, XmlElement request, XmlElement response)
		{
			throw new NotImplementedException();
		}

		// Token: 0x0600068D RID: 1677 RVA: 0x000043B9 File Offset: 0x000027B9
		protected bool GetClientInfo(string clientJid, out UserInfo.User user_info)
		{
			user_info = this.UserRepository.GetUserByOnlineId(clientJid);
			return user_info != null;
		}

		// Token: 0x0600068E RID: 1678 RVA: 0x000043D4 File Offset: 0x000027D4
		protected bool GetClientProfileId(string clientJid, out ulong profileId)
		{
			profileId = 0UL;
			UserInfo.User userByOnlineId = this.UserRepository.GetUserByOnlineId(clientJid);
			if (userByOnlineId == null || userByOnlineId.ProfileID == 0UL)
			{
				Log.Error<string>("GetClientProfileId failed. Invalid Jid: {0}", clientJid);
				return false;
			}
			profileId = userByOnlineId.ProfileID;
			return true;
		}

		// Token: 0x0600068F RID: 1679 RVA: 0x0000441B File Offset: 0x0000281B
		protected bool GetServerID(string jid, out string server_id)
		{
			server_id = this.ServerRepository.GetServerID(jid);
			if (string.IsNullOrEmpty(server_id))
			{
				Log.Error<string>("GetServerID failed. Invalid Jid: {0}", jid);
				return false;
			}
			return true;
		}

		// Token: 0x06000690 RID: 1680 RVA: 0x00004448 File Offset: 0x00002848
		protected bool IsQueryFromMs(string from)
		{
			Jid jid = Jid.Parse(from);
			return jid.Client.Equals("masterserver");
		}

		// Token: 0x170000A8 RID: 168
		// (get) Token: 0x06000691 RID: 1681 RVA: 0x0000446C File Offset: 0x0000286C
		// (set) Token: 0x06000692 RID: 1682 RVA: 0x00004474 File Offset: 0x00002874
		[Inject]
		public IServerRepository ServerRepository { get; set; }

		// Token: 0x170000A9 RID: 169
		// (get) Token: 0x06000693 RID: 1683 RVA: 0x0000447D File Offset: 0x0000287D
		// (set) Token: 0x06000694 RID: 1684 RVA: 0x00004485 File Offset: 0x00002885
		[Inject]
		public IUserRepository UserRepository { get; set; }

		// Token: 0x170000AA RID: 170
		// (get) Token: 0x06000695 RID: 1685 RVA: 0x0000448E File Offset: 0x0000288E
		// (set) Token: 0x06000696 RID: 1686 RVA: 0x00004496 File Offset: 0x00002896
		[Inject]
		public IApplicationService ApplicationService { get; set; }
	}
}

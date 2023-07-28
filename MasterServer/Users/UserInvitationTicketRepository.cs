using System;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using Util.Common;

namespace MasterServer.Users
{
	// Token: 0x020006E3 RID: 1763
	[Service]
	internal class UserInvitationTicketRepository : IUserInvitationTicketRepository, IDisposable
	{
		// Token: 0x060024FE RID: 9470 RVA: 0x0009A788 File Offset: 0x00098B88
		public UserInvitationTicketRepository()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Friends");
			TimeSpan cacheTimeout = TimeSpan.FromSeconds((double)int.Parse(section.Get("InvitationExpiration")));
			this.m_repository = new CacheDictionary<string, UserInvitation.Ticket>(cacheTimeout);
			this.m_repository.ItemExpired += this.OnItemExpired;
		}

		// Token: 0x1400009F RID: 159
		// (add) Token: 0x060024FF RID: 9471 RVA: 0x0009A7E8 File Offset: 0x00098BE8
		// (remove) Token: 0x06002500 RID: 9472 RVA: 0x0009A820 File Offset: 0x00098C20
		public event Action<UserInvitation.Ticket> TicketExpired;

		// Token: 0x06002501 RID: 9473 RVA: 0x0009A856 File Offset: 0x00098C56
		public void Dispose()
		{
			this.m_repository.ItemExpired -= this.OnItemExpired;
			this.m_repository.Dispose();
		}

		// Token: 0x06002502 RID: 9474 RVA: 0x0009A87A File Offset: 0x00098C7A
		public bool TryGetValue(string key, out UserInvitation.Ticket ticket)
		{
			return this.m_repository.TryGetValue(key, out ticket);
		}

		// Token: 0x06002503 RID: 9475 RVA: 0x0009A889 File Offset: 0x00098C89
		public void Add(string key, UserInvitation.Ticket ticket)
		{
			this.m_repository.Add(key, ticket);
		}

		// Token: 0x06002504 RID: 9476 RVA: 0x0009A899 File Offset: 0x00098C99
		public void Remove(string key)
		{
			this.m_repository.Remove(key);
		}

		// Token: 0x06002505 RID: 9477 RVA: 0x0009A8A8 File Offset: 0x00098CA8
		private void OnItemExpired(string key, UserInvitation.Ticket ticket)
		{
			this.TicketExpired.SafeInvokeEach(ticket);
		}

		// Token: 0x040012B0 RID: 4784
		private readonly CacheDictionary<string, UserInvitation.Ticket> m_repository;
	}
}

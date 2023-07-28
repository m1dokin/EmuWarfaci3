using System;
using System.Xml;
using MasterServer.GameRoomSystem;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001A2 RID: 418
	internal abstract class SessionQuery : BaseQuery
	{
		// Token: 0x060007D0 RID: 2000 RVA: 0x0001DEEB File Offset: 0x0001C2EB
		protected SessionQuery(ISessionStorage sessionStorage)
		{
			this.m_sessionStorage = sessionStorage;
		}

		// Token: 0x060007D1 RID: 2001 RVA: 0x0001DEFC File Offset: 0x0001C2FC
		public sealed override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			string attribute = request.GetAttribute("session_id");
			if (!this.ValidateSession(fromJid, attribute))
			{
				return -1;
			}
			return this.QueryGetResponseImpl(fromJid, request, response);
		}

		// Token: 0x060007D2 RID: 2002
		protected abstract int QueryGetResponseImpl(string fromJid, XmlElement request, XmlElement response);

		// Token: 0x060007D3 RID: 2003 RVA: 0x0001DF30 File Offset: 0x0001C330
		private bool ValidateSession(string jid, string sessionId)
		{
			string text;
			return this.m_sessionStorage.ValidateSession(jid, sessionId) && base.GetServerID(jid, out text);
		}

		// Token: 0x04000498 RID: 1176
		private readonly ISessionStorage m_sessionStorage;
	}
}

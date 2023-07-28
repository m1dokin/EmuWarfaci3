using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.CryOnlineNET;
using MasterServer.Users;

namespace MasterServer.GameLogic.RatingSystem.Queries
{
	// Token: 0x020000BF RID: 191
	[QueryAttributes(TagName = "rating_player_ban")]
	internal class RatingPlayerBanQuery : BaseQuery
	{
		// Token: 0x0600030B RID: 779 RVA: 0x0000E7F7 File Offset: 0x0000CBF7
		public RatingPlayerBanQuery(IRatingGameBanService ratingGameBanService, IServerRepository serverRepository)
		{
			this.m_ratingGameBanService = ratingGameBanService;
			this.m_serverRepository = serverRepository;
		}

		// Token: 0x0600030C RID: 780 RVA: 0x0000E810 File Offset: 0x0000CC10
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			if (!this.IsSenderValid(fromJid))
			{
				return -1;
			}
			List<ulong> list = new List<ulong>();
			IEnumerator enumerator = request.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					ulong item = ulong.Parse(xmlElement.GetAttribute("profile_id"));
					list.Add(item);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			this.m_ratingGameBanService.BanRatingGameForPlayers(list);
			return 0;
		}

		// Token: 0x0600030D RID: 781 RVA: 0x0000E8AC File Offset: 0x0000CCAC
		private bool IsSenderValid(string jid)
		{
			string serverID = this.m_serverRepository.GetServerID(jid);
			return !string.IsNullOrEmpty(serverID);
		}

		// Token: 0x0400014B RID: 331
		internal const string QueryName = "rating_player_ban";

		// Token: 0x0400014C RID: 332
		private readonly IRatingGameBanService m_ratingGameBanService;

		// Token: 0x0400014D RID: 333
		private readonly IServerRepository m_serverRepository;
	}
}

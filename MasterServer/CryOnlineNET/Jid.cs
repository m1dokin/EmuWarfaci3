using System;
using System.Text.RegularExpressions;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x02000175 RID: 373
	public class Jid
	{
		// Token: 0x060006B5 RID: 1717 RVA: 0x0001AAB0 File Offset: 0x00018EB0
		public Jid(string client, string host, string resource)
		{
			this.Client = client;
			this.Host = host;
			this.Resource = resource;
		}

		// Token: 0x060006B6 RID: 1718 RVA: 0x0001AAD0 File Offset: 0x00018ED0
		public static bool TryParse(string jidStr, out Jid jid)
		{
			Match match = Jid.m_parseJidRegExp.Match(jidStr);
			if (match.Success && match.Groups.Count == 4)
			{
				jid = new Jid(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
				return true;
			}
			jid = null;
			return false;
		}

		// Token: 0x060006B7 RID: 1719 RVA: 0x0001AB48 File Offset: 0x00018F48
		public static Jid Parse(string jid)
		{
			Match match = Jid.m_parseJidRegExp.Match(jid);
			if (match.Success && match.Groups.Count == 4)
			{
				return new Jid(match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
			}
			throw new FormatException(string.Format("Can't parse {0} as a jid", jid));
		}

		// Token: 0x060006B8 RID: 1720 RVA: 0x0001ABC6 File Offset: 0x00018FC6
		public override string ToString()
		{
			return string.Format("{0}@{1}/{2}", this.Client, this.Host, this.Resource);
		}

		// Token: 0x170000B3 RID: 179
		// (get) Token: 0x060006B9 RID: 1721 RVA: 0x0001ABE4 File Offset: 0x00018FE4
		// (set) Token: 0x060006BA RID: 1722 RVA: 0x0001ABEC File Offset: 0x00018FEC
		public string Client { get; private set; }

		// Token: 0x170000B4 RID: 180
		// (get) Token: 0x060006BB RID: 1723 RVA: 0x0001ABF5 File Offset: 0x00018FF5
		// (set) Token: 0x060006BC RID: 1724 RVA: 0x0001ABFD File Offset: 0x00018FFD
		public string Host { get; private set; }

		// Token: 0x170000B5 RID: 181
		// (get) Token: 0x060006BD RID: 1725 RVA: 0x0001AC06 File Offset: 0x00019006
		// (set) Token: 0x060006BE RID: 1726 RVA: 0x0001AC0E File Offset: 0x0001900E
		public string Resource { get; private set; }

		// Token: 0x04000417 RID: 1047
		private static readonly Regex m_parseJidRegExp = new Regex("(.*)@(.*)\\/(.*)", RegexOptions.Compiled);
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MasterServer.Users
{
	// Token: 0x0200074C RID: 1868
	internal class ClientVersionValidator : IClientVersionValidator
	{
		// Token: 0x06002688 RID: 9864 RVA: 0x000A35BC File Offset: 0x000A19BC
		private ClientVersionValidator(IEnumerable<Regex> versionsList)
		{
			this.m_versionsList = versionsList.ToList<Regex>();
		}

		// Token: 0x06002689 RID: 9865 RVA: 0x000A35D0 File Offset: 0x000A19D0
		public static IClientVersionValidator CreateInstance(IEnumerable<Regex> set)
		{
			return new ClientVersionValidator(set);
		}

		// Token: 0x0600268A RID: 9866 RVA: 0x000A35D8 File Offset: 0x000A19D8
		public bool Validate(ClientVersion version)
		{
			if (version == ClientVersion.Unknown)
			{
				return true;
			}
			if (version == ClientVersion.Empty)
			{
				return false;
			}
			string versionString = version.ToString();
			return this.m_versionsList.Any((Regex regex) => regex.IsMatch(versionString));
		}

		// Token: 0x040013D5 RID: 5077
		private readonly List<Regex> m_versionsList;
	}
}

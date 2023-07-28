using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;

namespace MasterServer.Users
{
	// Token: 0x02000749 RID: 1865
	[Service]
	[Singleton]
	internal class ClientVersionsConfigProvider : IClientVersionsConfigProvider
	{
		// Token: 0x0600267B RID: 9851 RVA: 0x000A2F0C File Offset: 0x000A130C
		public IEnumerable<string> GetInitialVersionSet()
		{
			yield return string.Format("^{0}$", Resources.MasterVersion);
			if (!Resources.DebugQueriesEnabled || !File.Exists(Resources.DevClientVersionsFile))
			{
				yield break;
			}
			foreach (Regex regex in ClientVersionsConfigProvider.GetVersionsFromFile(Resources.DevClientVersionsFile))
			{
				yield return regex.ToString();
			}
			yield break;
		}

		// Token: 0x0600267C RID: 9852 RVA: 0x000A2F28 File Offset: 0x000A1328
		public IEnumerable<Regex> GetSupportedVersions()
		{
			return ClientVersionsConfigProvider.GetSupportedVersionsInternal().Distinct(Utils.RegexStringComparer.Instance);
		}

		// Token: 0x0600267D RID: 9853 RVA: 0x000A2F3C File Offset: 0x000A133C
		private static IEnumerable<Regex> GetSupportedVersionsInternal()
		{
			yield return new Regex(string.Format("^{0}$", Resources.MasterVersion));
			IEnumerable<string> supportedClientVersionsFiles = Resources.SupportedClientVersionsFiles;
			if (ClientVersionsConfigProvider.<>f__mg$cache0 == null)
			{
				ClientVersionsConfigProvider.<>f__mg$cache0 = new Func<string, bool>(File.Exists);
			}
			List<string> clientVersionsFilePaths = supportedClientVersionsFiles.Where(ClientVersionsConfigProvider.<>f__mg$cache0).ToList<string>();
			if (!clientVersionsFilePaths.Any<string>())
			{
				yield break;
			}
			IEnumerable<string> source = clientVersionsFilePaths;
			if (ClientVersionsConfigProvider.<>f__mg$cache1 == null)
			{
				ClientVersionsConfigProvider.<>f__mg$cache1 = new Func<string, IEnumerable<Regex>>(ClientVersionsConfigProvider.GetVersionsFromFile);
			}
			foreach (Regex regex in source.SelectMany(ClientVersionsConfigProvider.<>f__mg$cache1))
			{
				yield return regex;
			}
			yield break;
		}

		// Token: 0x0600267E RID: 9854 RVA: 0x000A2F58 File Offset: 0x000A1358
		private static IEnumerable<Regex> GetVersionsFromFile(string path)
		{
			using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				using (StreamReader reader = new StreamReader(fileStream))
				{
					while (reader.Peek() >= 0)
					{
						string line = reader.ReadLine();
						if (string.IsNullOrEmpty(line))
						{
							yield break;
						}
						Regex regex = null;
						try
						{
							regex = new Regex(line);
						}
						catch (ArgumentException e)
						{
							Log.Error(e);
						}
						if (regex != null)
						{
							yield return regex;
						}
					}
				}
			}
			yield break;
		}

		// Token: 0x040013D3 RID: 5075
		[CompilerGenerated]
		private static Func<string, bool> <>f__mg$cache0;

		// Token: 0x040013D4 RID: 5076
		[CompilerGenerated]
		private static Func<string, IEnumerable<Regex>> <>f__mg$cache1;
	}
}

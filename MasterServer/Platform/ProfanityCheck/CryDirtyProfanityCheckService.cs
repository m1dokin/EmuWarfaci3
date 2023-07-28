using System;
using System.IO;
using System.Text;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006B0 RID: 1712
	[Service]
	[Singleton]
	internal class CryDirtyProfanityCheckService : ProfanityCheckService
	{
		// Token: 0x060023F4 RID: 9204 RVA: 0x00096C76 File Offset: 0x00095076
		public CryDirtyProfanityCheckService(IProfanityMetricsTracker profanityMetricsTracker, ICryDirty cryDirty) : base(profanityMetricsTracker)
		{
			this.m_cryDirty = cryDirty;
		}

		// Token: 0x060023F5 RID: 9205 RVA: 0x00096C88 File Offset: 0x00095088
		protected override void ReadConfigImpl(ConfigSection configSection)
		{
			string text = configSection.Get("profane_dict");
			string text2 = configSection.Get("reserved_dict");
			this.m_profanityDictFile = ((!Path.IsPathRooted(text)) ? Resources.GetResourceFullPath(text) : text);
			this.m_reservedDictFile = ((!Path.IsPathRooted(text2)) ? Resources.GetResourceFullPath(text2) : text2);
		}

		// Token: 0x060023F6 RID: 9206 RVA: 0x00096CE8 File Offset: 0x000950E8
		public override void Start()
		{
			base.Start();
			if (this.m_enabled)
			{
				this.m_cryDirty.Init();
				if (!this.m_cryDirty.ReadProfanityDict(this.m_profanityDictFile))
				{
					throw new Exception(string.Format("Failed to read dictionary: {0}", this.m_profanityDictFile));
				}
				if (!this.m_cryDirty.ReadReservedDict(this.m_reservedDictFile))
				{
					throw new Exception(string.Format("Failed to read dictionary: {0}", this.m_reservedDictFile));
				}
			}
		}

		// Token: 0x060023F7 RID: 9207 RVA: 0x00096D69 File Offset: 0x00095169
		public override void Stop()
		{
			this.m_cryDirty.Free();
			base.Stop();
		}

		// Token: 0x060023F8 RID: 9208 RVA: 0x00096D7C File Offset: 0x0009517C
		protected override ProfanityCheckResult CheckImpl(ProfanityCheckService.CheckType checkType, ulong userId, string userNickname, string str)
		{
			if (checkType == ProfanityCheckService.CheckType.ClanDescription)
			{
				throw new InvalidOperationException("Profanity checking is not defined for clan description. Filtering should be used instead.");
			}
			return (!this.m_cryDirty.HaveReservedWords(str)) ? ((!this.m_cryDirty.HaveProfanityWords(str)) ? ProfanityCheckResult.Succeeded : ProfanityCheckResult.Failed) : ProfanityCheckResult.Reserved;
		}

		// Token: 0x060023F9 RID: 9209 RVA: 0x00096DCC File Offset: 0x000951CC
		protected override ProfanityCheckResult FilterImpl(ProfanityCheckService.CheckType checkType, ulong userId, StringBuilder builder)
		{
			return (!this.m_cryDirty.FilterMessage(builder)) ? ProfanityCheckResult.Succeeded : ProfanityCheckResult.Failed;
		}

		// Token: 0x04001207 RID: 4615
		private readonly ICryDirty m_cryDirty;

		// Token: 0x04001208 RID: 4616
		private string m_profanityDictFile;

		// Token: 0x04001209 RID: 4617
		private string m_reservedDictFile;
	}
}

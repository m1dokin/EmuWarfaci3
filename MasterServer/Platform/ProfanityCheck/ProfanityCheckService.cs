using System;
using System.Diagnostics;
using System.Text;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Telemetry.Metrics;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006AB RID: 1707
	internal abstract class ProfanityCheckService : ServiceModule, IProfanityCheckService
	{
		// Token: 0x060023DD RID: 9181 RVA: 0x00095EF9 File Offset: 0x000942F9
		protected ProfanityCheckService(IProfanityMetricsTracker profanityMetricsTracker)
		{
			this.m_profanityMetricsTracker = profanityMetricsTracker;
		}

		// Token: 0x060023DE RID: 9182 RVA: 0x00095F10 File Offset: 0x00094310
		public override void Init()
		{
			base.Init();
			ConfigSection section = Resources.CommonSettings.GetSection("profanity_checker");
			section.OnConfigChanged += this.OnConfigChanged;
			this.ReadConfig(section);
		}

		// Token: 0x060023DF RID: 9183 RVA: 0x00095F4C File Offset: 0x0009434C
		public override void Stop()
		{
			ConfigSection section = Resources.CommonSettings.GetSection("profanity_checker");
			section.OnConfigChanged -= this.OnConfigChanged;
			base.Stop();
		}

		// Token: 0x060023E0 RID: 9184 RVA: 0x00095F81 File Offset: 0x00094381
		private void OnConfigChanged(ConfigEventArgs e)
		{
			this.ReadConfig(Resources.CommonSettings.GetSection("profanity_checker"));
		}

		// Token: 0x060023E1 RID: 9185 RVA: 0x00095F98 File Offset: 0x00094398
		private void ReadConfig(ConfigSection configSection)
		{
			configSection.Get("enable", out this.m_enabled);
			this.ReadConfigImpl(configSection);
		}

		// Token: 0x060023E2 RID: 9186
		protected abstract void ReadConfigImpl(ConfigSection configSection);

		// Token: 0x060023E3 RID: 9187
		protected abstract ProfanityCheckResult CheckImpl(ProfanityCheckService.CheckType checkType, ulong userId, string userNickname, string str);

		// Token: 0x060023E4 RID: 9188
		protected abstract ProfanityCheckResult FilterImpl(ProfanityCheckService.CheckType checkType, ulong userId, StringBuilder builder);

		// Token: 0x060023E5 RID: 9189 RVA: 0x00095FB2 File Offset: 0x000943B2
		public ProfanityCheckResult CheckProfileName(ulong userId, string profileName)
		{
			return this.Check(ProfanityCheckService.CheckType.ProfileName, userId, profileName);
		}

		// Token: 0x060023E6 RID: 9190 RVA: 0x00095FBD File Offset: 0x000943BD
		public ProfanityCheckResult CheckClanName(ulong userId, string clanName)
		{
			return this.Check(ProfanityCheckService.CheckType.ClanName, userId, clanName);
		}

		// Token: 0x060023E7 RID: 9191 RVA: 0x00095FC8 File Offset: 0x000943C8
		public ProfanityCheckResult FilterClanDescription(ulong userId, StringBuilder clanDescription)
		{
			return this.Filter(ProfanityCheckService.CheckType.ClanDescription, userId, clanDescription);
		}

		// Token: 0x060023E8 RID: 9192 RVA: 0x00095FD3 File Offset: 0x000943D3
		public ProfanityCheckResult CheckRoomName(ulong userId, string userNickname, string roomName)
		{
			return this.Check(ProfanityCheckService.CheckType.RoomName, userId, userNickname, roomName);
		}

		// Token: 0x060023E9 RID: 9193 RVA: 0x00095FDF File Offset: 0x000943DF
		private ProfanityCheckResult Check(ProfanityCheckService.CheckType checkType, ulong userId, string str)
		{
			return this.Check(checkType, userId, string.Empty, str);
		}

		// Token: 0x060023EA RID: 9194 RVA: 0x00095FF0 File Offset: 0x000943F0
		private ProfanityCheckResult Check(ProfanityCheckService.CheckType checkType, ulong userId, string userNickname, string str)
		{
			if (!this.m_enabled)
			{
				return ProfanityCheckResult.Succeeded;
			}
			this.m_profanityMetricsTracker.ReportProfanityRequest(checkType);
			Stopwatch stopwatch = Stopwatch.StartNew();
			ProfanityCheckResult result;
			try
			{
				ProfanityCheckResult profanityCheckResult = this.CheckImpl(checkType, userId, userNickname, str);
				stopwatch.Stop();
				Log.Info<ProfanityCheckService.CheckType, TimeSpan>("[Profanity: check request for {0} took {1}]", checkType, stopwatch.Elapsed);
				this.m_profanityMetricsTracker.ReportProfanityResult(checkType, profanityCheckResult);
				result = profanityCheckResult;
			}
			catch (Exception e)
			{
				stopwatch.Stop();
				this.m_profanityMetricsTracker.ReportProfanityRequestFailed(checkType);
				Log.Info<ProfanityCheckService.CheckType, TimeSpan>("[Profanity: check request for {0} failed in {1}]", checkType, stopwatch.Elapsed);
				Log.Error("Can't check profanity for CheckType:{0} UserId:{1} UserNick:{2} Str:{3}", new object[]
				{
					checkType,
					userId,
					userNickname,
					str
				});
				Log.Error(e);
				result = ProfanityCheckResult.Failed;
			}
			finally
			{
				this.m_profanityMetricsTracker.ReportProfanityRequestTime(checkType, stopwatch.Elapsed);
			}
			return result;
		}

		// Token: 0x060023EB RID: 9195 RVA: 0x000960E0 File Offset: 0x000944E0
		private ProfanityCheckResult Filter(ProfanityCheckService.CheckType checkType, ulong userId, StringBuilder builder)
		{
			if (!this.m_enabled)
			{
				return ProfanityCheckResult.Succeeded;
			}
			this.m_profanityMetricsTracker.ReportProfanityRequest(checkType);
			Stopwatch stopwatch = Stopwatch.StartNew();
			ProfanityCheckResult result;
			try
			{
				ProfanityCheckResult profanityCheckResult = this.FilterImpl(checkType, userId, builder);
				stopwatch.Stop();
				Log.Info<ProfanityCheckService.CheckType, TimeSpan>("[Profanity: filter request for {0} took {1}]", checkType, stopwatch.Elapsed);
				this.m_profanityMetricsTracker.ReportProfanityResult(checkType, profanityCheckResult);
				result = profanityCheckResult;
			}
			catch (Exception e)
			{
				stopwatch.Stop();
				this.m_profanityMetricsTracker.ReportProfanityRequestFailed(checkType);
				Log.Info<ProfanityCheckService.CheckType, TimeSpan>("[Profanity: filter request for {0} failed in {1}]", checkType, stopwatch.Elapsed);
				Log.Error<ProfanityCheckService.CheckType, ulong, string>("Can't perform filtering for CheckType:{0} UserId:{1} Str:{2}", checkType, userId, builder.ToString());
				Log.Error(e);
				result = ProfanityCheckResult.Failed;
			}
			finally
			{
				this.m_profanityMetricsTracker.ReportProfanityRequestTime(checkType, stopwatch.Elapsed);
			}
			return result;
		}

		// Token: 0x040011FA RID: 4602
		private readonly IProfanityMetricsTracker m_profanityMetricsTracker;

		// Token: 0x040011FB RID: 4603
		protected bool m_enabled = true;

		// Token: 0x020006AC RID: 1708
		public enum CheckType
		{
			// Token: 0x040011FD RID: 4605
			ProfileName = 1,
			// Token: 0x040011FE RID: 4606
			ClanName,
			// Token: 0x040011FF RID: 4607
			RoomName,
			// Token: 0x04001200 RID: 4608
			ClanDescription
		}
	}
}

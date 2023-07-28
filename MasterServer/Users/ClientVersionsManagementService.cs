using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Jobs;
using Util.Common;

namespace MasterServer.Users
{
	// Token: 0x0200074D RID: 1869
	[Service]
	[Singleton]
	internal class ClientVersionsManagementService : ServiceModule, IClientVersionsManagementService
	{
		// Token: 0x0600268B RID: 9867 RVA: 0x000A3650 File Offset: 0x000A1A50
		public ClientVersionsManagementService(IClientVersionsConfigProvider clientVersionsConfigProvider, IJobSchedulerService jobSchedulerService, IRegexClientVersionsStorage clientVersionsStorage)
		{
			this.m_clientVersionsConfigProvider = clientVersionsConfigProvider;
			this.m_jobSchedulerService = jobSchedulerService;
			this.m_clientVersionsStorage = clientVersionsStorage;
		}

		// Token: 0x140000A6 RID: 166
		// (add) Token: 0x0600268C RID: 9868 RVA: 0x000A36A0 File Offset: 0x000A1AA0
		// (remove) Token: 0x0600268D RID: 9869 RVA: 0x000A36D8 File Offset: 0x000A1AD8
		public event Action ClientVersionsChanged;

		// Token: 0x0600268E RID: 9870 RVA: 0x000A3710 File Offset: 0x000A1B10
		public override void Start()
		{
			Log.Info<string>("[ClientVersionsManagementService] Starting on {0} MS", (!ClientVersionsManagementService.IsUpdater) ? "regular" : "DAL");
			if (ClientVersionsManagementService.IsUpdater && !this.m_clientVersionsStorage.IsVersionsSetUpToDate())
			{
				Log.Info("[ClientVersionsManagementService] Existing versions set is outdated. Initial versions set will be loaded");
				this.SetClientVersions(this.m_clientVersionsConfigProvider.GetInitialVersionSet().ToArray<string>());
			}
			else
			{
				this.SyncClientVersions();
			}
			this.m_jobSchedulerService.AddJob("supported_client_versions_sync");
		}

		// Token: 0x170003A1 RID: 929
		// (get) Token: 0x0600268F RID: 9871 RVA: 0x000A3795 File Offset: 0x000A1B95
		private static bool IsUpdater
		{
			get
			{
				return Resources.RealmDBUpdaterPermission && !Resources.IsDevMode;
			}
		}

		// Token: 0x06002690 RID: 9872 RVA: 0x000A37AC File Offset: 0x000A1BAC
		public void SetClientVersions(params string[] versionsToSet)
		{
			IList<Regex> list = this.FilterSupportedVersionsOnly(versionsToSet);
			if (list.Any<Regex>())
			{
				this.UpdateClientVersions(list, (IEnumerable<Regex> _, IEnumerable<Regex> @new) => @new);
			}
		}

		// Token: 0x06002691 RID: 9873 RVA: 0x000A37F0 File Offset: 0x000A1BF0
		public void AddClientVersions(params string[] versionsToAdd)
		{
			IList<Regex> list = this.FilterSupportedVersionsOnly(versionsToAdd);
			if (list.Any<Regex>())
			{
				this.UpdateClientVersions(list, (IEnumerable<Regex> old, IEnumerable<Regex> @new) => old.Union(@new, Utils.RegexStringComparer.Instance));
			}
		}

		// Token: 0x06002692 RID: 9874 RVA: 0x000A3834 File Offset: 0x000A1C34
		public void RemoveClientVersions(params string[] versionsToRemove)
		{
			this.UpdateClientVersions(versionsToRemove.SafeSelect((string v) => new Regex(v)), (IEnumerable<Regex> old, IEnumerable<Regex> @new) => old.Except(@new, Utils.RegexStringComparer.Instance));
		}

		// Token: 0x06002693 RID: 9875 RVA: 0x000A3888 File Offset: 0x000A1C88
		public IEnumerable<string> GetClientVersions()
		{
			object @lock = this.m_lock;
			IEnumerable<string> result;
			lock (@lock)
			{
				List<string> list = (from r in this.m_supportedClientVersions
				select r.ToString()).ToList<string>();
				result = list;
			}
			return result;
		}

		// Token: 0x06002694 RID: 9876 RVA: 0x000A38F8 File Offset: 0x000A1CF8
		public void SyncClientVersions()
		{
			object @lock = this.m_lock;
			bool flag2;
			lock (@lock)
			{
				List<Regex> newVersions = this.m_clientVersionsStorage.GetVersions().ToList<Regex>();
				flag2 = this.OverwriteLocalVersionsCache(newVersions);
			}
			if (flag2)
			{
				this.ClientVersionsChanged.SafeInvokeEach();
			}
		}

		// Token: 0x06002695 RID: 9877 RVA: 0x000A3960 File Offset: 0x000A1D60
		public bool Validate(ClientVersion version)
		{
			object @lock = this.m_lock;
			IClientVersionValidator currentValidator;
			lock (@lock)
			{
				currentValidator = this.m_currentValidator;
			}
			return currentValidator.Validate(version);
		}

		// Token: 0x06002696 RID: 9878 RVA: 0x000A39B0 File Offset: 0x000A1DB0
		private IList<Regex> FilterSupportedVersionsOnly(IEnumerable<string> versions)
		{
			List<Regex> second = this.m_clientVersionsConfigProvider.GetSupportedVersions().ToList<Regex>();
			return (from v in versions
			select new Regex(v)).Intersect(second, Utils.RegexStringComparer.Instance).ToList<Regex>();
		}

		// Token: 0x06002697 RID: 9879 RVA: 0x000A3A04 File Offset: 0x000A1E04
		private void UpdateClientVersions(IEnumerable<Regex> newVersions, Func<IEnumerable<Regex>, IEnumerable<Regex>, IEnumerable<Regex>> mergeFunction)
		{
			object @lock = this.m_lock;
			bool flag2;
			lock (@lock)
			{
				List<Regex> arg = this.m_clientVersionsStorage.GetVersions().ToList<Regex>();
				List<Regex> list = mergeFunction(arg, newVersions).ToList<Regex>();
				flag2 = this.OverwriteLocalVersionsCache(list);
				this.m_clientVersionsStorage.StoreVersions(list);
			}
			if (flag2)
			{
				this.ClientVersionsChanged.SafeInvokeEach();
			}
		}

		// Token: 0x06002698 RID: 9880 RVA: 0x000A3A88 File Offset: 0x000A1E88
		private bool OverwriteLocalVersionsCache(IEnumerable<Regex> newVersions)
		{
			HashSet<Regex> hashSet = new HashSet<Regex>(newVersions, Utils.RegexStringComparer.Instance);
			bool flag = !this.m_supportedClientVersions.SetEquals(hashSet);
			this.m_supportedClientVersions = hashSet;
			if (flag)
			{
				this.m_currentValidator = ClientVersionValidator.CreateInstance(this.m_supportedClientVersions);
			}
			return flag;
		}

		// Token: 0x040013D6 RID: 5078
		private readonly IClientVersionsConfigProvider m_clientVersionsConfigProvider;

		// Token: 0x040013D7 RID: 5079
		private readonly IJobSchedulerService m_jobSchedulerService;

		// Token: 0x040013D8 RID: 5080
		private readonly IRegexClientVersionsStorage m_clientVersionsStorage;

		// Token: 0x040013D9 RID: 5081
		private readonly object m_lock = new object();

		// Token: 0x040013DA RID: 5082
		private HashSet<Regex> m_supportedClientVersions = new HashSet<Regex>();

		// Token: 0x040013DB RID: 5083
		private volatile IClientVersionValidator m_currentValidator = ClientVersionValidator.CreateInstance(Enumerable.Empty<Regex>());
	}
}

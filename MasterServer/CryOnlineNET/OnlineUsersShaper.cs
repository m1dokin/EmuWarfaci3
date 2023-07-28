using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Users;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001AA RID: 426
	internal class OnlineUsersShaper : IQoSShaper
	{
		// Token: 0x060007F3 RID: 2035 RVA: 0x0001E4D4 File Offset: 0x0001C8D4
		public OnlineUsersShaper()
		{
			ConfigSection section = Resources.QoSSettings.GetSection("online_users_shaper");
			this.m_login_queries = this.ParseQuerySet(section.Get("login_queries"));
			this.m_switch_queries = this.ParseQuerySet(section.Get("switch_queries"));
			section.OnConfigChanged += this.OnShaperConfigChanged;
		}

		// Token: 0x170000DC RID: 220
		// (get) Token: 0x060007F4 RID: 2036 RVA: 0x0001E537 File Offset: 0x0001C937
		private IUserRepository UserRepository
		{
			get
			{
				if (this.m_userRepository == null)
				{
					this.m_userRepository = ServicesManager.GetService<IUserRepository>();
				}
				return this.m_userRepository;
			}
		}

		// Token: 0x060007F5 RID: 2037 RVA: 0x0001E558 File Offset: 0x0001C958
		public ShaperDecision IncomingWorkItem(WorkItem item)
		{
			int num = -1;
			if (this.m_login_queries.Contains(item.shaping_info.query_name))
			{
				num = this.UserRepository.GetJoinedUsersLimit();
			}
			else if (this.m_switch_queries.Contains(item.shaping_info.query_name))
			{
				num = this.UserRepository.GetOnlineUsersLimit();
			}
			if (num == -1)
			{
				return ShaperDecision.Execute;
			}
			int onlineUsersCount = this.UserRepository.GetOnlineUsersCount();
			if (this.m_logins_in_progress + onlineUsersCount < num)
			{
				this.m_logins_in_progress++;
				return ShaperDecision.Execute;
			}
			Log.Warning<int>("[OnlineUsersShaper] QoS queue limit {0} of concurrent logins reached", this.m_logins_in_progress);
			return ShaperDecision.Discard;
		}

		// Token: 0x060007F6 RID: 2038 RVA: 0x0001E600 File Offset: 0x0001CA00
		public void WorkItemFinished(WorkItem finished)
		{
			if (this.m_login_queries.Contains(finished.shaping_info.query_name) || this.m_switch_queries.Contains(finished.shaping_info.query_name))
			{
				this.m_logins_in_progress = Math.Max(0, this.m_logins_in_progress - 1);
			}
		}

		// Token: 0x060007F7 RID: 2039 RVA: 0x0001E657 File Offset: 0x0001CA57
		public WorkItem DequeueWorkItem(WorkItem finished)
		{
			return null;
		}

		// Token: 0x060007F8 RID: 2040 RVA: 0x0001E65A File Offset: 0x0001CA5A
		public void FillMemoryUsageInfo(StringBuilder stringBuidler)
		{
		}

		// Token: 0x060007F9 RID: 2041 RVA: 0x0001E65C File Offset: 0x0001CA5C
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("[OnlineUsersShaper]: Logins in progress {0}", this.m_logins_in_progress));
			stringBuilder.AppendLine(string.Format("\tLogins:", new object[0]));
			foreach (string arg in ((IEnumerable<string>)this.m_login_queries))
			{
				stringBuilder.AppendLine(string.Format("\t\t{0}", arg));
			}
			stringBuilder.AppendLine(string.Format("\tSwitches:", new object[0]));
			foreach (string arg2 in ((IEnumerable<string>)this.m_switch_queries))
			{
				stringBuilder.AppendLine(string.Format("\t\t{0}", arg2));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x060007FA RID: 2042 RVA: 0x0001E770 File Offset: 0x0001CB70
		private Set<string> ParseQuerySet(string queries)
		{
			string[] source = queries.Split(new char[]
			{
				','
			}, StringSplitOptions.RemoveEmptyEntries);
			return new Set<string>(from q in source
			select q.Trim());
		}

		// Token: 0x060007FB RID: 2043 RVA: 0x0001E7B8 File Offset: 0x0001CBB8
		private void OnShaperConfigChanged(ConfigEventArgs args)
		{
			if (string.Compare(args.Name, "login_queries", true) == 0)
			{
				this.m_login_queries = this.ParseQuerySet(args.sValue);
				this.m_logins_in_progress = 0;
			}
			else if (string.Compare(args.Name, "switch_queries", true) == 0)
			{
				this.m_switch_queries = this.ParseQuerySet(args.sValue);
				this.m_logins_in_progress = 0;
			}
		}

		// Token: 0x060007FC RID: 2044 RVA: 0x0001E828 File Offset: 0x0001CC28
		public static int GetLoginsInProgress(IQoSQueue qosQueue)
		{
			OnlineUsersShaper onlineUsersShaper = qosQueue.GetShapers().OfType<OnlineUsersShaper>().First<OnlineUsersShaper>();
			return onlineUsersShaper.m_logins_in_progress;
		}

		// Token: 0x040004AA RID: 1194
		private int m_logins_in_progress;

		// Token: 0x040004AB RID: 1195
		private Set<string> m_login_queries;

		// Token: 0x040004AC RID: 1196
		private Set<string> m_switch_queries;

		// Token: 0x040004AD RID: 1197
		private IUserRepository m_userRepository;
	}
}

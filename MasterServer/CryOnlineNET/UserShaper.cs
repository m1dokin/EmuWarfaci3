using System;
using System.Collections.Generic;
using System.Text;
using MasterServer.Core;

namespace MasterServer.CryOnlineNET
{
	// Token: 0x020001AE RID: 430
	internal class UserShaper : IQoSShaper
	{
		// Token: 0x06000807 RID: 2055 RVA: 0x0001ECBB File Offset: 0x0001D0BB
		public UserShaper()
		{
			Resources.QoSSettings.GetSection("user_shaper").Get("max_user_queue", out this.max_queue_length);
		}

		// Token: 0x06000808 RID: 2056 RVA: 0x0001ECF8 File Offset: 0x0001D0F8
		public ShaperDecision IncomingWorkItem(WorkItem item)
		{
			UserShaper.UserData userData;
			if (!this.m_user_queues.TryGetValue(item.shaping_info.from_jid, out userData))
			{
				if (this.m_free_data.Count > 0)
				{
					userData = this.m_free_data.Pop();
				}
				else
				{
					userData = new UserShaper.UserData();
				}
				this.m_user_queues.Add(item.shaping_info.from_jid, userData);
			}
			if (!userData.task_in_progress)
			{
				userData.task_in_progress = true;
				return ShaperDecision.Execute;
			}
			if (this.max_queue_length == 0 || userData.tasks_queue.Count < this.max_queue_length)
			{
				userData.tasks_queue.Enqueue(item);
				return ShaperDecision.Queued;
			}
			Log.Warning<string, int>("User '{0}' QoS queue limit of {1} reached", item.shaping_info.from_jid, this.max_queue_length);
			return ShaperDecision.Discard;
		}

		// Token: 0x06000809 RID: 2057 RVA: 0x0001EDC0 File Offset: 0x0001D1C0
		public void WorkItemFinished(WorkItem finished)
		{
			UserShaper.UserData userData = this.m_user_queues[finished.shaping_info.from_jid];
			userData.task_in_progress = false;
		}

		// Token: 0x0600080A RID: 2058 RVA: 0x0001EDEC File Offset: 0x0001D1EC
		public WorkItem DequeueWorkItem(WorkItem finished)
		{
			UserShaper.UserData userData = this.m_user_queues[finished.shaping_info.from_jid];
			if (userData.tasks_queue.Count > 0)
			{
				userData.task_in_progress = true;
				return userData.tasks_queue.Dequeue();
			}
			this.m_user_queues.Remove(finished.shaping_info.from_jid);
			this.m_free_data.Push(userData);
			return null;
		}

		// Token: 0x0600080B RID: 2059 RVA: 0x0001EE58 File Offset: 0x0001D258
		public void FillMemoryUsageInfo(StringBuilder stringBuidler)
		{
			stringBuidler.AppendFormat("QoS User Queue: {0}\n", this.m_user_queues.Count);
			foreach (UserShaper.UserData userData in this.m_user_queues.Values)
			{
				stringBuidler.AppendFormat("\tQueue size: {0}\n", userData.tasks_queue.Count);
			}
		}

		// Token: 0x0600080C RID: 2060 RVA: 0x0001EEEC File Offset: 0x0001D2EC
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("[UserShaper]:");
			foreach (KeyValuePair<string, UserShaper.UserData> keyValuePair in this.m_user_queues)
			{
				stringBuilder.AppendLine(string.Format("\t{0}: task in progress {1}", keyValuePair.Key, keyValuePair.Value.task_in_progress));
				foreach (WorkItem arg in keyValuePair.Value.tasks_queue)
				{
					stringBuilder.AppendLine(string.Format("\t\t{0}:", arg));
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040004B5 RID: 1205
		private int max_queue_length;

		// Token: 0x040004B6 RID: 1206
		private Stack<UserShaper.UserData> m_free_data = new Stack<UserShaper.UserData>();

		// Token: 0x040004B7 RID: 1207
		private Dictionary<string, UserShaper.UserData> m_user_queues = new Dictionary<string, UserShaper.UserData>();

		// Token: 0x020001AF RID: 431
		private class UserData
		{
			// Token: 0x040004B8 RID: 1208
			public bool task_in_progress;

			// Token: 0x040004B9 RID: 1209
			public Queue<WorkItem> tasks_queue = new Queue<WorkItem>();
		}
	}
}

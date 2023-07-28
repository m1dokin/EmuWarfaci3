using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RewardSystem;
using Util.Common;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x0200025E RID: 606
	[QueryAttributes(TagName = "update_achievements")]
	internal class UpdateAchievementsQuery : BaseQuery
	{
		// Token: 0x06000D53 RID: 3411 RVA: 0x000349A4 File Offset: 0x00032DA4
		public UpdateAchievementsQuery(IAchievementSystem achievementSystem, INotificationService notificationService, IRewardService rewardService)
		{
			this.m_achievementSystem = achievementSystem;
			this.m_notificationService = notificationService;
			this.m_rewardService = rewardService;
		}

		// Token: 0x06000D54 RID: 3412 RVA: 0x000349C4 File Offset: 0x00032DC4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "UpdateAchievementsQuery"))
			{
				string text;
				if (!base.GetServerID(fromJid, out text))
				{
					result = -3;
				}
				else
				{
					Dictionary<ulong, List<AchievementUpdateChunk>> dictionary = this.ParseInputData(request);
					foreach (KeyValuePair<ulong, List<AchievementUpdateChunk>> keyValuePair in dictionary)
					{
						ulong key = keyValuePair.Key;
						List<AchievementUpdateChunk> value = keyValuePair.Value;
						this.ProcessAchievements(this.m_achievementSystem, key, value);
						this.m_notificationService.AddNotifications<AchievementUpdateChunk>(key, ENotificationType.Achievement, value, this.m_rewardService.AwardExpirationTime, EDeliveryType.SendNowOrLater, EConfirmationType.None);
					}
					result = 0;
				}
			}
			return result;
		}

		// Token: 0x06000D55 RID: 3413 RVA: 0x00034AA4 File Offset: 0x00032EA4
		private void ProcessAchievements(IAchievementSystem achievementSystem, ulong profileID, List<AchievementUpdateChunk> updates)
		{
			List<AchievementUpdateChunk> newUpdates = new List<AchievementUpdateChunk>();
			updates.SafeForEachEx(delegate(AchievementUpdateChunk update)
			{
				AchievementDescription achievementDesc = achievementSystem.GetAchievementDesc(update.achievementId);
				if (achievementDesc == null)
				{
					throw new KeyNotFoundException(string.Format("Achievement with id {0} not found", update.achievementId));
				}
				AchievementUpdateChunk item = new AchievementUpdateChunk(achievementDesc.Id, update.progress, update.completionTime);
				if (achievementSystem.SetAchievementProgress(profileID, achievementDesc, ref item))
				{
					newUpdates.Add(item);
				}
			});
			updates.Clear();
			updates.InsertRange(0, newUpdates);
		}

		// Token: 0x06000D56 RID: 3414 RVA: 0x00034AF8 File Offset: 0x00032EF8
		private Dictionary<ulong, List<AchievementUpdateChunk>> ParseInputData(XmlElement parentNode)
		{
			Dictionary<ulong, List<AchievementUpdateChunk>> dictionary = new Dictionary<ulong, List<AchievementUpdateChunk>>();
			IEnumerator enumerator = parentNode.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != "achievement"))
					{
						XmlElement xmlElement = xmlNode as XmlElement;
						ulong key = ulong.Parse(xmlElement.GetAttribute("profile_id"));
						List<AchievementUpdateChunk> list = new List<AchievementUpdateChunk>();
						dictionary.Add(key, list);
						IEnumerator enumerator2 = xmlElement.ChildNodes.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlNode xmlNode2 = (XmlNode)obj2;
								if (xmlNode2.NodeType == XmlNodeType.Element && !(xmlNode2.Name != "chunk"))
								{
									XmlElement xmlElement2 = xmlNode2 as XmlElement;
									uint aid = uint.Parse(xmlElement2.GetAttribute("achievement_id"));
									int val = int.Parse(xmlElement2.GetAttribute("progress"));
									ulong completionTime = ulong.Parse(xmlElement2.GetAttribute("completion_time"));
									list.Add(new AchievementUpdateChunk(aid, Math.Max(0, val), completionTime));
								}
							}
						}
						finally
						{
							IDisposable disposable;
							if ((disposable = (enumerator2 as IDisposable)) != null)
							{
								disposable.Dispose();
							}
						}
					}
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			return dictionary;
		}

		// Token: 0x04000621 RID: 1569
		private const string QueryName = "update_achievements";

		// Token: 0x04000622 RID: 1570
		private readonly IAchievementSystem m_achievementSystem;

		// Token: 0x04000623 RID: 1571
		private readonly INotificationService m_notificationService;

		// Token: 0x04000624 RID: 1572
		private readonly IRewardService m_rewardService;
	}
}

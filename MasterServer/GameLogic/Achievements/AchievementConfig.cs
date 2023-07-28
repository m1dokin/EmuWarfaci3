using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using MasterServer.Core;
using MasterServer.GameLogic.RewardSystem;
using MasterServer.GameLogic.StatsTracking;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x02000254 RID: 596
	public class AchievementConfig
	{
		// Token: 0x06000D28 RID: 3368 RVA: 0x000339F4 File Offset: 0x00031DF4
		public void ReadData(XmlDocument doc)
		{
			object achievementDesc = this.m_achievementDesc;
			lock (achievementDesc)
			{
				this.m_achievementDesc.Clear();
				XmlNodeList elementsByTagName = doc.GetElementsByTagName("achievement");
				IEnumerator enumerator = elementsByTagName.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlNode xmlNode = (XmlNode)obj;
						uint num = uint.Parse(xmlNode.Attributes["id"].Value);
						uint amount = uint.Parse(xmlNode.Attributes["amount"].Value);
						bool flag2 = uint.Parse(xmlNode.Attributes["MS_side"].Value) != 0U;
						string value = xmlNode.Attributes["name"].Value;
						EStatsEvent achKind = EStatsEvent.NON_MS_EVENT;
						if (flag2)
						{
							string value2 = xmlNode.Attributes["kind"].Value;
							switch (value2)
							{
							case "manual":
								achKind = EStatsEvent.MANUALLY_TRIGGERED;
								break;
							case "session_end":
								achKind = EStatsEvent.SESSION_END;
								break;
							case "rank":
								achKind = EStatsEvent.RANK_CHANGED;
								break;
							case "crown_collected":
								achKind = EStatsEvent.CROWN_COLLECTED;
								break;
							case "money":
								achKind = EStatsEvent.MONEY_AWARDED;
								break;
							case "add_friend":
								achKind = EStatsEvent.ADD_FRIEND;
								break;
							case "sponsor_progress":
								achKind = EStatsEvent.SPONSOR_PROGRESS;
								break;
							case "leader_board_update":
								achKind = EStatsEvent.LEADER_BOARD_UPDATE;
								break;
							case "hidden":
								achKind = EStatsEvent.HIDDEN;
								break;
							}
						}
						AchievementDescription achievementDescription = new AchievementDescription(num, achKind, amount, flag2, value);
						XmlNodeList childNodes = xmlNode.ChildNodes;
						IEnumerator enumerator2 = childNodes.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								object obj2 = enumerator2.Current;
								XmlNode xmlNode2 = (XmlNode)obj2;
								if (xmlNode2.NodeType == XmlNodeType.Element)
								{
									XmlElement xmlElement = xmlNode2 as XmlElement;
									if (flag2 && xmlElement.Name == "Filter")
									{
										IStatsFilter statsFilter = null;
										string attribute = xmlElement.GetAttribute("kind");
										if (attribute == null)
										{
											goto IL_30D;
										}
										if (!(attribute == "result"))
										{
											if (!(attribute == "leaderboard_position"))
											{
												goto IL_30D;
											}
											uint position = uint.Parse(xmlElement.GetAttribute("param"));
											statsFilter = new LeaderBoardPositionFilter(position);
										}
										else
										{
											string attribute2 = xmlElement.GetAttribute("param");
											statsFilter = new SessionOutcomeFilter((!(attribute2 == "win")) ? ((!(attribute2 == "lose")) ? SessionOutcome.Draw : SessionOutcome.Lost) : SessionOutcome.Won);
										}
										IL_32C:
										if (statsFilter != null)
										{
											achievementDescription.Filters.Add(statsFilter);
											continue;
										}
										continue;
										IL_30D:
										Log.Warning<string, uint, EStatsEvent>("XML file achievements_data.xml contains attribute {0} in filter section that can't be processed for achievment: ID {1}, kind {2} ", attribute, achievementDescription.Id, achievementDescription.Kind);
										goto IL_32C;
									}
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
						try
						{
							this.m_achievementDesc.Add(num, achievementDescription);
						}
						catch (ArgumentException innerException)
						{
							throw new ArgumentException(string.Format("Achievement with key {0} already exists in the dictionary", num), innerException);
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
			}
		}

		// Token: 0x06000D29 RID: 3369 RVA: 0x00033E34 File Offset: 0x00032234
		public AchievementDescription GetAchievementDesc(uint id)
		{
			object achievementDesc = this.m_achievementDesc;
			lock (achievementDesc)
			{
				if (this.m_achievementDesc.ContainsKey(id))
				{
					return this.m_achievementDesc[id];
				}
			}
			return null;
		}

		// Token: 0x06000D2A RID: 3370 RVA: 0x00033E98 File Offset: 0x00032298
		public Dictionary<uint, AchievementDescription> GetAllAchievementDescs()
		{
			object achievementDesc = this.m_achievementDesc;
			Dictionary<uint, AchievementDescription> result;
			lock (achievementDesc)
			{
				result = new Dictionary<uint, AchievementDescription>(this.m_achievementDesc);
			}
			return result;
		}

		// Token: 0x04000608 RID: 1544
		private readonly Dictionary<uint, AchievementDescription> m_achievementDesc = new Dictionary<uint, AchievementDescription>();
	}
}

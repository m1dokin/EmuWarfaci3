using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using MasterServer.Core;
using Util.Common;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x0200062D RID: 1581
	internal class SessionsSummaryXMLSerializer
	{
		// Token: 0x060021F5 RID: 8693 RVA: 0x0008CB34 File Offset: 0x0008AF34
		public static XmlDocument Serialize(SessionSummary summary)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.AppendChild(xmlDocument.CreateXmlDeclaration("1.0", "utf-8", null));
			XmlElement xmlElement = xmlDocument.CreateElement("Sessions");
			xmlDocument.AppendChild(xmlElement);
			XmlElement xmlElement2 = xmlDocument.CreateElement("Session");
			xmlElement.AppendChild(xmlElement2);
			XmlElement xmlElement3 = xmlDocument.CreateElement("SessionInfo");
			xmlElement2.AppendChild(xmlElement3);
			XmlElement xmlElement4 = xmlDocument.CreateElement("Map");
			xmlElement2.AppendChild(xmlElement4);
			XmlElement xmlElement5 = xmlDocument.CreateElement("Players");
			xmlElement2.AppendChild(xmlElement5);
			XmlElement xmlElement6 = xmlDocument.CreateElement("Observers");
			xmlElement2.AppendChild(xmlElement6);
			XmlElement xmlElement7 = xmlDocument.CreateElement("Rounds");
			xmlElement2.AppendChild(xmlElement7);
			xmlElement2.SetAttribute("version", 2.ToString());
			xmlElement2.SetAttribute("host", summary.Host);
			xmlElement2.SetAttribute("masterserver", summary.MasterServer);
			xmlElement3.SetAttribute("session_log", summary.SessionLog);
			xmlElement3.SetAttribute("start_time", TimeUtils.FormatISO8601(summary.StartTime));
			xmlElement3.SetAttribute("end_time", TimeUtils.FormatISO8601(summary.EndTime));
			xmlElement3.SetAttribute("duration", summary.SessionTime.ToString());
			xmlElement3.SetAttribute("mode", summary.Mode);
			xmlElement3.SetAttribute("submode", summary.SubMode);
			xmlElement3.SetAttribute("clanwar", (!summary.ClanWar) ? "0" : "1");
			xmlElement3.SetAttribute("difficulty", summary.Mission.difficulty);
			xmlElement3.SetAttribute("mission_type", summary.Mission.missionType.Name);
			xmlElement4.SetAttribute("uid", summary.Mission.uid);
			xmlElement4.SetAttribute("name", summary.Mission.name);
			xmlElement4.SetAttribute("path", summary.Mission.baseLevel.name);
			xmlElement7.SetAttribute("outcome", summary.EndOutcome);
			xmlElement7.SetAttribute("outcome_reason", summary.EndReason);
			xmlElement7.SetAttribute("overall_winning_team", summary.EndWinningTeam);
			foreach (SessionSummary.PlayerData playerData in summary.Players.Values)
			{
				XmlElement xmlElement8 = xmlDocument.CreateElement("Player");
				xmlElement5.AppendChild(xmlElement8);
				SessionsSummaryXMLSerializer.SerializePlayerData(xmlDocument, xmlElement8, playerData);
			}
			foreach (SessionSummary.ObserverData observerData in summary.Observers.Values)
			{
				XmlElement xmlElement9 = xmlDocument.CreateElement("Observer");
				xmlElement6.AppendChild(xmlElement9);
				SessionsSummaryXMLSerializer.SerializeObserverData(xmlDocument, xmlElement9, observerData);
			}
			foreach (SessionSummary.RoundData roundData in summary.Rounds)
			{
				XmlElement xmlElement10 = xmlDocument.CreateElement("Round");
				xmlElement7.AppendChild(xmlElement10);
				SessionsSummaryXMLSerializer.SerializeRoundData(xmlDocument, xmlElement10, roundData);
			}
			if (summary.Leaderboard.Count != 0)
			{
				XmlElement xmlElement11 = xmlDocument.CreateElement("Leaderboard");
				xmlElement2.AppendChild(xmlElement11);
				foreach (KeyValuePair<string, string> keyValuePair in summary.Leaderboard)
				{
					xmlElement11.SetAttribute(keyValuePair.Key, keyValuePair.Value);
				}
			}
			return xmlDocument;
		}

		// Token: 0x060021F6 RID: 8694 RVA: 0x0008CF48 File Offset: 0x0008B348
		private static void SerializePlayerData(XmlDocument doc, XmlElement player_el, SessionSummary.PlayerData playerData)
		{
			player_el.SetAttribute("profile_id", playerData.ProfileId.ToString());
			player_el.SetAttribute("nickname", playerData.Nickname);
			player_el.SetAttribute("rank", playerData.Rank.ToString(CultureInfo.InvariantCulture));
			player_el.SetAttribute("overall_result", playerData.OverallResult.ToString().ToLower());
			player_el.SetAttribute("first_win", playerData.FirstWin.ToString().ToLower());
			XmlElement xmlElement = doc.CreateElement("skill");
			xmlElement.SetAttribute("type", playerData.Skill.Type.ToString());
			xmlElement.SetAttribute("value", playerData.Skill.Value.ToString(CultureInfo.InvariantCulture));
			player_el.AppendChild(xmlElement);
			XmlElement xmlElement2 = doc.CreateElement("Playtimes");
			player_el.AppendChild(xmlElement2);
			foreach (SessionSummary.PlayerPlaytime playerPlaytime in playerData.Playtimes)
			{
				XmlElement xmlElement3 = doc.CreateElement("Playtime");
				xmlElement2.AppendChild(xmlElement3);
				xmlElement3.SetAttribute("team", playerPlaytime.TeamId.ToString());
				xmlElement3.SetAttribute("class", playerPlaytime.Class);
				xmlElement3.InnerText = playerPlaytime.Playtime.ToString();
			}
			if (playerData.Stats.Count > 0)
			{
				XmlElement xmlElement4 = doc.CreateElement("Stats");
				player_el.AppendChild(xmlElement4);
				foreach (KeyValuePair<string, string> keyValuePair in playerData.Stats)
				{
					XmlElement xmlElement5 = doc.CreateElement("Stat");
					xmlElement4.AppendChild(xmlElement5);
					xmlElement5.SetAttribute("name", keyValuePair.Key);
					xmlElement5.InnerText = keyValuePair.Value;
				}
			}
			if (playerData.Rewards.Count > 0)
			{
				XmlElement xmlElement6 = doc.CreateElement("Rewards");
				player_el.AppendChild(xmlElement6);
				foreach (SessionSummary.PlayerReward playerReward in playerData.Rewards)
				{
					XmlElement xmlElement7 = doc.CreateElement("Reward");
					xmlElement6.AppendChild(xmlElement7);
					xmlElement7.SetAttribute("name", playerReward.Name);
					foreach (KeyValuePair<string, string> keyValuePair2 in playerReward.Attrs)
					{
						xmlElement7.SetAttribute(keyValuePair2.Key, keyValuePair2.Value);
					}
					xmlElement7.InnerText = playerReward.Value;
				}
			}
		}

		// Token: 0x060021F7 RID: 8695 RVA: 0x0008D2A4 File Offset: 0x0008B6A4
		private static void SerializeObserverData(XmlDocument doc, XmlElement observer_el, SessionSummary.ObserverData observerData)
		{
			observer_el.SetAttribute("profile_id", observerData.ProfileId.ToString());
			observer_el.SetAttribute("nickname", observerData.Nickname);
			observer_el.SetAttribute("rank", observerData.Rank.ToString(CultureInfo.InvariantCulture));
			observer_el.SetAttribute("playtime", observerData.Playtime.ToString(CultureInfo.InvariantCulture));
			if (observerData.Stats.Count > 0)
			{
				XmlElement xmlElement = doc.CreateElement("Stats");
				observer_el.AppendChild(xmlElement);
				foreach (KeyValuePair<string, string> keyValuePair in observerData.Stats)
				{
					XmlElement xmlElement2 = doc.CreateElement("Stat");
					xmlElement.AppendChild(xmlElement2);
					xmlElement2.SetAttribute("name", keyValuePair.Key);
					xmlElement2.InnerText = keyValuePair.Value;
				}
			}
		}

		// Token: 0x060021F8 RID: 8696 RVA: 0x0008D3BC File Offset: 0x0008B7BC
		private static void SerializeRoundData(XmlDocument doc, XmlElement round_el, SessionSummary.RoundData roundData)
		{
			round_el.SetAttribute("id", roundData.Id.ToString());
			round_el.SetAttribute("begin", TimeUtils.FormatISO8601(roundData.BeginTime));
			round_el.SetAttribute("end", TimeUtils.FormatISO8601(roundData.EndTime));
			round_el.SetAttribute("duration", roundData.Duration.ToString());
			round_el.SetAttribute("winning_team", roundData.WinningTeamId.ToString());
		}

		// Token: 0x060021F9 RID: 8697 RVA: 0x0008D450 File Offset: 0x0008B850
		public static void AppendSessionLog(string logFile, SessionSummary data)
		{
			XmlDocument xmlDocument = SessionsSummaryXMLSerializer.Serialize(data);
			Directory.CreateDirectory(Resources.SessionSummariesDirectory);
			string path = Path.Combine(Resources.SessionSummariesDirectory, logFile);
			if (!File.Exists(path))
			{
				using (FileStream fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
				{
					XmlWriterSettings settings = new XmlWriterSettings
					{
						Indent = true
					};
					using (XmlWriter xmlWriter = XmlWriter.Create(fileStream, settings))
					{
						xmlDocument.WriteTo(xmlWriter);
					}
				}
			}
			else
			{
				XmlWriterSettings settings2 = new XmlWriterSettings
				{
					Indent = true,
					NewLineChars = "\n  ",
					ConformanceLevel = ConformanceLevel.Fragment
				};
				using (FileStream fileStream2 = new FileStream(path, FileMode.Open, FileAccess.ReadWrite, FileShare.None))
				{
					int length = "</Sessions>".Length;
					fileStream2.SetLength(fileStream2.Length - (long)length);
					fileStream2.Seek(0L, SeekOrigin.End);
					using (StreamWriter streamWriter = new StreamWriter(fileStream2))
					{
						streamWriter.Write("  ");
						XmlNode xmlNode = xmlDocument.SelectSingleNode("Sessions");
						using (XmlWriter xmlWriter2 = XmlWriter.Create(streamWriter, settings2))
						{
							xmlNode.WriteContentTo(xmlWriter2);
						}
						streamWriter.Write("\n</Sessions>");
					}
				}
			}
		}

		// Token: 0x060021FA RID: 8698 RVA: 0x0008D5F8 File Offset: 0x0008B9F8
		public static void MoveOldLogsToHistory(string currentLog)
		{
			foreach (string text in Directory.GetFiles(Resources.SessionSummariesDirectory))
			{
				string fileName = Path.GetFileName(text);
				if (string.Compare(currentLog, fileName) != 0)
				{
					try
					{
						Directory.CreateDirectory(Resources.SessionSummariesHistoryDirectory);
						string destFileName = Path.Combine(Resources.SessionSummariesHistoryDirectory, fileName);
						File.Move(text, destFileName);
					}
					catch (Exception e)
					{
						Log.Warning<string>("Failed to move session file {0} to history", fileName);
						Log.Warning(e);
					}
				}
			}
		}
	}
}

using System;
using System.Collections.Generic;
using System.Xml;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C4 RID: 964
	internal class MissionSerializer
	{
		// Token: 0x06001545 RID: 5445 RVA: 0x00058B2C File Offset: 0x00056F2C
		public string Serialize(MissionGenerator.Mission mission)
		{
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = xmlDocument.CreateElement("mission");
			xmlDocument.AppendChild(xmlElement);
			this.Serialize(mission, xmlElement);
			return xmlElement.OuterXml;
		}

		// Token: 0x06001546 RID: 5446 RVA: 0x00058B64 File Offset: 0x00056F64
		public void Serialize(MissionGenerator.Mission mission, XmlElement mission_elem)
		{
			mission_elem.SetAttribute("name", mission.Name);
			mission_elem.SetAttribute("setting", mission.Setting);
			mission_elem.SetAttribute("uid", mission.Uid.ToString());
			mission_elem.SetAttribute("difficulty", mission.Difficulty);
			mission_elem.SetAttribute("mission_type", mission.MissionType);
			mission_elem.SetAttribute("game_mode", mission.GameMode);
			mission_elem.SetAttribute("level_graph", mission.LevelGraph);
			mission_elem.SetAttribute("game_mode_cfg", string.Format("{0}_mode.cfg", mission.GameMode));
			mission_elem.SetAttribute("time_of_day", mission.TimeOfDay);
			mission_elem.SetAttribute("release_mission", (!mission.ReleaseMission) ? "0" : "1");
			mission_elem.SetAttribute("version", mission.Version.ToString());
			XmlElement xmlElement = mission_elem.OwnerDocument.CreateElement("UI");
			mission_elem.AppendChild(xmlElement);
			XmlElement xmlElement2 = mission_elem.OwnerDocument.CreateElement("Description");
			xmlElement.AppendChild(xmlElement2);
			xmlElement2.SetAttribute("text", mission.UI_Info.DescriptionText);
			xmlElement2.SetAttribute("icon", mission.UI_Info.DescriptionIcon);
			XmlElement xmlElement3 = mission_elem.OwnerDocument.CreateElement("GameMode");
			xmlElement.AppendChild(xmlElement3);
			xmlElement3.SetAttribute("text", mission.UI_Info.GameModeText);
			xmlElement3.SetAttribute("icon", mission.UI_Info.GameModeIcon);
			XmlElement xmlElement4 = mission_elem.OwnerDocument.CreateElement("Basemap");
			mission_elem.AppendChild(xmlElement4);
			xmlElement4.SetAttribute("name", mission.BaseLevel.Setting);
			XmlElement xmlElement5 = mission_elem.OwnerDocument.CreateElement("Sublevels");
			mission_elem.AppendChild(xmlElement5);
			if (mission.SubLevels.Count == 0 && mission.BaseLevelParams != null)
			{
				xmlElement5.SetAttribute("mission_flow", mission.BaseLevelParams.MissionFlow);
				xmlElement5.SetAttribute("score_pool", mission.BaseLevelParams.ScorePool.ToString());
				xmlElement5.SetAttribute("win_pool", mission.BaseLevelParams.WinPool.ToString());
				xmlElement5.SetAttribute("lose_pool", mission.BaseLevelParams.LosePool.ToString());
				xmlElement5.SetAttribute("draw_pool", mission.BaseLevelParams.DrawPool.ToString());
			}
			for (int num = 0; num != mission.SubLevels.Count; num++)
			{
				SubMissionConfig.ParameterSet parameterSet = mission.SubLevels[num];
				XmlElement xmlElement6 = xmlElement5.OwnerDocument.CreateElement("Sublevel");
				xmlElement5.AppendChild(xmlElement6);
				xmlElement6.SetAttribute("id", num.ToString());
				xmlElement6.SetAttribute("name", parameterSet.SubMission.LevelName);
				xmlElement6.SetAttribute("mission_flow", parameterSet.MissionFlow);
				xmlElement6.SetAttribute("score", parameterSet.Score.ToString());
				xmlElement6.SetAttribute("difficulty", parameterSet.Difficulty);
				if (string.IsNullOrEmpty(parameterSet.DifficultyCfg))
				{
					xmlElement6.SetAttribute("difficulty_cfg", string.Format("diff_{0}.cfg", parameterSet.Difficulty));
				}
				else
				{
					xmlElement6.SetAttribute("difficulty_cfg", parameterSet.DifficultyCfg);
				}
				xmlElement6.SetAttribute("score_pool", parameterSet.ScorePool.ToString());
				xmlElement6.SetAttribute("win_pool", parameterSet.WinPool.ToString());
				xmlElement6.SetAttribute("lose_pool", parameterSet.LosePool.ToString());
				xmlElement6.SetAttribute("draw_pool", parameterSet.DrawPool.ToString());
				if (parameterSet.CrownThresholds != null)
				{
					XmlElement xmlElement7 = xmlElement5.OwnerDocument.CreateElement("CrownRewardsThresholds");
					xmlElement6.AppendChild(xmlElement7);
					foreach (string text in parameterSet.CrownThresholds.Keys)
					{
						XmlElement xmlElement8 = xmlElement7.OwnerDocument.CreateElement(text);
						xmlElement7.AppendChild(xmlElement8);
						xmlElement8.SetAttribute("bronze", parameterSet.CrownThresholds[text].Bronze.ToString());
						xmlElement8.SetAttribute("silver", parameterSet.CrownThresholds[text].Silver.ToString());
						xmlElement8.SetAttribute("gold", parameterSet.CrownThresholds[text].Gold.ToString());
					}
				}
				if (parameterSet.PoolRewards != null)
				{
					XmlElement xmlElement9 = xmlElement5.OwnerDocument.CreateElement("RewardPools");
					xmlElement6.AppendChild(xmlElement9);
					foreach (KeyValuePair<string, string> keyValuePair in parameterSet.PoolRewards)
					{
						XmlElement xmlElement10 = xmlElement9.OwnerDocument.CreateElement("Pool");
						xmlElement10.SetAttribute("name", keyValuePair.Key);
						xmlElement10.SetAttribute("value", keyValuePair.Value);
						xmlElement9.AppendChild(xmlElement10);
					}
				}
			}
			XmlElement xmlElement11 = mission_elem.OwnerDocument.CreateElement("Teleports");
			mission_elem.AppendChild(xmlElement11);
			for (int num2 = 0; num2 != mission.Teleports.Count; num2++)
			{
				MissionGenerator.Teleport teleport = mission.Teleports[num2];
				XmlElement xmlElement12 = xmlElement11.OwnerDocument.CreateElement("Teleport");
				xmlElement11.AppendChild(xmlElement12);
				xmlElement12.SetAttribute("start_sublevel_id", num2.ToString());
				xmlElement12.SetAttribute("start_teleport", teleport.Start);
				xmlElement12.SetAttribute("finish_sublevel_id", (num2 + 1).ToString());
				xmlElement12.SetAttribute("finish_teleport", teleport.Finish);
			}
			XmlElement xmlElement13 = mission_elem.OwnerDocument.CreateElement("Objectives");
			mission_elem.AppendChild(xmlElement13);
			for (int num3 = 0; num3 != mission.m_objectivesList.Count; num3++)
			{
				xmlElement13.AppendChild(mission.m_objectivesList[num3].GetXml(xmlElement13.OwnerDocument));
			}
		}

		// Token: 0x06001547 RID: 5447 RVA: 0x00059278 File Offset: 0x00057678
		private SubMissionConfig.ParameterSet find_sublevel(XmlElement sl, IMissionSystem ms)
		{
			string name = sl.GetAttribute("name");
			string flow = sl.GetAttribute("mission_flow");
			string diff = sl.GetAttribute("difficulty");
			int score = int.Parse(sl.GetAttribute("score"));
			int sp = int.Parse(sl.GetAttribute("score_pool"));
			int wp = int.Parse(sl.GetAttribute("win_pool"));
			int lp = int.Parse(sl.GetAttribute("lose_pool"));
			int dp = int.Parse(sl.GetAttribute("draw_pool"));
			List<SubMissionConfig.ParameterSet> list = ms.SubMissionConfigRepository.MatchSubMissionParams((SubMissionConfig.ParameterSet PS) => string.Compare(PS.SubMission.LevelName, name, true) == 0 && string.Compare(PS.MissionFlow, flow, true) == 0 && string.Compare(PS.Difficulty, diff, true) == 0 && PS.Score == score && PS.ScorePool == sp && PS.WinPool == wp && PS.LosePool == lp && PS.DrawPool == dp);
			return (list.Count != 1) ? null : list[0];
		}
	}
}

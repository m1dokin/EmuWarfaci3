using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C3 RID: 963
	internal class MissionGraphRepository
	{
		// Token: 0x06001540 RID: 5440 RVA: 0x0005873C File Offset: 0x00056B3C
		public MissionGraphRepository()
		{
			this.m_missionGraphDirectory = Path.Combine(Resources.GetResourcesDirectory(), "libs/missiongraphs");
			string[] files = Directory.GetFiles(this.m_missionGraphDirectory, "*.xml");
			foreach (string path in files)
			{
				try
				{
					using (FileStream fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
					{
						MissionGraph missionGraph = this.LoadMissionGraph(fileStream);
						this.m_graphs.Add(missionGraph.Name, missionGraph);
					}
				}
				catch
				{
					Log.Warning<string>("Error while loading mission graph {0}, skipping", Path.GetFileName(path));
				}
			}
		}

		// Token: 0x06001541 RID: 5441 RVA: 0x00058810 File Offset: 0x00056C10
		public MissionGraph GetGraph(string name)
		{
			name = name.ToLower();
			MissionGraph missionGraph;
			return (!this.m_graphs.TryGetValue(name, out missionGraph)) ? null : missionGraph;
		}

		// Token: 0x06001542 RID: 5442 RVA: 0x0005883F File Offset: 0x00056C3F
		public List<MissionGraph> GetGraphs()
		{
			return new List<MissionGraph>(this.m_graphs.Values);
		}

		// Token: 0x06001543 RID: 5443 RVA: 0x00058854 File Offset: 0x00056C54
		private MissionGraph LoadMissionGraph(Stream s)
		{
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.Load(s);
			XmlElement documentElement = xmlDocument.DocumentElement;
			MissionGraph missionGraph = new MissionGraph();
			missionGraph.Name = documentElement.GetAttribute("name").ToLower();
			missionGraph.SettingRestriction = ((!documentElement.HasAttribute("setting")) ? string.Empty : documentElement.GetAttribute("setting").ToLower());
			missionGraph.GameMode = documentElement.GetAttribute("game_mode");
			missionGraph.Difficulty = documentElement.GetAttribute("target_difficulty");
			missionGraph.SecondaryObjectives = int.Parse(documentElement.GetAttribute("secondary_objectives"));
			List<MissionGraph.SubMissionPattern> list = new List<MissionGraph.SubMissionPattern>();
			IEnumerator enumerator = documentElement.GetElementsByTagName("SubMission").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					XmlElement xmlElement = xmlNode as XmlElement;
					list.Add(new MissionGraph.SubMissionPattern(xmlElement.GetAttribute("kind"), xmlElement.GetAttribute("difficulty"), xmlElement.GetAttribute("name"), xmlElement.GetAttribute("mission_flow")));
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			IEnumerator enumerator2 = documentElement.GetElementsByTagName("BaseMission").GetEnumerator();
			try
			{
				while (enumerator2.MoveNext())
				{
					object obj2 = enumerator2.Current;
					XmlNode xmlNode2 = (XmlNode)obj2;
					XmlElement xmlElement2 = xmlNode2 as XmlElement;
					missionGraph.BaseMissionPattern = new MissionGraph.SubMissionPattern(xmlElement2.GetAttribute("kind"), string.Empty, string.Empty, string.Empty);
				}
			}
			finally
			{
				IDisposable disposable2;
				if ((disposable2 = (enumerator2 as IDisposable)) != null)
				{
					disposable2.Dispose();
				}
			}
			missionGraph.SubMissionPatterns = list.ToArray();
			XmlElement xmlElement3 = documentElement.SelectSingleNode("UI/Description") as XmlElement;
			XmlElement xmlElement4 = documentElement.SelectSingleNode("UI/GameMode") as XmlElement;
			missionGraph.UI_Info.DisplayName = ((!documentElement.HasAttribute("display_name")) ? string.Empty : documentElement.GetAttribute("display_name").ToLower().Trim());
			if (string.IsNullOrEmpty(missionGraph.UI_Info.DisplayName))
			{
				Log.Warning<string>("Level graph '{0}' have no display name", missionGraph.Name);
			}
			if (xmlElement3 != null)
			{
				missionGraph.UI_Info.DescriptionText = xmlElement3.GetAttribute("text");
				missionGraph.UI_Info.DescriptionIcon = xmlElement3.GetAttribute("icon");
			}
			if (xmlElement4 != null)
			{
				missionGraph.UI_Info.GameModeText = xmlElement4.GetAttribute("text");
				missionGraph.UI_Info.GameModeIcon = xmlElement4.GetAttribute("icon");
			}
			return missionGraph;
		}

		// Token: 0x04000A25 RID: 2597
		private string m_missionGraphDirectory;

		// Token: 0x04000A26 RID: 2598
		private Dictionary<string, MissionGraph> m_graphs = new Dictionary<string, MissionGraph>();
	}
}

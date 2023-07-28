using System;
using System.Collections;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.Achievements
{
	// Token: 0x0200025D RID: 605
	[QueryAttributes(TagName = "get_achievements")]
	internal class GetAchievementsQuery : BaseQuery
	{
		// Token: 0x06000D52 RID: 3410 RVA: 0x000347C4 File Offset: 0x00032BC4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "GetAchievementsQuery"))
			{
				IDALService service = ServicesManager.GetService<IDALService>();
				IEnumerator enumerator = request.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement = (XmlElement)obj;
						if (!(xmlElement.Name != "achievement"))
						{
							ulong profileId = ulong.Parse(xmlElement.GetAttribute("profile_id"));
							XmlElement xmlElement2 = response.OwnerDocument.CreateElement("achievement");
							xmlElement2.SetAttribute("profile_id", profileId.ToString());
							response.AppendChild(xmlElement2);
							foreach (AchievementInfo achievementInfo in service.AchievementSystem.GetProfileAchievements(profileId))
							{
								XmlElement xmlElement3 = response.OwnerDocument.CreateElement("chunk");
								xmlElement3.SetAttribute("achievement_id", achievementInfo.ID.ToString());
								xmlElement3.SetAttribute("progress", achievementInfo.Progress.ToString());
								xmlElement3.SetAttribute("completion_time", achievementInfo.CompletionTime.ToString());
								xmlElement2.AppendChild(xmlElement3);
							}
						}
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
				result = 0;
			}
			return result;
		}
	}
}

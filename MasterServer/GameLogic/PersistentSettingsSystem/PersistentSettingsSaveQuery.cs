using System;
using System.Collections;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.Database;

namespace MasterServer.GameLogic.PersistentSettingsSystem
{
	// Token: 0x020003F8 RID: 1016
	[QueryAttributes(TagName = "persistent_settings_set")]
	internal class PersistentSettingsSaveQuery : BaseQuery
	{
		// Token: 0x060015F8 RID: 5624 RVA: 0x0005BBB4 File Offset: 0x00059FB4
		public override int QueryGetResponse(string fromJid, XmlElement request, XmlElement response)
		{
			int result;
			using (new FunctionProfiler(Profiler.EModule.BASE_QUERY, "PersistentSettingsSaveQuery"))
			{
				ulong profileId = 0UL;
				if (!base.GetClientProfileId(fromJid, out profileId))
				{
					result = -3;
				}
				else
				{
					IDALService service = ServicesManager.GetService<IDALService>();
					XmlElement xmlElement = (XmlElement)request.ChildNodes[0];
					IEnumerator enumerator = xmlElement.ChildNodes.GetEnumerator();
					try
					{
						while (enumerator.MoveNext())
						{
							object obj = enumerator.Current;
							XmlNode xmlNode = (XmlNode)obj;
							if (!xmlNode.Name.Equals("quickplay", StringComparison.InvariantCultureIgnoreCase))
							{
								service.ProfileSystem.SetPersistentSettings(profileId, xmlNode.Name, xmlNode.OuterXml);
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
			}
			return result;
		}
	}
}

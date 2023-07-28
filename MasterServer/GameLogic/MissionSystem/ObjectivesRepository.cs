using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using MasterServer.Core;

namespace MasterServer.GameLogic.MissionSystem
{
	// Token: 0x020003C7 RID: 967
	internal class ObjectivesRepository
	{
		// Token: 0x06001552 RID: 5458 RVA: 0x00059530 File Offset: 0x00057930
		public ObjectivesRepository()
		{
			this.m_objectives.Clear();
			string text = Path.Combine(Resources.GetResourcesDirectory(), "libs/config/secondaryobjectivesdesc.xml");
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(text);
				IEnumerator enumerator = xmlDocument.DocumentElement.ChildNodes.GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						object obj = enumerator.Current;
						XmlElement xmlElement = (XmlElement)obj;
						if (xmlElement.Name == "objective")
						{
							int id = int.Parse(xmlElement.GetAttribute("id"));
							XmlElement difficultiesElem = xmlElement.GetElementsByTagName("difficulties")[0] as XmlElement;
							this.m_objectives.Add(new SecondaryObjective(id, difficultiesElem, "easy"));
							this.m_objectives.Add(new SecondaryObjective(id, difficultiesElem, "normal"));
							this.m_objectives.Add(new SecondaryObjective(id, difficultiesElem, "hard"));
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
			}
			catch (Exception ex)
			{
				Log.Error<string, string>("Parse error file - {0}, error - {1}", text, ex.Message);
			}
		}

		// Token: 0x06001553 RID: 5459 RVA: 0x00059684 File Offset: 0x00057A84
		public List<SecondaryObjective> GetObjectives(Predicate<SecondaryObjective> pred)
		{
			return this.m_objectives.FindAll(pred);
		}

		// Token: 0x04000A2B RID: 2603
		private const string SECONDARY_OBJECTIVES_DESC_FILE = "libs/config/secondaryobjectivesdesc.xml";

		// Token: 0x04000A2C RID: 2604
		private List<SecondaryObjective> m_objectives = new List<SecondaryObjective>();
	}
}

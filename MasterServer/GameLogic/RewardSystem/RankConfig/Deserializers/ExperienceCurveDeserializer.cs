using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

namespace MasterServer.GameLogic.RewardSystem.RankConfig.Deserializers
{
	// Token: 0x020000D4 RID: 212
	public class ExperienceCurveDeserializer
	{
		// Token: 0x0600036F RID: 879 RVA: 0x0000F9F4 File Offset: 0x0000DDF4
		public List<ulong> Deserialize(XmlDocument doc)
		{
			List<ulong> list = new List<ulong>();
			IEnumerator enumerator = doc.DocumentElement.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					string name = xmlElement.Name;
					if (name.StartsWith("level"))
					{
						ulong item = ulong.Parse(xmlElement.GetAttribute("exp"));
						list.Add(item);
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
			list.Sort();
			return list;
		}
	}
}

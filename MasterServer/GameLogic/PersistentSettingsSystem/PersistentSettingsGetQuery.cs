using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.PersistentSettingsSystem
{
	// Token: 0x020003F9 RID: 1017
	[QueryAttributes(TagName = "persistent_settings_get")]
	internal class PersistentSettingsGetQuery : BaseQuery
	{
		// Token: 0x060015F9 RID: 5625 RVA: 0x0005BCAC File Offset: 0x0005A0AC
		public PersistentSettingsGetQuery(IDALService dalService)
		{
			this.m_dalService = dalService;
		}

		// Token: 0x060015FA RID: 5626 RVA: 0x0005BCBC File Offset: 0x0005A0BC
		public override int HandleRequest(SOnlineQuery query, XmlElement request, XmlElement response)
		{
			ulong profileId;
			if (!base.GetClientProfileId(query.online_id, out profileId))
			{
				return -3;
			}
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			IEnumerator enumerator = request.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement xmlElement = (XmlElement)obj;
					dictionary.Add(xmlElement.Name, xmlElement.GetAttribute("_hash_"));
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
			foreach (XmlElement newChild in this.GetSettings(response.OwnerDocument, profileId, dictionary))
			{
				response.AppendChild(newChild);
			}
			return 0;
		}

		// Token: 0x060015FB RID: 5627 RVA: 0x0005BDA8 File Offset: 0x0005A1A8
		private IEnumerable<XmlElement> GetSettings(XmlDocument factory, ulong profileId, Dictionary<string, string> clientHashes)
		{
			XmlElement tmp_el = factory.CreateElement("tmp");
			HashSet<string> clientGroups = new HashSet<string>(clientHashes.Keys);
			HashSet<string> serverGroups = new HashSet<string>();
			foreach (SPersistentSettings ps in this.m_dalService.ProfileSystem.GetPersistentSettings(profileId))
			{
				serverGroups.Add(ps.Group);
				string db_hash = this.GetHashCode(ps.Settings).ToString();
				string client_hash;
				if (!clientHashes.TryGetValue(ps.Group, out client_hash) || !(client_hash == db_hash))
				{
					tmp_el.InnerXml = ps.Settings;
					XmlElement el = (XmlElement)tmp_el.FirstChild;
					el.SetAttribute("_hash_", db_hash);
					yield return el;
				}
			}
			clientGroups.ExceptWith(serverGroups);
			foreach (string group in clientGroups)
			{
				XmlElement el2 = factory.CreateElement(group);
				el2.SetAttribute("_hash_", "NA");
				yield return el2;
			}
			yield break;
		}

		// Token: 0x060015FC RID: 5628 RVA: 0x0005BDE0 File Offset: 0x0005A1E0
		public int GetHashCode(string settings)
		{
			CRC32 crc = new CRC32();
			ASCIIEncoding asciiencoding = new ASCIIEncoding();
			crc.GetHash(asciiencoding.GetBytes(settings));
			return (int)crc.CRCVal;
		}

		// Token: 0x04000A89 RID: 2697
		private readonly IDALService m_dalService;
	}
}

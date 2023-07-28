using System;
using System.Text;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.Database
{
	// Token: 0x0200003E RID: 62
	[Service]
	public class DALCookieProvider : IDALCookieProvider
	{
		// Token: 0x06000104 RID: 260 RVA: 0x00008680 File Offset: 0x00006A80
		public string GetCookie()
		{
			uint num = 0U;
			CRC32 crc = new CRC32();
			ConfigSection section = Resources.DBMasterSettings.GetSection("master");
			crc.GetHash(Encoding.ASCII.GetBytes(section.Get("server")));
			num ^= crc.CRCVal;
			crc.GetHash(Encoding.ASCII.GetBytes(section.Get("database")));
			num ^= crc.CRCVal;
			crc.GetHash(Encoding.ASCII.GetBytes(section.Get("user")));
			num ^= crc.CRCVal;
			crc.GetHash(Encoding.ASCII.GetBytes(section.Get("password")));
			num ^= crc.CRCVal;
			return string.Format("warface.{0}", num);
		}
	}
}

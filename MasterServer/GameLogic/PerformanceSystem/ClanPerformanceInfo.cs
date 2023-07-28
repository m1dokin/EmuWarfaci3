using System;
using System.Collections.Generic;
using System.Xml;
using MasterServer.DAL;

namespace MasterServer.GameLogic.PerformanceSystem
{
	// Token: 0x020003E7 RID: 999
	public class ClanPerformanceInfo
	{
		// Token: 0x060015B3 RID: 5555 RVA: 0x0005A78E File Offset: 0x00058B8E
		public ClanPerformanceInfo(int _position, IEnumerable<ClanInfo> leaderBoard)
		{
			this.Position = _position;
			this.topClans = leaderBoard;
		}

		// Token: 0x170001FB RID: 507
		// (get) Token: 0x060015B4 RID: 5556 RVA: 0x0005A7A4 File Offset: 0x00058BA4
		// (set) Token: 0x060015B5 RID: 5557 RVA: 0x0005A7AC File Offset: 0x00058BAC
		public int Position { get; private set; }

		// Token: 0x060015B6 RID: 5558 RVA: 0x0005A7B8 File Offset: 0x00058BB8
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("clan_performance");
			xmlElement.SetAttribute("position", this.Position.ToString());
			foreach (ClanInfo clanInfo in this.topClans)
			{
				xmlElement.AppendChild(clanInfo.ToXml(factory, false));
			}
			return xmlElement;
		}

		// Token: 0x04000A59 RID: 2649
		private IEnumerable<ClanInfo> topClans;
	}
}

using System;
using System.IO;
using System.Xml;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configs;
using MasterServer.Core.Services;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x0200006D RID: 109
	[Service]
	[Singleton]
	internal class ItemXmlConfigProvider : ServiceModule, IConfigProvider<XmlDocument>
	{
		// Token: 0x060001A6 RID: 422 RVA: 0x0000AF3C File Offset: 0x0000933C
		public ItemXmlConfigProvider()
		{
			this.m_itemsData = new XmlDocument();
		}

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x060001A7 RID: 423 RVA: 0x0000AF50 File Offset: 0x00009350
		// (remove) Token: 0x060001A8 RID: 424 RVA: 0x0000AF88 File Offset: 0x00009388
		public event Action<XmlDocument> Changed;

		// Token: 0x060001A9 RID: 425 RVA: 0x0000AFC0 File Offset: 0x000093C0
		public override void Init()
		{
			base.Init();
			string filename = Path.Combine(Resources.GetUpdateDirectory(), "game_db", "data", "items.xml");
			this.m_itemsData.Load(filename);
		}

		// Token: 0x060001AA RID: 426 RVA: 0x0000AFF9 File Offset: 0x000093F9
		public XmlDocument Get()
		{
			return this.m_itemsData;
		}

		// Token: 0x040000C5 RID: 197
		private readonly XmlDocument m_itemsData;
	}
}

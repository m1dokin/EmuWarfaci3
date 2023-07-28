using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.ItemsSystem.WinModels
{
	// Token: 0x0200032C RID: 812
	[Service]
	[Singleton]
	public class ConfiguratinBasedWinModelFactory : IWinModelFactory
	{
		// Token: 0x0600124E RID: 4686 RVA: 0x00049076 File Offset: 0x00047476
		public ConfiguratinBasedWinModelFactory(IEnumerable<IWinModel> winModels)
		{
			this.m_winModels = winModels;
		}

		// Token: 0x0600124F RID: 4687 RVA: 0x00049088 File Offset: 0x00047488
		public IWinModel GetWinModel()
		{
			ConfigSection section = Resources.ModuleSettings.GetSection("Shop").GetSection("RandomBox");
			IWinModel winModel = this.m_winModels.FirstOrDefault((IWinModel wm) => wm.WinModel == Utils.ParseEnum<TopPrizeWinModel>(section.Get("win_model")));
			if (winModel == null)
			{
				Log.Warning("[ConfiguratinBasedWinModelFactory] Failed to pick win model using value provided by the module configuration. Using 'persistent' win model as default.");
				winModel = this.m_winModels.First((IWinModel wm) => wm.WinModel == TopPrizeWinModel.Persistent);
			}
			return winModel;
		}

		// Token: 0x0400086C RID: 2156
		private readonly IEnumerable<IWinModel> m_winModels;
	}
}

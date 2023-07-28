using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HK2Net;
using MasterServer.Core;
using MasterServer.Core.Configuration;
using MasterServer.Core.Services;
using MasterServer.Core.Services.Configuration;
using MasterServer.GameRoomSystem;

namespace MasterServer.GameLogic.RewardSystem.RewardCalculators.ClanPointCalculator
{
	// Token: 0x020000E3 RID: 227
	[Service]
	[Singleton]
	internal class ClanPointCalculatorConfigurationProvider : ServiceModule, IClanPointCalculatorConfigProvider
	{
		// Token: 0x060003B8 RID: 952 RVA: 0x00010584 File Offset: 0x0000E984
		public ClanPointCalculatorConfigurationProvider(IConfigurationService configurationService)
		{
			this.m_configurationService = configurationService;
			this.m_roomTypeMultipliers = new List<ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers>();
		}

		// Token: 0x060003B9 RID: 953 RVA: 0x0001059E File Offset: 0x0000E99E
		public override void Init()
		{
			base.Init();
			this.ParseConfiguration();
			this.ValidateConfiguration();
		}

		// Token: 0x060003BA RID: 954 RVA: 0x000105B4 File Offset: 0x0000E9B4
		public float GetRoomTypeMultiplier(GameRoomType roomType)
		{
			ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers roomTypeMultipliers = this.m_roomTypeMultipliers.FirstOrDefault((ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers m) => m.RoomType == roomType);
			return (roomTypeMultipliers != null) ? roomTypeMultipliers.RoomTypeMultiplier : 0f;
		}

		// Token: 0x060003BB RID: 955 RVA: 0x000105FC File Offset: 0x0000E9FC
		public float GetGroupSizeMultiplier(GameRoomType roomType, int groupSize)
		{
			ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers roomTypeMultipliers = this.m_roomTypeMultipliers.FirstOrDefault((ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers m) => m.RoomType == roomType);
			if (roomTypeMultipliers == null)
			{
				return 1f;
			}
			IDictionary<int, float> multipliersByGroupSize = roomTypeMultipliers.MultipliersByGroupSize;
			return (!multipliersByGroupSize.ContainsKey(groupSize)) ? 1f : multipliersByGroupSize[groupSize];
		}

		// Token: 0x060003BC RID: 956 RVA: 0x00010660 File Offset: 0x0000EA60
		private void ParseConfiguration()
		{
			Config config = this.m_configurationService.GetConfig(MsConfigInfo.RewardsConfiguration);
			ConfigSection section4 = config.GetSection("Rewards");
			ConfigSection section2 = section4.GetSection("ClanPointsMultiplier");
			IEnumerator enumerator = Enum.GetValues(typeof(GameRoomType)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					GameRoomType roomType = (GameRoomType)obj;
					ConfigSection section3 = section2.GetSection(roomType.ToString().ToLower());
					if (section3 != null)
					{
						ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers roomTypeMultipliers = new ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers
						{
							RoomType = roomType,
							RoomTypeMultiplier = this.ParseAttrib(section3, "room_type_multiplier", roomType)
						};
						Dictionary<string, List<ConfigSection>>.ValueCollection values = section3.GetAllSections().Values;
						foreach (ConfigSection config2 in values.SelectMany((List<ConfigSection> section) => section))
						{
							roomTypeMultipliers.MultipliersByGroupSize.Add((int)this.ParseAttrib(config2, "group_size", roomTypeMultipliers.RoomType), this.ParseAttrib(config2, "value", roomTypeMultipliers.RoomType));
						}
						this.m_roomTypeMultipliers.Add(roomTypeMultipliers);
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

		// Token: 0x060003BD RID: 957 RVA: 0x0001080C File Offset: 0x0000EC0C
		private float ParseAttrib(ConfigSection config, string attribName, GameRoomType roomType)
		{
			float num;
			if (!config.TryGet(attribName, out num, 0f))
			{
				throw new ClanPointParseConfigurationException("ClanPointsMultiplier", attribName, roomType);
			}
			if (num < 0f)
			{
				throw new ClanPointParseConfigurationException("ClanPointsMultiplier", attribName, roomType);
			}
			return num;
		}

		// Token: 0x060003BE RID: 958 RVA: 0x00010854 File Offset: 0x0000EC54
		private void ValidateConfiguration()
		{
			foreach (ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers roomTypeMultipliers in this.m_roomTypeMultipliers)
			{
				float num = 0f;
				foreach (KeyValuePair<int, float> keyValuePair in from r in roomTypeMultipliers.MultipliersByGroupSize
				orderby r.Key
				select r)
				{
					if (num > keyValuePair.Value)
					{
						throw new ClanPointParseValidationException("ClanPointsMultiplier", keyValuePair.Key, keyValuePair.Value, roomTypeMultipliers.RoomType);
					}
					num = keyValuePair.Value;
				}
			}
		}

		// Token: 0x04000189 RID: 393
		private const int DefaultGroupSizeMultiplier = 1;

		// Token: 0x0400018A RID: 394
		private const string RewardsSectionName = "Rewards";

		// Token: 0x0400018B RID: 395
		private const string RoomTypeMultiplierAttribName = "room_type_multiplier";

		// Token: 0x0400018C RID: 396
		private const string GroupSizeAttribName = "group_size";

		// Token: 0x0400018D RID: 397
		private const string ValueAttribName = "value";

		// Token: 0x0400018E RID: 398
		private const string ClanPointsSectionName = "ClanPointsMultiplier";

		// Token: 0x0400018F RID: 399
		private readonly IConfigurationService m_configurationService;

		// Token: 0x04000190 RID: 400
		private readonly List<ClanPointCalculatorConfigurationProvider.RoomTypeMultipliers> m_roomTypeMultipliers;

		// Token: 0x020000E4 RID: 228
		private class RoomTypeMultipliers
		{
			// Token: 0x060003C1 RID: 961 RVA: 0x00010958 File Offset: 0x0000ED58
			internal RoomTypeMultipliers()
			{
				this.MultipliersByGroupSize = new Dictionary<int, float>();
			}

			// Token: 0x17000085 RID: 133
			// (get) Token: 0x060003C2 RID: 962 RVA: 0x0001096B File Offset: 0x0000ED6B
			// (set) Token: 0x060003C3 RID: 963 RVA: 0x00010973 File Offset: 0x0000ED73
			internal float RoomTypeMultiplier { get; set; }

			// Token: 0x17000086 RID: 134
			// (get) Token: 0x060003C4 RID: 964 RVA: 0x0001097C File Offset: 0x0000ED7C
			// (set) Token: 0x060003C5 RID: 965 RVA: 0x00010984 File Offset: 0x0000ED84
			internal IDictionary<int, float> MultipliersByGroupSize { get; private set; }

			// Token: 0x17000087 RID: 135
			// (get) Token: 0x060003C6 RID: 966 RVA: 0x0001098D File Offset: 0x0000ED8D
			// (set) Token: 0x060003C7 RID: 967 RVA: 0x00010995 File Offset: 0x0000ED95
			internal GameRoomType RoomType { get; set; }
		}
	}
}

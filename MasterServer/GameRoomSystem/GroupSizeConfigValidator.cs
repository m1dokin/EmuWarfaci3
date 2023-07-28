using System;
using HK2Net;
using MasterServer.Core.Configs;

namespace MasterServer.GameRoomSystem
{
	// Token: 0x02000010 RID: 16
	[Service]
	internal class GroupSizeConfigValidator : IConfigValidator<GroupSizeConfig>
	{
		// Token: 0x06000043 RID: 67 RVA: 0x000050F4 File Offset: 0x000034F4
		public void Validate(GroupSizeConfig config)
		{
			this.ValidatePveAutostart(config);
			this.ValidatePvpAutostart(config);
			this.ValidatePvpRating(config);
		}

		// Token: 0x06000044 RID: 68 RVA: 0x0000510B File Offset: 0x0000350B
		private void ValidatePveAutostart(GroupSizeConfig config)
		{
			this.ValidateGroupSize(config, GameRoomType.PvE_AutoStart, 5);
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00005118 File Offset: 0x00003518
		private void ValidatePvpAutostart(GroupSizeConfig config)
		{
			int maxRestriction = 8;
			this.ValidateGroupSize(config, GameRoomType.PvP_AutoStart, maxRestriction);
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00005130 File Offset: 0x00003530
		private void ValidatePvpRating(GroupSizeConfig config)
		{
			int maxRestriction = 8;
			this.ValidateGroupSize(config, GameRoomType.PvP_Rating, maxRestriction);
		}

		// Token: 0x06000047 RID: 71 RVA: 0x0000514C File Offset: 0x0000354C
		private void ValidateGroupSize(GroupSizeConfig config, GameRoomType roomType, int maxRestriction)
		{
			int num;
			if (!config.TryGet(roomType, out num))
			{
				throw new GroupSizeConfigValidationException(string.Format("Can't find group size, for room type {0}", roomType));
			}
			if (num < 1)
			{
				throw new GroupSizeConfigValidationException(string.Format("Group size can't be less than {0}, for room type {1}", 1, roomType));
			}
			if (num > maxRestriction)
			{
				throw new GroupSizeConfigValidationException(string.Format("Group size can't be bigger than {0}, for room type {1}", maxRestriction, roomType));
			}
		}

		// Token: 0x04000020 RID: 32
		public const int MinGroupSize = 1;
	}
}

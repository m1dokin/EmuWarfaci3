using System;
using MasterServer.Common;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason.Exceptions;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A7 RID: 167
	public class RatingSeasonRuleConfigValidator
	{
		// Token: 0x0600029D RID: 669 RVA: 0x0000D120 File Offset: 0x0000B520
		public void Validate(RatingSeasonRuleConfig config)
		{
			this.ValidateConfigNotEmpty(config);
			this.ValidateDateRanges(config);
			this.ValidateCorrectLengthOfSeasonIdTemplate(config.SeasonIdTemplate);
		}

		// Token: 0x0600029E RID: 670 RVA: 0x0000D140 File Offset: 0x0000B540
		private void ValidateDateRanges(RatingSeasonRuleConfig config)
		{
			if (config.AnnouncementEndDate > config.GamesEndDate)
			{
				string message = string.Format("Announcement date(UTC) can't be greater than end date(UTC). {0} > {1}", config.AnnouncementEndDate, config.GamesEndDate);
				throw new RatingSeasonConfigValidationException(message);
			}
		}

		// Token: 0x0600029F RID: 671 RVA: 0x0000D18F File Offset: 0x0000B58F
		private void ValidateConfigNotEmpty(RatingSeasonRuleConfig config)
		{
			if (config.SeasonIdTemplate.IsNullOrEmpty<char>())
			{
				throw new RatingSeasonConfigValidationException("Season ID Base should contain value");
			}
		}

		// Token: 0x060002A0 RID: 672 RVA: 0x0000D1B0 File Offset: 0x0000B5B0
		private void ValidateCorrectLengthOfSeasonIdTemplate(string seasonIdTemplate)
		{
			if ((long)seasonIdTemplate.Length > 30L)
			{
				string message = string.Format("Season ID Template can have {0} symbols max", 30U);
				throw new RatingSeasonConfigValidationException(message);
			}
		}

		// Token: 0x0400011F RID: 287
		public const uint SEASON_ID_TEMPLATE_MAX_LENGTH = 30U;
	}
}

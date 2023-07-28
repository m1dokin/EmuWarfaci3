using System;
using System.IO;
using MasterServer.DAL.CustomRules;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x0200009F RID: 159
	[CustomRuleStateSerializer("rating_season_rule", 4, typeof(RatingSeasonRule))]
	public class RatingSeasonRuleStateSerializer : ICustomRuleStateSerializer
	{
		// Token: 0x0600026F RID: 623 RVA: 0x0000C764 File Offset: 0x0000AB64
		public CustomRuleRawState Serialize(object state)
		{
			RatingSeasonRuleState ratingSeasonRuleState = (RatingSeasonRuleState)state;
			CustomRuleRawState customRuleRawState = new CustomRuleRawState
			{
				Key = ratingSeasonRuleState.Key
			};
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(0);
					binaryWriter.Write(ratingSeasonRuleState.LastActivationTime.ToBinary());
					binaryWriter.Write(ratingSeasonRuleState.MaxRatingLevelAchieved);
					binaryWriter.Write(ratingSeasonRuleState.SeasonId ?? string.Empty);
					binaryWriter.Write(ratingSeasonRuleState.SeasonResultRewardName ?? string.Empty);
					customRuleRawState.Data = memoryStream.ToArray();
				}
			}
			return customRuleRawState;
		}

		// Token: 0x06000270 RID: 624 RVA: 0x0000C848 File Offset: 0x0000AC48
		public CustomRuleState Deserialize(CustomRuleRawState rawState)
		{
			RatingSeasonRuleState ratingSeasonRuleState = new RatingSeasonRuleState
			{
				Key = rawState.Key
			};
			if (rawState.Data != null)
			{
				using (MemoryStream memoryStream = new MemoryStream(rawState.Data))
				{
					using (BinaryReader binaryReader = new BinaryReader(memoryStream))
					{
						int num = binaryReader.ReadInt32();
						if (num != 0)
						{
							string message = string.Format("Serialization is not implemented for provided RatingSeasonRuleState 'Data' with version {0}", num);
							throw new NotImplementedException(message);
						}
						ratingSeasonRuleState.LastActivationTime = DateTime.FromBinary(binaryReader.ReadInt64());
						ratingSeasonRuleState.MaxRatingLevelAchieved = binaryReader.ReadUInt32();
						ratingSeasonRuleState.SeasonId = binaryReader.ReadString();
						ratingSeasonRuleState.SeasonResultRewardName = binaryReader.ReadString();
					}
				}
			}
			return ratingSeasonRuleState;
		}
	}
}

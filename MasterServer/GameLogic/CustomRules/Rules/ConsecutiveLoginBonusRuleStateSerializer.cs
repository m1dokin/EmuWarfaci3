using System;
using System.IO;
using MasterServer.DAL.CustomRules;

namespace MasterServer.GameLogic.CustomRules.Rules
{
	// Token: 0x020002C6 RID: 710
	[CustomRuleStateSerializer("consecutive_login_bonus", 2, typeof(ConsecutiveLoginBonusRule))]
	internal class ConsecutiveLoginBonusRuleStateSerializer : ICustomRuleStateSerializer
	{
		// Token: 0x06000F28 RID: 3880 RVA: 0x0003BA60 File Offset: 0x00039E60
		public CustomRuleRawState Serialize(object stateobj)
		{
			ConsecutiveLoginBonusRuleState consecutiveLoginBonusRuleState = (ConsecutiveLoginBonusRuleState)stateobj;
			CustomRuleRawState customRuleRawState = new CustomRuleRawState
			{
				Key = consecutiveLoginBonusRuleState.Key
			};
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(0);
					binaryWriter.Write(consecutiveLoginBonusRuleState.PrevStreak);
					binaryWriter.Write(consecutiveLoginBonusRuleState.PrevReward);
					binaryWriter.Write(consecutiveLoginBonusRuleState.LastActivationTime.ToBinary());
					customRuleRawState.Data = memoryStream.ToArray();
				}
			}
			return customRuleRawState;
		}

		// Token: 0x06000F29 RID: 3881 RVA: 0x0003BB20 File Offset: 0x00039F20
		public CustomRuleState Deserialize(CustomRuleRawState rawState)
		{
			ConsecutiveLoginBonusRuleState consecutiveLoginBonusRuleState = new ConsecutiveLoginBonusRuleState
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
							string message = string.Format("Serialization is not implemented for provided 'Data' with version {0}", num);
							throw new NotImplementedException(message);
						}
						consecutiveLoginBonusRuleState.PrevStreak = binaryReader.ReadInt32();
						consecutiveLoginBonusRuleState.PrevReward = binaryReader.ReadInt32();
						consecutiveLoginBonusRuleState.LastActivationTime = DateTime.FromBinary(binaryReader.ReadInt64());
					}
				}
			}
			return consecutiveLoginBonusRuleState;
		}
	}
}

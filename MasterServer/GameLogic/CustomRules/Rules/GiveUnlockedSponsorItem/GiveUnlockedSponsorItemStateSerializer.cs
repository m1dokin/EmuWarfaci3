using System;
using System.IO;
using MasterServer.DAL.CustomRules;

namespace MasterServer.GameLogic.CustomRules.Rules.GiveUnlockedSponsorItem
{
	// Token: 0x020002CD RID: 717
	[CustomRuleStateSerializer("give_unlocked_sponsor_item", 3, typeof(GiveUnlockedSponsorItemRule))]
	internal class GiveUnlockedSponsorItemStateSerializer : ICustomRuleStateSerializer
	{
		// Token: 0x06000F50 RID: 3920 RVA: 0x0003D748 File Offset: 0x0003BB48
		public CustomRuleRawState Serialize(object state)
		{
			GiveUnlockedSponsorItemRuleState giveUnlockedSponsorItemRuleState = (GiveUnlockedSponsorItemRuleState)state;
			CustomRuleRawState customRuleRawState = new CustomRuleRawState
			{
				Key = giveUnlockedSponsorItemRuleState.Key
			};
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(0);
					binaryWriter.Write(giveUnlockedSponsorItemRuleState.LastActivationTime.ToBinary());
					customRuleRawState.Data = memoryStream.ToArray();
				}
			}
			return customRuleRawState;
		}

		// Token: 0x06000F51 RID: 3921 RVA: 0x0003D7EC File Offset: 0x0003BBEC
		public CustomRuleState Deserialize(CustomRuleRawState rawState)
		{
			GiveUnlockedSponsorItemRuleState giveUnlockedSponsorItemRuleState = new GiveUnlockedSponsorItemRuleState
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
						giveUnlockedSponsorItemRuleState.LastActivationTime = DateTime.FromBinary(binaryReader.ReadInt64());
					}
				}
			}
			return giveUnlockedSponsorItemRuleState;
		}
	}
}

using System;

namespace MasterServer.GameLogic.SkillSystem.Exceptions
{
	// Token: 0x020000FD RID: 253
	internal class ChannelSkillConfigNotFoundException : SkillServiceException
	{
		// Token: 0x0600042A RID: 1066 RVA: 0x0001227C File Offset: 0x0001067C
		public ChannelSkillConfigNotFoundException(SkillType skillType, string channelName) : base(string.Format("{0} skill config for {1} channel not found", skillType, channelName))
		{
		}
	}
}

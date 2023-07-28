using System;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000008 RID: 8
	internal enum NameValidationResult
	{
		// Token: 0x04000009 RID: 9
		NoError,
		// Token: 0x0400000A RID: 10
		LengthTooShort,
		// Token: 0x0400000B RID: 11
		LengthTooLong,
		// Token: 0x0400000C RID: 12
		UnsupportedCharacter
	}
}

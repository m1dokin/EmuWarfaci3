using System;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000636 RID: 1590
	public enum GErrorCode
	{
		// Token: 0x040010CC RID: 4300
		Unknown = -1,
		// Token: 0x040010CD RID: 4301
		NoError,
		// Token: 0x040010CE RID: 4302
		ParserError = -2,
		// Token: 0x040010CF RID: 4303
		NetworkError = -3,
		// Token: 0x040010D0 RID: 4304
		FatalNetworkError = -4,
		// Token: 0x040010D1 RID: 4305
		RequestError = -5,
		// Token: 0x040010D2 RID: 4306
		RequestPoolTimeout = -6,
		// Token: 0x040010D3 RID: 4307
		InvalidSession = 10001,
		// Token: 0x040010D4 RID: 4308
		UserLocked,
		// Token: 0x040010D5 RID: 4309
		InvalidAPIKey,
		// Token: 0x040010D6 RID: 4310
		InvalidCredential = 10010,
		// Token: 0x040010D7 RID: 4311
		SeedNotExist = 40004,
		// Token: 0x040010D8 RID: 4312
		SeedHasBeenRemoved,
		// Token: 0x040010D9 RID: 4313
		NoWallet = 50001,
		// Token: 0x040010DA RID: 4314
		NotEnoughPoints = 50003,
		// Token: 0x040010DB RID: 4315
		NotEnoughCredits,
		// Token: 0x040010DC RID: 4316
		InternalError = 90001,
		// Token: 0x040010DD RID: 4317
		IncorrectArguments = 90003
	}
}

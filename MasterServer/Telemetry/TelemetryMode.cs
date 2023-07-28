using System;

namespace MasterServer.Telemetry
{
	// Token: 0x020007CE RID: 1998
	[Flags]
	internal enum TelemetryMode
	{
		// Token: 0x040015C7 RID: 5575
		Disabled = 0,
		// Token: 0x040015C8 RID: 5576
		Session = 1,
		// Token: 0x040015C9 RID: 5577
		Realm = 2,
		// Token: 0x040015CA RID: 5578
		Presence = 4,
		// Token: 0x040015CB RID: 5579
		Generic = 8,
		// Token: 0x040015CC RID: 5580
		Monitoring = 16,
		// Token: 0x040015CD RID: 5581
		StatsMerging = 32,
		// Token: 0x040015CE RID: 5582
		RemoveStreams = 64,
		// Token: 0x040015CF RID: 5583
		Enabled = 127
	}
}

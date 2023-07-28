using System;
using System.Collections.Generic;
using HK2Net;
using OLAPHypervisor;
using StatsDataSource.Storage;

namespace MasterServer.Telemetry
{
	// Token: 0x020007D0 RID: 2000
	[Contract]
	internal interface ITelemetryService
	{
		// Token: 0x170003C6 RID: 966
		// (get) Token: 0x060028E8 RID: 10472
		// (set) Token: 0x060028E9 RID: 10473
		TelemetryMode Mode { get; set; }

		// Token: 0x060028EA RID: 10474
		bool CheckMode(TelemetryMode mode);

		// Token: 0x170003C7 RID: 967
		// (get) Token: 0x060028EB RID: 10475
		StatsProcessor StatsProcessor { get; }

		// Token: 0x170003C8 RID: 968
		// (get) Token: 0x060028EC RID: 10476
		DeferredTelemetryStream DeferredStream { get; }

		// Token: 0x060028ED RID: 10477
		void AddMeasure(Measure msr);

		// Token: 0x060028EE RID: 10478
		void AddMeasure(IEnumerable<Measure> msrs);

		// Token: 0x060028EF RID: 10479
		void AddMeasure(long value, params object[] args);

		// Token: 0x060028F0 RID: 10480
		Measure MakeMeasure(long value, params object[] args);

		// Token: 0x060028F1 RID: 10481
		Measure[] MeasureFromUpdates(List<DataUpdate> updates);

		// Token: 0x060028F2 RID: 10482
		void RunAggregation();
	}
}

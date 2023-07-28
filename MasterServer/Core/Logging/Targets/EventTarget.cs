using System;
using NLog;
using NLog.Targets;

namespace MasterServer.Core.Logging.Targets
{
	// Token: 0x02000104 RID: 260
	[Target("event")]
	public class EventTarget : TargetWithLayout
	{
		// Token: 0x06000441 RID: 1089 RVA: 0x000126AC File Offset: 0x00010AAC
		protected override void Write(LogEventInfo logEvent)
		{
			string line = this.Layout.Render(logEvent);
			Log.RaiseOnLogMessageEvent(line);
		}

		// Token: 0x040001C0 RID: 448
		public const string TargetName = "event";
	}
}

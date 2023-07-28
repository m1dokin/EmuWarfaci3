using System;
using System.Collections.Generic;
using System.Text;
using Util.Common;

namespace MasterServer.Core
{
	// Token: 0x0200014A RID: 330
	public class RConCallback : MarshalByRefObject
	{
		// Token: 0x060005BE RID: 1470 RVA: 0x000170F0 File Offset: 0x000154F0
		private void GetCommandOutput(string line)
		{
			object cmdOut = this.m_cmdOut;
			lock (cmdOut)
			{
				if (this.m_cmdOut.Count == 100)
				{
					this.m_cmdOut.Dequeue();
				}
				this.m_cmdOut.Enqueue(line);
			}
		}

		// Token: 0x060005BF RID: 1471 RVA: 0x00017158 File Offset: 0x00015558
		private string FormatOutput()
		{
			object cmdOut = this.m_cmdOut;
			string result;
			lock (cmdOut)
			{
				StringBuilder stringBuilder = new StringBuilder();
				while (this.m_cmdOut.Count > 0)
				{
					stringBuilder.Append(this.m_cmdOut.Dequeue()).AppendLine();
				}
				result = stringBuilder.ToString();
			}
			return result;
		}

		// Token: 0x060005C0 RID: 1472 RVA: 0x000171D0 File Offset: 0x000155D0
		public string ExecCmd(string cmd)
		{
			CultureHelpers.SetNeutralThreadCulture();
			Log.OnLogMessage += this.GetCommandOutput;
			ConsoleCmdManager.ExecuteCmd(cmd);
			Log.OnLogMessage -= this.GetCommandOutput;
			return this.FormatOutput();
		}

		// Token: 0x060005C1 RID: 1473 RVA: 0x00017205 File Offset: 0x00015605
		public override object InitializeLifetimeService()
		{
			return null;
		}

		// Token: 0x040003BD RID: 957
		private const int MaxLines = 100;

		// Token: 0x040003BE RID: 958
		private readonly Queue<string> m_cmdOut = new Queue<string>(100);
	}
}

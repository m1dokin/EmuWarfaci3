using System;
using System.Text;
using DedicatedPoolServer.Model;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C5 RID: 1733
	public class ServerEntity : ICloneable
	{
		// Token: 0x0600244B RID: 9291 RVA: 0x000977F1 File Offset: 0x00095BF1
		public ServerEntity()
		{
			this.Status = EGameServerStatus.None;
			this.Mode = DedicatedMode.PurePVP;
		}

		// Token: 0x0600244C RID: 9292 RVA: 0x00097808 File Offset: 0x00095C08
		public ServerEntity(SServerEntity record)
		{
			this.ServerID = record.ServerId;
			this.OnlineID = record.OnlineId;
			this.Mode = ((!(record.Mode == "pure_pvp")) ? DedicatedMode.PVP_PVE : DedicatedMode.PurePVP);
			this.Hostname = record.Hostname;
			this.Port = record.Port;
			this.Node = record.Node;
			this.SessionID = null;
			this.Mission = record.MissionKey;
			this.Status = (EGameServerStatus)record.Status;
			this.BuildType = record.BuildType;
			this.MasterServerId = record.MasterServerId;
			this.PerformanceIndex = record.PerformanceIndex;
		}

		// Token: 0x17000380 RID: 896
		// (get) Token: 0x0600244D RID: 9293 RVA: 0x000978C7 File Offset: 0x00095CC7
		// (set) Token: 0x0600244E RID: 9294 RVA: 0x000978CF File Offset: 0x00095CCF
		public string ServerID { get; set; }

		// Token: 0x17000381 RID: 897
		// (get) Token: 0x0600244F RID: 9295 RVA: 0x000978D8 File Offset: 0x00095CD8
		// (set) Token: 0x06002450 RID: 9296 RVA: 0x000978E0 File Offset: 0x00095CE0
		public string OnlineID { get; set; }

		// Token: 0x17000382 RID: 898
		// (get) Token: 0x06002451 RID: 9297 RVA: 0x000978E9 File Offset: 0x00095CE9
		// (set) Token: 0x06002452 RID: 9298 RVA: 0x000978F1 File Offset: 0x00095CF1
		public DedicatedMode Mode { get; set; }

		// Token: 0x17000383 RID: 899
		// (get) Token: 0x06002453 RID: 9299 RVA: 0x000978FA File Offset: 0x00095CFA
		// (set) Token: 0x06002454 RID: 9300 RVA: 0x00097902 File Offset: 0x00095D02
		public string Hostname { get; set; }

		// Token: 0x17000384 RID: 900
		// (get) Token: 0x06002455 RID: 9301 RVA: 0x0009790B File Offset: 0x00095D0B
		// (set) Token: 0x06002456 RID: 9302 RVA: 0x00097913 File Offset: 0x00095D13
		public int Port { get; set; }

		// Token: 0x17000385 RID: 901
		// (get) Token: 0x06002457 RID: 9303 RVA: 0x0009791C File Offset: 0x00095D1C
		// (set) Token: 0x06002458 RID: 9304 RVA: 0x00097924 File Offset: 0x00095D24
		public string Node { get; set; }

		// Token: 0x17000386 RID: 902
		// (get) Token: 0x06002459 RID: 9305 RVA: 0x0009792D File Offset: 0x00095D2D
		// (set) Token: 0x0600245A RID: 9306 RVA: 0x00097935 File Offset: 0x00095D35
		public string SessionID { get; set; }

		// Token: 0x17000387 RID: 903
		// (get) Token: 0x0600245B RID: 9307 RVA: 0x0009793E File Offset: 0x00095D3E
		// (set) Token: 0x0600245C RID: 9308 RVA: 0x00097946 File Offset: 0x00095D46
		public string Mission { get; set; }

		// Token: 0x17000388 RID: 904
		// (get) Token: 0x0600245D RID: 9309 RVA: 0x0009794F File Offset: 0x00095D4F
		// (set) Token: 0x0600245E RID: 9310 RVA: 0x00097957 File Offset: 0x00095D57
		public EGameServerStatus Status { get; set; }

		// Token: 0x17000389 RID: 905
		// (get) Token: 0x0600245F RID: 9311 RVA: 0x00097960 File Offset: 0x00095D60
		// (set) Token: 0x06002460 RID: 9312 RVA: 0x00097968 File Offset: 0x00095D68
		public string BuildType { get; set; }

		// Token: 0x1700038A RID: 906
		// (get) Token: 0x06002461 RID: 9313 RVA: 0x00097971 File Offset: 0x00095D71
		// (set) Token: 0x06002462 RID: 9314 RVA: 0x00097979 File Offset: 0x00095D79
		public string MasterServerId { get; set; }

		// Token: 0x1700038B RID: 907
		// (get) Token: 0x06002463 RID: 9315 RVA: 0x00097982 File Offset: 0x00095D82
		// (set) Token: 0x06002464 RID: 9316 RVA: 0x0009798A File Offset: 0x00095D8A
		public float PerformanceIndex { get; set; }

		// Token: 0x06002465 RID: 9317 RVA: 0x00097993 File Offset: 0x00095D93
		public void Dump()
		{
			Log.Info(this.ToString());
		}

		// Token: 0x06002466 RID: 9318 RVA: 0x000979A0 File Offset: 0x00095DA0
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("ServerEntity: srvId    = {0}\n", this.ServerID);
			stringBuilder.AppendFormat("ServerEntity: onlineId = {0}\n", this.OnlineID);
			stringBuilder.AppendFormat("ServerEntity: hostname = {0}\n", this.Hostname);
			stringBuilder.AppendFormat("ServerEntity: port     = {0}\n", this.Port);
			stringBuilder.AppendFormat("ServerEntity: node     = {0}\n", this.Node);
			stringBuilder.AppendFormat("ServerEntity: status   = {0}\n", this.Status);
			stringBuilder.AppendFormat("ServerEntity: mission  = {0}\n", this.Mission);
			stringBuilder.AppendFormat("ServerEntity: mode     = {0}\n", this.Mode);
			stringBuilder.AppendFormat("ServerEntity: session  = {0}\n", this.SessionID);
			stringBuilder.AppendFormat("ServerEntity: buildtype= {0}\n", this.BuildType);
			stringBuilder.AppendFormat("ServerEntity: msid     = {0}\n", this.MasterServerId);
			stringBuilder.AppendFormat("ServerEntity: perf     = {0}\n", this.PerformanceIndex);
			return stringBuilder.ToString();
		}

		// Token: 0x06002467 RID: 9319 RVA: 0x00097AA5 File Offset: 0x00095EA5
		public object Clone()
		{
			return base.MemberwiseClone();
		}
	}
}

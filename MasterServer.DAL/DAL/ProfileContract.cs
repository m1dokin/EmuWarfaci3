using System;
using System.Xml;

namespace MasterServer.DAL
{
	// Token: 0x02000023 RID: 35
	[Serializable]
	public class ProfileContract
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000059 RID: 89 RVA: 0x00002FF0 File Offset: 0x000013F0
		public ProfileContract.ContractStatus Status
		{
			get
			{
				if (this.TotalProgress == 0U)
				{
					return ProfileContract.ContractStatus.eCS_None;
				}
				if (this.CurrentProgress >= this.TotalProgress)
				{
					return ProfileContract.ContractStatus.eCS_Completed;
				}
				if (this.ProfileItemId > 0UL)
				{
					return ProfileContract.ContractStatus.eCS_InProgress;
				}
				return ProfileContract.ContractStatus.eCS_Failed;
			}
		}

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x0600005A RID: 90 RVA: 0x00003024 File Offset: 0x00001424
		public bool IsExpired
		{
			get
			{
				return (DateTime.UtcNow - this.RotationTimeUTC).TotalSeconds > 0.0;
			}
		}

		// Token: 0x0600005B RID: 91 RVA: 0x00003054 File Offset: 0x00001454
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("contract");
			xmlElement.SetAttribute("profile_id", this.ProfileId.ToString());
			xmlElement.SetAttribute("rotation_id", this.RotationId.ToString());
			xmlElement.SetAttribute("contract_name", this.ContractName);
			xmlElement.SetAttribute("current", this.CurrentProgress.ToString());
			xmlElement.SetAttribute("total", this.TotalProgress.ToString());
			xmlElement.SetAttribute("rotation_time", (this.RotationTimeUTC - DateTime.UtcNow).TotalSeconds.ToString());
			xmlElement.SetAttribute("status", ((int)this.Status).ToString());
			return xmlElement;
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00003140 File Offset: 0x00001540
		public override string ToString()
		{
			return string.Format("RotationId: {0}, ProfileItemId: {1}, ContractName: {2}, CurrentProgress: {3}, TotalProgress: {4}, RotationTimeUTC: {5}, Status: {6}", new object[]
			{
				this.RotationId,
				this.ProfileItemId,
				this.ContractName,
				this.CurrentProgress,
				this.TotalProgress,
				this.RotationTimeUTC,
				this.Status
			});
		}

		// Token: 0x04000054 RID: 84
		public ulong ProfileId;

		// Token: 0x04000055 RID: 85
		public uint RotationId;

		// Token: 0x04000056 RID: 86
		public string ContractName;

		// Token: 0x04000057 RID: 87
		public ulong ProfileItemId;

		// Token: 0x04000058 RID: 88
		public uint CurrentProgress;

		// Token: 0x04000059 RID: 89
		public uint TotalProgress;

		// Token: 0x0400005A RID: 90
		public DateTime RotationTimeUTC;

		// Token: 0x02000024 RID: 36
		public enum ContractStatus
		{
			// Token: 0x0400005C RID: 92
			eCS_None,
			// Token: 0x0400005D RID: 93
			eCS_InProgress,
			// Token: 0x0400005E RID: 94
			eCS_Completed,
			// Token: 0x0400005F RID: 95
			eCS_Failed
		}
	}
}

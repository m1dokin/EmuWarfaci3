using System;
using System.Collections.Generic;
using System.Xml;

namespace MasterServer.ElectronicCatalog
{
	// Token: 0x0200023E RID: 574
	[Serializable]
	public class RepairEquipmentOperationResult
	{
		// Token: 0x06000C56 RID: 3158 RVA: 0x0003063A File Offset: 0x0002EA3A
		public RepairEquipmentOperationResult()
		{
			this.ItemsToRepair = new List<RepairItemStatus>();
		}

		// Token: 0x06000C57 RID: 3159 RVA: 0x0003064D File Offset: 0x0002EA4D
		public override string ToString()
		{
			return string.Format("ProfileId: {0}, RepairGlobalStatus: {1}", this.ProfileId, this.OperationGlobalStatus);
		}

		// Token: 0x06000C58 RID: 3160 RVA: 0x00030670 File Offset: 0x0002EA70
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("RepairResult");
			xmlElement.SetAttribute("profile_id", this.ProfileId.ToString());
			XmlElement xmlElement2 = xmlElement;
			string name = "operation_status";
			int operationGlobalStatus = (int)this.OperationGlobalStatus;
			xmlElement2.SetAttribute(name, operationGlobalStatus.ToString());
			foreach (RepairItemStatus repairItemStatus in this.ItemsToRepair)
			{
				XmlElement xmlElement3 = factory.CreateElement("ProfileItem");
				xmlElement3.SetAttribute("profile_item_id", repairItemStatus.ProfileItemId.ToString());
				XmlElement xmlElement4 = xmlElement3;
				string name2 = "repair_status";
				int repairStatus = (int)repairItemStatus.RepairStatus;
				xmlElement4.SetAttribute(name2, repairStatus.ToString());
				xmlElement3.SetAttribute("money_spent", repairItemStatus.MoneySpent.ToString());
				xmlElement3.SetAttribute("total_durability", repairItemStatus.TotalDurability.ToString());
				xmlElement3.SetAttribute("durability", repairItemStatus.Durability.ToString());
				xmlElement.AppendChild(xmlElement3);
			}
			return xmlElement;
		}

		// Token: 0x040005B4 RID: 1460
		public ulong ProfileId;

		// Token: 0x040005B5 RID: 1461
		public readonly IList<RepairItemStatus> ItemsToRepair;

		// Token: 0x040005B6 RID: 1462
		public RepairStatus OperationGlobalStatus;

		// Token: 0x040005B7 RID: 1463
		public ulong TotalRepairCost;

		// Token: 0x040005B8 RID: 1464
		public ulong MoneyBeforeRepair;
	}
}

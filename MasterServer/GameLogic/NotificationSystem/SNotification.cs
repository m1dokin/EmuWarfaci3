using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x02000577 RID: 1399
	public class SNotification
	{
		// Token: 0x06001E0F RID: 7695 RVA: 0x00079E7A File Offset: 0x0007827A
		public SNotification()
		{
		}

		// Token: 0x06001E10 RID: 7696 RVA: 0x00079E90 File Offset: 0x00078290
		public SNotification(SPendingNotification notif)
		{
			this.ID = notif.ID;
			this.Type = (ENotificationType)((notif.Type >= uint.MaxValue) ? 1U : notif.Type);
			this.ConfirmationType = (EConfirmationType)notif.ConfirmationType;
			this.ExpirationTimeUTC = notif.ExpirationTimeUTC;
			this.Data = notif.Data;
			this.Message = notif.Message;
		}

		// Token: 0x17000321 RID: 801
		// (get) Token: 0x06001E11 RID: 7697 RVA: 0x00079F0F File Offset: 0x0007830F
		public bool IsExpired
		{
			get
			{
				return DateTime.UtcNow > this.ExpirationTimeUTC;
			}
		}

		// Token: 0x06001E12 RID: 7698 RVA: 0x00079F24 File Offset: 0x00078324
		public XmlElement ToXml(INotificationService notifService, XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("notif");
			xmlElement.SetAttribute("id", this.ID.ToString());
			XmlElement xmlElement2 = xmlElement;
			string name = "type";
			uint type = (uint)this.Type;
			xmlElement2.SetAttribute(name, type.ToString());
			xmlElement.SetAttribute("confirmation", (this.ConfirmationType != EConfirmationType.Confirmation) ? "0" : "1");
			xmlElement.SetAttribute("from_jid", Resources.Jid);
			xmlElement.SetAttribute("message", this.Message);
			try
			{
				INotificationSerializer notificationSerializer = notifService.GetNotificationSerializer(this.Type);
				xmlElement.AppendChild(notificationSerializer.Serialize(this.Data, xmlElement));
			}
			catch (NotificationServiceException e)
			{
				Log.Error(e);
			}
			return xmlElement;
		}

		// Token: 0x06001E13 RID: 7699 RVA: 0x0007A004 File Offset: 0x00078404
		public static string ToLogString(SNotification notification)
		{
			return string.Format("{{id:{0},type:\"{1}\"}}", notification.ID, notification.Type);
		}

		// Token: 0x06001E14 RID: 7700 RVA: 0x0007A026 File Offset: 0x00078426
		public static string ToLogString(List<SNotification> notifications)
		{
			string str = "[";
			string separator = ",";
			if (SNotification.<>f__mg$cache0 == null)
			{
				SNotification.<>f__mg$cache0 = new Func<SNotification, string>(SNotification.ToLogString);
			}
			return str + string.Join(separator, notifications.Select(SNotification.<>f__mg$cache0)) + "]";
		}

		// Token: 0x04000EAA RID: 3754
		public ulong ID;

		// Token: 0x04000EAB RID: 3755
		public ENotificationType Type;

		// Token: 0x04000EAC RID: 3756
		public EConfirmationType ConfirmationType;

		// Token: 0x04000EAD RID: 3757
		public DateTime ExpirationTimeUTC;

		// Token: 0x04000EAE RID: 3758
		public byte[] Data;

		// Token: 0x04000EAF RID: 3759
		public string Message = string.Empty;

		// Token: 0x04000EB0 RID: 3760
		[CompilerGenerated]
		private static Func<SNotification, string> <>f__mg$cache0;
	}
}

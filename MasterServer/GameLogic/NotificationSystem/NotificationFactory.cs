using System;
using MasterServer.Common;

namespace MasterServer.GameLogic.NotificationSystem
{
	// Token: 0x020003DC RID: 988
	internal class NotificationFactory
	{
		// Token: 0x06001595 RID: 5525 RVA: 0x0005A492 File Offset: 0x00058892
		public static SNotification CreateNotification<T>(ENotificationType type, T data, TimeSpan expiration, EConfirmationType confirmation)
		{
			return NotificationFactory.CreateNotification<T>(type, data, expiration, confirmation, string.Empty);
		}

		// Token: 0x06001596 RID: 5526 RVA: 0x0005A4A4 File Offset: 0x000588A4
		public static SNotification CreateNotification<T>(ENotificationType type, T data, TimeSpan expiration, EConfirmationType confirmation, string message)
		{
			return new SNotification
			{
				Type = type,
				ConfirmationType = confirmation,
				ExpirationTimeUTC = DateTime.UtcNow + expiration,
				Data = Utils.CreateByteArrayFromType<T>(data),
				Message = message
			};
		}
	}
}

using System;

namespace MasterServer.GameRoom.RoomExtensions
{
	// Token: 0x020004BF RID: 1215
	internal class PlayTimeNotFoundException : ApplicationException
	{
		// Token: 0x06001A4B RID: 6731 RVA: 0x0006C388 File Offset: 0x0006A788
		public PlayTimeNotFoundException(string sessioId) : base(string.Format("Can't find PlayTime in session storage for session '{0}'", sessioId))
		{
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x0006C39B File Offset: 0x0006A79B
		public PlayTimeNotFoundException(string sessioId, ulong profileId) : base(string.Format("Can't find PlayTime in session storage for session '{0}', player '{1}'", sessioId, profileId))
		{
		}
	}
}

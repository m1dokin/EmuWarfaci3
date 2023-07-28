using System;

namespace MasterServer.DAL.Exceptions
{
	// Token: 0x02000030 RID: 48
	public class DALBinaryDataLengthViolationException : Exception
	{
		// Token: 0x0600007A RID: 122 RVA: 0x0000348B File Offset: 0x0000188B
		public DALBinaryDataLengthViolationException(string tableName, string columnName, int currentLength) : this(DALBinaryDataLengthViolationException.ComposeMessage(tableName, columnName, currentLength))
		{
		}

		// Token: 0x0600007B RID: 123 RVA: 0x0000349B File Offset: 0x0000189B
		private DALBinaryDataLengthViolationException(string message) : base(message)
		{
		}

		// Token: 0x0600007C RID: 124 RVA: 0x000034A4 File Offset: 0x000018A4
		private static string ComposeMessage(string tableName, string columnName, int currentLength)
		{
			return string.Format("Binary data length ({0} B) violates maximum length restriction for '{1}.{2}'.", currentLength, tableName, columnName);
		}
	}
}

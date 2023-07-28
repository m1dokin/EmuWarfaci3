using System;

namespace MasterServer.Core
{
	// Token: 0x0200013B RID: 315
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public class LogCategoryAttribute : Attribute
	{
		// Token: 0x06000521 RID: 1313 RVA: 0x000166B8 File Offset: 0x00014AB8
		public LogCategoryAttribute(string name, string comment)
		{
			this.Name = name;
			this.Comment = comment;
		}

		// Token: 0x06000522 RID: 1314 RVA: 0x000166CE File Offset: 0x00014ACE
		public LogCategoryAttribute(string name) : this(name, string.Empty)
		{
		}

		// Token: 0x04000230 RID: 560
		public string Name;

		// Token: 0x04000231 RID: 561
		public string Comment;
	}
}

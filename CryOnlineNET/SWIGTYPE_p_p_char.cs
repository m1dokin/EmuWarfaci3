using System;
using System.Runtime.InteropServices;

// Token: 0x0200003E RID: 62
public class SWIGTYPE_p_p_char
{
	// Token: 0x0600025A RID: 602 RVA: 0x000051D6 File Offset: 0x000033D6
	internal SWIGTYPE_p_p_char(IntPtr cPtr, bool futureUse)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x0600025B RID: 603 RVA: 0x000051EB File Offset: 0x000033EB
	protected SWIGTYPE_p_p_char()
	{
		this.swigCPtr = new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600025C RID: 604 RVA: 0x00005204 File Offset: 0x00003404
	internal static HandleRef getCPtr(SWIGTYPE_p_p_char obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x040000B6 RID: 182
	private HandleRef swigCPtr;
}

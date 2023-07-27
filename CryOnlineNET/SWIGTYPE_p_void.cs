using System;
using System.Runtime.InteropServices;

// Token: 0x02000040 RID: 64
public class SWIGTYPE_p_void
{
	// Token: 0x06000260 RID: 608 RVA: 0x0000526E File Offset: 0x0000346E
	internal SWIGTYPE_p_void(IntPtr cPtr, bool futureUse)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000261 RID: 609 RVA: 0x00005283 File Offset: 0x00003483
	protected SWIGTYPE_p_void()
	{
		this.swigCPtr = new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000262 RID: 610 RVA: 0x0000529C File Offset: 0x0000349C
	internal static HandleRef getCPtr(SWIGTYPE_p_void obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x040000B8 RID: 184
	private HandleRef swigCPtr;
}

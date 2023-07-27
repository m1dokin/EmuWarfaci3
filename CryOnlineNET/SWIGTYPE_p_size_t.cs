using System;
using System.Runtime.InteropServices;

// Token: 0x0200003F RID: 63
public class SWIGTYPE_p_size_t
{
	// Token: 0x0600025D RID: 605 RVA: 0x00005222 File Offset: 0x00003422
	internal SWIGTYPE_p_size_t(IntPtr cPtr, bool futureUse)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00005237 File Offset: 0x00003437
	protected SWIGTYPE_p_size_t()
	{
		this.swigCPtr = new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600025F RID: 607 RVA: 0x00005250 File Offset: 0x00003450
	internal static HandleRef getCPtr(SWIGTYPE_p_size_t obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x040000B7 RID: 183
	private HandleRef swigCPtr;
}

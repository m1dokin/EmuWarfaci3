using System;
using System.Runtime.InteropServices;

// Token: 0x02000016 RID: 22
public class ICertificateValidator : IDisposable
{
	// Token: 0x060000F2 RID: 242 RVA: 0x0000259E File Offset: 0x0000079E
	internal ICertificateValidator(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x000025BA File Offset: 0x000007BA
	internal static HandleRef getCPtr(ICertificateValidator obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x000025D8 File Offset: 0x000007D8
	~ICertificateValidator()
	{
		this.Dispose();
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x00002608 File Offset: 0x00000808
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_ICertificateValidator(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00002690 File Offset: 0x00000890
	public virtual bool Query(SWIGTYPE_p_void cert)
	{
		return CryOnlinePINVOKE.ICertificateValidator_Query(this.swigCPtr, SWIGTYPE_p_void.getCPtr(cert));
	}

	// Token: 0x04000065 RID: 101
	private HandleRef swigCPtr;

	// Token: 0x04000066 RID: 102
	protected bool swigCMemOwn;
}

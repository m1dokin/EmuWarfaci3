using System;
using System.Runtime.InteropServices;

// Token: 0x0200003C RID: 60
public class SQueryError : SBaseQuery
{
	// Token: 0x0600023B RID: 571 RVA: 0x00004E2E File Offset: 0x0000302E
	internal SQueryError(IntPtr cPtr, bool cMemoryOwn) : base(CryOnlinePINVOKE.SQueryErrorUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00004E4A File Offset: 0x0000304A
	public SQueryError() : this(CryOnlinePINVOKE.new_SQueryError(), true)
	{
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00004E58 File Offset: 0x00003058
	internal static HandleRef getCPtr(SQueryError obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00004E78 File Offset: 0x00003078
	~SQueryError()
	{
		this.Dispose();
	}

	// Token: 0x0600023F RID: 575 RVA: 0x00004EA8 File Offset: 0x000030A8
	public override void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_SQueryError(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
			base.Dispose();
		}
	}

	// Token: 0x17000008 RID: 8
	// (get) Token: 0x06000241 RID: 577 RVA: 0x00004F44 File Offset: 0x00003144
	// (set) Token: 0x06000240 RID: 576 RVA: 0x00004F34 File Offset: 0x00003134
	public EOnlineError online_error
	{
		get
		{
			return (EOnlineError)CryOnlinePINVOKE.SQueryError_online_error_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryError_online_error_set(this.swigCPtr, (int)value);
		}
	}

	// Token: 0x17000009 RID: 9
	// (get) Token: 0x06000243 RID: 579 RVA: 0x00004F6C File Offset: 0x0000316C
	// (set) Token: 0x06000242 RID: 578 RVA: 0x00004F5E File Offset: 0x0000315E
	public int custom_code
	{
		get
		{
			return CryOnlinePINVOKE.SQueryError_custom_code_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryError_custom_code_set(this.swigCPtr, value);
		}
	}

	// Token: 0x040000B2 RID: 178
	private HandleRef swigCPtr;
}

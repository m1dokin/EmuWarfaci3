using System;
using System.Runtime.InteropServices;

// Token: 0x0200003B RID: 59
public class SOnlineQuery : SBaseQuery
{
	// Token: 0x06000234 RID: 564 RVA: 0x00004D00 File Offset: 0x00002F00
	internal SOnlineQuery(IntPtr cPtr, bool cMemoryOwn) : base(CryOnlinePINVOKE.SOnlineQueryUpcast(cPtr), cMemoryOwn)
	{
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000235 RID: 565 RVA: 0x00004D1C File Offset: 0x00002F1C
	public SOnlineQuery() : this(CryOnlinePINVOKE.new_SOnlineQuery(), true)
	{
	}

	// Token: 0x06000236 RID: 566 RVA: 0x00004D2A File Offset: 0x00002F2A
	internal static HandleRef getCPtr(SOnlineQuery obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00004D48 File Offset: 0x00002F48
	~SOnlineQuery()
	{
		this.Dispose();
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00004D78 File Offset: 0x00002F78
	public override void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_SOnlineQuery(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
			base.Dispose();
		}
	}

	// Token: 0x17000007 RID: 7
	// (get) Token: 0x0600023A RID: 570 RVA: 0x00004E14 File Offset: 0x00003014
	// (set) Token: 0x06000239 RID: 569 RVA: 0x00004E04 File Offset: 0x00003004
	public EQueryType type
	{
		get
		{
			return (EQueryType)CryOnlinePINVOKE.SOnlineQuery_type_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SOnlineQuery_type_set(this.swigCPtr, (int)value);
		}
	}

	// Token: 0x040000B1 RID: 177
	private HandleRef swigCPtr;
}

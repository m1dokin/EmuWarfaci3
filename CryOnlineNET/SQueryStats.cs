using System;
using System.Runtime.InteropServices;

// Token: 0x0200003D RID: 61
public class SQueryStats : IDisposable
{
	// Token: 0x06000244 RID: 580 RVA: 0x00004F86 File Offset: 0x00003186
	internal SQueryStats(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000245 RID: 581 RVA: 0x00004FA2 File Offset: 0x000031A2
	public SQueryStats() : this(CryOnlinePINVOKE.new_SQueryStats(), true)
	{
	}

	// Token: 0x06000246 RID: 582 RVA: 0x00004FB0 File Offset: 0x000031B0
	internal static HandleRef getCPtr(SQueryStats obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000247 RID: 583 RVA: 0x00004FD0 File Offset: 0x000031D0
	~SQueryStats()
	{
		this.Dispose();
	}

	// Token: 0x06000248 RID: 584 RVA: 0x00005000 File Offset: 0x00003200
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_SQueryStats(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x1700000A RID: 10
	// (get) Token: 0x0600024A RID: 586 RVA: 0x00005098 File Offset: 0x00003298
	// (set) Token: 0x06000249 RID: 585 RVA: 0x00005088 File Offset: 0x00003288
	public string Query
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_Query_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_Query_set(this.swigCPtr, value);
		}
	}

	// Token: 0x1700000B RID: 11
	// (get) Token: 0x0600024C RID: 588 RVA: 0x000050C0 File Offset: 0x000032C0
	// (set) Token: 0x0600024B RID: 587 RVA: 0x000050B2 File Offset: 0x000032B2
	public ulong ResponseTime
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_ResponseTime_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_ResponseTime_set(this.swigCPtr, value);
		}
	}

	// Token: 0x1700000C RID: 12
	// (get) Token: 0x0600024E RID: 590 RVA: 0x000050E8 File Offset: 0x000032E8
	// (set) Token: 0x0600024D RID: 589 RVA: 0x000050DA File Offset: 0x000032DA
	public uint InboundDataSize
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_InboundDataSize_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_InboundDataSize_set(this.swigCPtr, value);
		}
	}

	// Token: 0x1700000D RID: 13
	// (get) Token: 0x06000250 RID: 592 RVA: 0x00005110 File Offset: 0x00003310
	// (set) Token: 0x0600024F RID: 591 RVA: 0x00005102 File Offset: 0x00003302
	public uint InboundCompressedSize
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_InboundCompressedSize_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_InboundCompressedSize_set(this.swigCPtr, value);
		}
	}

	// Token: 0x1700000E RID: 14
	// (get) Token: 0x06000252 RID: 594 RVA: 0x00005138 File Offset: 0x00003338
	// (set) Token: 0x06000251 RID: 593 RVA: 0x0000512A File Offset: 0x0000332A
	public uint OutboundDataSize
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_OutboundDataSize_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_OutboundDataSize_set(this.swigCPtr, value);
		}
	}

	// Token: 0x1700000F RID: 15
	// (get) Token: 0x06000254 RID: 596 RVA: 0x00005160 File Offset: 0x00003360
	// (set) Token: 0x06000253 RID: 595 RVA: 0x00005152 File Offset: 0x00003352
	public uint OutboundCompressedSize
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_OutboundCompressedSize_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_OutboundCompressedSize_set(this.swigCPtr, value);
		}
	}

	// Token: 0x17000010 RID: 16
	// (get) Token: 0x06000256 RID: 598 RVA: 0x00005188 File Offset: 0x00003388
	// (set) Token: 0x06000255 RID: 597 RVA: 0x0000517A File Offset: 0x0000337A
	public bool Request
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_Request_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_Request_set(this.swigCPtr, value);
		}
	}

	// Token: 0x17000011 RID: 17
	// (get) Token: 0x06000258 RID: 600 RVA: 0x000051B0 File Offset: 0x000033B0
	// (set) Token: 0x06000257 RID: 599 RVA: 0x000051A2 File Offset: 0x000033A2
	public bool Failed
	{
		get
		{
			return CryOnlinePINVOKE.SQueryStats_Failed_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SQueryStats_Failed_set(this.swigCPtr, value);
		}
	}

	// Token: 0x040000B3 RID: 179
	private HandleRef swigCPtr;

	// Token: 0x040000B4 RID: 180
	protected bool swigCMemOwn;

	// Token: 0x040000B5 RID: 181
	public static readonly int NAME_LEN = CryOnlinePINVOKE.SQueryStats_NAME_LEN_get();
}

using System;
using System.Runtime.InteropServices;

// Token: 0x0200003A RID: 58
public class SBaseQuery : IDisposable
{
	// Token: 0x06000220 RID: 544 RVA: 0x00004AE5 File Offset: 0x00002CE5
	internal SBaseQuery(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000221 RID: 545 RVA: 0x00004B01 File Offset: 0x00002D01
	public SBaseQuery() : this(CryOnlinePINVOKE.new_SBaseQuery(), true)
	{
	}

	// Token: 0x06000222 RID: 546 RVA: 0x00004B0F File Offset: 0x00002D0F
	internal static HandleRef getCPtr(SBaseQuery obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000223 RID: 547 RVA: 0x00004B30 File Offset: 0x00002D30
	~SBaseQuery()
	{
		this.Dispose();
	}

	// Token: 0x06000224 RID: 548 RVA: 0x00004B60 File Offset: 0x00002D60
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_SBaseQuery(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000226 RID: 550 RVA: 0x00004BF8 File Offset: 0x00002DF8
	// (set) Token: 0x06000225 RID: 549 RVA: 0x00004BE8 File Offset: 0x00002DE8
	public string sId
	{
		get
		{
			return CryOnlinePINVOKE.SBaseQuery_sId_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SBaseQuery_sId_set(this.swigCPtr, value);
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000228 RID: 552 RVA: 0x00004C20 File Offset: 0x00002E20
	// (set) Token: 0x06000227 RID: 551 RVA: 0x00004C12 File Offset: 0x00002E12
	public string online_id
	{
		get
		{
			return CryOnlinePINVOKE.SBaseQuery_online_id_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SBaseQuery_online_id_set(this.swigCPtr, value);
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x0600022A RID: 554 RVA: 0x00004C48 File Offset: 0x00002E48
	// (set) Token: 0x06000229 RID: 553 RVA: 0x00004C3A File Offset: 0x00002E3A
	public string tag
	{
		get
		{
			return CryOnlinePINVOKE.SBaseQuery_tag_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SBaseQuery_tag_set(this.swigCPtr, value);
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x0600022C RID: 556 RVA: 0x00004C70 File Offset: 0x00002E70
	// (set) Token: 0x0600022B RID: 555 RVA: 0x00004C62 File Offset: 0x00002E62
	public string description
	{
		get
		{
			return CryOnlinePINVOKE.SBaseQuery_description_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SBaseQuery_description_set(this.swigCPtr, value);
		}
	}

	// Token: 0x17000006 RID: 6
	// (get) Token: 0x0600022E RID: 558 RVA: 0x00004C98 File Offset: 0x00002E98
	// (set) Token: 0x0600022D RID: 557 RVA: 0x00004C8A File Offset: 0x00002E8A
	public int id
	{
		get
		{
			return CryOnlinePINVOKE.SBaseQuery_id_get(this.swigCPtr);
		}
		set
		{
			CryOnlinePINVOKE.SBaseQuery_id_set(this.swigCPtr, value);
		}
	}

	// Token: 0x0600022F RID: 559 RVA: 0x00004CB2 File Offset: 0x00002EB2
	public void SetSID(string sid)
	{
		CryOnlinePINVOKE.SBaseQuery_SetSID(this.swigCPtr, sid);
	}

	// Token: 0x06000230 RID: 560 RVA: 0x00004CC0 File Offset: 0x00002EC0
	public void SetOnlineID(string onlineId)
	{
		CryOnlinePINVOKE.SBaseQuery_SetOnlineID(this.swigCPtr, onlineId);
	}

	// Token: 0x06000231 RID: 561 RVA: 0x00004CCE File Offset: 0x00002ECE
	public void SetTag(string tg)
	{
		CryOnlinePINVOKE.SBaseQuery_SetTag(this.swigCPtr, tg);
	}

	// Token: 0x06000232 RID: 562 RVA: 0x00004CDC File Offset: 0x00002EDC
	public void SetDescription(string descr)
	{
		CryOnlinePINVOKE.SBaseQuery_SetDescription(this.swigCPtr, descr);
	}

	// Token: 0x040000AD RID: 173
	private HandleRef swigCPtr;

	// Token: 0x040000AE RID: 174
	protected bool swigCMemOwn;

	// Token: 0x040000AF RID: 175
	public static readonly int QUERY_DESC_LEN = CryOnlinePINVOKE.SBaseQuery_QUERY_DESC_LEN_get();

	// Token: 0x040000B0 RID: 176
	public static readonly int QUERY_BUF_LEN = CryOnlinePINVOKE.SBaseQuery_QUERY_BUF_LEN_get();
}

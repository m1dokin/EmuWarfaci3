using System;
using System.Runtime.InteropServices;

// Token: 0x02000017 RID: 23
public class IOnline : IDisposable
{
	// Token: 0x060000F7 RID: 247 RVA: 0x000026B0 File Offset: 0x000008B0
	internal IOnline(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x000026CC File Offset: 0x000008CC
	internal static HandleRef getCPtr(IOnline obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x000026EC File Offset: 0x000008EC
	~IOnline()
	{
		this.Dispose();
	}

	// Token: 0x060000FA RID: 250 RVA: 0x0000271C File Offset: 0x0000091C
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnline(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x060000FB RID: 251 RVA: 0x000027A4 File Offset: 0x000009A4
	public virtual void Tick()
	{
		CryOnlinePINVOKE.IOnline_Tick(this.swigCPtr);
	}

	// Token: 0x060000FC RID: 252 RVA: 0x000027B1 File Offset: 0x000009B1
	public virtual void Shutdown()
	{
		CryOnlinePINVOKE.IOnline_Shutdown(this.swigCPtr);
	}

	// Token: 0x060000FD RID: 253 RVA: 0x000027C0 File Offset: 0x000009C0
	public virtual IOnlineConnection CreateConnection(string domain)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnline_CreateConnection(this.swigCPtr, domain);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineConnection(intPtr, false) : null;
	}

	// Token: 0x060000FE RID: 254 RVA: 0x000027F9 File Offset: 0x000009F9
	public virtual void ReleaseConnection(string domain)
	{
		CryOnlinePINVOKE.IOnline_ReleaseConnection(this.swigCPtr, domain);
	}

	// Token: 0x060000FF RID: 255 RVA: 0x00002807 File Offset: 0x00000A07
	public virtual void RegisterLog(IOnlineLog log)
	{
		CryOnlinePINVOKE.IOnline_RegisterLog(this.swigCPtr, IOnlineLog.getCPtr(log));
	}

	// Token: 0x06000100 RID: 256 RVA: 0x0000281A File Offset: 0x00000A1A
	public virtual void UnregisterLog()
	{
		CryOnlinePINVOKE.IOnline_UnregisterLog(this.swigCPtr);
	}

	// Token: 0x06000101 RID: 257 RVA: 0x00002828 File Offset: 0x00000A28
	public virtual IOnlineLog GetLog()
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnline_GetLog(this.swigCPtr);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineLog(intPtr, false) : null;
	}

	// Token: 0x06000102 RID: 258 RVA: 0x00002860 File Offset: 0x00000A60
	public virtual IOnlineConfiguration GetConfiguration(string domain)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnline_GetConfiguration(this.swigCPtr, domain);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineConfiguration(intPtr, false) : null;
	}

	// Token: 0x06000103 RID: 259 RVA: 0x0000289C File Offset: 0x00000A9C
	public virtual IOnlineConnection GetConnection(string domain)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnline_GetConnection(this.swigCPtr, domain);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineConnection(intPtr, false) : null;
	}

	// Token: 0x06000104 RID: 260 RVA: 0x000028D8 File Offset: 0x00000AD8
	public virtual bool RegisterConnectionListener(IOnlineConnectionListener listener, string domain)
	{
		return CryOnlinePINVOKE.IOnline_RegisterConnectionListener(this.swigCPtr, IOnlineConnectionListener.getCPtr(listener), domain);
	}

	// Token: 0x06000105 RID: 261 RVA: 0x000028FC File Offset: 0x00000AFC
	public virtual bool UnregisterConnectionListener(IOnlineConnectionListener listener)
	{
		return CryOnlinePINVOKE.IOnline_UnregisterConnectionListener(this.swigCPtr, IOnlineConnectionListener.getCPtr(listener));
	}

	// Token: 0x06000106 RID: 262 RVA: 0x0000291C File Offset: 0x00000B1C
	public virtual void RegisterQueryBinder(IOnlineQueryBinder binder, string tag)
	{
		CryOnlinePINVOKE.IOnline_RegisterQueryBinder(this.swigCPtr, IOnlineQueryBinder.getCPtr(binder), tag);
	}

	// Token: 0x06000107 RID: 263 RVA: 0x00002930 File Offset: 0x00000B30
	public virtual void UnregisterQueryBinder(IOnlineQueryBinder binder)
	{
		CryOnlinePINVOKE.IOnline_UnregisterQueryBinder(this.swigCPtr, IOnlineQueryBinder.getCPtr(binder));
	}

	// Token: 0x04000067 RID: 103
	private HandleRef swigCPtr;

	// Token: 0x04000068 RID: 104
	protected bool swigCMemOwn;
}

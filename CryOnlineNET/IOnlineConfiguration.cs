using System;
using System.Runtime.InteropServices;

// Token: 0x02000023 RID: 35
public class IOnlineConfiguration : IDisposable
{
	// Token: 0x06000159 RID: 345 RVA: 0x000032FD File Offset: 0x000014FD
	internal IOnlineConfiguration(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00003319 File Offset: 0x00001519
	internal static HandleRef getCPtr(IOnlineConfiguration obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600015B RID: 347 RVA: 0x00003338 File Offset: 0x00001538
	~IOnlineConfiguration()
	{
		this.Dispose();
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00003368 File Offset: 0x00001568
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineConfiguration(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x000033F0 File Offset: 0x000015F0
	public virtual string GetDomain()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetDomain(this.swigCPtr);
	}

	// Token: 0x0600015E RID: 350 RVA: 0x0000340A File Offset: 0x0000160A
	public virtual void SetDomain(string domain)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetDomain(this.swigCPtr, domain);
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00003418 File Offset: 0x00001618
	public virtual string GetServer()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetServer(this.swigCPtr);
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00003432 File Offset: 0x00001632
	public virtual void SetServer(string server)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetServer(this.swigCPtr, server);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00003440 File Offset: 0x00001640
	public virtual int GetServerPort()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetServerPort(this.swigCPtr);
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0000345A File Offset: 0x0000165A
	public virtual void SetServerPort(int port)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetServerPort(this.swigCPtr, port);
	}

	// Token: 0x06000163 RID: 355 RVA: 0x00003468 File Offset: 0x00001668
	public virtual string GetHost()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetHost(this.swigCPtr);
	}

	// Token: 0x06000164 RID: 356 RVA: 0x00003482 File Offset: 0x00001682
	public virtual void SetHost(string host)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetHost(this.swigCPtr, host);
	}

	// Token: 0x06000165 RID: 357 RVA: 0x00003490 File Offset: 0x00001690
	public virtual string GetResource()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetResource(this.swigCPtr);
	}

	// Token: 0x06000166 RID: 358 RVA: 0x000034AA File Offset: 0x000016AA
	public virtual void SetResource(string resource)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetResource(this.swigCPtr, resource);
	}

	// Token: 0x06000167 RID: 359 RVA: 0x000034B8 File Offset: 0x000016B8
	public virtual string GetPassword()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetPassword(this.swigCPtr);
	}

	// Token: 0x06000168 RID: 360 RVA: 0x000034D2 File Offset: 0x000016D2
	public virtual void SetPassword(string pwd)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetPassword(this.swigCPtr, pwd);
	}

	// Token: 0x06000169 RID: 361 RVA: 0x000034E0 File Offset: 0x000016E0
	public virtual string GetOnlineId()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetOnlineId(this.swigCPtr);
	}

	// Token: 0x0600016A RID: 362 RVA: 0x000034FA File Offset: 0x000016FA
	public virtual void SetOnlineId(string onlineId)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetOnlineId(this.swigCPtr, onlineId);
	}

	// Token: 0x0600016B RID: 363 RVA: 0x00003508 File Offset: 0x00001708
	public virtual string GetFSProxy()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetFSProxy(this.swigCPtr);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x00003522 File Offset: 0x00001722
	public virtual void SetFSProxy(string proxy)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetFSProxy(this.swigCPtr, proxy);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x00003530 File Offset: 0x00001730
	public virtual EOnlineTLSPolicy GetTLSPolicy()
	{
		return (EOnlineTLSPolicy)CryOnlinePINVOKE.IOnlineConfiguration_GetTLSPolicy(this.swigCPtr);
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0000354A File Offset: 0x0000174A
	public virtual void SetTLSPolicy(EOnlineTLSPolicy policy)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetTLSPolicy(this.swigCPtr, (int)policy);
	}

	// Token: 0x0600016F RID: 367 RVA: 0x00003558 File Offset: 0x00001758
	public virtual int GetFSProxyPort()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetFSProxyPort(this.swigCPtr);
	}

	// Token: 0x06000170 RID: 368 RVA: 0x00003572 File Offset: 0x00001772
	public virtual void SetFSProxyPort(int port)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetFSProxyPort(this.swigCPtr, port);
	}

	// Token: 0x06000171 RID: 369 RVA: 0x00003580 File Offset: 0x00001780
	public virtual void SetDefaultCompression(ECompressType compr)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetDefaultCompression(this.swigCPtr, (int)compr);
	}

	// Token: 0x06000172 RID: 370 RVA: 0x00003590 File Offset: 0x00001790
	public virtual ECompressType GetDefaultCompression()
	{
		return (ECompressType)CryOnlinePINVOKE.IOnlineConfiguration_GetDefaultCompression(this.swigCPtr);
	}

	// Token: 0x06000173 RID: 371 RVA: 0x000035AA File Offset: 0x000017AA
	public virtual void SetSendDelay(int delay)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetSendDelay(this.swigCPtr, delay);
	}

	// Token: 0x06000174 RID: 372 RVA: 0x000035B8 File Offset: 0x000017B8
	public virtual int GetSendDelay()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetSendDelay(this.swigCPtr);
	}

	// Token: 0x06000175 RID: 373 RVA: 0x000035D2 File Offset: 0x000017D2
	public virtual void SetTcpReceiveBufferSize(int size)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetTcpReceiveBufferSize(this.swigCPtr, size);
	}

	// Token: 0x06000176 RID: 374 RVA: 0x000035E0 File Offset: 0x000017E0
	public virtual int GetTcpReceiveBufferSize()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetTcpReceiveBufferSize(this.swigCPtr);
	}

	// Token: 0x06000177 RID: 375 RVA: 0x000035FC File Offset: 0x000017FC
	public virtual string GetFullOnlineId()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetFullOnlineId(this.swigCPtr);
	}

	// Token: 0x06000178 RID: 376 RVA: 0x00003616 File Offset: 0x00001816
	public virtual void SetOnlineVerbose(int onlineVerbose)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetOnlineVerbose(this.swigCPtr, onlineVerbose);
	}

	// Token: 0x06000179 RID: 377 RVA: 0x00003624 File Offset: 0x00001824
	public virtual bool IsOnlineVerbose()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_IsOnlineVerbose(this.swigCPtr);
	}

	// Token: 0x0600017A RID: 378 RVA: 0x0000363E File Offset: 0x0000183E
	public virtual void SetThreadMode(int threadMode)
	{
		CryOnlinePINVOKE.IOnlineConfiguration_SetThreadMode(this.swigCPtr, threadMode);
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0000364C File Offset: 0x0000184C
	public virtual int GetThreadMode()
	{
		return CryOnlinePINVOKE.IOnlineConfiguration_GetThreadMode(this.swigCPtr);
	}

	// Token: 0x0400007F RID: 127
	private HandleRef swigCPtr;

	// Token: 0x04000080 RID: 128
	protected bool swigCMemOwn;
}

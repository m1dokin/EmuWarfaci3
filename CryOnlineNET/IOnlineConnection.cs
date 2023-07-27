using System;
using System.Runtime.InteropServices;

// Token: 0x02000024 RID: 36
public class IOnlineConnection : IDisposable
{
	// Token: 0x0600017C RID: 380 RVA: 0x00003666 File Offset: 0x00001866
	internal IOnlineConnection(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00003682 File Offset: 0x00001882
	internal static HandleRef getCPtr(IOnlineConnection obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600017E RID: 382 RVA: 0x000036A0 File Offset: 0x000018A0
	~IOnlineConnection()
	{
		this.Dispose();
	}

	// Token: 0x0600017F RID: 383 RVA: 0x000036D0 File Offset: 0x000018D0
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineConnection(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00003758 File Offset: 0x00001958
	public virtual bool Connect(EConnectionMode mode)
	{
		return CryOnlinePINVOKE.IOnlineConnection_Connect__SWIG_0(this.swigCPtr, (int)mode);
	}

	// Token: 0x06000181 RID: 385 RVA: 0x00003774 File Offset: 0x00001974
	public virtual bool Connect()
	{
		return CryOnlinePINVOKE.IOnlineConnection_Connect__SWIG_1(this.swigCPtr);
	}

	// Token: 0x06000182 RID: 386 RVA: 0x00003790 File Offset: 0x00001990
	public virtual int Query(string query, string receiverId)
	{
		return CryOnlinePINVOKE.IOnlineConnection_Query__SWIG_0(this.swigCPtr, query, receiverId);
	}

	// Token: 0x06000183 RID: 387 RVA: 0x000037AC File Offset: 0x000019AC
	public virtual int Query(string query)
	{
		return CryOnlinePINVOKE.IOnlineConnection_Query__SWIG_1(this.swigCPtr, query);
	}

	// Token: 0x06000184 RID: 388 RVA: 0x000037C8 File Offset: 0x000019C8
	public virtual int Query(string query, string receiverId, IOnlineQueryBinder binder, ECompressType comprType)
	{
		return CryOnlinePINVOKE.IOnlineConnection_Query__SWIG_2(this.swigCPtr, query, receiverId, IOnlineQueryBinder.getCPtr(binder), (int)comprType);
	}

	// Token: 0x06000185 RID: 389 RVA: 0x000037EC File Offset: 0x000019EC
	public virtual int Query(IXmlNodeHandler query, string receiverId, IOnlineQueryBinder binder, ECompressType comprType, ERequestSendType requestSendType)
	{
		return CryOnlinePINVOKE.IOnlineConnection_Query__SWIG_3(this.swigCPtr, IXmlNodeHandler.getCPtr(query), receiverId, IOnlineQueryBinder.getCPtr(binder), (int)comprType, (int)requestSendType);
	}

	// Token: 0x06000186 RID: 390 RVA: 0x00003818 File Offset: 0x00001A18
	public virtual int Query(IXmlNodeHandler query, string receiverId, IOnlineQueryBinder binder, ECompressType comprType)
	{
		return CryOnlinePINVOKE.IOnlineConnection_Query__SWIG_4(this.swigCPtr, IXmlNodeHandler.getCPtr(query), receiverId, IOnlineQueryBinder.getCPtr(binder), (int)comprType);
	}

	// Token: 0x06000187 RID: 391 RVA: 0x00003841 File Offset: 0x00001A41
	public virtual void Response(SOnlineQuery query, string payload, ECompressType comprType)
	{
		CryOnlinePINVOKE.IOnlineConnection_Response(this.swigCPtr, SOnlineQuery.getCPtr(query), payload, (int)comprType);
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
	}

	// Token: 0x06000188 RID: 392 RVA: 0x00003866 File Offset: 0x00001A66
	public virtual void ResponseError(SQueryError error, string payload)
	{
		CryOnlinePINVOKE.IOnlineConnection_ResponseError(this.swigCPtr, SQueryError.getCPtr(error), payload);
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
	}

	// Token: 0x06000189 RID: 393 RVA: 0x0000388A File Offset: 0x00001A8A
	public virtual void ScheduleFailedQuery(EOnlineError error, string message, IOnlineQueryBinder binder)
	{
		CryOnlinePINVOKE.IOnlineConnection_ScheduleFailedQuery(this.swigCPtr, (int)error, message, IOnlineQueryBinder.getCPtr(binder));
	}

	// Token: 0x0600018A RID: 394 RVA: 0x000038A0 File Offset: 0x00001AA0
	public virtual EOnlineConnectionState GetConnectionState()
	{
		return (EOnlineConnectionState)CryOnlinePINVOKE.IOnlineConnection_GetConnectionState(this.swigCPtr);
	}

	// Token: 0x0600018B RID: 395 RVA: 0x000038BC File Offset: 0x00001ABC
	public virtual IOnlineConfiguration GetConfiguration()
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnlineConnection_GetConfiguration(this.swigCPtr);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineConfiguration(intPtr, false) : null;
	}

	// Token: 0x0600018C RID: 396 RVA: 0x000038F4 File Offset: 0x00001AF4
	public virtual IOnlineChat GetChat(string service_id)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnlineConnection_GetChat(this.swigCPtr, service_id);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineChat(intPtr, false) : null;
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0000392D File Offset: 0x00001B2D
	public virtual void RegisterQueryStatsListener(IOnlineQueryStatsListener l)
	{
		CryOnlinePINVOKE.IOnlineConnection_RegisterQueryStatsListener(this.swigCPtr, IOnlineQueryStatsListener.getCPtr(l));
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00003940 File Offset: 0x00001B40
	public virtual void UnregisterQueryStatsListener(IOnlineQueryStatsListener l)
	{
		CryOnlinePINVOKE.IOnlineConnection_UnregisterQueryStatsListener(this.swigCPtr, IOnlineQueryStatsListener.getCPtr(l));
	}

	// Token: 0x0600018F RID: 399 RVA: 0x00003954 File Offset: 0x00001B54
	public virtual IOnlineBandwidthStats GetBandwidthStatistics()
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnlineConnection_GetBandwidthStatistics(this.swigCPtr);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineBandwidthStats(intPtr, false) : null;
	}

	// Token: 0x06000190 RID: 400 RVA: 0x0000398C File Offset: 0x00001B8C
	public virtual void RegisterDataProtect(IOnlineDataProtect protect)
	{
		CryOnlinePINVOKE.IOnlineConnection_RegisterDataProtect(this.swigCPtr, IOnlineDataProtect.getCPtr(protect));
	}

	// Token: 0x06000191 RID: 401 RVA: 0x0000399F File Offset: 0x00001B9F
	public virtual void UnregisterDataProtect()
	{
		CryOnlinePINVOKE.IOnlineConnection_UnregisterDataProtect(this.swigCPtr);
	}

	// Token: 0x06000192 RID: 402 RVA: 0x000039AC File Offset: 0x00001BAC
	public virtual void SendProtectionData(string tag, string data, uint size)
	{
		CryOnlinePINVOKE.IOnlineConnection_SendProtectionData(this.swigCPtr, tag, data, size);
	}

	// Token: 0x06000193 RID: 403 RVA: 0x000039BC File Offset: 0x00001BBC
	public virtual void SetCertificateValidator(ICertificateValidator pValidator)
	{
		CryOnlinePINVOKE.IOnlineConnection_SetCertificateValidator(this.swigCPtr, ICertificateValidator.getCPtr(pValidator));
	}

	// Token: 0x04000081 RID: 129
	private HandleRef swigCPtr;

	// Token: 0x04000082 RID: 130
	protected bool swigCMemOwn;
}

using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x02000037 RID: 55
public class IOnlineQueryStatsListener : IDisposable
{
	// Token: 0x06000208 RID: 520 RVA: 0x00004757 File Offset: 0x00002957
	internal IOnlineQueryStatsListener(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000209 RID: 521 RVA: 0x00004773 File Offset: 0x00002973
	public IOnlineQueryStatsListener() : this(CryOnlinePINVOKE.new_IOnlineQueryStatsListener(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x0600020A RID: 522 RVA: 0x00004787 File Offset: 0x00002987
	internal static HandleRef getCPtr(IOnlineQueryStatsListener obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600020B RID: 523 RVA: 0x000047A8 File Offset: 0x000029A8
	~IOnlineQueryStatsListener()
	{
		this.Dispose();
	}

	// Token: 0x0600020C RID: 524 RVA: 0x000047D8 File Offset: 0x000029D8
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineQueryStatsListener(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x0600020D RID: 525 RVA: 0x00004860 File Offset: 0x00002A60
	public virtual void OnQueryStats(SQueryStats stats)
	{
		CryOnlinePINVOKE.IOnlineQueryStatsListener_OnQueryStats(this.swigCPtr, SQueryStats.getCPtr(stats));
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
	}

	// Token: 0x0600020E RID: 526 RVA: 0x00004883 File Offset: 0x00002A83
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("OnQueryStats", IOnlineQueryStatsListener.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineQueryStatsListener.SwigDelegateIOnlineQueryStatsListener_0(this.SwigDirectorOnQueryStats);
		}
		CryOnlinePINVOKE.IOnlineQueryStatsListener_director_connect(this.swigCPtr, this.swigDelegate0);
	}

	// Token: 0x0600020F RID: 527 RVA: 0x000048C0 File Offset: 0x00002AC0
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineQueryStatsListener));
	}

	// Token: 0x06000210 RID: 528 RVA: 0x000048F6 File Offset: 0x00002AF6
	private void SwigDirectorOnQueryStats(IntPtr stats)
	{
		this.OnQueryStats(new SQueryStats(stats, false));
	}

	// Token: 0x040000A7 RID: 167
	private HandleRef swigCPtr;

	// Token: 0x040000A8 RID: 168
	protected bool swigCMemOwn;

	// Token: 0x040000A9 RID: 169
	private IOnlineQueryStatsListener.SwigDelegateIOnlineQueryStatsListener_0 swigDelegate0;

	// Token: 0x040000AA RID: 170
	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(SQueryStats)
	};

	// Token: 0x02000038 RID: 56
	// (Invoke) Token: 0x06000213 RID: 531
	public delegate void SwigDelegateIOnlineQueryStatsListener_0(IntPtr stats);
}

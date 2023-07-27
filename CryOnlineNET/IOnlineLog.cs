using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x0200002E RID: 46
public class IOnlineLog : IDisposable
{
	// Token: 0x060001CE RID: 462 RVA: 0x00004111 File Offset: 0x00002311
	internal IOnlineLog(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x060001CF RID: 463 RVA: 0x0000412D File Offset: 0x0000232D
	public IOnlineLog() : this(CryOnlinePINVOKE.new_IOnlineLog(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x060001D0 RID: 464 RVA: 0x00004141 File Offset: 0x00002341
	internal static HandleRef getCPtr(IOnlineLog obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x060001D1 RID: 465 RVA: 0x00004160 File Offset: 0x00002360
	~IOnlineLog()
	{
		this.Dispose();
	}

	// Token: 0x060001D2 RID: 466 RVA: 0x00004190 File Offset: 0x00002390
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineLog(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x060001D3 RID: 467 RVA: 0x00004218 File Offset: 0x00002418
	public virtual void OnLogMessage(EOnlineLogLevel level, string message)
	{
		CryOnlinePINVOKE.IOnlineLog_OnLogMessage(this.swigCPtr, (int)level, message);
	}

	// Token: 0x060001D4 RID: 468 RVA: 0x00004227 File Offset: 0x00002427
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("OnLogMessage", IOnlineLog.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineLog.SwigDelegateIOnlineLog_0(this.SwigDirectorOnLogMessage);
		}
		CryOnlinePINVOKE.IOnlineLog_director_connect(this.swigCPtr, this.swigDelegate0);
	}

	// Token: 0x060001D5 RID: 469 RVA: 0x00004264 File Offset: 0x00002464
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineLog));
	}

	// Token: 0x060001D6 RID: 470 RVA: 0x0000429A File Offset: 0x0000249A
	private void SwigDirectorOnLogMessage(int level, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string message)
	{
		this.OnLogMessage((EOnlineLogLevel)level, message);
	}

	// Token: 0x04000095 RID: 149
	private HandleRef swigCPtr;

	// Token: 0x04000096 RID: 150
	protected bool swigCMemOwn;

	// Token: 0x04000097 RID: 151
	private IOnlineLog.SwigDelegateIOnlineLog_0 swigDelegate0;

	// Token: 0x04000098 RID: 152
	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(EOnlineLogLevel),
		typeof(string)
	};

	// Token: 0x0200002F RID: 47
	// (Invoke) Token: 0x060001D9 RID: 473
	public delegate void SwigDelegateIOnlineLog_0(int level, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string message);
}

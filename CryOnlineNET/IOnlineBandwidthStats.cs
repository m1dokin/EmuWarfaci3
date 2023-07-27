using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x02000018 RID: 24
public class IOnlineBandwidthStats : IDisposable
{
	// Token: 0x06000108 RID: 264 RVA: 0x00002943 File Offset: 0x00000B43
	internal IOnlineBandwidthStats(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000109 RID: 265 RVA: 0x0000295F File Offset: 0x00000B5F
	public IOnlineBandwidthStats() : this(CryOnlinePINVOKE.new_IOnlineBandwidthStats(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x0600010A RID: 266 RVA: 0x00002973 File Offset: 0x00000B73
	internal static HandleRef getCPtr(IOnlineBandwidthStats obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600010B RID: 267 RVA: 0x00002994 File Offset: 0x00000B94
	~IOnlineBandwidthStats()
	{
		this.Dispose();
	}

	// Token: 0x0600010C RID: 268 RVA: 0x000029C4 File Offset: 0x00000BC4
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineBandwidthStats(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x0600010D RID: 269 RVA: 0x00002A4C File Offset: 0x00000C4C
	public virtual int GetTotalBytesSent()
	{
		return CryOnlinePINVOKE.IOnlineBandwidthStats_GetTotalBytesSent(this.swigCPtr);
	}

	// Token: 0x0600010E RID: 270 RVA: 0x00002A68 File Offset: 0x00000C68
	public virtual int GetTotalBytesRecv()
	{
		return CryOnlinePINVOKE.IOnlineBandwidthStats_GetTotalBytesRecv(this.swigCPtr);
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00002A84 File Offset: 0x00000C84
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("GetTotalBytesSent", IOnlineBandwidthStats.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineBandwidthStats.SwigDelegateIOnlineBandwidthStats_0(this.SwigDirectorGetTotalBytesSent);
		}
		if (this.SwigDerivedClassHasMethod("GetTotalBytesRecv", IOnlineBandwidthStats.swigMethodTypes1))
		{
			this.swigDelegate1 = new IOnlineBandwidthStats.SwigDelegateIOnlineBandwidthStats_1(this.SwigDirectorGetTotalBytesRecv);
		}
		CryOnlinePINVOKE.IOnlineBandwidthStats_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1);
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00002AF8 File Offset: 0x00000CF8
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineBandwidthStats));
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00002B2E File Offset: 0x00000D2E
	private int SwigDirectorGetTotalBytesSent()
	{
		return this.GetTotalBytesSent();
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00002B36 File Offset: 0x00000D36
	private int SwigDirectorGetTotalBytesRecv()
	{
		return this.GetTotalBytesRecv();
	}

	// Token: 0x04000069 RID: 105
	private HandleRef swigCPtr;

	// Token: 0x0400006A RID: 106
	protected bool swigCMemOwn;

	// Token: 0x0400006B RID: 107
	private IOnlineBandwidthStats.SwigDelegateIOnlineBandwidthStats_0 swigDelegate0;

	// Token: 0x0400006C RID: 108
	private IOnlineBandwidthStats.SwigDelegateIOnlineBandwidthStats_1 swigDelegate1;

	// Token: 0x0400006D RID: 109
	private static Type[] swigMethodTypes0 = new Type[0];

	// Token: 0x0400006E RID: 110
	private static Type[] swigMethodTypes1 = new Type[0];

	// Token: 0x02000019 RID: 25
	// (Invoke) Token: 0x06000115 RID: 277
	public delegate int SwigDelegateIOnlineBandwidthStats_0();

	// Token: 0x0200001A RID: 26
	// (Invoke) Token: 0x06000119 RID: 281
	public delegate int SwigDelegateIOnlineBandwidthStats_1();
}

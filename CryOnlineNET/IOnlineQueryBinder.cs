using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x02000030 RID: 48
public class IOnlineQueryBinder : IDisposable
{
	// Token: 0x060001DC RID: 476 RVA: 0x000042CB File Offset: 0x000024CB
	internal IOnlineQueryBinder(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x060001DD RID: 477 RVA: 0x000042E7 File Offset: 0x000024E7
	public IOnlineQueryBinder() : this(CryOnlinePINVOKE.new_IOnlineQueryBinder(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x060001DE RID: 478 RVA: 0x000042FB File Offset: 0x000024FB
	internal static HandleRef getCPtr(IOnlineQueryBinder obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x060001DF RID: 479 RVA: 0x0000431C File Offset: 0x0000251C
	~IOnlineQueryBinder()
	{
		this.Dispose();
	}

	// Token: 0x060001E0 RID: 480 RVA: 0x0000434C File Offset: 0x0000254C
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineQueryBinder(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x060001E1 RID: 481 RVA: 0x000043D4 File Offset: 0x000025D4
	public virtual string Tag()
	{
		return CryOnlinePINVOKE.IOnlineQueryBinder_Tag(this.swigCPtr);
	}

	// Token: 0x060001E2 RID: 482 RVA: 0x000043F0 File Offset: 0x000025F0
	public virtual ECompressType GetCompressionType()
	{
		return (ECompressType)CryOnlinePINVOKE.IOnlineQueryBinder_GetCompressionType(this.swigCPtr);
	}

	// Token: 0x060001E3 RID: 483 RVA: 0x0000440C File Offset: 0x0000260C
	public virtual string GetReceiverId()
	{
		return CryOnlinePINVOKE.IOnlineQueryBinder_GetReceiverId(this.swigCPtr);
	}

	// Token: 0x060001E4 RID: 484 RVA: 0x00004426 File Offset: 0x00002626
	public virtual void OnQueryCompleted(IOnlineConnection connection, SOnlineQuery query, string payload)
	{
		CryOnlinePINVOKE.IOnlineQueryBinder_OnQueryCompleted(this.swigCPtr, IOnlineConnection.getCPtr(connection), SOnlineQuery.getCPtr(query), payload);
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
	}

	// Token: 0x060001E5 RID: 485 RVA: 0x00004450 File Offset: 0x00002650
	public virtual void OnRequest(IOnlineConnection connection, SOnlineQuery query, string payload)
	{
		CryOnlinePINVOKE.IOnlineQueryBinder_OnRequest(this.swigCPtr, IOnlineConnection.getCPtr(connection), SOnlineQuery.getCPtr(query), payload);
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
	}

	// Token: 0x060001E6 RID: 486 RVA: 0x0000447A File Offset: 0x0000267A
	public virtual void OnQueryError(IOnlineConnection connection, SQueryError error)
	{
		CryOnlinePINVOKE.IOnlineQueryBinder_OnQueryError(this.swigCPtr, IOnlineConnection.getCPtr(connection), SQueryError.getCPtr(error));
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
	}

	// Token: 0x060001E7 RID: 487 RVA: 0x000044A4 File Offset: 0x000026A4
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("Tag", IOnlineQueryBinder.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_0(this.SwigDirectorTag);
		}
		if (this.SwigDerivedClassHasMethod("GetCompressionType", IOnlineQueryBinder.swigMethodTypes1))
		{
			this.swigDelegate1 = new IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_1(this.SwigDirectorGetCompressionType);
		}
		if (this.SwigDerivedClassHasMethod("GetReceiverId", IOnlineQueryBinder.swigMethodTypes2))
		{
			this.swigDelegate2 = new IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_2(this.SwigDirectorGetReceiverId);
		}
		if (this.SwigDerivedClassHasMethod("OnQueryCompleted", IOnlineQueryBinder.swigMethodTypes3))
		{
			this.swigDelegate3 = new IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_3(this.SwigDirectorOnQueryCompleted);
		}
		if (this.SwigDerivedClassHasMethod("OnRequest", IOnlineQueryBinder.swigMethodTypes4))
		{
			this.swigDelegate4 = new IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_4(this.SwigDirectorOnRequest);
		}
		if (this.SwigDerivedClassHasMethod("OnQueryError", IOnlineQueryBinder.swigMethodTypes5))
		{
			this.swigDelegate5 = new IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_5(this.SwigDirectorOnQueryError);
		}
		CryOnlinePINVOKE.IOnlineQueryBinder_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1, this.swigDelegate2, this.swigDelegate3, this.swigDelegate4, this.swigDelegate5);
	}

	// Token: 0x060001E8 RID: 488 RVA: 0x000045CC File Offset: 0x000027CC
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineQueryBinder));
	}

	// Token: 0x060001E9 RID: 489 RVA: 0x00004602 File Offset: 0x00002802
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	private string SwigDirectorTag()
	{
		return this.Tag();
	}

	// Token: 0x060001EA RID: 490 RVA: 0x0000460A File Offset: 0x0000280A
	private int SwigDirectorGetCompressionType()
	{
		return (int)this.GetCompressionType();
	}

	// Token: 0x060001EB RID: 491 RVA: 0x00004612 File Offset: 0x00002812
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	private string SwigDirectorGetReceiverId()
	{
		return this.GetReceiverId();
	}

	// Token: 0x060001EC RID: 492 RVA: 0x0000461A File Offset: 0x0000281A
	private void SwigDirectorOnQueryCompleted(IntPtr connection, IntPtr query, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string payload)
	{
		this.OnQueryCompleted((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null, new SOnlineQuery(query, false), payload);
	}

	// Token: 0x060001ED RID: 493 RVA: 0x00004647 File Offset: 0x00002847
	private void SwigDirectorOnRequest(IntPtr connection, IntPtr query, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string payload)
	{
		this.OnRequest((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null, new SOnlineQuery(query, false), payload);
	}

	// Token: 0x060001EE RID: 494 RVA: 0x00004674 File Offset: 0x00002874
	private void SwigDirectorOnQueryError(IntPtr connection, IntPtr error)
	{
		this.OnQueryError((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null, new SQueryError(error, false));
	}

	// Token: 0x04000099 RID: 153
	private HandleRef swigCPtr;

	// Token: 0x0400009A RID: 154
	protected bool swigCMemOwn;

	// Token: 0x0400009B RID: 155
	private IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_0 swigDelegate0;

	// Token: 0x0400009C RID: 156
	private IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_1 swigDelegate1;

	// Token: 0x0400009D RID: 157
	private IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_2 swigDelegate2;

	// Token: 0x0400009E RID: 158
	private IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_3 swigDelegate3;

	// Token: 0x0400009F RID: 159
	private IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_4 swigDelegate4;

	// Token: 0x040000A0 RID: 160
	private IOnlineQueryBinder.SwigDelegateIOnlineQueryBinder_5 swigDelegate5;

	// Token: 0x040000A1 RID: 161
	private static Type[] swigMethodTypes0 = new Type[0];

	// Token: 0x040000A2 RID: 162
	private static Type[] swigMethodTypes1 = new Type[0];

	// Token: 0x040000A3 RID: 163
	private static Type[] swigMethodTypes2 = new Type[0];

	// Token: 0x040000A4 RID: 164
	private static Type[] swigMethodTypes3 = new Type[]
	{
		typeof(IOnlineConnection),
		typeof(SOnlineQuery),
		typeof(string)
	};

	// Token: 0x040000A5 RID: 165
	private static Type[] swigMethodTypes4 = new Type[]
	{
		typeof(IOnlineConnection),
		typeof(SOnlineQuery),
		typeof(string)
	};

	// Token: 0x040000A6 RID: 166
	private static Type[] swigMethodTypes5 = new Type[]
	{
		typeof(IOnlineConnection),
		typeof(SQueryError)
	};

	// Token: 0x02000031 RID: 49
	// (Invoke) Token: 0x060001F1 RID: 497
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public delegate string SwigDelegateIOnlineQueryBinder_0();

	// Token: 0x02000032 RID: 50
	// (Invoke) Token: 0x060001F5 RID: 501
	public delegate int SwigDelegateIOnlineQueryBinder_1();

	// Token: 0x02000033 RID: 51
	// (Invoke) Token: 0x060001F9 RID: 505
	[return: MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)]
	public delegate string SwigDelegateIOnlineQueryBinder_2();

	// Token: 0x02000034 RID: 52
	// (Invoke) Token: 0x060001FD RID: 509
	public delegate void SwigDelegateIOnlineQueryBinder_3(IntPtr connection, IntPtr query, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string payload);

	// Token: 0x02000035 RID: 53
	// (Invoke) Token: 0x06000201 RID: 513
	public delegate void SwigDelegateIOnlineQueryBinder_4(IntPtr connection, IntPtr query, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string payload);

	// Token: 0x02000036 RID: 54
	// (Invoke) Token: 0x06000205 RID: 517
	public delegate void SwigDelegateIOnlineQueryBinder_5(IntPtr connection, IntPtr error);
}

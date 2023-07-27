using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x02000025 RID: 37
public class IOnlineConnectionListener : IDisposable
{
	// Token: 0x06000194 RID: 404 RVA: 0x000039CF File Offset: 0x00001BCF
	internal IOnlineConnectionListener(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000195 RID: 405 RVA: 0x000039EB File Offset: 0x00001BEB
	public IOnlineConnectionListener() : this(CryOnlinePINVOKE.new_IOnlineConnectionListener(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x06000196 RID: 406 RVA: 0x000039FF File Offset: 0x00001BFF
	internal static HandleRef getCPtr(IOnlineConnectionListener obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000197 RID: 407 RVA: 0x00003A20 File Offset: 0x00001C20
	~IOnlineConnectionListener()
	{
		this.Dispose();
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00003A50 File Offset: 0x00001C50
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineConnectionListener(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00003AD8 File Offset: 0x00001CD8
	public virtual void OnConnectionAvailable(IOnlineConnection connection)
	{
		CryOnlinePINVOKE.IOnlineConnectionListener_OnConnectionAvailable(this.swigCPtr, IOnlineConnection.getCPtr(connection));
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00003AEB File Offset: 0x00001CEB
	public virtual void OnConnectionLost(IOnlineConnection connection, EOnlineError reason, string errorDesc)
	{
		CryOnlinePINVOKE.IOnlineConnectionListener_OnConnectionLost(this.swigCPtr, IOnlineConnection.getCPtr(connection), (int)reason, errorDesc);
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00003B00 File Offset: 0x00001D00
	public virtual void OnConnectionTick(IOnlineConnection connection)
	{
		CryOnlinePINVOKE.IOnlineConnectionListener_OnConnectionTick(this.swigCPtr, IOnlineConnection.getCPtr(connection));
	}

	// Token: 0x0600019C RID: 412 RVA: 0x00003B13 File Offset: 0x00001D13
	public virtual void OnPresence(IOnlineConnection connection, string online_id, EOnlinePresence presence)
	{
		CryOnlinePINVOKE.IOnlineConnectionListener_OnPresence(this.swigCPtr, IOnlineConnection.getCPtr(connection), online_id, (int)presence);
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00003B28 File Offset: 0x00001D28
	public virtual void OnUserStatus(IOnlineConnection connection, string user_id, string prev_status, string new_status)
	{
		CryOnlinePINVOKE.IOnlineConnectionListener_OnUserStatus(this.swigCPtr, IOnlineConnection.getCPtr(connection), user_id, prev_status, new_status);
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00003B40 File Offset: 0x00001D40
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("OnConnectionAvailable", IOnlineConnectionListener.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_0(this.SwigDirectorOnConnectionAvailable);
		}
		if (this.SwigDerivedClassHasMethod("OnConnectionLost", IOnlineConnectionListener.swigMethodTypes1))
		{
			this.swigDelegate1 = new IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_1(this.SwigDirectorOnConnectionLost);
		}
		if (this.SwigDerivedClassHasMethod("OnConnectionTick", IOnlineConnectionListener.swigMethodTypes2))
		{
			this.swigDelegate2 = new IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_2(this.SwigDirectorOnConnectionTick);
		}
		if (this.SwigDerivedClassHasMethod("OnPresence", IOnlineConnectionListener.swigMethodTypes3))
		{
			this.swigDelegate3 = new IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_3(this.SwigDirectorOnPresence);
		}
		if (this.SwigDerivedClassHasMethod("OnUserStatus", IOnlineConnectionListener.swigMethodTypes4))
		{
			this.swigDelegate4 = new IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_4(this.SwigDirectorOnUserStatus);
		}
		CryOnlinePINVOKE.IOnlineConnectionListener_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1, this.swigDelegate2, this.swigDelegate3, this.swigDelegate4);
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00003C3C File Offset: 0x00001E3C
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineConnectionListener));
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00003C72 File Offset: 0x00001E72
	private void SwigDirectorOnConnectionAvailable(IntPtr connection)
	{
		this.OnConnectionAvailable((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null);
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00003C97 File Offset: 0x00001E97
	private void SwigDirectorOnConnectionLost(IntPtr connection, int reason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string errorDesc)
	{
		this.OnConnectionLost((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null, (EOnlineError)reason, errorDesc);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00003CBE File Offset: 0x00001EBE
	private void SwigDirectorOnConnectionTick(IntPtr connection)
	{
		this.OnConnectionTick((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00003CE3 File Offset: 0x00001EE3
	private void SwigDirectorOnPresence(IntPtr connection, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string online_id, int presence)
	{
		this.OnPresence((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null, online_id, (EOnlinePresence)presence);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00003D0A File Offset: 0x00001F0A
	private void SwigDirectorOnUserStatus(IntPtr connection, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string user_id, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string prev_status, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string new_status)
	{
		this.OnUserStatus((!(connection == IntPtr.Zero)) ? new IOnlineConnection(connection, false) : null, user_id, prev_status, new_status);
	}

	// Token: 0x04000083 RID: 131
	private HandleRef swigCPtr;

	// Token: 0x04000084 RID: 132
	protected bool swigCMemOwn;

	// Token: 0x04000085 RID: 133
	private IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_0 swigDelegate0;

	// Token: 0x04000086 RID: 134
	private IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_1 swigDelegate1;

	// Token: 0x04000087 RID: 135
	private IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_2 swigDelegate2;

	// Token: 0x04000088 RID: 136
	private IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_3 swigDelegate3;

	// Token: 0x04000089 RID: 137
	private IOnlineConnectionListener.SwigDelegateIOnlineConnectionListener_4 swigDelegate4;

	// Token: 0x0400008A RID: 138
	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(IOnlineConnection)
	};

	// Token: 0x0400008B RID: 139
	private static Type[] swigMethodTypes1 = new Type[]
	{
		typeof(IOnlineConnection),
		typeof(EOnlineError),
		typeof(string)
	};

	// Token: 0x0400008C RID: 140
	private static Type[] swigMethodTypes2 = new Type[]
	{
		typeof(IOnlineConnection)
	};

	// Token: 0x0400008D RID: 141
	private static Type[] swigMethodTypes3 = new Type[]
	{
		typeof(IOnlineConnection),
		typeof(string),
		typeof(EOnlinePresence)
	};

	// Token: 0x0400008E RID: 142
	private static Type[] swigMethodTypes4 = new Type[]
	{
		typeof(IOnlineConnection),
		typeof(string),
		typeof(string),
		typeof(string)
	};

	// Token: 0x02000026 RID: 38
	// (Invoke) Token: 0x060001A7 RID: 423
	public delegate void SwigDelegateIOnlineConnectionListener_0(IntPtr connection);

	// Token: 0x02000027 RID: 39
	// (Invoke) Token: 0x060001AB RID: 427
	public delegate void SwigDelegateIOnlineConnectionListener_1(IntPtr connection, int reason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string errorDesc);

	// Token: 0x02000028 RID: 40
	// (Invoke) Token: 0x060001AF RID: 431
	public delegate void SwigDelegateIOnlineConnectionListener_2(IntPtr connection);

	// Token: 0x02000029 RID: 41
	// (Invoke) Token: 0x060001B3 RID: 435
	public delegate void SwigDelegateIOnlineConnectionListener_3(IntPtr connection, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string online_id, int presence);

	// Token: 0x0200002A RID: 42
	// (Invoke) Token: 0x060001B7 RID: 439
	public delegate void SwigDelegateIOnlineConnectionListener_4(IntPtr connection, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string user_id, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string prev_status, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string new_status);
}

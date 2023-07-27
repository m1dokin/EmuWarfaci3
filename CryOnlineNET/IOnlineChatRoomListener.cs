using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x0200001F RID: 31
public class IOnlineChatRoomListener : IDisposable
{
	// Token: 0x0600013F RID: 319 RVA: 0x00003046 File Offset: 0x00001246
	internal IOnlineChatRoomListener(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00003062 File Offset: 0x00001262
	public IOnlineChatRoomListener() : this(CryOnlinePINVOKE.new_IOnlineChatRoomListener(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00003076 File Offset: 0x00001276
	internal static HandleRef getCPtr(IOnlineChatRoomListener obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00003094 File Offset: 0x00001294
	~IOnlineChatRoomListener()
	{
		this.Dispose();
	}

	// Token: 0x06000143 RID: 323 RVA: 0x000030C4 File Offset: 0x000012C4
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineChatRoomListener(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000314C File Offset: 0x0000134C
	public virtual void OnChatRoomMessage(string nick, string message, bool isPrivate)
	{
		CryOnlinePINVOKE.IOnlineChatRoomListener_OnChatRoomMessage(this.swigCPtr, nick, message, isPrivate);
	}

	// Token: 0x06000145 RID: 325 RVA: 0x0000315C File Offset: 0x0000135C
	public virtual void OnChatRoomPresence(string nick, EOnlinePresence presence)
	{
		CryOnlinePINVOKE.IOnlineChatRoomListener_OnChatRoomPresence(this.swigCPtr, nick, (int)presence);
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000316B File Offset: 0x0000136B
	public virtual void OnChatRoomParticipant(string nick, bool isMoreAvailable)
	{
		CryOnlinePINVOKE.IOnlineChatRoomListener_OnChatRoomParticipant(this.swigCPtr, nick, isMoreAvailable);
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0000317C File Offset: 0x0000137C
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("OnChatRoomMessage", IOnlineChatRoomListener.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_0(this.SwigDirectorOnChatRoomMessage);
		}
		if (this.SwigDerivedClassHasMethod("OnChatRoomPresence", IOnlineChatRoomListener.swigMethodTypes1))
		{
			this.swigDelegate1 = new IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_1(this.SwigDirectorOnChatRoomPresence);
		}
		if (this.SwigDerivedClassHasMethod("OnChatRoomParticipant", IOnlineChatRoomListener.swigMethodTypes2))
		{
			this.swigDelegate2 = new IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_2(this.SwigDirectorOnChatRoomParticipant);
		}
		CryOnlinePINVOKE.IOnlineChatRoomListener_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1, this.swigDelegate2);
	}

	// Token: 0x06000148 RID: 328 RVA: 0x0000321C File Offset: 0x0000141C
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineChatRoomListener));
	}

	// Token: 0x06000149 RID: 329 RVA: 0x00003252 File Offset: 0x00001452
	private void SwigDirectorOnChatRoomMessage([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string nick, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string message, bool isPrivate)
	{
		this.OnChatRoomMessage(nick, message, isPrivate);
	}

	// Token: 0x0600014A RID: 330 RVA: 0x0000325D File Offset: 0x0000145D
	private void SwigDirectorOnChatRoomPresence([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string nick, int presence)
	{
		this.OnChatRoomPresence(nick, (EOnlinePresence)presence);
	}

	// Token: 0x0600014B RID: 331 RVA: 0x00003267 File Offset: 0x00001467
	private void SwigDirectorOnChatRoomParticipant([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string nick, bool isMoreAvailable)
	{
		this.OnChatRoomParticipant(nick, isMoreAvailable);
	}

	// Token: 0x04000077 RID: 119
	private HandleRef swigCPtr;

	// Token: 0x04000078 RID: 120
	protected bool swigCMemOwn;

	// Token: 0x04000079 RID: 121
	private IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_0 swigDelegate0;

	// Token: 0x0400007A RID: 122
	private IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_1 swigDelegate1;

	// Token: 0x0400007B RID: 123
	private IOnlineChatRoomListener.SwigDelegateIOnlineChatRoomListener_2 swigDelegate2;

	// Token: 0x0400007C RID: 124
	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(string),
		typeof(string),
		typeof(bool)
	};

	// Token: 0x0400007D RID: 125
	private static Type[] swigMethodTypes1 = new Type[]
	{
		typeof(string),
		typeof(EOnlinePresence)
	};

	// Token: 0x0400007E RID: 126
	private static Type[] swigMethodTypes2 = new Type[]
	{
		typeof(string),
		typeof(bool)
	};

	// Token: 0x02000020 RID: 32
	// (Invoke) Token: 0x0600014E RID: 334
	public delegate void SwigDelegateIOnlineChatRoomListener_0([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string nick, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string message, bool isPrivate);

	// Token: 0x02000021 RID: 33
	// (Invoke) Token: 0x06000152 RID: 338
	public delegate void SwigDelegateIOnlineChatRoomListener_1([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string nick, int presence);

	// Token: 0x02000022 RID: 34
	// (Invoke) Token: 0x06000156 RID: 342
	public delegate void SwigDelegateIOnlineChatRoomListener_2([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string nick, bool isMoreAvailable);
}

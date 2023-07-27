using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x0200001C RID: 28
public class IOnlineChatListener : IDisposable
{
	// Token: 0x06000125 RID: 293 RVA: 0x00002D0D File Offset: 0x00000F0D
	internal IOnlineChatListener(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000126 RID: 294 RVA: 0x00002D29 File Offset: 0x00000F29
	public IOnlineChatListener() : this(CryOnlinePINVOKE.new_IOnlineChatListener(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x06000127 RID: 295 RVA: 0x00002D3D File Offset: 0x00000F3D
	internal static HandleRef getCPtr(IOnlineChatListener obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00002D5C File Offset: 0x00000F5C
	~IOnlineChatListener()
	{
		this.Dispose();
	}

	// Token: 0x06000129 RID: 297 RVA: 0x00002D8C File Offset: 0x00000F8C
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineChatListener(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x0600012A RID: 298 RVA: 0x00002E14 File Offset: 0x00001014
	public virtual void OnChatRoomDiscovered(string roomId)
	{
		CryOnlinePINVOKE.IOnlineChatListener_OnChatRoomDiscovered(this.swigCPtr, roomId);
	}

	// Token: 0x0600012B RID: 299 RVA: 0x00002E22 File Offset: 0x00001022
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("OnChatRoomDiscovered", IOnlineChatListener.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineChatListener.SwigDelegateIOnlineChatListener_0(this.SwigDirectorOnChatRoomDiscovered);
		}
		CryOnlinePINVOKE.IOnlineChatListener_director_connect(this.swigCPtr, this.swigDelegate0);
	}

	// Token: 0x0600012C RID: 300 RVA: 0x00002E5C File Offset: 0x0000105C
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineChatListener));
	}

	// Token: 0x0600012D RID: 301 RVA: 0x00002E92 File Offset: 0x00001092
	private void SwigDirectorOnChatRoomDiscovered([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string roomId)
	{
		this.OnChatRoomDiscovered(roomId);
	}

	// Token: 0x04000071 RID: 113
	private HandleRef swigCPtr;

	// Token: 0x04000072 RID: 114
	protected bool swigCMemOwn;

	// Token: 0x04000073 RID: 115
	private IOnlineChatListener.SwigDelegateIOnlineChatListener_0 swigDelegate0;

	// Token: 0x04000074 RID: 116
	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(string)
	};

	// Token: 0x0200001D RID: 29
	// (Invoke) Token: 0x06000130 RID: 304
	public delegate void SwigDelegateIOnlineChatListener_0([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string roomId);
}

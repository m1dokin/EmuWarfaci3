using System;
using System.Runtime.InteropServices;

// Token: 0x0200001B RID: 27
public class IOnlineChat : IDisposable
{
	// Token: 0x0600011C RID: 284 RVA: 0x00002B56 File Offset: 0x00000D56
	internal IOnlineChat(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x0600011D RID: 285 RVA: 0x00002B72 File Offset: 0x00000D72
	internal static HandleRef getCPtr(IOnlineChat obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x0600011E RID: 286 RVA: 0x00002B90 File Offset: 0x00000D90
	~IOnlineChat()
	{
		this.Dispose();
	}

	// Token: 0x0600011F RID: 287 RVA: 0x00002BC0 File Offset: 0x00000DC0
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineChat(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x06000120 RID: 288 RVA: 0x00002C48 File Offset: 0x00000E48
	public virtual bool RegisterListener(IOnlineChatListener listener)
	{
		return CryOnlinePINVOKE.IOnlineChat_RegisterListener(this.swigCPtr, IOnlineChatListener.getCPtr(listener));
	}

	// Token: 0x06000121 RID: 289 RVA: 0x00002C68 File Offset: 0x00000E68
	public virtual bool UnregisterListener(IOnlineChatListener listener)
	{
		return CryOnlinePINVOKE.IOnlineChat_UnregisterListener(this.swigCPtr, IOnlineChatListener.getCPtr(listener));
	}

	// Token: 0x06000122 RID: 290 RVA: 0x00002C88 File Offset: 0x00000E88
	public virtual void DiscoverChatRooms()
	{
		CryOnlinePINVOKE.IOnlineChat_DiscoverChatRooms(this.swigCPtr);
	}

	// Token: 0x06000123 RID: 291 RVA: 0x00002C98 File Offset: 0x00000E98
	public virtual IOnlineChatRoom GetChatRoom(string roomId, string nickname)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnlineChat_GetChatRoom__SWIG_0(this.swigCPtr, roomId, nickname);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineChatRoom(intPtr, false) : null;
	}

	// Token: 0x06000124 RID: 292 RVA: 0x00002CD4 File Offset: 0x00000ED4
	public virtual IOnlineChatRoom GetChatRoom(string roomId)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IOnlineChat_GetChatRoom__SWIG_1(this.swigCPtr, roomId);
		return (!(intPtr == IntPtr.Zero)) ? new IOnlineChatRoom(intPtr, false) : null;
	}

	// Token: 0x0400006F RID: 111
	private HandleRef swigCPtr;

	// Token: 0x04000070 RID: 112
	protected bool swigCMemOwn;
}

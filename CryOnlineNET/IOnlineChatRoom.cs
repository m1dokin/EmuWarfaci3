using System;
using System.Runtime.InteropServices;

// Token: 0x0200001E RID: 30
public class IOnlineChatRoom : IDisposable
{
	// Token: 0x06000133 RID: 307 RVA: 0x00002EB5 File Offset: 0x000010B5
	internal IOnlineChatRoom(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000134 RID: 308 RVA: 0x00002ED1 File Offset: 0x000010D1
	internal static HandleRef getCPtr(IOnlineChatRoom obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000135 RID: 309 RVA: 0x00002EF0 File Offset: 0x000010F0
	~IOnlineChatRoom()
	{
		this.Dispose();
	}

	// Token: 0x06000136 RID: 310 RVA: 0x00002F20 File Offset: 0x00001120
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineChatRoom(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00002FA8 File Offset: 0x000011A8
	public virtual bool RegisterListener(IOnlineChatRoomListener listener)
	{
		return CryOnlinePINVOKE.IOnlineChatRoom_RegisterListener(this.swigCPtr, IOnlineChatRoomListener.getCPtr(listener));
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00002FC8 File Offset: 0x000011C8
	public virtual bool UnregisterListener(IOnlineChatRoomListener listener)
	{
		return CryOnlinePINVOKE.IOnlineChatRoom_UnregisterListener(this.swigCPtr, IOnlineChatRoomListener.getCPtr(listener));
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00002FE8 File Offset: 0x000011E8
	public virtual bool IsListenersEmpty()
	{
		return CryOnlinePINVOKE.IOnlineChatRoom_IsListenersEmpty(this.swigCPtr);
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00003002 File Offset: 0x00001202
	public virtual void Join()
	{
		CryOnlinePINVOKE.IOnlineChatRoom_Join(this.swigCPtr);
	}

	// Token: 0x0600013B RID: 315 RVA: 0x0000300F File Offset: 0x0000120F
	public virtual void Leave()
	{
		CryOnlinePINVOKE.IOnlineChatRoom_Leave(this.swigCPtr);
	}

	// Token: 0x0600013C RID: 316 RVA: 0x0000301C File Offset: 0x0000121C
	public virtual void EnumerateParticipants()
	{
		CryOnlinePINVOKE.IOnlineChatRoom_EnumerateParticipants(this.swigCPtr);
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00003029 File Offset: 0x00001229
	public virtual void Send(string message)
	{
		CryOnlinePINVOKE.IOnlineChatRoom_Send(this.swigCPtr, message);
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00003037 File Offset: 0x00001237
	public virtual void SendPrivate(string nick, string message)
	{
		CryOnlinePINVOKE.IOnlineChatRoom_SendPrivate(this.swigCPtr, nick, message);
	}

	// Token: 0x04000075 RID: 117
	private HandleRef swigCPtr;

	// Token: 0x04000076 RID: 118
	protected bool swigCMemOwn;
}

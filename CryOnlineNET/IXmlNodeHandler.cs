using System;
using System.Runtime.InteropServices;

// Token: 0x02000039 RID: 57
public class IXmlNodeHandler : IDisposable
{
	// Token: 0x06000216 RID: 534 RVA: 0x0000491F File Offset: 0x00002B1F
	internal IXmlNodeHandler(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x06000217 RID: 535 RVA: 0x0000493B File Offset: 0x00002B3B
	internal static HandleRef getCPtr(IXmlNodeHandler obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x06000218 RID: 536 RVA: 0x0000495C File Offset: 0x00002B5C
	~IXmlNodeHandler()
	{
		this.Dispose();
	}

	// Token: 0x06000219 RID: 537 RVA: 0x0000498C File Offset: 0x00002B8C
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IXmlNodeHandler(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x0600021A RID: 538 RVA: 0x00004A14 File Offset: 0x00002C14
	public virtual string GetTag()
	{
		return CryOnlinePINVOKE.IXmlNodeHandler_GetTag(this.swigCPtr);
	}

	// Token: 0x0600021B RID: 539 RVA: 0x00004A30 File Offset: 0x00002C30
	public virtual string GetContent()
	{
		return CryOnlinePINVOKE.IXmlNodeHandler_GetContent(this.swigCPtr);
	}

	// Token: 0x0600021C RID: 540 RVA: 0x00004A4C File Offset: 0x00002C4C
	public virtual int GetNumAttributes()
	{
		return CryOnlinePINVOKE.IXmlNodeHandler_GetNumAttributes(this.swigCPtr);
	}

	// Token: 0x0600021D RID: 541 RVA: 0x00004A68 File Offset: 0x00002C68
	public virtual bool GetAttributeByIndex(int index, SWIGTYPE_p_p_char key, SWIGTYPE_p_p_char value)
	{
		return CryOnlinePINVOKE.IXmlNodeHandler_GetAttributeByIndex(this.swigCPtr, index, SWIGTYPE_p_p_char.getCPtr(key), SWIGTYPE_p_p_char.getCPtr(value));
	}

	// Token: 0x0600021E RID: 542 RVA: 0x00004A90 File Offset: 0x00002C90
	public virtual int GetChildCount()
	{
		return CryOnlinePINVOKE.IXmlNodeHandler_GetChildCount(this.swigCPtr);
	}

	// Token: 0x0600021F RID: 543 RVA: 0x00004AAC File Offset: 0x00002CAC
	public virtual IXmlNodeHandler GetChild(int i)
	{
		IntPtr intPtr = CryOnlinePINVOKE.IXmlNodeHandler_GetChild(this.swigCPtr, i);
		return (!(intPtr == IntPtr.Zero)) ? new IXmlNodeHandler(intPtr, false) : null;
	}

	// Token: 0x040000AB RID: 171
	private HandleRef swigCPtr;

	// Token: 0x040000AC RID: 172
	protected bool swigCMemOwn;
}

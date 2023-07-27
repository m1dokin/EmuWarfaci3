using System;
using System.Reflection;
using System.Runtime.InteropServices;

// Token: 0x0200002B RID: 43
public class IOnlineDataProtect : IDisposable
{
	// Token: 0x060001BA RID: 442 RVA: 0x00003E14 File Offset: 0x00002014
	internal IOnlineDataProtect(IntPtr cPtr, bool cMemoryOwn)
	{
		this.swigCMemOwn = cMemoryOwn;
		this.swigCPtr = new HandleRef(this, cPtr);
	}

	// Token: 0x060001BB RID: 443 RVA: 0x00003E30 File Offset: 0x00002030
	public IOnlineDataProtect() : this(CryOnlinePINVOKE.new_IOnlineDataProtect(), true)
	{
		this.SwigDirectorConnect();
	}

	// Token: 0x060001BC RID: 444 RVA: 0x00003E44 File Offset: 0x00002044
	internal static HandleRef getCPtr(IOnlineDataProtect obj)
	{
		return (obj != null) ? obj.swigCPtr : new HandleRef(null, IntPtr.Zero);
	}

	// Token: 0x060001BD RID: 445 RVA: 0x00003E64 File Offset: 0x00002064
	~IOnlineDataProtect()
	{
		this.Dispose();
	}

	// Token: 0x060001BE RID: 446 RVA: 0x00003E94 File Offset: 0x00002094
	public virtual void Dispose()
	{
		lock (this)
		{
			if (this.swigCPtr.Handle != IntPtr.Zero && this.swigCMemOwn)
			{
				this.swigCMemOwn = false;
				CryOnlinePINVOKE.delete_IOnlineDataProtect(this.swigCPtr);
			}
			this.swigCPtr = new HandleRef(null, IntPtr.Zero);
			GC.SuppressFinalize(this);
		}
	}

	// Token: 0x060001BF RID: 447 RVA: 0x00003F1C File Offset: 0x0000211C
	public virtual int protectData(string tag, string inBuffer, uint inSize, string outBuffer, SWIGTYPE_p_size_t outSize)
	{
		int result = CryOnlinePINVOKE.IOnlineDataProtect_protectData(this.swigCPtr, tag, inBuffer, inSize, outBuffer, SWIGTYPE_p_size_t.getCPtr(outSize));
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	// Token: 0x060001C0 RID: 448 RVA: 0x00003F54 File Offset: 0x00002154
	public virtual int unprotectData(string inBuffer, uint inSize, SWIGTYPE_p_size_t readSize, string outBuffer, SWIGTYPE_p_size_t outSize)
	{
		int result = CryOnlinePINVOKE.IOnlineDataProtect_unprotectData(this.swigCPtr, inBuffer, inSize, SWIGTYPE_p_size_t.getCPtr(readSize), outBuffer, SWIGTYPE_p_size_t.getCPtr(outSize));
		if (CryOnlinePINVOKE.SWIGPendingException.Pending)
		{
			throw CryOnlinePINVOKE.SWIGPendingException.Retrieve();
		}
		return result;
	}

	// Token: 0x060001C1 RID: 449 RVA: 0x00003F90 File Offset: 0x00002190
	private void SwigDirectorConnect()
	{
		if (this.SwigDerivedClassHasMethod("protectData", IOnlineDataProtect.swigMethodTypes0))
		{
			this.swigDelegate0 = new IOnlineDataProtect.SwigDelegateIOnlineDataProtect_0(this.SwigDirectorprotectData);
		}
		if (this.SwigDerivedClassHasMethod("unprotectData", IOnlineDataProtect.swigMethodTypes1))
		{
			this.swigDelegate1 = new IOnlineDataProtect.SwigDelegateIOnlineDataProtect_1(this.SwigDirectorunprotectData);
		}
		CryOnlinePINVOKE.IOnlineDataProtect_director_connect(this.swigCPtr, this.swigDelegate0, this.swigDelegate1);
	}

	// Token: 0x060001C2 RID: 450 RVA: 0x00004004 File Offset: 0x00002204
	private bool SwigDerivedClassHasMethod(string methodName, Type[] methodTypes)
	{
		MethodInfo method = base.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, methodTypes, null);
		return method.DeclaringType.IsSubclassOf(typeof(IOnlineDataProtect));
	}

	// Token: 0x060001C3 RID: 451 RVA: 0x0000403A File Offset: 0x0000223A
	private int SwigDirectorprotectData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string tag, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string inBuffer, uint inSize, string outBuffer, IntPtr outSize)
	{
		return this.protectData(tag, inBuffer, inSize, outBuffer, new SWIGTYPE_p_size_t(outSize, false));
	}

	// Token: 0x060001C4 RID: 452 RVA: 0x0000404F File Offset: 0x0000224F
	private int SwigDirectorunprotectData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string inBuffer, uint inSize, IntPtr readSize, string outBuffer, IntPtr outSize)
	{
		return this.unprotectData(inBuffer, inSize, new SWIGTYPE_p_size_t(readSize, false), outBuffer, new SWIGTYPE_p_size_t(outSize, false));
	}

	// Token: 0x0400008F RID: 143
	private HandleRef swigCPtr;

	// Token: 0x04000090 RID: 144
	protected bool swigCMemOwn;

	// Token: 0x04000091 RID: 145
	private IOnlineDataProtect.SwigDelegateIOnlineDataProtect_0 swigDelegate0;

	// Token: 0x04000092 RID: 146
	private IOnlineDataProtect.SwigDelegateIOnlineDataProtect_1 swigDelegate1;

	// Token: 0x04000093 RID: 147
	private static Type[] swigMethodTypes0 = new Type[]
	{
		typeof(string),
		typeof(string),
		typeof(uint),
		typeof(string),
		typeof(SWIGTYPE_p_size_t)
	};

	// Token: 0x04000094 RID: 148
	private static Type[] swigMethodTypes1 = new Type[]
	{
		typeof(string),
		typeof(uint),
		typeof(SWIGTYPE_p_size_t),
		typeof(string),
		typeof(SWIGTYPE_p_size_t)
	};

	// Token: 0x0200002C RID: 44
	// (Invoke) Token: 0x060001C7 RID: 455
	public delegate int SwigDelegateIOnlineDataProtect_0([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string tag, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string inBuffer, uint inSize, string outBuffer, IntPtr outSize);

	// Token: 0x0200002D RID: 45
	// (Invoke) Token: 0x060001CB RID: 459
	public delegate int SwigDelegateIOnlineDataProtect_1([MarshalAs(UnmanagedType.CustomMarshaler, MarshalTypeRef = CryOnline/UTF8Marshaler)] string inBuffer, uint inSize, IntPtr readSize, string outBuffer, IntPtr outSize);
}

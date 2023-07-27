using System;
using System.Runtime.InteropServices;
using System.Text;

// Token: 0x02000002 RID: 2
public class CryOnline
{
	// Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
	public static IOnline CryOnlineInit()
	{
		IntPtr intPtr = CryOnlinePINVOKE.CryOnlineInit();
		return (!(intPtr == IntPtr.Zero)) ? new IOnline(intPtr, false) : null;
	}

	// Token: 0x06000003 RID: 3 RVA: 0x0000208A File Offset: 0x0000028A
	public static void CryOnlineShutdown()
	{
		CryOnlinePINVOKE.CryOnlineShutdown();
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00002094 File Offset: 0x00000294
	public static IOnline CryOnlineGetInstance()
	{
		IntPtr intPtr = CryOnlinePINVOKE.CryOnlineGetInstance();
		return (!(intPtr == IntPtr.Zero)) ? new IOnline(intPtr, false) : null;
	}

	// Token: 0x02000003 RID: 3
	public class UTF8Marshaler : ICustomMarshaler
	{
		// Token: 0x06000006 RID: 6 RVA: 0x000020CE File Offset: 0x000002CE
		public static ICustomMarshaler GetInstance(string cookie)
		{
			return CryOnline.UTF8Marshaler.marshaler;
		}

		// Token: 0x06000007 RID: 7 RVA: 0x000020D5 File Offset: 0x000002D5
		public void CleanUpManagedData(object ManagedObj)
		{
		}

		// Token: 0x06000008 RID: 8 RVA: 0x000020D7 File Offset: 0x000002D7
		public void CleanUpNativeData(IntPtr pNativeData)
		{
			Marshal.FreeHGlobal(pNativeData);
		}

		// Token: 0x06000009 RID: 9 RVA: 0x000020DF File Offset: 0x000002DF
		public int GetNativeDataSize()
		{
			return -1;
		}

		// Token: 0x0600000A RID: 10 RVA: 0x000020E4 File Offset: 0x000002E4
		public IntPtr MarshalManagedToNative(object ManagedObj)
		{
			if (ManagedObj == null)
			{
				return IntPtr.Zero;
			}
			string s = (string)ManagedObj;
			return Marshal.StringToHGlobalAnsi(s);
		}

		// Token: 0x0600000B RID: 11 RVA: 0x0000210C File Offset: 0x0000030C
		public object MarshalNativeToManaged(IntPtr pNativeData)
		{
			if (pNativeData == IntPtr.Zero)
			{
				return null;
			}
			string text = Marshal.PtrToStringAnsi(pNativeData);
			if (text == null)
			{
				int num = 0;
				for (byte b = Marshal.ReadByte(pNativeData, num); b > 0; b = Marshal.ReadByte(pNativeData, num))
				{
					num++;
				}
				byte[] array = new byte[num];
				Marshal.Copy(pNativeData, array, 0, num);
				text = Encoding.UTF8.GetString(array);
			}
			return (text == null) ? string.Empty : text;
		}

		// Token: 0x04000001 RID: 1
		private static CryOnline.UTF8Marshaler marshaler = new CryOnline.UTF8Marshaler();
	}
}

using System;
using System.Runtime.InteropServices;
using System.Text;
using HK2Net;

namespace MasterServer.Platform.ProfanityCheck
{
	// Token: 0x020006A7 RID: 1703
	[Service]
	internal class CryDirtyExternal : ICryDirty
	{
		// Token: 0x060023C5 RID: 9157 RVA: 0x0009684C File Offset: 0x00094C4C
		void ICryDirty.Init()
		{
			if (this.m_instance != IntPtr.Zero)
			{
				throw new ApplicationException("Try to init CryDirty lib twice!");
			}
			this.m_instance = CryDirtyExternal.CryDirtyInit();
			if (this.m_instance == IntPtr.Zero)
			{
				throw new Exception("Failed to create CryDirty instance");
			}
		}

		// Token: 0x060023C6 RID: 9158 RVA: 0x000968A4 File Offset: 0x00094CA4
		void ICryDirty.Free()
		{
			if (this.m_instance != IntPtr.Zero)
			{
				CryDirtyExternal.CryDirtyFree(this.m_instance);
			}
		}

		// Token: 0x060023C7 RID: 9159 RVA: 0x000968C6 File Offset: 0x00094CC6
		bool ICryDirty.ReadProfanityDict(string path)
		{
			return CryDirtyExternal.ReadProfanityDict(this.m_instance, path);
		}

		// Token: 0x060023C8 RID: 9160 RVA: 0x000968D4 File Offset: 0x00094CD4
		bool ICryDirty.ReadReservedDict(string path)
		{
			return CryDirtyExternal.ReadReservedDict(this.m_instance, path);
		}

		// Token: 0x060023C9 RID: 9161 RVA: 0x000968E4 File Offset: 0x00094CE4
		bool ICryDirty.HaveProfanityWords(string message)
		{
			bool result;
			using (CryDirtyExternal.Utf8String utf8String = new CryDirtyExternal.Utf8String(message))
			{
				result = CryDirtyExternal.HaveProfanityWords(this.m_instance, utf8String.IntPtr);
			}
			return result;
		}

		// Token: 0x060023CA RID: 9162 RVA: 0x00096930 File Offset: 0x00094D30
		bool ICryDirty.HaveReservedWords(string message)
		{
			bool result;
			using (CryDirtyExternal.Utf8String utf8String = new CryDirtyExternal.Utf8String(message))
			{
				result = CryDirtyExternal.HaveReservedWords(this.m_instance, utf8String.IntPtr);
			}
			return result;
		}

		// Token: 0x060023CB RID: 9163 RVA: 0x0009697C File Offset: 0x00094D7C
		bool ICryDirty.FilterMessage(StringBuilder message)
		{
			string text = message.ToString();
			bool result;
			using (CryDirtyExternal.Utf8String utf8String = new CryDirtyExternal.Utf8String(text))
			{
				bool flag = CryDirtyExternal.FilterMessage(this.m_instance, utf8String.IntPtr);
				if (flag)
				{
					byte[] array = new byte[utf8String.Length];
					Marshal.Copy(utf8String.IntPtr, array, 0, array.Length);
					string @string = Encoding.UTF8.GetString(array);
					message.Replace(text, @string);
				}
				result = flag;
			}
			return result;
		}

		// Token: 0x060023CC RID: 9164
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cry_dirty_init")]
		private static extern IntPtr CryDirtyInit();

		// Token: 0x060023CD RID: 9165
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "cry_dirty_free")]
		private static extern void CryDirtyFree(IntPtr instance);

		// Token: 0x060023CE RID: 9166
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "read_profanity_dict")]
		private static extern bool ReadProfanityDict(IntPtr instance, string profanityDictFile);

		// Token: 0x060023CF RID: 9167
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "read_reserved_dict")]
		private static extern bool ReadReservedDict(IntPtr instance, string reservedDictFile);

		// Token: 0x060023D0 RID: 9168
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "have_profanity_words")]
		private static extern bool HaveProfanityWords(IntPtr instance, IntPtr message);

		// Token: 0x060023D1 RID: 9169
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "have_reserved_words")]
		private static extern bool HaveReservedWords(IntPtr instance, IntPtr message);

		// Token: 0x060023D2 RID: 9170
		[DllImport("CryDirty", CallingConvention = CallingConvention.Cdecl, EntryPoint = "filter_message")]
		private static extern bool FilterMessage(IntPtr instance, IntPtr message);

		// Token: 0x040011F6 RID: 4598
		private const string m_libCryDyirty = "CryDirty";

		// Token: 0x040011F7 RID: 4599
		private IntPtr m_instance;

		// Token: 0x020006A8 RID: 1704
		private class Utf8String : IDisposable
		{
			// Token: 0x060023D3 RID: 9171 RVA: 0x00096A0C File Offset: 0x00094E0C
			public Utf8String(string str)
			{
				if (str != null)
				{
					byte[] bytes = Encoding.UTF8.GetBytes(str);
					this.Length = bytes.Length;
					this.IntPtr = Marshal.AllocHGlobal(this.Length + 1);
					Marshal.Copy(bytes, 0, this.IntPtr, this.Length);
					Marshal.WriteByte(this.IntPtr, this.Length, 0);
				}
				else
				{
					this.IntPtr = IntPtr.Zero;
				}
			}

			// Token: 0x17000373 RID: 883
			// (get) Token: 0x060023D4 RID: 9172 RVA: 0x00096A82 File Offset: 0x00094E82
			// (set) Token: 0x060023D5 RID: 9173 RVA: 0x00096A8A File Offset: 0x00094E8A
			public IntPtr IntPtr { get; private set; }

			// Token: 0x17000374 RID: 884
			// (get) Token: 0x060023D6 RID: 9174 RVA: 0x00096A93 File Offset: 0x00094E93
			// (set) Token: 0x060023D7 RID: 9175 RVA: 0x00096A9B File Offset: 0x00094E9B
			public int Length { get; private set; }

			// Token: 0x060023D8 RID: 9176 RVA: 0x00096AA4 File Offset: 0x00094EA4
			public void Dispose()
			{
				if (this.IntPtr != IntPtr.Zero)
				{
					Marshal.FreeHGlobal(this.IntPtr);
					this.IntPtr = IntPtr.Zero;
				}
			}
		}
	}
}

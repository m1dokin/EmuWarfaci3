using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using MasterServer.DAL.Utils;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000011 RID: 17
	public class DataArchiveSerializer<T>
	{
		// Token: 0x06000086 RID: 134 RVA: 0x00004ED8 File Offset: 0x000030D8
		public DataArchiveSerializer(IDataSerializer<T> ser)
		{
			this.m_serializer = ser;
		}

		// Token: 0x06000087 RID: 135 RVA: 0x00004EE8 File Offset: 0x000030E8
		public byte[] Serialize(T data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Compress))
				{
					using (StreamWriter streamWriter = new StreamWriter(gzipStream, Encoding.UTF8))
					{
						this.m_serializer.Serialize(data, streamWriter);
					}
				}
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000088 RID: 136 RVA: 0x00004F84 File Offset: 0x00003184
		public T Deserialize(byte[] data, DBVersion version)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				using (GZipStream gzipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					using (StreamReader streamReader = new StreamReader(gzipStream, Encoding.UTF8))
					{
						T t = this.m_serializer.Deserialize(streamReader, version);
						result = t;
					}
				}
			}
			return result;
		}

		// Token: 0x04000040 RID: 64
		private readonly IDataSerializer<T> m_serializer;
	}
}

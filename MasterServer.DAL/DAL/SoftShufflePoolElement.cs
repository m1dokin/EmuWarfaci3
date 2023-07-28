using System;

namespace MasterServer.DAL
{
	// Token: 0x02000036 RID: 54
	[Serializable]
	public class SoftShufflePoolElement
	{
		// Token: 0x06000084 RID: 132 RVA: 0x00003519 File Offset: 0x00001919
		public SoftShufflePoolElement(string key)
		{
			this.m_key = key;
		}

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x06000085 RID: 133 RVA: 0x00003528 File Offset: 0x00001928
		// (set) Token: 0x06000086 RID: 134 RVA: 0x00003530 File Offset: 0x00001930
		public string Key
		{
			get
			{
				return this.m_key;
			}
			set
			{
				this.m_key = value;
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000087 RID: 135 RVA: 0x00003539 File Offset: 0x00001939
		// (set) Token: 0x06000088 RID: 136 RVA: 0x00003541 File Offset: 0x00001941
		public int Pos
		{
			get
			{
				return this.m_pos;
			}
			set
			{
				this.m_pos = value;
			}
		}

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000089 RID: 137 RVA: 0x0000354A File Offset: 0x0000194A
		// (set) Token: 0x0600008A RID: 138 RVA: 0x00003552 File Offset: 0x00001952
		public int UsageCount
		{
			get
			{
				return this.m_usageCount;
			}
			set
			{
				this.m_usageCount = value;
			}
		}

		// Token: 0x0600008B RID: 139 RVA: 0x0000355B File Offset: 0x0000195B
		public bool Empty()
		{
			return string.IsNullOrEmpty(this.Key);
		}

		// Token: 0x04000083 RID: 131
		private string m_key;

		// Token: 0x04000084 RID: 132
		private int m_pos;

		// Token: 0x04000085 RID: 133
		private int m_usageCount;
	}
}

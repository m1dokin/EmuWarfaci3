using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.DAL.Utils
{
	// Token: 0x0200009D RID: 157
	public class DBSchema : IComparable<DBSchema>
	{
		// Token: 0x17000038 RID: 56
		// (get) Token: 0x060001FB RID: 507 RVA: 0x00005DF8 File Offset: 0x000041F8
		public DBVersion LatestVersion
		{
			get
			{
				return (this.m_versions.Count == 0) ? DBVersion.Zero : this.m_versions.Last<DBVersion>();
			}
		}

		// Token: 0x17000039 RID: 57
		// (get) Token: 0x060001FC RID: 508 RVA: 0x00005E1F File Offset: 0x0000421F
		public List<DBVersion> Versions
		{
			get
			{
				return this.m_versions;
			}
		}

		// Token: 0x060001FD RID: 509 RVA: 0x00005E27 File Offset: 0x00004227
		public static bool operator ==(DBSchema lhs, DBSchema rhs)
		{
			return lhs.CompareTo(rhs) == 0;
		}

		// Token: 0x060001FE RID: 510 RVA: 0x00005E33 File Offset: 0x00004233
		public static bool operator !=(DBSchema lhs, DBSchema rhs)
		{
			return lhs.CompareTo(rhs) != 0;
		}

		// Token: 0x060001FF RID: 511 RVA: 0x00005E44 File Offset: 0x00004244
		public void Add(DBVersion version)
		{
			if (!this.m_versions.Exists((DBVersion e) => e == version))
			{
				this.m_versions.Add(version);
				this.m_versions.Sort();
			}
		}

		// Token: 0x06000200 RID: 512 RVA: 0x00005E98 File Offset: 0x00004298
		public bool CanBeUpdatedTo(DBSchema newSchema, out IEnumerable<DBVersion> missedVersions)
		{
			List<DBVersion> list = new List<DBVersion>();
			List<DBVersion> list2 = new List<DBVersion>();
			if (!this.LatestVersion.IsZero())
			{
				list2.Add(this.LatestVersion);
			}
			using (List<DBVersion>.Enumerator enumerator = newSchema.Versions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DBVersion version = enumerator.Current;
					if (version.IsMajor() && !this.m_versions.Exists((DBVersion v) => v == version) && version < this.LatestVersion)
					{
						list.Add(version);
					}
					if (version > this.LatestVersion)
					{
						list2.Add(version);
					}
				}
			}
			if (list2.Count > 1)
			{
				list2.Sort();
				for (int i = 0; i < list2.Count - 1; i++)
				{
					DBVersion dbversion = list2[i];
					DBVersion dbversion2 = list2[i + 1];
					while (!dbversion.IsPrevious(dbversion2))
					{
						dbversion2 = dbversion2.GetPrevious();
						list.Add(dbversion2);
					}
				}
			}
			list.Sort();
			missedVersions = list;
			return !missedVersions.Any<DBVersion>();
		}

		// Token: 0x06000201 RID: 513 RVA: 0x00006018 File Offset: 0x00004418
		public int CompareTo(DBSchema other)
		{
			if (this.m_versions.Count != other.m_versions.Count)
			{
				return -1;
			}
			using (List<DBVersion>.Enumerator enumerator = this.m_versions.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					DBVersion v = enumerator.Current;
					if (!other.m_versions.Exists((DBVersion e) => e == v))
					{
						return -1;
					}
				}
			}
			using (List<DBVersion>.Enumerator enumerator2 = other.m_versions.GetEnumerator())
			{
				while (enumerator2.MoveNext())
				{
					DBVersion v = enumerator2.Current;
					if (!this.m_versions.Exists((DBVersion e) => e == v))
					{
						return -1;
					}
				}
			}
			return 0;
		}

		// Token: 0x06000202 RID: 514 RVA: 0x00006130 File Offset: 0x00004530
		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is DBSchema))
			{
				return false;
			}
			DBSchema other = (DBSchema)obj;
			return this.CompareTo(other) == 0;
		}

		// Token: 0x06000203 RID: 515 RVA: 0x0000616C File Offset: 0x0000456C
		public override int GetHashCode()
		{
			int num = 0;
			foreach (DBVersion dbversion in this.m_versions)
			{
				num ^= dbversion.GetHashCode();
			}
			return num;
		}

		// Token: 0x0400018F RID: 399
		private readonly List<DBVersion> m_versions = new List<DBVersion>();
	}
}

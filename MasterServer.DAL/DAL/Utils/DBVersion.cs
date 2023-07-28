using System;
using System.IO;

namespace MasterServer.DAL.Utils
{
	// Token: 0x0200009C RID: 156
	[Serializable]
	public struct DBVersion : IComparable<DBVersion>
	{
		// Token: 0x060001E6 RID: 486 RVA: 0x00005A12 File Offset: 0x00003E12
		public DBVersion(int major, int minor, int fork)
		{
			this.Major = major;
			this.Minor = minor;
			this.Fork = fork;
		}

		// Token: 0x060001E7 RID: 487 RVA: 0x00005A29 File Offset: 0x00003E29
		public DBVersion(int major, int minor)
		{
			this = new DBVersion(major, minor, 0);
		}

		// Token: 0x060001E8 RID: 488 RVA: 0x00005A34 File Offset: 0x00003E34
		public static DBVersion Parse(string str)
		{
			DBVersion result;
			if (!DBVersion.TryParse(str, out result))
			{
				throw new FormatException(string.Format("'{0}' is invalid version format", str));
			}
			return result;
		}

		// Token: 0x060001E9 RID: 489 RVA: 0x00005A60 File Offset: 0x00003E60
		public static bool TryParse(string str, out DBVersion v)
		{
			string[] array = str.Split(new char[]
			{
				'.'
			});
			int fork = 0;
			int major;
			int minor;
			if (int.TryParse(array[0], out major) && int.TryParse(array[1], out minor) && (array.Length == 2 || int.TryParse(array[2], out fork)))
			{
				v = new DBVersion(major, minor, fork);
				return true;
			}
			v = DBVersion.Zero;
			return false;
		}

		// Token: 0x060001EA RID: 490 RVA: 0x00005ACE File Offset: 0x00003ECE
		public static bool operator ==(DBVersion lhs, DBVersion rhs)
		{
			return lhs.CompareTo(rhs) == 0;
		}

		// Token: 0x060001EB RID: 491 RVA: 0x00005ADB File Offset: 0x00003EDB
		public static bool operator !=(DBVersion lhs, DBVersion rhs)
		{
			return lhs.CompareTo(rhs) != 0;
		}

		// Token: 0x060001EC RID: 492 RVA: 0x00005AEB File Offset: 0x00003EEB
		public static bool operator <(DBVersion lhs, DBVersion rhs)
		{
			return lhs.CompareTo(rhs) < 0;
		}

		// Token: 0x060001ED RID: 493 RVA: 0x00005AF8 File Offset: 0x00003EF8
		public static bool operator <=(DBVersion lhs, DBVersion rhs)
		{
			return lhs.CompareTo(rhs) <= 0;
		}

		// Token: 0x060001EE RID: 494 RVA: 0x00005B08 File Offset: 0x00003F08
		public static bool operator >(DBVersion lhs, DBVersion rhs)
		{
			return lhs.CompareTo(rhs) > 0;
		}

		// Token: 0x060001EF RID: 495 RVA: 0x00005B15 File Offset: 0x00003F15
		public static bool operator >=(DBVersion lhs, DBVersion rhs)
		{
			return lhs.CompareTo(rhs) >= 0;
		}

		// Token: 0x060001F0 RID: 496 RVA: 0x00005B28 File Offset: 0x00003F28
		public override string ToString()
		{
			if (this.Fork > 0)
			{
				return string.Format("{0}.{1}.{2}", this.Major, this.Minor, this.Fork);
			}
			return string.Format("{0}.{1}", this.Major, this.Minor);
		}

		// Token: 0x060001F1 RID: 497 RVA: 0x00005B90 File Offset: 0x00003F90
		public int CompareTo(DBVersion other)
		{
			if (this.Major > other.Major)
			{
				return 1;
			}
			if (this.Major != other.Major)
			{
				return -1;
			}
			if (this.Minor > other.Minor)
			{
				return 1;
			}
			if (this.Minor != other.Minor)
			{
				return -1;
			}
			if (this.Fork > other.Fork)
			{
				return 1;
			}
			return (this.Fork == other.Fork) ? 0 : -1;
		}

		// Token: 0x060001F2 RID: 498 RVA: 0x00005C1C File Offset: 0x0000401C
		public override bool Equals(object obj)
		{
			if (!(obj is DBVersion))
			{
				return false;
			}
			DBVersion dbversion = (DBVersion)obj;
			return dbversion.Major == this.Major && dbversion.Minor == this.Minor && dbversion.Fork == this.Fork;
		}

		// Token: 0x060001F3 RID: 499 RVA: 0x00005C73 File Offset: 0x00004073
		public override int GetHashCode()
		{
			return this.Major ^ this.Minor ^ this.Fork;
		}

		// Token: 0x060001F4 RID: 500 RVA: 0x00005C89 File Offset: 0x00004089
		public bool IsZero()
		{
			return this == DBVersion.Zero;
		}

		// Token: 0x060001F5 RID: 501 RVA: 0x00005C9B File Offset: 0x0000409B
		public bool IsMinor()
		{
			return this.Fork > 0;
		}

		// Token: 0x060001F6 RID: 502 RVA: 0x00005CA6 File Offset: 0x000040A6
		public bool IsMajor()
		{
			return !this.IsMinor();
		}

		// Token: 0x060001F7 RID: 503 RVA: 0x00005CB4 File Offset: 0x000040B4
		public bool IsPrevious(DBVersion next)
		{
			if (next.Major == this.Major)
			{
				if (next.Minor == this.Minor)
				{
					return next.Fork - this.Fork == 1;
				}
				if (next.Minor - this.Minor == 1)
				{
					return next.Fork == 0;
				}
			}
			else if (next.Major - this.Major == 1)
			{
				return next.Minor == 0 && next.Fork == 0;
			}
			return false;
		}

		// Token: 0x060001F8 RID: 504 RVA: 0x00005D4C File Offset: 0x0000414C
		public DBVersion GetPrevious()
		{
			if (this.Fork > 0)
			{
				return new DBVersion(this.Major, this.Minor, this.Fork - 1);
			}
			if (this.Minor > 0)
			{
				return new DBVersion(this.Major, this.Minor - 1, 0);
			}
			if (this.Major > 0)
			{
				return new DBVersion(this.Major - 1, 0, 0);
			}
			throw new InvalidDataException(string.Format("Can't calculate previous DBVersion for {0}", this));
		}

		// Token: 0x0400018B RID: 395
		public static DBVersion Zero = new DBVersion(0, 0, 0);

		// Token: 0x0400018C RID: 396
		public readonly int Major;

		// Token: 0x0400018D RID: 397
		public readonly int Minor;

		// Token: 0x0400018E RID: 398
		public readonly int Fork;
	}
}

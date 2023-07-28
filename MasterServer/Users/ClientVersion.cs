using System;
using System.Linq;
using MasterServer.Common;

namespace MasterServer.Users
{
	// Token: 0x02000747 RID: 1863
	internal struct ClientVersion : IEquatable<ClientVersion>
	{
		// Token: 0x1700039C RID: 924
		// (get) Token: 0x06002662 RID: 9826 RVA: 0x000A2BDF File Offset: 0x000A0FDF
		// (set) Token: 0x06002663 RID: 9827 RVA: 0x000A2BE7 File Offset: 0x000A0FE7
		public ushort MajorVersion { get; private set; }

		// Token: 0x1700039D RID: 925
		// (get) Token: 0x06002664 RID: 9828 RVA: 0x000A2BF0 File Offset: 0x000A0FF0
		// (set) Token: 0x06002665 RID: 9829 RVA: 0x000A2BF8 File Offset: 0x000A0FF8
		public ushort ReleaseNumber { get; private set; }

		// Token: 0x1700039E RID: 926
		// (get) Token: 0x06002666 RID: 9830 RVA: 0x000A2C01 File Offset: 0x000A1001
		// (set) Token: 0x06002667 RID: 9831 RVA: 0x000A2C09 File Offset: 0x000A1009
		public ushort BuildNumber { get; private set; }

		// Token: 0x1700039F RID: 927
		// (get) Token: 0x06002668 RID: 9832 RVA: 0x000A2C12 File Offset: 0x000A1012
		// (set) Token: 0x06002669 RID: 9833 RVA: 0x000A2C1A File Offset: 0x000A101A
		public ushort BranchCode { get; private set; }

		// Token: 0x170003A0 RID: 928
		// (get) Token: 0x0600266A RID: 9834 RVA: 0x000A2C23 File Offset: 0x000A1023
		// (set) Token: 0x0600266B RID: 9835 RVA: 0x000A2C2B File Offset: 0x000A102B
		private string StringRepresentation { get; set; }

		// Token: 0x0600266C RID: 9836 RVA: 0x000A2C34 File Offset: 0x000A1034
		private static ClientVersion FromStrings(params string[] parts)
		{
			ushort num = ushort.Parse(parts[0]);
			ushort num2 = ushort.Parse(parts[1]);
			ushort num3 = ushort.Parse(parts[2]);
			ushort num4 = ushort.Parse(parts[3]);
			return new ClientVersion
			{
				MajorVersion = num,
				ReleaseNumber = num2,
				BuildNumber = num3,
				BranchCode = num4,
				StringRepresentation = string.Format("{0}.{1}.{2}.{3}", new object[]
				{
					num,
					num2,
					num3,
					num4
				})
			};
		}

		// Token: 0x0600266D RID: 9837 RVA: 0x000A2CCC File Offset: 0x000A10CC
		public static ClientVersion Parse(string versionString)
		{
			if (string.IsNullOrEmpty(versionString))
			{
				throw new ArgumentNullException("versionString");
			}
			string[] array = versionString.Split(new string[]
			{
				"."
			}, StringSplitOptions.None);
			if (array.Length == 4)
			{
				if (!array.Any((string part) => !part.ConsistsOfDigitsOnly()))
				{
					return ClientVersion.FromStrings(array);
				}
			}
			throw new ArgumentException(string.Format("Version string '{0}' is invalid and can't be parsed.", versionString));
		}

		// Token: 0x0600266E RID: 9838 RVA: 0x000A2D50 File Offset: 0x000A1150
		public static bool TryParse(string versionString, out ClientVersion version)
		{
			version = ClientVersion.Empty;
			if (string.IsNullOrEmpty(versionString))
			{
				return false;
			}
			string[] array = versionString.Split(new string[]
			{
				"."
			}, StringSplitOptions.None);
			if (array.Length == 4)
			{
				if (!array.Any((string part) => !part.ConsistsOfDigitsOnly()))
				{
					version = ClientVersion.FromStrings(array);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600266F RID: 9839 RVA: 0x000A2DCB File Offset: 0x000A11CB
		public override string ToString()
		{
			return this.StringRepresentation;
		}

		// Token: 0x06002670 RID: 9840 RVA: 0x000A2DD3 File Offset: 0x000A11D3
		public override bool Equals(object obj)
		{
			return !object.ReferenceEquals(null, obj) && obj is ClientVersion && this.Equals((ClientVersion)obj);
		}

		// Token: 0x06002671 RID: 9841 RVA: 0x000A2E00 File Offset: 0x000A1200
		public bool Equals(ClientVersion other)
		{
			return (this.IsUnknown && other.IsUnknown) || (!this.IsUnknown && !other.IsUnknown && (this.MajorVersion == other.MajorVersion && this.ReleaseNumber == other.ReleaseNumber && this.BuildNumber == other.BuildNumber) && this.BranchCode == other.BranchCode);
		}

		// Token: 0x06002672 RID: 9842 RVA: 0x000A2E87 File Offset: 0x000A1287
		public override int GetHashCode()
		{
			return (int)(this.MajorVersion ^ this.ReleaseNumber ^ this.BuildNumber ^ this.BranchCode);
		}

		// Token: 0x06002673 RID: 9843 RVA: 0x000A2EA4 File Offset: 0x000A12A4
		public static bool operator ==(ClientVersion v1, ClientVersion v2)
		{
			return v1.Equals(v2);
		}

		// Token: 0x06002674 RID: 9844 RVA: 0x000A2EAE File Offset: 0x000A12AE
		public static bool operator !=(ClientVersion v1, ClientVersion v2)
		{
			return !v1.Equals(v2);
		}

		// Token: 0x040013C8 RID: 5064
		public const string VERSION_PARTS_SEPARATOR = ".";

		// Token: 0x040013CD RID: 5069
		private bool IsUnknown;

		// Token: 0x040013CF RID: 5071
		public static ClientVersion Empty = default(ClientVersion);

		// Token: 0x040013D0 RID: 5072
		public static ClientVersion Unknown = new ClientVersion
		{
			IsUnknown = true
		};
	}
}

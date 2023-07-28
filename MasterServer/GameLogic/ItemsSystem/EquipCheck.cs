using System;
using System.Collections.Generic;

namespace MasterServer.GameLogic.ItemsSystem
{
	// Token: 0x02000778 RID: 1912
	internal static class EquipCheck
	{
		// Token: 0x060027AA RID: 10154 RVA: 0x000A93E8 File Offset: 0x000A77E8
		public static bool ParseDecalClasses(string config, out ulong classes)
		{
			bool result = false;
			classes = 0UL;
			string[] array = config.Split(new char[]
			{
				';'
			}, StringSplitOptions.RemoveEmptyEntries);
			foreach (string text in array)
			{
				string[] array3 = text.Split(new char[]
				{
					'='
				}, StringSplitOptions.RemoveEmptyEntries);
				if (array3.Length == 2 && array3[0] == "decal_config")
				{
					result = true;
					string text2 = array3[1];
					for (int j = 0; j < "RHSME".Length; j++)
					{
						string value = "{" + "RHSME"[j] + "{";
						if (config.IndexOf(value) != -1)
						{
							classes |= 1UL << j;
						}
					}
				}
			}
			return result;
		}

		// Token: 0x060027AB RID: 10155 RVA: 0x000A94C4 File Offset: 0x000A78C4
		public static ulong ConvertSlotIdsToEquipped(ulong slotIds)
		{
			ulong num = 31UL;
			ulong num2 = 0UL;
			for (int i = 0; i < 5; i++)
			{
				if ((slotIds & num) != 0UL)
				{
					num2 |= 1UL << i;
				}
				num <<= 5;
			}
			return num2;
		}

		// Token: 0x060027AC RID: 10156 RVA: 0x000A9504 File Offset: 0x000A7904
		public static int EncodeSlotsMask(HashSet<int> excludeSlots)
		{
			int num = 0;
			for (int i = 0; i < 31; i++)
			{
				if (!excludeSlots.Contains(i))
				{
					num |= 1 << i;
				}
			}
			return num;
		}

		// Token: 0x060027AD RID: 10157 RVA: 0x000A9544 File Offset: 0x000A7944
		public static HashSet<int> DecodeSlotsMask(int slotsMask)
		{
			HashSet<int> hashSet = new HashSet<int>();
			for (int i = 0; i < 31; i++)
			{
				if ((slotsMask & 1 << i) != 0)
				{
					hashSet.Add(i);
				}
			}
			return hashSet;
		}

		// Token: 0x040014B4 RID: 5300
		public const int BITS_PER_SLOT = 5;

		// Token: 0x040014B5 RID: 5301
		public const ulong SLOT_MASK_BASE = 31UL;

		// Token: 0x040014B6 RID: 5302
		public const int CLASS_COUNT = 5;

		// Token: 0x040014B7 RID: 5303
		public const int ALL_SLOTS_BITS = 31;

		// Token: 0x040014B8 RID: 5304
		public const string CLASS_LETTERS = "RHSME";
	}
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace MasterServer.Common
{
	// Token: 0x0200001A RID: 26
	public static class PositionApproximation
	{
		// Token: 0x06000061 RID: 97 RVA: 0x00006148 File Offset: 0x00004548
		public static IEnumerable<KeyValuePair<float, float>> CalculateSlice(int slice_size, int in_offset, int in_step, List<ulong> entries)
		{
			List<KeyValuePair<float, float>> list = new List<KeyValuePair<float, float>>(slice_size);
			for (int i = 0; i < slice_size; i++)
			{
				int num = in_offset + in_step * i;
				if (i == 0)
				{
					list.Add(new KeyValuePair<float, float>((float)(num + 1), entries[num]));
				}
				else
				{
					ulong num2 = ulong.MaxValue;
					int num3 = num;
					for (int j = 0; j < in_step - 1; j++)
					{
						int index = in_step * (i - 1) + j + in_offset;
						int num4 = in_step * (i - 1) + j + 1 + in_offset;
						ulong num5 = entries[index] - entries[num4];
						if (num5 < num2)
						{
							num2 = num5;
							num3 = num4;
						}
					}
					list.Add(new KeyValuePair<float, float>((float)(num3 + 1), entries[num3]));
				}
			}
			return list;
		}

		// Token: 0x06000062 RID: 98 RVA: 0x0000620C File Offset: 0x0000460C
		public static int GetPosition(List<KeyValuePair<float, float>> stats, ulong newValue)
		{
			if (stats.Count == 0)
			{
				return 1;
			}
			if (stats.Count == 1)
			{
				return (stats[0].Value > newValue) ? 2 : 1;
			}
			List<SamplePoint> list = new List<SamplePoint>(stats.Count);
			list.AddRange(from s in stats
			select new SamplePoint(s.Value, s.Key));
			float[] tangents = MonotoneCubicInterpolation.InitTangents(list);
			float num = MonotoneCubicInterpolation.Approximate(list, tangents, newValue);
			return (int)Math.Ceiling((double)num);
		}
	}
}

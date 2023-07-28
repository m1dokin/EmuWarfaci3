using System;
using System.Collections.Generic;

namespace MasterServer.Common
{
	// Token: 0x0200001F RID: 31
	public static class MonotoneCubicInterpolation
	{
		// Token: 0x0600006E RID: 110 RVA: 0x00006530 File Offset: 0x00004930
		public static float[] InitTangents(List<SamplePoint> points)
		{
			int count = points.Count;
			float[] array = new float[count];
			float[] array2 = new float[count - 1];
			for (int i = 0; i < count - 1; i++)
			{
				array2[i] = (points[i + 1].Y - points[i].Y) / (points[i + 1].X - points[i].X);
			}
			array[0] = array2[0];
			array[count - 1] = array2[count - 2];
			for (int j = 1; j < count - 2; j++)
			{
				array[j] = (array2[j - 1] + array2[j]) / 2f;
			}
			for (int k = 0; k < count - 1; k++)
			{
				if (array2[k] == 0f)
				{
					array[k] = (array[k + 1] = 0f);
				}
				else
				{
					double num = (double)(array[k] / array2[k]);
					double num2 = (double)(array[k + 1] / array2[k]);
					double num3 = Math.Pow(num, 2.0) + Math.Pow(num2, 2.0);
					if (num3 > 9.0)
					{
						double num4 = 3.0 / Math.Sqrt(num3);
						array[k] = (float)(num4 * num * (double)array2[k]);
						array[k + 1] = (float)(num4 * num2 * (double)array2[k]);
					}
				}
			}
			return array;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x000066B4 File Offset: 0x00004AB4
		public static float Approximate(List<SamplePoint> points, float[] tangents, float x)
		{
			int num = -1;
			for (int i = 0; i < points.Count - 1; i++)
			{
				if (points[i + 1].X <= x && x <= points[i].X)
				{
					num = i;
					break;
				}
			}
			if (num == -1)
			{
				return 0f;
			}
			SamplePoint samplePoint = points[num + 1];
			SamplePoint samplePoint2 = points[num];
			float num2 = samplePoint2.X - samplePoint.X;
			float num3 = (x - samplePoint.X) / num2;
			float num4 = (float)(2.0 * Math.Pow((double)num3, 3.0) - 3.0 * Math.Pow((double)num3, 2.0) + 1.0);
			float num5 = (float)(Math.Pow((double)num3, 3.0) - 2.0 * Math.Pow((double)num3, 2.0) + (double)num3);
			float num6 = (float)(-2.0 * Math.Pow((double)num3, 3.0) + 3.0 * Math.Pow((double)num3, 2.0));
			float num7 = (float)(Math.Pow((double)num3, 3.0) - Math.Pow((double)num3, 2.0));
			return num4 * samplePoint.Y + num5 * num2 * tangents[num] + num6 * samplePoint2.Y + num7 * num2 * tangents[num + 1];
		}
	}
}

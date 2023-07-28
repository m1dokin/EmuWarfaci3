using System;

namespace MasterServer.Common
{
	// Token: 0x0200001E RID: 30
	public static class LagrangePolynomial
	{
		// Token: 0x0600006D RID: 109 RVA: 0x0000649C File Offset: 0x0000489C
		public static float Approximate(SamplePoint[] points, float x)
		{
			float num = 0f;
			int num2 = points.Length - 1;
			for (int i = 0; i <= num2; i++)
			{
				float num3 = 1f;
				for (int j = 0; j <= num2; j++)
				{
					if (i != j)
					{
						num3 = num3 * (x - points[j].X) / (points[i].X - points[j].X);
					}
				}
				num += num3 * points[i].Y;
			}
			return num;
		}
	}
}

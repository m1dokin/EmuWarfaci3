using System;
using System.Collections.Generic;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x02000436 RID: 1078
	[Service]
	[Singleton]
	public class SkillCalculator : ISkillCalculator
	{
		// Token: 0x0600170B RID: 5899 RVA: 0x0005FEC0 File Offset: 0x0005E2C0
		public double CalculateSkillFromSkillPoints(double skillPoints, double curveCoef)
		{
			if (skillPoints <= 5E-324)
			{
				return 0.0;
			}
			if (curveCoef <= 5E-324)
			{
				throw new ArgumentException(string.Format("curveCoef need be more 0. curveCoef = '{0}'", curveCoef));
			}
			if (skillPoints >= 100000.0)
			{
				return curveCoef;
			}
			return Math.Pow(1.0 + this.GetParameters(curveCoef).LnMaxChannelSkill / skillPoints, skillPoints);
		}

		// Token: 0x0600170C RID: 5900 RVA: 0x0005FF40 File Offset: 0x0005E340
		public double[] GetCoefInterpolationPolynomial(double skillValue, double curveCoef)
		{
			if (skillValue <= 5E-324 || curveCoef <= 5E-324)
			{
				throw new ArgumentException(string.Format("Both arguments need be more 0. curveCoef = '{0}', skillValue = '{1}'.", curveCoef, skillValue));
			}
			SkillCalculator.Parameters parameters = this.GetParameters(curveCoef);
			double[] array = new double[4];
			double num = parameters.CoefPolynomial[0] - Math.Log(skillValue) / parameters.LnMaxChannelSkill;
			array[0] = 1.0;
			array[1] = parameters.CoefPolynomial[1] / num;
			array[2] = parameters.CoefPolynomial[2] / num;
			array[3] = parameters.CoefPolynomial[3] / num;
			return array;
		}

		// Token: 0x0600170D RID: 5901 RVA: 0x0005FFE8 File Offset: 0x0005E3E8
		private SkillCalculator.Parameters GetParameters(double maxChannelSkill)
		{
			SkillCalculator.Parameters parameters;
			if (this.m_dictParams.TryGetValue(maxChannelSkill, out parameters))
			{
				return parameters;
			}
			parameters = default(SkillCalculator.Parameters);
			double num = Math.Log(maxChannelSkill);
			parameters.LnMaxChannelSkill = num;
			double num2 = num * num;
			double num3 = num2 * num;
			parameters.CoefPolynomial = new double[4];
			parameters.CoefPolynomial[0] = 1.0;
			parameters.CoefPolynomial[1] = -0.5 * num;
			parameters.CoefPolynomial[2] = num2 / 3.0;
			parameters.CoefPolynomial[3] = -0.25 * num3;
			this.m_dictParams[maxChannelSkill] = parameters;
			return parameters;
		}

		// Token: 0x04000B1D RID: 2845
		private readonly IDictionary<double, SkillCalculator.Parameters> m_dictParams = new Dictionary<double, SkillCalculator.Parameters>();

		// Token: 0x02000437 RID: 1079
		private struct Parameters
		{
			// Token: 0x04000B1E RID: 2846
			public double LnMaxChannelSkill;

			// Token: 0x04000B1F RID: 2847
			public double[] CoefPolynomial;
		}
	}
}

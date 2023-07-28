using System;
using System.Collections.Generic;
using System.Linq;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x0200043A RID: 1082
	[Singleton]
	[Service]
	internal class SkillConverterService : ISkillConverterService
	{
		// Token: 0x0600171E RID: 5918 RVA: 0x000601B4 File Offset: 0x0005E5B4
		public SkillConverterService(ISkillCalculator calculator)
		{
			this.m_calculator = calculator;
		}

		// Token: 0x0600171F RID: 5919 RVA: 0x000601C4 File Offset: 0x0005E5C4
		public double ConvertSkillPointsToNewCurve(double skillPoints, double currentCurveCoef, double newCurveCoef)
		{
			if (skillPoints <= 5E-324)
			{
				return 0.0;
			}
			if (newCurveCoef <= 5E-324 || currentCurveCoef <= 5E-324)
			{
				throw new ArgumentException(string.Format("newCurveCoef and currentCurveCoef need be more 0. newCurveCoef = '{0}', currentCurveCoef = '{1}'.", newCurveCoef, currentCurveCoef));
			}
			if (newCurveCoef - currentCurveCoef <= 5E-324)
			{
				return skillPoints;
			}
			double skillValue = this.m_calculator.CalculateSkillFromSkillPoints(skillPoints, currentCurveCoef);
			double num = this.ApproximateSkillPoints(skillValue, newCurveCoef, 1f);
			if (double.IsNaN(num) || num < 5E-324)
			{
				num = skillPoints;
			}
			return num;
		}

		// Token: 0x06001720 RID: 5920 RVA: 0x00060270 File Offset: 0x0005E670
		public double ConvertSkillToSkillPoints(double skillValue, double curveCoef)
		{
			if (skillValue <= 5E-324)
			{
				return 0.0;
			}
			if (curveCoef <= 5E-324)
			{
				throw new ArgumentException(string.Format("curveCoef need be more 0. curveCoef = '{0}'.", curveCoef));
			}
			if (skillValue >= curveCoef)
			{
				return 100000.0;
			}
			return this.ApproximateSkillPoints(skillValue, curveCoef, 0.0001f);
		}

		// Token: 0x06001721 RID: 5921 RVA: 0x000602DC File Offset: 0x0005E6DC
		private double ApproximateSkillPoints(double skillValue, double curveCoef, float delta)
		{
			double coefCardanoPvar;
			double coefCardanoQvar;
			double coefCardanoBvar;
			this.InitCoefCardano(skillValue, curveCoef, out coefCardanoPvar, out coefCardanoQvar, out coefCardanoBvar);
			double[] array = this.FindCubeRootCandidates(coefCardanoPvar, coefCardanoQvar, coefCardanoBvar);
			return this.IterationApproximation(array[0], array[1], skillValue, curveCoef, delta, 20);
		}

		// Token: 0x06001722 RID: 5922 RVA: 0x00060314 File Offset: 0x0005E714
		private void InitCoefCardano(double skillValue, double curveCoef, out double coefCardanoPvar, out double coefCardanoQvar, out double coefCardanoBvar)
		{
			double[] coefInterpolationPolynomial = this.m_calculator.GetCoefInterpolationPolynomial(skillValue, curveCoef);
			double num = coefInterpolationPolynomial[1] * coefInterpolationPolynomial[1];
			coefCardanoPvar = -num / 3.0 + coefInterpolationPolynomial[2];
			coefCardanoQvar = 2.0 * num * coefInterpolationPolynomial[1] / 27.0 - coefInterpolationPolynomial[1] * coefInterpolationPolynomial[2] / 3.0 + coefInterpolationPolynomial[3];
			coefCardanoBvar = coefInterpolationPolynomial[1];
		}

		// Token: 0x06001723 RID: 5923 RVA: 0x00060384 File Offset: 0x0005E784
		private double[] FindCubeRootCandidates(double coefCardanoPvar, double coefCardanoQvar, double coefCardanoBvar)
		{
			double d = 0.25 * coefCardanoQvar * coefCardanoQvar + coefCardanoPvar * coefCardanoPvar * coefCardanoPvar / 27.0;
			double num = Math.Sqrt(d);
			double x = -0.5 * coefCardanoQvar + num;
			double num2 = Math.Pow(x, 0.3333333333333333);
			double x2 = -0.5 * coefCardanoQvar - num;
			double num3 = Math.Pow(x2, 0.3333333333333333);
			double num4 = 0.0;
			num4 = ((!double.IsNaN(num2)) ? (num4 + num2) : num4);
			num4 = ((!double.IsNaN(num3)) ? (num4 + num3) : num4);
			if (num4 < 5E-324)
			{
				num4 = 1.0;
			}
			List<double> list = new List<double>
			{
				num4,
				-0.5 * num4
			};
			List<double> list2 = new List<double>();
			foreach (double num5 in list)
			{
				double item = num5 - coefCardanoBvar / 3.0;
				list2.Add(item);
			}
			return list2.Distinct<double>().ToArray<double>();
		}

		// Token: 0x06001724 RID: 5924 RVA: 0x000604E8 File Offset: 0x0005E8E8
		private double IterationApproximation(double first, double second, double skillValue, double curveCoef, float delta, short countIterations)
		{
			if (first < 5E-324 && second < 5E-324)
			{
				return double.NaN;
			}
			if (first > second)
			{
				return this.IterationApproximation(second, first, skillValue, curveCoef, delta, countIterations);
			}
			if (first < 5E-324)
			{
				return this.IterationApproximation(0.0001, second, skillValue, curveCoef, delta, countIterations -= 1);
			}
			if (countIterations == 0)
			{
				return (first <= double.Epsilon) ? second : first;
			}
			double num = skillValue - this.m_calculator.CalculateSkillFromSkillPoints(first, curveCoef);
			if (Math.Abs(num) < (double)delta)
			{
				return first;
			}
			double num2 = skillValue - this.m_calculator.CalculateSkillFromSkillPoints(second, curveCoef);
			if (Math.Abs(num2) < (double)delta)
			{
				return second;
			}
			if (Math.Sign(num) > 0 && Math.Sign(num2) > 0)
			{
				return this.IterationApproximation(second, 2.0 * second - first, skillValue, curveCoef, delta, countIterations -= 1);
			}
			if (Math.Sign(num) < 0 && Math.Sign(num2) < 0)
			{
				return this.IterationApproximation(2.0 * first - second, first, skillValue, curveCoef, delta, countIterations -= 1);
			}
			double first2 = first;
			double second2 = second;
			double num3 = (first + second) * 0.5;
			double num4 = skillValue - this.m_calculator.CalculateSkillFromSkillPoints(num3, curveCoef);
			if (Math.Abs(num4 - num) > Math.Abs(num4 - num2))
			{
				first2 = num3;
			}
			else
			{
				second2 = num3;
			}
			return this.IterationApproximation(first2, second2, skillValue, curveCoef, delta, countIterations -= 1);
		}

		// Token: 0x04000B2C RID: 2860
		private readonly ISkillCalculator m_calculator;
	}
}

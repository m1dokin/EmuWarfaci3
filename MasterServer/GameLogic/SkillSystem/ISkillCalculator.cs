using System;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x02000435 RID: 1077
	[Contract]
	internal interface ISkillCalculator
	{
		// Token: 0x06001708 RID: 5896
		double CalculateSkillFromSkillPoints(double skillPoints, double curveCoef);

		// Token: 0x06001709 RID: 5897
		double[] GetCoefInterpolationPolynomial(double skillValue, double curveCoef);
	}
}

using System;
using HK2Net;

namespace MasterServer.GameLogic.SkillSystem
{
	// Token: 0x02000439 RID: 1081
	[Contract]
	internal interface ISkillConverterService
	{
		// Token: 0x0600171C RID: 5916
		double ConvertSkillPointsToNewCurve(double skillPoints, double currentCurveCoef, double newCurveCoef);

		// Token: 0x0600171D RID: 5917
		double ConvertSkillToSkillPoints(double skillValue, double currentCurveCoef);
	}
}

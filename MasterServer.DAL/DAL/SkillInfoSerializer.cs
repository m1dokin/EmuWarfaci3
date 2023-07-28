using System;

namespace MasterServer.DAL
{
	// Token: 0x02000097 RID: 151
	public class SkillInfoSerializer : IDBSerializer<SkillInfo>
	{
		// Token: 0x060001C4 RID: 452 RVA: 0x0000581C File Offset: 0x00003C1C
		public void Deserialize(IDataReaderEx reader, out SkillInfo ret)
		{
			ret = default(SkillInfo);
			SkillInfo skillInfo = ret;
			skillInfo.Type = reader["type"].ToString();
			skillInfo.Points = double.Parse(reader["value"].ToString());
			skillInfo.CurveCoef = double.Parse(reader["curve_coef"].ToString());
			ret = skillInfo;
		}
	}
}

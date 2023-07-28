using System;
using System.Xml;
using MasterServer.Core;
using MasterServer.Core.Configuration;

namespace MasterServer.GameLogic.RewardSystem
{
	// Token: 0x020005B6 RID: 1462
	internal class RewardPool : IRewardPool
	{
		// Token: 0x06001F62 RID: 8034 RVA: 0x0007F93E File Offset: 0x0007DD3E
		public RewardPool()
		{
			this.Score = 0U;
			this.Win = 0U;
			this.Lose = 0U;
			this.Draw = 0U;
		}

		// Token: 0x06001F63 RID: 8035 RVA: 0x0007F962 File Offset: 0x0007DD62
		public RewardPool(RewardPool pool)
		{
			this.Score = pool.Score;
			this.Win = pool.Win;
			this.Lose = pool.Lose;
			this.Draw = pool.Draw;
		}

		// Token: 0x06001F64 RID: 8036 RVA: 0x0007F99A File Offset: 0x0007DD9A
		public uint GetByOutcome(SessionOutcome outcome)
		{
			return (outcome != SessionOutcome.Draw) ? ((outcome != SessionOutcome.Won) ? this.Lose : this.Win) : this.Draw;
		}

		// Token: 0x06001F65 RID: 8037 RVA: 0x0007F9C8 File Offset: 0x0007DDC8
		public void Dump()
		{
			Log.Info<uint>("Score: {0}", this.Score);
			Log.Info<uint>("Win: {0}", this.Win);
			Log.Info<uint>("Lose: {0}", this.Lose);
			Log.Info<uint>("Draw: {0}", this.Draw);
		}

		// Token: 0x06001F66 RID: 8038 RVA: 0x0007FA18 File Offset: 0x0007DE18
		public void Plus(RewardPool pool)
		{
			this.Score += pool.Score;
			this.Win += pool.Win;
			this.Lose += pool.Lose;
			this.Draw += pool.Draw;
		}

		// Token: 0x06001F67 RID: 8039 RVA: 0x0007FA74 File Offset: 0x0007DE74
		public void Mul(float scoreMul, float winMul, float loseMul, float drawMul)
		{
			this.Score = (uint)(0.5f + this.Score * scoreMul);
			this.Win = (uint)(0.5f + this.Win * winMul);
			this.Lose = (uint)(0.5f + this.Lose * loseMul);
			this.Draw = (uint)(0.5f + this.Draw * drawMul);
		}

		// Token: 0x06001F68 RID: 8040 RVA: 0x0007FAE0 File Offset: 0x0007DEE0
		public bool IsValid()
		{
			bool flag = false;
			if (this.Lose <= this.Draw && this.Draw <= this.Win)
			{
				flag = true;
			}
			if (!flag)
			{
				this.Win = this.Lose;
				this.Draw = this.Lose;
			}
			return flag;
		}

		// Token: 0x06001F69 RID: 8041 RVA: 0x0007FB34 File Offset: 0x0007DF34
		public void ToDefault()
		{
			ConfigSection section = Resources.Rewards.GetSection("Rewards");
			this.Win = uint.Parse(section.Get("WinPoolDefault"));
			this.Lose = uint.Parse(section.Get("LosePoolDefault"));
			this.Draw = uint.Parse(section.Get("DrawPoolDefault"));
			this.Score = uint.Parse(section.Get("ScorePoolDefault"));
		}

		// Token: 0x06001F6A RID: 8042 RVA: 0x0007FBAC File Offset: 0x0007DFAC
		public bool TryParse(XmlTextReader reader)
		{
			if (uint.TryParse(reader.GetAttribute("win_pool"), out this.Win) && uint.TryParse(reader.GetAttribute("lose_pool"), out this.Lose) && uint.TryParse(reader.GetAttribute("score_pool"), out this.Score))
			{
				if (!uint.TryParse(reader.GetAttribute("draw_pool"), out this.Draw))
				{
					this.Draw = this.Lose;
				}
				this.IsValid();
				return true;
			}
			return false;
		}

		// Token: 0x06001F6B RID: 8043 RVA: 0x0007FC3B File Offset: 0x0007E03B
		public override string ToString()
		{
			return string.Format("{0}/{1}/{2}", this.Win, this.Lose, this.Draw);
		}

		// Token: 0x04000F40 RID: 3904
		public uint Score;

		// Token: 0x04000F41 RID: 3905
		public uint Win;

		// Token: 0x04000F42 RID: 3906
		public uint Lose;

		// Token: 0x04000F43 RID: 3907
		public uint Draw;
	}
}

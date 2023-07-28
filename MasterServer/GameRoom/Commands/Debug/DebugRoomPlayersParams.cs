using System;
using System.Collections.Generic;
using System.Linq;
using CommandLine;
using MasterServer.DAL;

namespace MasterServer.GameRoom.Commands.Debug
{
	// Token: 0x02000467 RID: 1127
	internal class DebugRoomPlayersParams
	{
		// Token: 0x060017BE RID: 6078 RVA: 0x00061D08 File Offset: 0x00060108
		public DebugRoomPlayersParams()
		{
			this.DefaultProgression = new SProfileProgression
			{
				ClassUnlocked = 268435455,
				TutorialUnlocked = 268435455,
				TutorialPassed = 268435455,
				MissionUnlocked = 268435455
			};
		}

		// Token: 0x1700023E RID: 574
		// (get) Token: 0x060017BF RID: 6079 RVA: 0x00061D5A File Offset: 0x0006015A
		// (set) Token: 0x060017C0 RID: 6080 RVA: 0x00061D62 File Offset: 0x00060162
		[Option('n', "n", Required = true, HelpText = "Number of players to be added to the room.")]
		public byte Quantity { get; set; }

		// Token: 0x1700023F RID: 575
		// (get) Token: 0x060017C1 RID: 6081 RVA: 0x00061D6B File Offset: 0x0006016B
		// (set) Token: 0x060017C2 RID: 6082 RVA: 0x00061D73 File Offset: 0x00060173
		[OptionArray("rank", HelpText = "Players rank interval used to get a random rank for each player.", DefaultValue = new byte[]
		{
			1
		})]
		public byte[] Rank { get; set; }

		// Token: 0x17000240 RID: 576
		// (get) Token: 0x060017C3 RID: 6083 RVA: 0x00061D7C File Offset: 0x0006017C
		public byte RankMin
		{
			get
			{
				return this.Rank[0];
			}
		}

		// Token: 0x17000241 RID: 577
		// (get) Token: 0x060017C4 RID: 6084 RVA: 0x00061D86 File Offset: 0x00060186
		public byte RankMax
		{
			get
			{
				return (this.Rank.Length <= 1) ? this.RankMin : this.Rank[1];
			}
		}

		// Token: 0x17000242 RID: 578
		// (get) Token: 0x060017C5 RID: 6085 RVA: 0x00061DA9 File Offset: 0x000601A9
		// (set) Token: 0x060017C6 RID: 6086 RVA: 0x00061DB1 File Offset: 0x000601B1
		[OptionArray('s', "skill", HelpText = "Players skill interval used to get a random skill for each player.", DefaultValue = new float[]
		{
			1f,
			3.4028235E+38f
		})]
		public float[] Skill { get; set; }

		// Token: 0x17000243 RID: 579
		// (get) Token: 0x060017C7 RID: 6087 RVA: 0x00061DBA File Offset: 0x000601BA
		public float SkillMin
		{
			get
			{
				return this.Skill[0];
			}
		}

		// Token: 0x17000244 RID: 580
		// (get) Token: 0x060017C8 RID: 6088 RVA: 0x00061DC4 File Offset: 0x000601C4
		public float SkillMax
		{
			get
			{
				return (this.Skill.Length <= 1) ? this.SkillMin : this.Skill[1];
			}
		}

		// Token: 0x17000245 RID: 581
		// (get) Token: 0x060017C9 RID: 6089 RVA: 0x00061DE7 File Offset: 0x000601E7
		public bool IsDefaultSkillMax
		{
			get
			{
				return Math.Abs(float.MaxValue - this.SkillMax) <= float.Epsilon;
			}
		}

		// Token: 0x17000246 RID: 582
		// (get) Token: 0x060017CA RID: 6090 RVA: 0x00061E04 File Offset: 0x00060204
		// (set) Token: 0x060017CB RID: 6091 RVA: 0x00061E0C File Offset: 0x0006020C
		[OptionArray('p', "group", HelpText = "Players group (party) interval used to get a random group for each player.", DefaultValue = new byte[]
		{
			0
		})]
		public byte[] Group { get; set; }

		// Token: 0x17000247 RID: 583
		// (get) Token: 0x060017CC RID: 6092 RVA: 0x00061E15 File Offset: 0x00060215
		public byte GroupMin
		{
			get
			{
				return this.Group[0];
			}
		}

		// Token: 0x17000248 RID: 584
		// (get) Token: 0x060017CD RID: 6093 RVA: 0x00061E1F File Offset: 0x0006021F
		public byte GroupMax
		{
			get
			{
				return (this.Group.Length <= 1) ? this.GroupMin : this.Group[1];
			}
		}

		// Token: 0x17000249 RID: 585
		// (get) Token: 0x060017CE RID: 6094 RVA: 0x00061E42 File Offset: 0x00060242
		// (set) Token: 0x060017CF RID: 6095 RVA: 0x00061E4A File Offset: 0x0006024A
		[Option('i', "team_id", HelpText = "Number of team player will be added to (should be 1 or 2).", DefaultValue = 0)]
		public byte TeamId { get; set; }

		// Token: 0x1700024A RID: 586
		// (get) Token: 0x060017D0 RID: 6096 RVA: 0x00061E53 File Offset: 0x00060253
		// (set) Token: 0x060017D1 RID: 6097 RVA: 0x00061E5B File Offset: 0x0006025B
		[OptionArray('c', "class_pattern", HelpText = "Class pattern represented by the array of (class) leters. Allowed classes - R(ifleman), E(ngineer), S(niper), M(edic).", DefaultValue = new char[]
		{
			'R'
		})]
		public char[] ClassPattern { get; set; }

		// Token: 0x1700024B RID: 587
		// (get) Token: 0x060017D2 RID: 6098 RVA: 0x00061E64 File Offset: 0x00060264
		public byte[] ClassIds
		{
			get
			{
				return DebugRoomPlayersParams.ParseClassIds(this.ClassPattern);
			}
		}

		// Token: 0x1700024C RID: 588
		// (get) Token: 0x060017D3 RID: 6099 RVA: 0x00061E71 File Offset: 0x00060271
		// (set) Token: 0x060017D4 RID: 6100 RVA: 0x00061E79 File Offset: 0x00060279
		[Option("region_id", Required = false, HelpText = "Region id of players to be added to the room.")]
		public string Region { get; set; }

		// Token: 0x1700024D RID: 589
		// (get) Token: 0x060017D5 RID: 6101 RVA: 0x00061E82 File Offset: 0x00060282
		public string RegionId
		{
			get
			{
				return (!string.IsNullOrEmpty(this.Region)) ? this.Region : "global";
			}
		}

		// Token: 0x060017D6 RID: 6102 RVA: 0x00061EA4 File Offset: 0x000602A4
		private static byte[] ParseClassIds(IEnumerable<char> pattern)
		{
			return (from classLetter in pattern
			select new
			{
				classLetter = classLetter,
				classLetterIndex = "RHSME".IndexOf(char.ToUpper(classLetter))
			} into <>__TranspIdent6
			where <>__TranspIdent6.classLetterIndex != -1 && <>__TranspIdent6.classLetterIndex != 1
			select (byte)<>__TranspIdent6.classLetterIndex).ToArray<byte>();
		}

		// Token: 0x04000B78 RID: 2936
		public const byte DefaultGroupId = 0;

		// Token: 0x04000B79 RID: 2937
		public readonly SProfileProgression DefaultProgression;
	}
}

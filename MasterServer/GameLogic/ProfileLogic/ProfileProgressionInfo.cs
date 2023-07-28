using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.Database;

namespace MasterServer.GameLogic.ProfileLogic
{
	// Token: 0x02000558 RID: 1368
	internal class ProfileProgressionInfo
	{
		// Token: 0x06001D78 RID: 7544 RVA: 0x000778A6 File Offset: 0x00075CA6
		public ProfileProgressionInfo(ulong profileId, IDALService dalService)
		{
			this.m_dalService = dalService;
			this.ProfileId = profileId;
		}

		// Token: 0x06001D79 RID: 7545 RVA: 0x000778BC File Offset: 0x00075CBC
		public ProfileProgressionInfo(SProfileProgression progression, IDALService dalService, IProfileProgressionService profileProgressionService)
		{
			this.m_dalService = dalService;
			this.ProfileId = progression.ProfileId;
			this.MissionPassCounter = progression.MissionPassCounter;
			this.ZombieMissionPassCounter = progression.ZombieMissionPassCounter;
			this.CampaignPassCounter = progression.CampaignPassCounter;
			this.VolcanoCampaignPassCounter = progression.VolcanoCampaignPasCounter;
			this.AnubisCampaignPassCounter = progression.AnubisCampaignPassCounter;
			this.ZombieTowerCampaignPassCounter = progression.ZombieTowerCampaignPassCounter;
			this.IceBreakerCampaignPassCounter = progression.IceBreakerCampaignPassCounter;
			this.TutorialUnlocked = (ProfileProgressionInfo.Tutorial)((!profileProgressionService.IsEnabled) ? 268435455 : progression.TutorialUnlocked);
			this.TutorialPassed = (ProfileProgressionInfo.Tutorial)progression.TutorialPassed;
			this.ClassUnlocked = (ProfileProgressionInfo.PlayerClass)((!profileProgressionService.IsEnabled) ? 268435455 : progression.ClassUnlocked);
			this.MissionUnlocked = (ProfileProgressionInfo.MissionType)((!profileProgressionService.IsEnabled) ? 268435455 : progression.MissionUnlocked);
		}

		// Token: 0x17000313 RID: 787
		// (get) Token: 0x06001D7A RID: 7546 RVA: 0x000779B1 File Offset: 0x00075DB1
		// (set) Token: 0x06001D7B RID: 7547 RVA: 0x000779B9 File Offset: 0x00075DB9
		public ulong ProfileId { get; private set; }

		// Token: 0x17000314 RID: 788
		// (get) Token: 0x06001D7C RID: 7548 RVA: 0x000779C2 File Offset: 0x00075DC2
		// (set) Token: 0x06001D7D RID: 7549 RVA: 0x000779CA File Offset: 0x00075DCA
		public int MissionPassCounter { get; set; }

		// Token: 0x17000315 RID: 789
		// (get) Token: 0x06001D7E RID: 7550 RVA: 0x000779D3 File Offset: 0x00075DD3
		// (set) Token: 0x06001D7F RID: 7551 RVA: 0x000779DB File Offset: 0x00075DDB
		public int ZombieMissionPassCounter { get; private set; }

		// Token: 0x17000316 RID: 790
		// (get) Token: 0x06001D80 RID: 7552 RVA: 0x000779E4 File Offset: 0x00075DE4
		// (set) Token: 0x06001D81 RID: 7553 RVA: 0x000779EC File Offset: 0x00075DEC
		public int CampaignPassCounter { get; private set; }

		// Token: 0x17000317 RID: 791
		// (get) Token: 0x06001D82 RID: 7554 RVA: 0x000779F5 File Offset: 0x00075DF5
		// (set) Token: 0x06001D83 RID: 7555 RVA: 0x000779FD File Offset: 0x00075DFD
		public int VolcanoCampaignPassCounter { get; private set; }

		// Token: 0x17000318 RID: 792
		// (get) Token: 0x06001D84 RID: 7556 RVA: 0x00077A06 File Offset: 0x00075E06
		// (set) Token: 0x06001D85 RID: 7557 RVA: 0x00077A0E File Offset: 0x00075E0E
		public int AnubisCampaignPassCounter { get; private set; }

		// Token: 0x17000319 RID: 793
		// (get) Token: 0x06001D86 RID: 7558 RVA: 0x00077A17 File Offset: 0x00075E17
		// (set) Token: 0x06001D87 RID: 7559 RVA: 0x00077A1F File Offset: 0x00075E1F
		public int ZombieTowerCampaignPassCounter { get; private set; }

		// Token: 0x1700031A RID: 794
		// (get) Token: 0x06001D88 RID: 7560 RVA: 0x00077A28 File Offset: 0x00075E28
		// (set) Token: 0x06001D89 RID: 7561 RVA: 0x00077A30 File Offset: 0x00075E30
		public int IceBreakerCampaignPassCounter { get; private set; }

		// Token: 0x1700031B RID: 795
		// (get) Token: 0x06001D8A RID: 7562 RVA: 0x00077A39 File Offset: 0x00075E39
		// (set) Token: 0x06001D8B RID: 7563 RVA: 0x00077A41 File Offset: 0x00075E41
		public ProfileProgressionInfo.Tutorial TutorialUnlocked { get; private set; }

		// Token: 0x1700031C RID: 796
		// (get) Token: 0x06001D8C RID: 7564 RVA: 0x00077A4A File Offset: 0x00075E4A
		// (set) Token: 0x06001D8D RID: 7565 RVA: 0x00077A52 File Offset: 0x00075E52
		public ProfileProgressionInfo.Tutorial TutorialPassed { get; private set; }

		// Token: 0x1700031D RID: 797
		// (get) Token: 0x06001D8E RID: 7566 RVA: 0x00077A5B File Offset: 0x00075E5B
		// (set) Token: 0x06001D8F RID: 7567 RVA: 0x00077A63 File Offset: 0x00075E63
		public ProfileProgressionInfo.PlayerClass ClassUnlocked { get; private set; }

		// Token: 0x1700031E RID: 798
		// (get) Token: 0x06001D90 RID: 7568 RVA: 0x00077A6C File Offset: 0x00075E6C
		// (set) Token: 0x06001D91 RID: 7569 RVA: 0x00077A74 File Offset: 0x00075E74
		public ProfileProgressionInfo.MissionType MissionUnlocked { get; private set; }

		// Token: 0x06001D92 RID: 7570 RVA: 0x00077A7D File Offset: 0x00075E7D
		internal bool IsTutorialUnlocked(int idx)
		{
			return this.IsTutorialUnlocked(Utils.ConvertToEnumFlag<ProfileProgressionInfo.Tutorial>(idx));
		}

		// Token: 0x06001D93 RID: 7571 RVA: 0x00077A8B File Offset: 0x00075E8B
		public bool IsTutorialUnlocked(ProfileProgressionInfo.Tutorial idx)
		{
			return this.TutorialUnlocked.HasAnyFlag(idx);
		}

		// Token: 0x06001D94 RID: 7572 RVA: 0x00077AA3 File Offset: 0x00075EA3
		public bool IsTutorialPassed(int idx)
		{
			return this.IsTutorialPassed(Utils.ConvertToEnumFlag<ProfileProgressionInfo.Tutorial>(idx));
		}

		// Token: 0x06001D95 RID: 7573 RVA: 0x00077AB1 File Offset: 0x00075EB1
		public bool IsTutorialPassed(ProfileProgressionInfo.Tutorial idx)
		{
			return this.TutorialPassed.HasAnyFlag(idx);
		}

		// Token: 0x06001D96 RID: 7574 RVA: 0x00077AC9 File Offset: 0x00075EC9
		public bool IsClassUnlocked(int idx)
		{
			return this.IsClassUnlocked(Utils.ConvertToEnumFlag<ProfileProgressionInfo.PlayerClass>(idx));
		}

		// Token: 0x06001D97 RID: 7575 RVA: 0x00077AD7 File Offset: 0x00075ED7
		public bool IsClassUnlocked(ProfileProgressionInfo.PlayerClass idx)
		{
			return this.ClassUnlocked.HasAnyFlag(idx);
		}

		// Token: 0x06001D98 RID: 7576 RVA: 0x00077AF0 File Offset: 0x00075EF0
		public bool IsMissionTypeUnlocked(string missionType)
		{
			ProfileProgressionInfo.MissionType missionType2;
			return Utils.TryParse<ProfileProgressionInfo.MissionType>(missionType, out missionType2) && this.IsMissionTypeUnlocked(missionType2);
		}

		// Token: 0x06001D99 RID: 7577 RVA: 0x00077B14 File Offset: 0x00075F14
		public bool IsMissionTypeUnlocked(ProfileProgressionInfo.MissionType missionType)
		{
			return missionType == ProfileProgressionInfo.MissionType.None || this.MissionUnlocked.HasAnyFlag(missionType);
		}

		// Token: 0x06001D9A RID: 7578 RVA: 0x00077B38 File Offset: 0x00075F38
		public void UnlockTutorial(ProfileProgressionInfo.Tutorial idx, ILogGroup logGroup)
		{
			ProfileProgressionInfo.Tutorial tutorialUnlocked = this.TutorialUnlocked;
			this.TutorialUnlocked = (ProfileProgressionInfo.Tutorial)this.SetBit((int)this.TutorialUnlocked, (int)idx);
			if (tutorialUnlocked != this.TutorialUnlocked)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(this.ProfileId);
				logGroup.TutorialUnlockedLog(profileInfo.UserID, this.ProfileId, idx, profileInfo.RankInfo.RankId);
			}
		}

		// Token: 0x06001D9B RID: 7579 RVA: 0x00077BA4 File Offset: 0x00075FA4
		public void PassTutorial(ProfileProgressionInfo.Tutorial idx, ILogGroup logGroup)
		{
			ProfileProgressionInfo.Tutorial tutorialPassed = this.TutorialPassed;
			this.TutorialPassed = (ProfileProgressionInfo.Tutorial)this.SetBit((int)this.TutorialPassed, (int)idx);
			if (tutorialPassed != this.TutorialPassed)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(this.ProfileId);
				logGroup.TutorialPassedLog(profileInfo.UserID, this.ProfileId, idx, profileInfo.RankInfo.RankId);
			}
		}

		// Token: 0x06001D9C RID: 7580 RVA: 0x00077C10 File Offset: 0x00076010
		public void UnlockClass(ProfileProgressionInfo.PlayerClass idx, ILogGroup logGroup)
		{
			if (idx == ProfileProgressionInfo.PlayerClass.Heavy)
			{
				throw new Exception(string.Format("Profile id {0} trying to unclock class Heavy", this.ProfileId));
			}
			ProfileProgressionInfo.PlayerClass classUnlocked = this.ClassUnlocked;
			this.ClassUnlocked = (ProfileProgressionInfo.PlayerClass)this.SetBit((int)this.ClassUnlocked, (int)idx);
			if (classUnlocked != this.ClassUnlocked)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(this.ProfileId);
				logGroup.ClassUnlockedLog(profileInfo.UserID, this.ProfileId, idx, profileInfo.RankInfo.RankId);
			}
		}

		// Token: 0x06001D9D RID: 7581 RVA: 0x00077C9C File Offset: 0x0007609C
		public void UnlockMission(ProfileProgressionInfo.MissionType idx, ILogGroup logGroup)
		{
			ProfileProgressionInfo.MissionType missionUnlocked = this.MissionUnlocked;
			this.MissionUnlocked = (ProfileProgressionInfo.MissionType)this.SetBit((int)this.MissionUnlocked, (int)idx);
			if (missionUnlocked != this.MissionUnlocked)
			{
				SProfileInfo profileInfo = this.m_dalService.ProfileSystem.GetProfileInfo(this.ProfileId);
				logGroup.MissionUnlockedLog(profileInfo.UserID, this.ProfileId, idx.ToString().ToLower(), profileInfo.RankInfo.RankId);
			}
		}

		// Token: 0x06001D9E RID: 7582 RVA: 0x00077D17 File Offset: 0x00076117
		public IEnumerable<ProfileProgressionInfo.MissionType> GetUnlockedMissions()
		{
			return from ProfileProgressionInfo.MissionType unlockedMission in Enum.GetValues(typeof(ProfileProgressionInfo.MissionType))
			where this.IsMissionTypeUnlocked(unlockedMission)
			select unlockedMission;
		}

		// Token: 0x06001D9F RID: 7583 RVA: 0x00077D40 File Offset: 0x00076140
		public static ProfileProgressionInfo operator ^(ProfileProgressionInfo pp1, ProfileProgressionInfo pp2)
		{
			if (pp1.ProfileId != pp2.ProfileId)
			{
				throw new Exception(string.Format("Trying to 'xor' progression info for different profiles {0} and {1}", pp1.ProfileId, pp2.ProfileId));
			}
			return new ProfileProgressionInfo(pp1.ProfileId, null)
			{
				TutorialPassed = (pp1.TutorialPassed ^ pp2.TutorialPassed),
				TutorialUnlocked = (pp1.TutorialUnlocked ^ pp2.TutorialUnlocked),
				ClassUnlocked = (pp1.ClassUnlocked ^ pp2.ClassUnlocked),
				MissionUnlocked = (pp1.MissionUnlocked ^ pp2.MissionUnlocked),
				MissionPassCounter = Math.Max(pp1.MissionPassCounter, pp2.MissionPassCounter),
				ZombieMissionPassCounter = Math.Max(pp1.ZombieMissionPassCounter, pp2.ZombieMissionPassCounter),
				CampaignPassCounter = Math.Max(pp1.CampaignPassCounter, pp2.CampaignPassCounter),
				VolcanoCampaignPassCounter = Math.Max(pp1.VolcanoCampaignPassCounter, pp2.VolcanoCampaignPassCounter),
				AnubisCampaignPassCounter = Math.Max(pp1.AnubisCampaignPassCounter, pp2.AnubisCampaignPassCounter),
				ZombieTowerCampaignPassCounter = Math.Max(pp1.ZombieTowerCampaignPassCounter, pp2.ZombieTowerCampaignPassCounter),
				IceBreakerCampaignPassCounter = Math.Max(pp1.IceBreakerCampaignPassCounter, pp2.IceBreakerCampaignPassCounter)
			};
		}

		// Token: 0x06001DA0 RID: 7584 RVA: 0x00077E80 File Offset: 0x00076280
		public static ProfileProgressionInfo operator |(ProfileProgressionInfo pp1, ProfileProgressionInfo pp2)
		{
			if (pp1.ProfileId != pp2.ProfileId)
			{
				throw new Exception(string.Format("Trying to 'or' progression info for different profiles {0} and {1}", pp1.ProfileId, pp2.ProfileId));
			}
			return new ProfileProgressionInfo(pp1.ProfileId, null)
			{
				TutorialPassed = (pp1.TutorialPassed | pp2.TutorialPassed),
				TutorialUnlocked = (pp1.TutorialUnlocked | pp2.TutorialUnlocked),
				ClassUnlocked = (pp1.ClassUnlocked | pp2.ClassUnlocked),
				MissionUnlocked = (pp1.MissionUnlocked | pp2.MissionUnlocked),
				MissionPassCounter = Math.Max(pp1.MissionPassCounter, pp2.MissionPassCounter),
				ZombieMissionPassCounter = Math.Max(pp1.ZombieMissionPassCounter, pp2.ZombieMissionPassCounter),
				CampaignPassCounter = Math.Max(pp1.CampaignPassCounter, pp2.CampaignPassCounter),
				VolcanoCampaignPassCounter = Math.Max(pp1.VolcanoCampaignPassCounter, pp2.VolcanoCampaignPassCounter),
				AnubisCampaignPassCounter = Math.Max(pp1.AnubisCampaignPassCounter, pp2.AnubisCampaignPassCounter),
				ZombieTowerCampaignPassCounter = Math.Max(pp1.ZombieTowerCampaignPassCounter, pp2.ZombieTowerCampaignPassCounter),
				IceBreakerCampaignPassCounter = Math.Max(pp1.IceBreakerCampaignPassCounter, pp2.IceBreakerCampaignPassCounter)
			};
		}

		// Token: 0x06001DA1 RID: 7585 RVA: 0x00077FC0 File Offset: 0x000763C0
		public static string ClassNameLongToShort(string longName)
		{
			longName = longName.Trim();
			if (longName.Length > 0)
			{
				if (ProfileProgressionInfo.m_classShortToClassName.Values.Contains(longName, StringComparer.InvariantCultureIgnoreCase))
				{
					return longName[0].ToString().ToUpperInvariant();
				}
				if (longName.Equals(ProfileProgressionInfo.ObsoleteSniperClassName, StringComparison.InvariantCultureIgnoreCase))
				{
					return "S";
				}
			}
			return "NA";
		}

		// Token: 0x06001DA2 RID: 7586 RVA: 0x00078034 File Offset: 0x00076434
		public static string ClassNameShortToLong(string shortName)
		{
			shortName = shortName.Trim();
			string text;
			return (!ProfileProgressionInfo.m_classShortToClassName.TryGetValue(shortName, out text)) ? "NA" : text;
		}

		// Token: 0x06001DA3 RID: 7587 RVA: 0x00078066 File Offset: 0x00076466
		public XmlElement ToXml(XmlDocument factory)
		{
			return this.ToXml(factory, false);
		}

		// Token: 0x06001DA4 RID: 7588 RVA: 0x00078070 File Offset: 0x00076470
		public XmlElement ToXml(XmlDocument factory, bool updateMode)
		{
			XmlElement xmlElement = (!updateMode) ? factory.CreateElement("profile_progression_state") : factory.CreateElement("profile_progression_update");
			xmlElement.SetAttribute("profile_id", this.ProfileId.ToString(CultureInfo.InvariantCulture));
			if (!updateMode || this.MissionUnlocked != ProfileProgressionInfo.MissionType.None)
			{
				xmlElement.SetAttribute("mission_unlocked", string.Join(",", (from x in this.GetUnlockedMissions()
				select x.ToString().ToLower()).ToArray<string>()));
			}
			if (!updateMode || this.TutorialUnlocked != ProfileProgressionInfo.Tutorial.None)
			{
				xmlElement.SetAttribute("tutorial_unlocked", ((int)this.TutorialUnlocked).ToString(CultureInfo.InvariantCulture));
			}
			if (!updateMode || this.TutorialPassed != ProfileProgressionInfo.Tutorial.None)
			{
				xmlElement.SetAttribute("tutorial_passed", ((int)this.TutorialPassed).ToString(CultureInfo.InvariantCulture));
			}
			if (!updateMode || this.ClassUnlocked != ProfileProgressionInfo.PlayerClass.None)
			{
				xmlElement.SetAttribute("class_unlocked", ((int)this.ClassUnlocked).ToString(CultureInfo.InvariantCulture));
			}
			return xmlElement;
		}

		// Token: 0x06001DA5 RID: 7589 RVA: 0x000781A0 File Offset: 0x000765A0
		public override string ToString()
		{
			return string.Format("ProfileID: {0}, MissionPassCounter: {1}, ZombieMissionPassCounter: {2}, CampaignPassCounter: {3}, VolcanoCampaignPassConter: {4}, AnubisCampaignPassCounter: {5}, ZombieTowerCompaighPassCounter: {6}, IceBreakerCampaignPassCounter: {7}, TutorialUnlocked: {8}, TutorialPassed: {9}, ClassUnlocked: {10}", new object[]
			{
				this.ProfileId,
				this.MissionPassCounter,
				this.ZombieMissionPassCounter,
				this.CampaignPassCounter,
				this.VolcanoCampaignPassCounter,
				this.AnubisCampaignPassCounter,
				this.ZombieTowerCampaignPassCounter,
				this.IceBreakerCampaignPassCounter,
				this.TutorialUnlocked,
				this.TutorialPassed,
				this.ClassUnlocked
			});
		}

		// Token: 0x06001DA6 RID: 7590 RVA: 0x0007825A File Offset: 0x0007665A
		private int SetBit(int bits, int bit)
		{
			bits |= bit;
			return bits;
		}

		// Token: 0x06001DA7 RID: 7591 RVA: 0x00078264 File Offset: 0x00076664
		public static ProfileProgressionInfo.PlayerClass PlayerClassFromClassId(int classId)
		{
			int num = 1 << classId;
			return (ProfileProgressionInfo.PlayerClass)((!Enum.IsDefined(typeof(ProfileProgressionInfo.PlayerClass), num)) ? 0 : num);
		}

		// Token: 0x06001DA8 RID: 7592 RVA: 0x00078299 File Offset: 0x00076699
		public static ProfileProgressionInfo.PlayerClass PlayerClassFromClassId(uint classId)
		{
			return ProfileProgressionInfo.PlayerClassFromClassId((int)classId);
		}

		// Token: 0x06001DA9 RID: 7593 RVA: 0x000782A1 File Offset: 0x000766A1
		public static ProfileProgressionInfo.PlayerClass PlayerClassFromClassChar(char classChar)
		{
			classChar = char.ToLower(classChar);
			if (ProfileProgressionInfo.ClassCharToClass.ContainsKey(classChar))
			{
				return ProfileProgressionInfo.ClassCharToClass[classChar];
			}
			throw new ArgumentException(string.Format("Invalid class char {0}", classChar));
		}

		// Token: 0x04000E13 RID: 3603
		public static readonly string MedicClassName = ProfileProgressionInfo.PlayerClass.Medic.ToString();

		// Token: 0x04000E14 RID: 3604
		public static readonly string RiflemanClassName = ProfileProgressionInfo.PlayerClass.Rifleman.ToString();

		// Token: 0x04000E15 RID: 3605
		public static readonly string EngineerClassName = ProfileProgressionInfo.PlayerClass.Engineer.ToString();

		// Token: 0x04000E16 RID: 3606
		public static readonly string SniperClassName = ProfileProgressionInfo.PlayerClass.Sniper.ToString();

		// Token: 0x04000E17 RID: 3607
		public static readonly string ObsoleteSniperClassName = "Recon";

		// Token: 0x04000E24 RID: 3620
		private readonly IDALService m_dalService;

		// Token: 0x04000E25 RID: 3621
		private static readonly List<Tuple<ProfileProgressionInfo.PlayerClass, string, char>> m_classes = new List<Tuple<ProfileProgressionInfo.PlayerClass, string, char>>
		{
			Tuple.Create<ProfileProgressionInfo.PlayerClass, string, char>(ProfileProgressionInfo.PlayerClass.Rifleman, ProfileProgressionInfo.PlayerClass.Rifleman.ToString(), ProfileProgressionInfo.PlayerClass.Rifleman.ToString().First<char>()),
			Tuple.Create<ProfileProgressionInfo.PlayerClass, string, char>(ProfileProgressionInfo.PlayerClass.Sniper, ProfileProgressionInfo.PlayerClass.Sniper.ToString(), ProfileProgressionInfo.PlayerClass.Sniper.ToString().First<char>()),
			Tuple.Create<ProfileProgressionInfo.PlayerClass, string, char>(ProfileProgressionInfo.PlayerClass.Medic, ProfileProgressionInfo.PlayerClass.Medic.ToString(), ProfileProgressionInfo.PlayerClass.Medic.ToString().First<char>()),
			Tuple.Create<ProfileProgressionInfo.PlayerClass, string, char>(ProfileProgressionInfo.PlayerClass.Engineer, ProfileProgressionInfo.PlayerClass.Engineer.ToString(), ProfileProgressionInfo.PlayerClass.Engineer.ToString().First<char>())
		};

		// Token: 0x04000E26 RID: 3622
		private static readonly Dictionary<string, string> m_classShortToClassName = ProfileProgressionInfo.m_classes.ToDictionary((Tuple<ProfileProgressionInfo.PlayerClass, string, char> e) => e.Item3.ToString(), (Tuple<ProfileProgressionInfo.PlayerClass, string, char> e) => e.Item2, StringComparer.InvariantCultureIgnoreCase);

		// Token: 0x04000E27 RID: 3623
		public static readonly Dictionary<char, ProfileProgressionInfo.PlayerClass> ClassCharToClass = ProfileProgressionInfo.m_classes.ToDictionary((Tuple<ProfileProgressionInfo.PlayerClass, string, char> e) => e.Item3, (Tuple<ProfileProgressionInfo.PlayerClass, string, char> e) => e.Item1);

		// Token: 0x04000E28 RID: 3624
		public static readonly Dictionary<ProfileProgressionInfo.PlayerClass, char> ClassToClassChar = ProfileProgressionInfo.m_classes.ToDictionary((Tuple<ProfileProgressionInfo.PlayerClass, string, char> e) => e.Item1, (Tuple<ProfileProgressionInfo.PlayerClass, string, char> e) => e.Item3);

		// Token: 0x02000559 RID: 1369
		[Flags]
		internal enum Tutorial
		{
			// Token: 0x04000E2B RID: 3627
			None = 0,
			// Token: 0x04000E2C RID: 3628
			Tutorial_1 = 1,
			// Token: 0x04000E2D RID: 3629
			Tutorial_2 = 2,
			// Token: 0x04000E2E RID: 3630
			Tutorial_3 = 4,
			// Token: 0x04000E2F RID: 3631
			Tutorial_4 = 8,
			// Token: 0x04000E30 RID: 3632
			Tutorial_5 = 16,
			// Token: 0x04000E31 RID: 3633
			Tutorial_6 = 32,
			// Token: 0x04000E32 RID: 3634
			Tutorial_7 = 64,
			// Token: 0x04000E33 RID: 3635
			Tutorial_8 = 128,
			// Token: 0x04000E34 RID: 3636
			All = 268435455
		}

		// Token: 0x0200055A RID: 1370
		[Flags]
		internal enum PlayerClass
		{
			// Token: 0x04000E36 RID: 3638
			None = 0,
			// Token: 0x04000E37 RID: 3639
			Rifleman = 1,
			// Token: 0x04000E38 RID: 3640
			Heavy = 2,
			// Token: 0x04000E39 RID: 3641
			Sniper = 4,
			// Token: 0x04000E3A RID: 3642
			Medic = 8,
			// Token: 0x04000E3B RID: 3643
			Engineer = 16,
			// Token: 0x04000E3C RID: 3644
			All = 268435455
		}

		// Token: 0x0200055B RID: 1371
		[Flags]
		internal enum MissionType
		{
			// Token: 0x04000E3E RID: 3646
			None = 0,
			// Token: 0x04000E3F RID: 3647
			TrainingMission = 1,
			// Token: 0x04000E40 RID: 3648
			EasyMission = 2,
			// Token: 0x04000E41 RID: 3649
			NormalMission = 4,
			// Token: 0x04000E42 RID: 3650
			HardMission = 8,
			// Token: 0x04000E43 RID: 3651
			SurvivalMission = 16,
			// Token: 0x04000E44 RID: 3652
			ZombieEasy = 32,
			// Token: 0x04000E45 RID: 3653
			ZombieNormal = 64,
			// Token: 0x04000E46 RID: 3654
			ZombieHard = 128,
			// Token: 0x04000E47 RID: 3655
			CampaignSections = 256,
			// Token: 0x04000E48 RID: 3656
			CampaignSection1 = 512,
			// Token: 0x04000E49 RID: 3657
			CampaignSection2 = 1024,
			// Token: 0x04000E4A RID: 3658
			CampaignSection3 = 2048,
			// Token: 0x04000E4B RID: 3659
			TutorialMission = 4096,
			// Token: 0x04000E4C RID: 3660
			VolcanoEasy = 8192,
			// Token: 0x04000E4D RID: 3661
			VolcanoNormal = 16384,
			// Token: 0x04000E4E RID: 3662
			VolcanoHard = 32768,
			// Token: 0x04000E4F RID: 3663
			VolcanoSurvival = 65536,
			// Token: 0x04000E50 RID: 3664
			AnubisEasy = 131072,
			// Token: 0x04000E51 RID: 3665
			AnubisNormal = 262144,
			// Token: 0x04000E52 RID: 3666
			AnubisHard = 524288,
			// Token: 0x04000E53 RID: 3667
			ZombieTowerEasy = 1048576,
			// Token: 0x04000E54 RID: 3668
			ZombieTowerNormal = 2097152,
			// Token: 0x04000E55 RID: 3669
			ZombieTowerHard = 4194304,
			// Token: 0x04000E56 RID: 3670
			IceBreakerEasy = 8388608,
			// Token: 0x04000E57 RID: 3671
			IceBreakerNormal = 16777216,
			// Token: 0x04000E58 RID: 3672
			IceBreakerHard = 33554432,
			// Token: 0x04000E59 RID: 3673
			All = 268435455
		}
	}
}

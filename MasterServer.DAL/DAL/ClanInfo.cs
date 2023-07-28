using System;
using System.Text;
using System.Xml;
using Util.Common;

namespace MasterServer.DAL
{
	// Token: 0x02000017 RID: 23
	[Serializable]
	public class ClanInfo
	{
		// Token: 0x06000043 RID: 67 RVA: 0x000028A0 File Offset: 0x00000CA0
		public XmlElement ToXml(XmlDocument factory, bool withDescription = true)
		{
			XmlElement xmlElement = factory.CreateElement("clan");
			xmlElement.SetAttribute("name", this.Name);
			xmlElement.SetAttribute("clan_id", this.ClanID.ToString());
			if (withDescription)
			{
				xmlElement.SetAttribute("description", this.Description);
			}
			xmlElement.SetAttribute("creation_date", this.CreationDate.ToString());
			xmlElement.SetAttribute("master", this.MasterNickname);
			xmlElement.SetAttribute("clan_points", this.ClanPoints.ToString());
			xmlElement.SetAttribute("members", this.MembersCount.ToString());
			xmlElement.SetAttribute("master_badge", this.MasterBanner.Badge.ToString());
			xmlElement.SetAttribute("master_stripe", this.MasterBanner.Stripe.ToString());
			xmlElement.SetAttribute("master_mark", this.MasterBanner.Mark.ToString());
			return xmlElement;
		}

		// Token: 0x06000044 RID: 68 RVA: 0x000029C8 File Offset: 0x00000DC8
		public override string ToString()
		{
			return string.Format("ClanID={0}, Name={1}, Description={2}, CreationDate={3}, ClanPoints={4}, MasterNickname={5}, MembersCount={6}, Badge={7}, Mark={8}, Stripe={9}, MasterId={10}", new object[]
			{
				this.ClanID,
				this.Name,
				Encoding.UTF8.GetString(Convert.FromBase64String(this.Description)),
				TimeUtils.UTCTimestampToUTCTime(this.CreationDate),
				this.ClanPoints,
				this.MasterNickname,
				this.MembersCount,
				this.MasterBanner.Badge,
				this.MasterBanner.Mark,
				this.MasterBanner.Stripe,
				this.MasterId
			});
		}

		// Token: 0x04000031 RID: 49
		public ulong ClanID;

		// Token: 0x04000032 RID: 50
		public string Name;

		// Token: 0x04000033 RID: 51
		public string Description;

		// Token: 0x04000034 RID: 52
		public ulong CreationDate;

		// Token: 0x04000035 RID: 53
		public ulong ClanPoints;

		// Token: 0x04000036 RID: 54
		public string MasterNickname;

		// Token: 0x04000037 RID: 55
		public int MembersCount;

		// Token: 0x04000038 RID: 56
		public SBannerInfo MasterBanner;

		// Token: 0x04000039 RID: 57
		public ulong MasterId;
	}
}

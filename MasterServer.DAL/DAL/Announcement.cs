using System;
using System.Xml;

namespace MasterServer.DAL
{
	// Token: 0x02000010 RID: 16
	[Serializable]
	public class Announcement
	{
		// Token: 0x0600001F RID: 31 RVA: 0x00002474 File Offset: 0x00000874
		public bool ReadyToSend()
		{
			DateTime utcNow = DateTime.UtcNow;
			return this.StartTimeUTC <= utcNow && utcNow <= this.EndTimeUTC;
		}

		// Token: 0x06000020 RID: 32 RVA: 0x000024A7 File Offset: 0x000008A7
		public bool IsServerSupported(string serverName)
		{
			return string.IsNullOrEmpty(this.Server) || this.Server.Equals(serverName, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000021 RID: 33 RVA: 0x000024C9 File Offset: 0x000008C9
		public bool IsChannelSupported(string channel)
		{
			return string.IsNullOrEmpty(this.Channel) || this.Channel.Equals(channel, StringComparison.OrdinalIgnoreCase);
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000024EC File Offset: 0x000008EC
		public XmlElement ToXml(XmlDocument factory)
		{
			XmlElement xmlElement = factory.CreateElement("announcement");
			DateTime utcNow = DateTime.UtcNow;
			double num = (this.RepeatTimes <= 0U) ? 0.0 : ((this.EndTimeUTC - this.StartTimeUTC).TotalSeconds / this.RepeatTimes);
			uint num2 = this.RepeatTimes - ((num <= 0.0) ? 0U : ((uint)Math.Truncate((utcNow - this.StartTimeUTC).TotalSeconds / num)));
			xmlElement.SetAttribute("id", this.ID.ToString());
			xmlElement.SetAttribute("is_system", (!this.IsSystem) ? "0" : "1");
			xmlElement.SetAttribute("frequency", num.ToString());
			xmlElement.SetAttribute("repeat_time", num2.ToString());
			xmlElement.SetAttribute("message", this.Message);
			xmlElement.SetAttribute("server", this.Server);
			xmlElement.SetAttribute("channel", this.Channel.ToLower());
			XmlElement xmlElement2 = xmlElement;
			string name = "place";
			int place = (int)this.Place;
			xmlElement2.SetAttribute(name, place.ToString());
			return xmlElement;
		}

		// Token: 0x06000023 RID: 35 RVA: 0x00002650 File Offset: 0x00000A50
		public override string ToString()
		{
			return string.Format("ID: {0}, Message: {1}, StartTime: {2}, EndTime: {3}, IsSystem: {4}, RepeatTimes: {5}, Target: {6}, Channel: {7}, Server: {8}, Place: {9}", new object[]
			{
				this.ID,
				this.Message,
				this.StartTimeUTC.ToLocalTime(),
				this.EndTimeUTC.ToLocalTime(),
				this.IsSystem,
				this.RepeatTimes,
				this.Target,
				this.Channel,
				this.Server,
				this.Place
			});
		}

		// Token: 0x04000020 RID: 32
		public ulong ID;

		// Token: 0x04000021 RID: 33
		public DateTime StartTimeUTC;

		// Token: 0x04000022 RID: 34
		public DateTime EndTimeUTC;

		// Token: 0x04000023 RID: 35
		public bool IsSystem;

		// Token: 0x04000024 RID: 36
		public uint RepeatTimes;

		// Token: 0x04000025 RID: 37
		public ulong Target;

		// Token: 0x04000026 RID: 38
		public string Channel;

		// Token: 0x04000027 RID: 39
		public string Server;

		// Token: 0x04000028 RID: 40
		public EAnnouncementPlace Place;

		// Token: 0x04000029 RID: 41
		public string Message;
	}
}

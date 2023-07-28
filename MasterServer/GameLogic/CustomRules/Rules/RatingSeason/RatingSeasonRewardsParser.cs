using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.GameLogic.CustomRules.Rules.RatingSeason.Exceptions;

namespace MasterServer.GameLogic.CustomRules.Rules.RatingSeason
{
	// Token: 0x020000A4 RID: 164
	public class RatingSeasonRewardsParser
	{
		// Token: 0x0600028D RID: 653 RVA: 0x0000CD70 File Offset: 0x0000B170
		public Dictionary<uint, RatingSeasonReward> Parse(XmlElement config)
		{
			Dictionary<uint, RatingSeasonReward> dictionary = new Dictionary<uint, RatingSeasonReward>();
			Dictionary<uint, string> dictionary2 = this.ParseRewards(config, "season_result_rewards");
			Dictionary<uint, string> dictionary3 = this.ParseRewards(config, "rating_achieved_rewards");
			IEnumerable<uint> enumerable = dictionary2.Keys.Concat(dictionary3.Keys).Distinct<uint>();
			foreach (uint key in enumerable)
			{
				string seasonResultRewardName;
				dictionary2.TryGetValue(key, out seasonResultRewardName);
				string ratingAchievedRewardName;
				dictionary3.TryGetValue(key, out ratingAchievedRewardName);
				dictionary.Add(key, new RatingSeasonReward(seasonResultRewardName, ratingAchievedRewardName));
			}
			return dictionary;
		}

		// Token: 0x0600028E RID: 654 RVA: 0x0000CE24 File Offset: 0x0000B224
		private Dictionary<uint, string> ParseRewards(XmlElement config, string rewardType)
		{
			Dictionary<uint, string> dictionary = new Dictionary<uint, string>();
			XmlNode xmlNode = config.SelectSingleNode(rewardType);
			if (xmlNode == null)
			{
				return dictionary;
			}
			XmlNodeList xmlNodeList = xmlNode.SelectNodes("reward");
			IEnumerator enumerator = xmlNodeList.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode2 = (XmlNode)obj;
					try
					{
						string attributeFromXmlNode = this.GetAttributeFromXmlNode(xmlNode2, "rating_level");
						uint key = uint.Parse(attributeFromXmlNode);
						string attributeFromXmlNode2 = this.GetAttributeFromXmlNode(xmlNode2, "name");
						dictionary.Add(key, attributeFromXmlNode2);
					}
					catch (Exception innerException)
					{
						string message = string.Format("Unable to parse rating season reward under {0} section: {1}", rewardType, xmlNode2.OuterXml);
						throw new RatingSeasonRewardsParseException(message, innerException);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			return dictionary;
		}

		// Token: 0x0600028F RID: 655 RVA: 0x0000CF08 File Offset: 0x0000B308
		private string GetAttributeFromXmlNode(XmlNode node, string attributeName)
		{
			XmlAttribute xmlAttribute = node.Attributes[attributeName];
			if (xmlAttribute != null)
			{
				return xmlAttribute.Value;
			}
			string message = string.Format("Mandatory attribute {0} not found", attributeName);
			throw new RatingSeasonRewardsParseException(message);
		}
	}
}

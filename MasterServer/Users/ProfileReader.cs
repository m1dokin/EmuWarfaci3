using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using MasterServer.Common;
using MasterServer.Core;
using MasterServer.DAL;
using MasterServer.DAL.RatingSystem;
using MasterServer.GameLogic.CustomRules.Rules;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.GameLogic.LobbyChat;
using MasterServer.GameLogic.NotificationSystem;
using MasterServer.GameLogic.RatingSystem;
using MasterServer.Platform.Payment.Exceptions;

namespace MasterServer.Users
{
	// Token: 0x020007DF RID: 2015
	internal class ProfileReader
	{
		// Token: 0x06002937 RID: 10551 RVA: 0x000B27B7 File Offset: 0x000B0BB7
		public ProfileReader(ProfileProxy profile)
		{
			this.m_profile = profile;
			this.m_tagService = ServicesManager.GetService<ITagService>();
			this.m_ratingSeasonService = ServicesManager.GetService<IRatingSeasonService>();
			this.m_ratingGameBanService = ServicesManager.GetService<IRatingGameBanService>();
			this.m_repairRepository = ServicesManager.GetService<IItemRepairDescriptionRepository>();
		}

		// Token: 0x06002938 RID: 10552 RVA: 0x000B27F4 File Offset: 0x000B0BF4
		public void FillCharacterInfo(XmlElement parentNode)
		{
			Rating playerRating = this.m_ratingSeasonService.GetPlayerRating(this.m_profile.ProfileID);
			parentNode.SetAttribute("nick", this.m_profile.ProfileInfo.Nickname);
			parentNode.SetAttribute("gender", this.m_profile.ProfileInfo.Gender);
			parentNode.SetAttribute("height", this.m_profile.ProfileInfo.Height.ToString());
			parentNode.SetAttribute("fatness", (this.m_profile.ProfileInfo.Fatness - 1f).ToString());
			parentNode.SetAttribute("head", this.m_profile.ProfileInfo.Head);
			parentNode.SetAttribute("current_class", this.m_profile.ProfileInfo.CurrentClass.ToString());
			parentNode.SetAttribute("experience", this.m_profile.ProfileInfo.RankInfo.Points.ToString());
			parentNode.SetAttribute("pvp_rating_points", playerRating.Points.ToString());
			parentNode.SetAttribute("banner_badge", this.m_profile.ProfileInfo.Banner.Badge.ToString());
			parentNode.SetAttribute("banner_mark", this.m_profile.ProfileInfo.Banner.Mark.ToString());
			parentNode.SetAttribute("banner_stripe", this.m_profile.ProfileInfo.Banner.Stripe.ToString());
			RatingGamePlayerBanInfo playerBanInfo = this.m_ratingGameBanService.GetPlayerBanInfo(this.m_profile.ProfileID);
			parentNode.AppendChild(playerBanInfo.ToXml(parentNode.OwnerDocument));
		}

		// Token: 0x06002939 RID: 10553 RVA: 0x000B2A08 File Offset: 0x000B0E08
		public void FillProfileTags(XmlElement parentNode)
		{
			UserTags userTags = this.m_tagService.GetUserTags(this.m_profile.UserID);
			if (userTags.HasAny())
			{
				XmlElement xmlElement = parentNode.OwnerDocument.CreateElement("profile_tags");
				foreach (string value in userTags.List)
				{
					XmlElement xmlElement2 = parentNode.OwnerDocument.CreateElement("tag");
					xmlElement2.SetAttribute("name", value);
					xmlElement.AppendChild(xmlElement2);
				}
				parentNode.AppendChild(xmlElement);
			}
		}

		// Token: 0x0600293A RID: 10554 RVA: 0x000B2AC0 File Offset: 0x000B0EC0
		public void ReadProfileMoney(XmlElement parentNode)
		{
			Dictionary<Currency, ulong> dictionary = new Dictionary<Currency, ulong>();
			foreach (Currency currency in Enum.GetValues(typeof(Currency)).Cast<Currency>())
			{
				dictionary[currency] = 0UL;
				try
				{
					dictionary[currency] = this.m_profile.Account(currency).Money;
				}
				catch (PaymentServiceException e)
				{
					Log.Error(e);
				}
				catch (ArgumentException)
				{
				}
			}
			parentNode.SetAttribute("game_money", dictionary[Currency.GameMoney].ToString());
			parentNode.SetAttribute("cry_money", dictionary[Currency.CryMoney].ToString());
			parentNode.SetAttribute("crown_money", dictionary[Currency.CrownMoney].ToString());
		}

		// Token: 0x0600293B RID: 10555 RVA: 0x000B2BE0 File Offset: 0x000B0FE0
		public void ReadProfileItems(XmlElement parentNode)
		{
			bool flag = DefaultProfile.ResetProfileItems(this.m_profile);
			if (flag)
			{
				Log.Warning<ulong>("Equipment for profile {0} has been reset to default", this.m_profile.ProfileID);
			}
			Dictionary<ulong, SProfileItem> profileItems = this.m_profile.ProfileItems;
			Dictionary<ulong, SItem> dictionary = new Dictionary<ulong, SItem>(this.m_profile.UnlockedItems);
			IItemsValidator service = ServicesManager.GetService<IItemsValidator>();
			foreach (ulong key in profileItems.Keys)
			{
				SProfileItem sprofileItem = profileItems[key];
				SItem gameItem = sprofileItem.GameItem;
				if (gameItem.IsAttachmentItem)
				{
					if (!dictionary.ContainsKey(gameItem.ID))
					{
						if (!sprofileItem.IsDefault)
						{
							Log.Warning<ulong, string>("Profile {0} not-unlocked attachment {1} deleted", this.m_profile.ProfileID, gameItem.Name);
							this.m_profile.DeleteProfileItem(sprofileItem.ProfileItemID);
						}
					}
					else
					{
						dictionary.Remove(gameItem.ID);
						ulong slotIds = service.GetSlotIds(gameItem);
						if (slotIds != sprofileItem.SlotIDs)
						{
							Log.Warning("Profile {0} fix slot ids {1} -> {2} for attachment {3}", new object[]
							{
								this.m_profile.ProfileID,
								sprofileItem.SlotIDs,
								slotIds,
								gameItem.Name
							});
							this.m_profile.UpdateProfileItem(sprofileItem.ProfileItemID, slotIds, sprofileItem.AttachedTo, sprofileItem.Config);
						}
					}
				}
			}
			foreach (SProfileItem item in this.m_profile.ProfileItems.Values)
			{
				XmlElement xml = ServerItem.GetXml(item, parentNode.OwnerDocument, "item");
				this.AppendRepairCostIfBroken(item, xml);
				parentNode.AppendChild(xml);
			}
		}

		// Token: 0x0600293C RID: 10556 RVA: 0x000B2E10 File Offset: 0x000B1210
		public void ReadDurableAndConsumableItems(XmlElement parentNode)
		{
			Dictionary<ulong, SProfileItem> profileItems = this.m_profile.ProfileItems;
			foreach (SProfileItem sprofileItem in profileItems.Values)
			{
				OfferType offerType = sprofileItem.OfferType;
				if (offerType != OfferType.Permanent)
				{
					if (offerType == OfferType.Consumable)
					{
						parentNode.AppendChild(ServerItem.GetXml(sprofileItem, parentNode.OwnerDocument, "consumable_item"));
					}
				}
				else
				{
					XmlElement xml = ServerItem.GetXml(sprofileItem, parentNode.OwnerDocument, "durability_item");
					this.AppendRepairCostIfBroken(sprofileItem, xml);
					parentNode.AppendChild(xml);
				}
			}
		}

		// Token: 0x0600293D RID: 10557 RVA: 0x000B2ED4 File Offset: 0x000B12D4
		private void AppendRepairCostIfBroken(SProfileItem item, XmlElement itemNode)
		{
			if (!item.IsBroken)
			{
				return;
			}
			SRepairItemDesc rid;
			if (!this.m_repairRepository.GetRepairItemDesc(item.ItemID, item.CustomerItem.CatalogItem.ID, out rid))
			{
				return;
			}
			itemNode.SetAttribute("repair_cost", item.CalculateRepairCost(rid).ToString());
		}

		// Token: 0x0600293E RID: 10558 RVA: 0x000B2F38 File Offset: 0x000B1338
		public bool ReadExpiredItems(XmlElement parentNode)
		{
			Dictionary<ulong, SProfileItem> expiredProfileItems = this.m_profile.GetExpiredProfileItems();
			foreach (SProfileItem sprofileItem in expiredProfileItems.Values)
			{
				XmlElement xmlElement = parentNode.OwnerDocument.CreateElement("expired_item");
				xmlElement.SetAttribute("id", sprofileItem.ProfileItemID.ToString());
				xmlElement.SetAttribute("name", sprofileItem.GameItem.Name);
				xmlElement.SetAttribute("slot_ids", sprofileItem.SlotIDs.ToString());
				parentNode.AppendChild(xmlElement);
			}
			return expiredProfileItems.Count > 0;
		}

		// Token: 0x0600293F RID: 10559 RVA: 0x000B3014 File Offset: 0x000B1414
		public void ReadSponsorInfo(XmlElement parentNode)
		{
			IItemCache service = ServicesManager.GetService<IItemCache>();
			Dictionary<ulong, SItem> allItems = service.GetAllItems();
			XmlElement xmlElement = parentNode.OwnerDocument.CreateElement("sponsor_info");
			foreach (SSponsorPoints ssponsorPoints in this.m_profile.SponsorPoints)
			{
				XmlElement xmlElement2 = parentNode.OwnerDocument.CreateElement("sponsor");
				xmlElement2.SetAttribute("sponsor_id", ssponsorPoints.SponsorID.ToString());
				xmlElement2.SetAttribute("sponsor_points", ssponsorPoints.RankInfo.Points.ToString());
				string value = string.Empty;
				if (allItems.ContainsKey(ssponsorPoints.NextUnlockItemId))
				{
					value = allItems[ssponsorPoints.NextUnlockItemId].Name;
				}
				xmlElement2.SetAttribute("next_unlock_item", value);
				xmlElement.AppendChild(xmlElement2);
			}
			parentNode.AppendChild(xmlElement);
		}

		// Token: 0x06002940 RID: 10560 RVA: 0x000B3130 File Offset: 0x000B1530
		public void ReadUnlockedItems(XmlElement parentNode)
		{
			foreach (SItem sitem in this.m_profile.UnlockedItems.Values)
			{
				XmlElement xmlElement = parentNode.OwnerDocument.CreateElement("unlocked_item");
				xmlElement.SetAttribute("id", sitem.ID.ToString());
				parentNode.AppendChild(xmlElement);
			}
		}

		// Token: 0x06002941 RID: 10561 RVA: 0x000B31C4 File Offset: 0x000B15C4
		public void ReadPendingNotifications(XmlElement parentNode)
		{
			INotificationService service = ServicesManager.GetService<INotificationService>();
			IAnnouncementService service2 = ServicesManager.GetService<IAnnouncementService>();
			foreach (SNotification snotification in service.PopPending(this.m_profile.ProfileID))
			{
				parentNode.AppendChild(snotification.ToXml(service, parentNode.OwnerDocument));
			}
			foreach (Announcement announcement in service2.GetAnnouncementsToSend())
			{
				if (announcement.Target == 0UL || announcement.Target == this.m_profile.ProfileID)
				{
					SNotification snotification2 = new SNotification
					{
						Type = ENotificationType.Announcement,
						ConfirmationType = EConfirmationType.None,
						ExpirationTimeUTC = DateTime.UtcNow,
						Data = Utils.CreateByteArrayFromType<Announcement>(announcement)
					};
					parentNode.AppendChild(snotification2.ToXml(service, parentNode.OwnerDocument));
				}
			}
		}

		// Token: 0x06002942 RID: 10562 RVA: 0x000B3300 File Offset: 0x000B1700
		public void ReadProgression(XmlElement parentNode)
		{
			parentNode.AppendChild(this.m_profile.UserInfo.ProfileProgression.ToXml(parentNode.OwnerDocument));
		}

		// Token: 0x06002943 RID: 10563 RVA: 0x000B3324 File Offset: 0x000B1724
		public void ReadChatChannels(XmlElement node)
		{
			IChatConferences service = ServicesManager.GetService<IChatConferences>();
			XmlElement xmlElement = node.OwnerDocument.CreateElement("chat_channels");
			node.AppendChild(xmlElement);
			foreach (EChatChannel echatChannel in ProfileReader.CHANNEL_CHATS)
			{
				ChatChannelID chatChannelID = service.GenerateChannelId(echatChannel, this.m_profile.ProfileID);
				if (!chatChannelID.IsEmpty())
				{
					XmlElement xmlElement2 = node.OwnerDocument.CreateElement("chat");
					xmlElement.AppendChild(xmlElement2);
					XmlElement xmlElement3 = xmlElement2;
					string name = "channel";
					int num = (int)echatChannel;
					xmlElement3.SetAttribute(name, num.ToString());
					xmlElement2.SetAttribute("channel_id", chatChannelID.ChannelID);
					xmlElement2.SetAttribute("service_id", chatChannelID.ConferenceID);
				}
			}
		}

		// Token: 0x06002944 RID: 10564 RVA: 0x000B33F0 File Offset: 0x000B17F0
		public void ReadLoginBonusProgress(XmlElement node)
		{
			ConsecutiveLoginBonusRuleState loginBonusState = this.m_profile.LoginBonusState;
			if (loginBonusState != null)
			{
				XmlElement xmlElement = node.OwnerDocument.CreateElement("login_bonus");
				node.AppendChild(xmlElement);
				xmlElement.SetAttribute("current_streak", loginBonusState.PrevStreak.ToString());
				xmlElement.SetAttribute("current_reward", loginBonusState.PrevReward.ToString());
			}
		}

		// Token: 0x040015F5 RID: 5621
		private static readonly EChatChannel[] CHANNEL_CHATS = new EChatChannel[1];

		// Token: 0x040015F6 RID: 5622
		private readonly ProfileProxy m_profile;

		// Token: 0x040015F7 RID: 5623
		private readonly ITagService m_tagService;

		// Token: 0x040015F8 RID: 5624
		private readonly IRatingSeasonService m_ratingSeasonService;

		// Token: 0x040015F9 RID: 5625
		private readonly IRatingGameBanService m_ratingGameBanService;

		// Token: 0x040015FA RID: 5626
		private readonly IItemRepairDescriptionRepository m_repairRepository;
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using HK2Net;
using MasterServer.DAL;
using MasterServer.ElectronicCatalog;
using MasterServer.GameLogic.ItemsSystem;
using MasterServer.Users;

namespace MasterServer.GameLogic.RandomBoxValidationSystem
{
	// Token: 0x02000592 RID: 1426
	[Service]
	[Singleton]
	internal class RandomBoxValidationService : IMultipleOfferValidationProvider, IDisposable
	{
		// Token: 0x06001EAB RID: 7851 RVA: 0x0007C8F9 File Offset: 0x0007ACF9
		public RandomBoxValidationService(IUserRepository userRepository, IShopService shopService)
		{
			this.m_userRepository = userRepository;
			this.m_shopService = shopService;
			this.m_usersRandomBoxes = new Dictionary<ulong, RandomBoxValidationService.ValidationSequence>();
			this.m_sortedRandomBoxOffers = new Dictionary<ulong, List<StoreOffer>>();
		}

		// Token: 0x06001EAC RID: 7852 RVA: 0x0007C925 File Offset: 0x0007AD25
		public void Initialize()
		{
			this.m_shopService.OffersUpdated += this.OnOffersUpdated;
			this.m_userRepository.UserLoggedOut += this.OnUserLoggedOut;
		}

		// Token: 0x06001EAD RID: 7853 RVA: 0x0007C955 File Offset: 0x0007AD55
		public void Dispose()
		{
			this.m_shopService.OffersUpdated -= this.OnOffersUpdated;
			this.m_userRepository.UserLoggedOut -= this.OnUserLoggedOut;
		}

		// Token: 0x06001EAE RID: 7854 RVA: 0x0007C988 File Offset: 0x0007AD88
		public IEnumerable<ulong> Validate(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			IEnumerable<StoreOffer> in_progress_boxes = from offer in this.m_sortedRandomBoxOffers.Values.SelectMany((List<StoreOffer> x) => x)
			where offerIds.Contains(offer.StoreID)
			select offer;
			if (!in_progress_boxes.Any<StoreOffer>())
			{
				return offerIds;
			}
			string randomBoxName = in_progress_boxes.First<StoreOffer>().Content.Item.Name;
			bool flag = in_progress_boxes.Any((StoreOffer x) => x.Content.Item.Name != randomBoxName);
			if (flag)
			{
				throw new RandomBoxValidationException(string.Format("User {0} trying to buy different boxes at the same batch", user.ProfileID));
			}
			IEnumerable<ulong> source = from oid in offerIds
			where in_progress_boxes.Any((StoreOffer offer) => offer.StoreID == oid)
			select oid;
			bool flag2 = (from n in source
			group n by n).Any((IGrouping<ulong, ulong> c) => c.Count<ulong>() > 1);
			if (flag2)
			{
				throw new RandomBoxValidationException(string.Format("User {0} trying to buy the same box at the same batch", user.ProfileID));
			}
			object usersRandomBoxes = this.m_usersRandomBoxes;
			bool flag3 = false;
			IEnumerable<ulong> result;
			try
			{
				Monitor.Enter(usersRandomBoxes, ref flag3);
				RandomBoxValidationService.ValidationSequence currentSeq;
				if (!this.m_usersRandomBoxes.TryGetValue(user.ProfileID, out currentSeq))
				{
					currentSeq = new RandomBoxValidationService.ValidationSequence(in_progress_boxes.First<StoreOffer>().Content.Item.ID);
					this.m_usersRandomBoxes.Add(user.ProfileID, currentSeq);
				}
				if (currentSeq.OfferItemID != in_progress_boxes.First<StoreOffer>().Content.Item.ID)
				{
					currentSeq.Reset(in_progress_boxes.First<StoreOffer>().Content.Item.ID);
				}
				IEnumerable<StoreOffer> source2 = from offer in this.m_sortedRandomBoxOffers.Values.SelectMany((List<StoreOffer> x) => x)
				where offer.Content.Item.Name == randomBoxName
				select offer;
				if (in_progress_boxes.First<StoreOffer>().StoreID == source2.First<StoreOffer>().StoreID)
				{
					currentSeq.Reset(in_progress_boxes.First<StoreOffer>().Content.Item.ID);
				}
				IEnumerable<StoreOffer> first = from offer in source2
				where currentSeq.Confirmed.Contains(offer.StoreID)
				select offer;
				bool flag4 = first.Intersect(in_progress_boxes).Any<StoreOffer>();
				if (flag4)
				{
					throw new RandomBoxValidationException(string.Format("User {0} trying to buy offer in wrong order", user.ProfileID));
				}
				IEnumerable<StoreOffer> enumerable = first.Concat(in_progress_boxes);
				bool flag5 = enumerable.Except(source2.Take(enumerable.Count<StoreOffer>())).Any<StoreOffer>();
				if (flag5)
				{
					throw new RandomBoxValidationException(string.Format("User {0} trying to buy offer in wrong order", user.ProfileID));
				}
				currentSeq.InProgress = (from x in in_progress_boxes
				select x.StoreID).ToList<ulong>();
				result = currentSeq.InProgress.Concat(offerIds.Except(currentSeq.InProgress)).ToList<ulong>();
			}
			finally
			{
				if (flag3)
				{
					Monitor.Exit(usersRandomBoxes);
				}
			}
			return result;
		}

		// Token: 0x06001EAF RID: 7855 RVA: 0x0007CD5C File Offset: 0x0007B15C
		public void Confirm(UserInfo.User user, int supplierId, IEnumerable<ulong> offerIds)
		{
			object usersRandomBoxes = this.m_usersRandomBoxes;
			lock (usersRandomBoxes)
			{
				RandomBoxValidationService.ValidationSequence validationSequence;
				if (this.m_usersRandomBoxes.TryGetValue(user.ProfileID, out validationSequence))
				{
					if (validationSequence.InProgress != null)
					{
						validationSequence.Confirmed = validationSequence.Confirmed.Concat(validationSequence.InProgress.Intersect(offerIds)).ToList<ulong>();
					}
					validationSequence.InProgress = null;
				}
			}
		}

		// Token: 0x06001EB0 RID: 7856 RVA: 0x0007CDE8 File Offset: 0x0007B1E8
		private void OnOffersUpdated(IEnumerable<StoreOffer> offers)
		{
			Dictionary<ulong, List<StoreOffer>> dictionary = new Dictionary<ulong, List<StoreOffer>>();
			foreach (StoreOffer storeOffer in offers)
			{
				if (storeOffer.Content.Item.Type == "random_box" && !storeOffer.IsKeyPriceOffer())
				{
					List<StoreOffer> list;
					if (dictionary.TryGetValue(storeOffer.Content.Item.ID, out list))
					{
						list.Add(storeOffer);
					}
					else
					{
						dictionary.Add(storeOffer.Content.Item.ID, new List<StoreOffer>
						{
							storeOffer
						});
					}
				}
			}
			foreach (KeyValuePair<ulong, List<StoreOffer>> keyValuePair in dictionary)
			{
				List<StoreOffer> value = keyValuePair.Value;
				if (RandomBoxValidationService.<>f__mg$cache0 == null)
				{
					RandomBoxValidationService.<>f__mg$cache0 = new Comparison<StoreOffer>(RandomBoxValidationService.OffersComparator);
				}
				value.Sort(RandomBoxValidationService.<>f__mg$cache0);
			}
			Interlocked.Exchange<Dictionary<ulong, List<StoreOffer>>>(ref this.m_sortedRandomBoxOffers, dictionary);
		}

		// Token: 0x06001EB1 RID: 7857 RVA: 0x0007CF2C File Offset: 0x0007B32C
		private void OnUserLoggedOut(UserInfo.User user, ELogoutType logout_type)
		{
			object usersRandomBoxes = this.m_usersRandomBoxes;
			lock (usersRandomBoxes)
			{
				if (this.m_usersRandomBoxes.ContainsKey(user.ProfileID))
				{
					this.m_usersRandomBoxes.Remove(user.ProfileID);
				}
			}
		}

		// Token: 0x06001EB2 RID: 7858 RVA: 0x0007CF94 File Offset: 0x0007B394
		private static int OffersComparator(StoreOffer a, StoreOffer b)
		{
			if (a.Prices.Count != b.Prices.Count)
			{
				throw new RandomBoxValidationException(string.Format("Offer {0} and offer {1} have different amount elements in prices list", a.StoreID, b.StoreID));
			}
			PriceTag priceTag = a.GetPriceTag();
			PriceTag priceTag2 = b.GetPriceTag();
			if (priceTag.Currency == priceTag2.Currency)
			{
				return -1 * priceTag.Price.CompareTo(priceTag2.Price);
			}
			throw new RandomBoxValidationException(string.Format("Offer {0} and offer {1} have prices in different currencies", a.StoreID, b.StoreID));
		}

		// Token: 0x04000EEB RID: 3819
		private readonly IUserRepository m_userRepository;

		// Token: 0x04000EEC RID: 3820
		private readonly IShopService m_shopService;

		// Token: 0x04000EED RID: 3821
		private readonly Dictionary<ulong, RandomBoxValidationService.ValidationSequence> m_usersRandomBoxes;

		// Token: 0x04000EEE RID: 3822
		private Dictionary<ulong, List<StoreOffer>> m_sortedRandomBoxOffers;

		// Token: 0x04000EF4 RID: 3828
		[CompilerGenerated]
		private static Comparison<StoreOffer> <>f__mg$cache0;

		// Token: 0x02000593 RID: 1427
		private class ValidationSequence
		{
			// Token: 0x06001EB8 RID: 7864 RVA: 0x0007D05A File Offset: 0x0007B45A
			public ValidationSequence(ulong ofId)
			{
				this.Reset(ofId);
			}

			// Token: 0x06001EB9 RID: 7865 RVA: 0x0007D069 File Offset: 0x0007B469
			public void Reset(ulong ofId)
			{
				this.OfferItemID = ofId;
				this.Confirmed = new List<ulong>();
			}

			// Token: 0x04000EF5 RID: 3829
			public ulong OfferItemID;

			// Token: 0x04000EF6 RID: 3830
			public List<ulong> Confirmed;

			// Token: 0x04000EF7 RID: 3831
			public List<ulong> InProgress;
		}
	}
}

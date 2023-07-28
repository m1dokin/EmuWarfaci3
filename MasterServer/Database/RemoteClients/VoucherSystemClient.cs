using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using MasterServer.DAL.VoucherSystem;
using MasterServer.GameLogic.VoucherSystem;
using MasterServer.GameLogic.VoucherSystem.Serializers;

namespace MasterServer.Database.RemoteClients
{
	// Token: 0x020001FC RID: 508
	internal class VoucherSystemClient : DALCacheProxy<IDALService>, IVoucherSystemClient
	{
		// Token: 0x06000A31 RID: 2609 RVA: 0x0002615F File Offset: 0x0002455F
		public void Reset(IVoucherSystem voucherSystem)
		{
			this.m_voucherSystem = voucherSystem;
		}

		// Token: 0x06000A32 RID: 2610 RVA: 0x00026168 File Offset: 0x00024568
		public ulong GetCurrentIndex(string gameStateKey)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.voucher_globals,
				set_func = (() => this.m_voucherSystem.GetCurrentIndex(gameStateKey))
			};
			return base.SetAndStore<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A33 RID: 2611 RVA: 0x000261BC File Offset: 0x000245BC
		public void SetCurrentIndex(string gameStateKey, ulong gameStateValue)
		{
			DALCacheProxy<IDALService>.SetOptionsScalar<ulong> options = new DALCacheProxy<IDALService>.SetOptionsScalar<ulong>
			{
				cache_domain = cache_domains.voucher_globals,
				set_func = (() => this.m_voucherSystem.SetCurrentIndex(gameStateKey, gameStateValue))
			};
			base.SetAndStore<ulong>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A34 RID: 2612 RVA: 0x00026218 File Offset: 0x00024618
		public bool AddVoucher(Voucher voucher)
		{
			DalVoucher dalVoucher = this.CreateDalVoucher(voucher);
			DALCacheProxy<IDALService>.SetOptionsScalar<bool> options = new DALCacheProxy<IDALService>.SetOptionsScalar<bool>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.user[voucher.UserId].voucher,
					cache_domains.user[voucher.UserId].all_voucher
				},
				set_func = (() => this.m_voucherSystem.AddVoucher(dalVoucher))
			};
			return base.SetAndClearScalar<bool>(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A35 RID: 2613 RVA: 0x000262A8 File Offset: 0x000246A8
		public IEnumerable<Voucher> GetCorruptedVouchers(ulong startIndex, int count)
		{
			DALCacheProxy<IDALService>.Options<DalVoucher> options = new DALCacheProxy<IDALService>.Options<DalVoucher>
			{
				get_data_stream = (() => this.m_voucherSystem.GetCorruptedVouchers(startIndex, count))
			};
			IEnumerable<DalVoucher> dataStream = base.GetDataStream<DalVoucher>(MethodBase.GetCurrentMethod(), options);
			VoucherXmlSerializer @object = new VoucherXmlSerializer();
			return dataStream.Select(new Func<DalVoucher, Voucher>(@object.Deserialize)).ToList<Voucher>();
		}

		// Token: 0x06000A36 RID: 2614 RVA: 0x00026318 File Offset: 0x00024718
		public IEnumerable<Voucher> GetNewVouchers(ulong userId)
		{
			DALCacheProxy<IDALService>.Options<DalVoucher> options = new DALCacheProxy<IDALService>.Options<DalVoucher>
			{
				cache_domain = cache_domains.user[userId].voucher,
				get_data_stream = (() => this.m_voucherSystem.GetNewVouchers(userId))
			};
			IEnumerable<DalVoucher> dataStream = base.GetDataStream<DalVoucher>(MethodBase.GetCurrentMethod(), options);
			VoucherXmlSerializer @object = new VoucherXmlSerializer();
			return dataStream.Select(new Func<DalVoucher, Voucher>(@object.Deserialize)).ToList<Voucher>();
		}

		// Token: 0x06000A37 RID: 2615 RVA: 0x000263A0 File Offset: 0x000247A0
		public IEnumerable<Voucher> GetAllVouchers(ulong userId)
		{
			DALCacheProxy<IDALService>.Options<DalVoucher> options = new DALCacheProxy<IDALService>.Options<DalVoucher>
			{
				cache_domain = cache_domains.user[userId].all_voucher,
				get_data_stream = (() => this.m_voucherSystem.GetAllVouchers(userId))
			};
			IEnumerable<DalVoucher> dataStream = base.GetDataStream<DalVoucher>(MethodBase.GetCurrentMethod(), options);
			VoucherXmlSerializer @object = new VoucherXmlSerializer();
			return dataStream.Select(new Func<DalVoucher, Voucher>(@object.Deserialize)).ToList<Voucher>();
		}

		// Token: 0x06000A38 RID: 2616 RVA: 0x00026428 File Offset: 0x00024828
		public Voucher UpdateVoucher(Voucher voucher)
		{
			DalVoucher dalVoucher = this.CreateDalVoucher(voucher);
			DALCacheProxy<IDALService>.SetOptionsScalar<DalVoucher> options = new DALCacheProxy<IDALService>.SetOptionsScalar<DalVoucher>
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.user[voucher.UserId].voucher,
					cache_domains.user[voucher.UserId].all_voucher
				},
				set_func = (() => this.m_voucherSystem.UpdateVoucher(dalVoucher))
			};
			dalVoucher = base.SetAndClearScalar<DalVoucher>(MethodBase.GetCurrentMethod(), options);
			VoucherXmlSerializer voucherXmlSerializer = new VoucherXmlSerializer();
			return voucherXmlSerializer.Deserialize(dalVoucher);
		}

		// Token: 0x06000A39 RID: 2617 RVA: 0x000264D4 File Offset: 0x000248D4
		public void CleanUpVouchers(ulong userId)
		{
			DALCacheProxy<IDALService>.SetOptions options = new DALCacheProxy<IDALService>.SetOptions
			{
				cache_domains = new cache_domain[]
				{
					cache_domains.user[userId].voucher,
					cache_domains.user[userId].all_voucher
				},
				set_func = (() => this.m_voucherSystem.CleanUpVouchers(userId))
			};
			base.SetAndClear(MethodBase.GetCurrentMethod(), options);
		}

		// Token: 0x06000A3A RID: 2618 RVA: 0x00026560 File Offset: 0x00024960
		private DalVoucher CreateDalVoucher(Voucher voucher)
		{
			XmlDocument xmlDocument = new XmlDocument();
			VoucherXmlSerializer voucherXmlSerializer = new VoucherXmlSerializer(xmlDocument);
			XmlElement xmlElement = voucherXmlSerializer.Serialize(voucher);
			DalVoucher result;
			result.Id = voucher.Id;
			result.UserId = voucher.UserId;
			result.Status = voucher.Status;
			result.Data = xmlElement.OuterXml;
			return result;
		}

		// Token: 0x04000553 RID: 1363
		private IVoucherSystem m_voucherSystem;
	}
}

using System;
using MasterServer.DAL.VoucherSystem;
using MasterServer.ElectronicCatalog;

namespace MasterServer.DAL.Impl
{
	// Token: 0x02000028 RID: 40
	public class VoucherSystem : IVoucherSystem
	{
		// Token: 0x060001A0 RID: 416 RVA: 0x0000EF97 File Offset: 0x0000D197
		public VoucherSystem(DAL dal)
		{
			this.m_dal = dal;
		}

		// Token: 0x1700001E RID: 30
		// (get) Token: 0x060001A1 RID: 417 RVA: 0x0000EFB1 File Offset: 0x0000D1B1
		private ECatConnectionPool ConnectionPool
		{
			get
			{
				return this.m_dal.ConnectionPool;
			}
		}

		// Token: 0x060001A2 RID: 418 RVA: 0x0000EFC0 File Offset: 0x0000D1C0
		public DALResult<ulong> GetCurrentIndex(string globalKey)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("SELECT GetCurrentIndex(?globalKey)", new object[]
			{
				"?globalKey",
				globalKey
			});
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x060001A3 RID: 419 RVA: 0x0000F024 File Offset: 0x0000D224
		public DALResult<ulong> SetCurrentIndex(string globalKey, ulong globalValue)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("SELECT SetCurrentIndex(?globalKey, ?globalValue)", new object[]
			{
				"?globalKey",
				globalKey,
				"?globalValue",
				globalValue
			});
			ulong val = (ulong)this.m_dal.CacheProxy.SetScalar(setOptions);
			return new DALResult<ulong>(val, setOptions.stats);
		}

		// Token: 0x060001A4 RID: 420 RVA: 0x0000F0A0 File Offset: 0x0000D2A0
		public DALResultMulti<DalVoucher> GetNewVouchers(ulong userId)
		{
			CacheProxy.Options<DalVoucher> options = new CacheProxy.Options<DalVoucher>
			{
				db_serializer = this.m_dalVoucherSerializer,
				connection_pool = this.ConnectionPool
			};
			options.query("CALL GetVouchers(?uid)", new object[]
			{
				"?uid",
				userId
			});
			return this.m_dal.CacheProxy.GetStream<DalVoucher>(options);
		}

		// Token: 0x060001A5 RID: 421 RVA: 0x0000F100 File Offset: 0x0000D300
		public DALResultMulti<DalVoucher> GetAllVouchers(ulong userId)
		{
			CacheProxy.Options<DalVoucher> options = new CacheProxy.Options<DalVoucher>
			{
				db_serializer = this.m_dalVoucherSerializer,
				connection_pool = this.ConnectionPool
			};
			options.query("CALL GetAllVouchers(?uid)", new object[]
			{
				"?uid",
				userId
			});
			return this.m_dal.CacheProxy.GetStream<DalVoucher>(options);
		}

		// Token: 0x060001A6 RID: 422 RVA: 0x0000F160 File Offset: 0x0000D360
		public DALResultMulti<DalVoucher> GetCorruptedVouchers(ulong startIndex, int count)
		{
			CacheProxy.Options<DalVoucher> options = new CacheProxy.Options<DalVoucher>
			{
				db_serializer = this.m_dalVoucherSerializer,
				connection_pool = this.ConnectionPool
			};
			options.query("CALL GetCorruptedVouchers(?startIndex, ?count)", new object[]
			{
				"?startIndex",
				startIndex,
				"?count",
				count
			});
			return this.m_dal.CacheProxy.GetStream<DalVoucher>(options);
		}

		// Token: 0x060001A7 RID: 423 RVA: 0x0000F1D4 File Offset: 0x0000D3D4
		public DALResult<bool> AddVoucher(DalVoucher voucher)
		{
			string text = voucher.Status.ToString().ToLower();
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			setOptions.query("SELECT AddVoucher(?id, ?uid, ?data, ?status)", new object[]
			{
				"?id",
				voucher.Id,
				"?uid",
				voucher.UserId,
				"?data",
				voucher.Data,
				"?status",
				text
			});
			int num = int.Parse(this.m_dal.CacheProxy.SetScalar(setOptions).ToString());
			return new DALResult<bool>(num > 0, setOptions.stats);
		}

		// Token: 0x060001A8 RID: 424 RVA: 0x0000F29C File Offset: 0x0000D49C
		public DALResult<DalVoucher> UpdateVoucher(DalVoucher voucher)
		{
			string text = voucher.Status.ToString().ToLower();
			CacheProxy.Options<DalVoucher> options = new CacheProxy.Options<DalVoucher>
			{
				db_serializer = this.m_dalVoucherSerializer,
				connection_pool = this.ConnectionPool,
				db_transaction = true
			};
			options.query("CALL UpdateVoucher(?id, ?userId, ?data, ?status)", new object[]
			{
				"?id",
				voucher.Id,
				"?userId",
				voucher.UserId,
				"?data",
				voucher.Data,
				"?status",
				text
			});
			options.db_transaction = true;
			return this.m_dal.CacheProxy.Get<DalVoucher>(options);
		}

		// Token: 0x060001A9 RID: 425 RVA: 0x0000F360 File Offset: 0x0000D560
		public DALResultVoid CleanUpVouchers(ulong userId)
		{
			CacheProxy.SetOptions setOptions = new CacheProxy.SetOptions
			{
				connection_pool = this.ConnectionPool
			};
			setOptions.query("CALL CleanUpVouchers(?s)", new object[]
			{
				"?s",
				userId
			});
			return this.m_dal.CacheProxy.Set(setOptions);
		}

		// Token: 0x0400007D RID: 125
		private readonly DAL m_dal;

		// Token: 0x0400007E RID: 126
		private readonly DalVoucherSerializer m_dalVoucherSerializer = new DalVoucherSerializer();
	}
}

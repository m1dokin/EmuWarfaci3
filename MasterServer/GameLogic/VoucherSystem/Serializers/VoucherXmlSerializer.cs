using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using MasterServer.Core;
using MasterServer.DAL.VoucherSystem;
using MasterServer.ElectronicCatalog.ShopSupplier.Serializers;
using MasterServer.GameLogic.VoucherSystem.Exceptions;

namespace MasterServer.GameLogic.VoucherSystem.Serializers
{
	// Token: 0x02000480 RID: 1152
	internal class VoucherXmlSerializer : IXmlSerializer<Voucher>
	{
		// Token: 0x06001845 RID: 6213 RVA: 0x0006456D File Offset: 0x0006296D
		public VoucherXmlSerializer() : this(new XmlDocument())
		{
		}

		// Token: 0x06001846 RID: 6214 RVA: 0x0006457A File Offset: 0x0006297A
		public VoucherXmlSerializer(XmlDocument xmlDocument)
		{
			this.m_xmlDocument = xmlDocument;
		}

		// Token: 0x06001847 RID: 6215 RVA: 0x0006458C File Offset: 0x0006298C
		public Voucher Deserialize(DalVoucher dalVoucher)
		{
			this.m_xmlDocument.LoadXml(dalVoucher.Data);
			Voucher voucher = this.Deserialize(this.m_xmlDocument.DocumentElement);
			voucher.Status = dalVoucher.Status;
			return voucher;
		}

		// Token: 0x06001848 RID: 6216 RVA: 0x000645CC File Offset: 0x000629CC
		public Voucher Deserialize(XmlElement element)
		{
			ulong num = ulong.Parse(element.GetAttribute("id"));
			ulong userId = ulong.Parse(element.GetAttribute("user_id"));
			string message = (!element.HasAttribute("message")) ? string.Empty : element.GetAttribute("message");
			string value = (!element.HasAttribute("status")) ? string.Empty : element.GetAttribute("status");
			VoucherStatus status;
			if (!Enum.TryParse<VoucherStatus>(value, true, out status))
			{
				status = VoucherStatus.New;
			}
			List<VoucherItem> list = new List<VoucherItem>();
			VoucherItemXmlSerializer voucherItemXmlSerializer = new VoucherItemXmlSerializer(this.m_xmlDocument);
			bool flag = false;
			IEnumerator enumerator = element.GetElementsByTagName("item").GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlElement element2 = (XmlElement)obj;
					try
					{
						VoucherItem item = voucherItemXmlSerializer.Deserialize(element2);
						list.Add(item);
					}
					catch (Exception innerException)
					{
						flag = true;
						Log.Error(new VoucherDeserializationException(string.Format("Error on voucher deserialization. Voucher ID: {0}", num), innerException));
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
			if (flag)
			{
				list.Clear();
			}
			return new Voucher(num, userId, status, message, list);
		}

		// Token: 0x06001849 RID: 6217 RVA: 0x00064734 File Offset: 0x00062B34
		public XmlElement Serialize(Voucher voucher)
		{
			XmlElement xmlElement = this.m_xmlDocument.CreateElement("voucher");
			xmlElement.SetAttribute("id", voucher.Id.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("user_id", voucher.UserId.ToString(CultureInfo.InvariantCulture));
			xmlElement.SetAttribute("message", voucher.Message);
			VoucherItemXmlSerializer voucherItemXmlSerializer = new VoucherItemXmlSerializer(this.m_xmlDocument);
			foreach (VoucherItem item in voucher)
			{
				XmlElement newChild = voucherItemXmlSerializer.Serialize(item);
				xmlElement.AppendChild(newChild);
			}
			return xmlElement;
		}

		// Token: 0x04000BAC RID: 2988
		private readonly XmlDocument m_xmlDocument;
	}
}

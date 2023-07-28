using System;
using System.Collections.Generic;
using System.Security;
using System.Text.RegularExpressions;
using System.Xml;

namespace MasterServer.GFaceAPI
{
	// Token: 0x0200052D RID: 1325
	public static class GFaceCurrencyHelper
	{
		// Token: 0x06001CC6 RID: 7366 RVA: 0x000739E2 File Offset: 0x00071DE2
		static GFaceCurrencyHelper()
		{
			GFaceCurrencyHelper.RegisterCurrency(1, "WFCREDITS");
		}

		// Token: 0x06001CC7 RID: 7367 RVA: 0x00073A03 File Offset: 0x00071E03
		public static void RegisterCurrency(int currencyId, string currencyCode)
		{
			GFaceCurrencyHelper.m_id2code[currencyId] = currencyCode;
			GFaceCurrencyHelper.m_code2id[currencyCode] = currencyId;
		}

		// Token: 0x06001CC8 RID: 7368 RVA: 0x00073A20 File Offset: 0x00071E20
		public static string GetCurrencyCode(int currencyId)
		{
			string result;
			if (GFaceCurrencyHelper.m_id2code.TryGetValue(currencyId, out result))
			{
				return result;
			}
			return null;
		}

		// Token: 0x06001CC9 RID: 7369 RVA: 0x00073A42 File Offset: 0x00071E42
		public static bool LookupCurrencyId(out int currencyId, string currencyCode)
		{
			return GFaceCurrencyHelper.m_code2id.TryGetValue(currencyCode, out currencyId);
		}

		// Token: 0x06001CCA RID: 7370 RVA: 0x00073A50 File Offset: 0x00071E50
		public static void Clear()
		{
			GFaceCurrencyHelper.m_code2id.Clear();
			GFaceCurrencyHelper.m_id2code.Clear();
		}

		// Token: 0x06001CCB RID: 7371 RVA: 0x00073A68 File Offset: 0x00071E68
		public static string GeneratePurchaseRecord(ulong userId, string productCode, int currencyId, long price, ulong? quantity, string remark)
		{
			string currencyCode = GFaceCurrencyHelper.GetCurrencyCode(currencyId);
			if (currencyCode == null)
			{
				throw new ArgumentOutOfRangeException("currencyId", currencyId, "Unregistered Currency Id.");
			}
			if (productCode == null || !Regex.IsMatch(productCode, "^[\\w\\._-]{1,256}$"))
			{
				throw new ArgumentOutOfRangeException("productCode", productCode, "Not well-formed Product Code.");
			}
			XmlDocument xmlDocument = new XmlDocument();
			XmlElement xmlElement = (XmlElement)xmlDocument.AppendChild(xmlDocument.CreateElement("record"));
			xmlElement.AppendChild(xmlDocument.CreateElement("userid")).InnerText = userId.ToString();
			xmlElement.AppendChild(xmlDocument.CreateElement("productcode")).InnerText = productCode;
			xmlElement.AppendChild(xmlDocument.CreateElement("price")).InnerText = price.ToString();
			xmlElement.AppendChild(xmlDocument.CreateElement("currency")).InnerText = currencyCode;
			xmlElement.AppendChild(xmlDocument.CreateElement("quantity")).InnerText = ((quantity == null) ? 1UL : quantity.Value).ToString();
			xmlElement.AppendChild(xmlDocument.CreateElement("notes")).InnerText = ((remark != null && remark.Length != 0) ? SecurityElement.Escape(remark) : string.Empty);
			return xmlElement.OuterXml;
		}

		// Token: 0x04000DB5 RID: 3509
		private static Dictionary<int, string> m_id2code = new Dictionary<int, string>();

		// Token: 0x04000DB6 RID: 3510
		private static Dictionary<string, int> m_code2id = new Dictionary<string, int>();
	}
}

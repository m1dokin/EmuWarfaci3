using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;

namespace MasterServer.GFaceAPI
{
	// Token: 0x02000652 RID: 1618
	public class GFaceResponseParserImpl : IDisposable
	{
		// Token: 0x0600226F RID: 8815 RVA: 0x0009009C File Offset: 0x0008E49C
		public GFaceResponseParserImpl(string str, Type expectedType) : this(str, expectedType, false)
		{
		}

		// Token: 0x06002270 RID: 8816 RVA: 0x000900A8 File Offset: 0x0008E4A8
		public GFaceResponseParserImpl(string str, Type expectedType, bool bThrow)
		{
			this.m_expectedType = expectedType;
			this.Do1stStep(str);
			if (this.HasError && bThrow)
			{
				this.Error.Rethrow();
			}
			if (this.m_expectedType == null)
			{
				return;
			}
			this.Do2ndStep(this.m_expectedType);
			if (this.HasError && bThrow)
			{
				this.Error.Rethrow();
			}
		}

		// Token: 0x1700036B RID: 875
		// (get) Token: 0x06002271 RID: 8817 RVA: 0x00090125 File Offset: 0x0008E525
		public GFaceError Error
		{
			get
			{
				return this.m_error;
			}
		}

		// Token: 0x1700036C RID: 876
		// (get) Token: 0x06002272 RID: 8818 RVA: 0x0009012D File Offset: 0x0008E52D
		public bool HasError
		{
			get
			{
				return this.m_error.HasError;
			}
		}

		// Token: 0x1700036D RID: 877
		// (get) Token: 0x06002273 RID: 8819 RVA: 0x0009013A File Offset: 0x0008E53A
		public GErrorCode ErrorCode
		{
			get
			{
				return this.m_error.ErrorCode;
			}
		}

		// Token: 0x1700036E RID: 878
		// (get) Token: 0x06002274 RID: 8820 RVA: 0x00090147 File Offset: 0x0008E547
		public string ErrorMessage
		{
			get
			{
				return this.m_error.ErrorMessage;
			}
		}

		// Token: 0x06002275 RID: 8821 RVA: 0x00090154 File Offset: 0x0008E554
		public void Dispose()
		{
		}

		// Token: 0x06002276 RID: 8822 RVA: 0x00090158 File Offset: 0x0008E558
		public T GetResponse<T>()
		{
			if (this.m_expectedType == null)
			{
				throw new ApplicationException("Trying to fetch response from a simply parsed data.");
			}
			Type typeFromHandle = typeof(T);
			if (typeFromHandle != this.m_expectedType && !typeFromHandle.IsSubclassOf(this.m_expectedType))
			{
				throw new ApplicationException("Trying to fetch response data with an inconsistent type, expecting " + this.m_expectedType.FullName + ", given: " + typeFromHandle.FullName);
			}
			return (T)((object)this.m_response);
		}

		// Token: 0x06002277 RID: 8823 RVA: 0x000901E0 File Offset: 0x0008E5E0
		private void Do1stStep(string rawStr)
		{
			try
			{
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.LoadXml(rawStr);
				XmlElement documentElement = xmlDocument.DocumentElement;
				if (documentElement.LocalName != "rsp")
				{
					this.ThrowEarlyFailureException();
				}
				this.m_root = documentElement;
				this.m_error = this.ParseErrorInfo();
			}
			catch (XmlException ex)
			{
				this.m_error = new GFaceError(GErrorCode.ParserError, ex.Message);
			}
		}

		// Token: 0x06002278 RID: 8824 RVA: 0x00090260 File Offset: 0x0008E660
		private void Do2ndStep(Type hintType)
		{
			if (this.m_error.HasError)
			{
				return;
			}
			if (!RspAttribUtils.IsResponseType(hintType))
			{
				this.ThrowEarlyFailureException();
			}
			Stack<string> fieldLocator = new Stack<string>();
			this.m_response = this.ParseRecursive(this.m_root, hintType, fieldLocator);
		}

		// Token: 0x06002279 RID: 8825 RVA: 0x000902AC File Offset: 0x0008E6AC
		private object ParseRecursive(XmlElement elem, Type typeinfo, Stack<string> fieldLocator)
		{
			object obj = Activator.CreateInstance(typeinfo);
			FieldInfo[] fields = typeinfo.GetFields();
			FieldInfo[] array = fields;
			for (int i = 0; i < array.Length; i++)
			{
				FieldInfo f = array[i];
				using (new GFaceResponseParserImpl.AutoGuard(delegate()
				{
					fieldLocator.Push(f.Name);
				}, delegate()
				{
					fieldLocator.Pop();
				}))
				{
					object obj2 = null;
					if (GFaceResponseParserImpl.TypeTemplateEqual(f.FieldType, typeof(List<>)))
					{
						Type wrappedType = GFaceResponseParserImpl.GetWrappedType(f.FieldType);
						obj2 = Activator.CreateInstance(f.FieldType);
						foreach (XmlElement childNode in GFaceResponseParserImpl.EnumChildElements(elem, f.Name))
						{
							((IList)obj2).Add(this.ExtractFieldData(wrappedType, childNode, fieldLocator));
						}
					}
					else
					{
						XmlElement xmlElement = GFaceResponseParserImpl.FindFirstInChildElements(elem, f.Name);
						if (xmlElement == null)
						{
							if (!GFaceResponseParserImpl.IsOptional(f))
							{
								this.ThrowFieldNotFoundInResponseException(fieldLocator);
							}
							goto IL_1D9;
						}
						Type fieldType = (!GFaceResponseParserImpl.IsOptional(f) || !GFaceResponseParserImpl.TypeTemplateEqual(f.FieldType, typeof(Nullable<>))) ? f.FieldType : GFaceResponseParserImpl.GetWrappedType(f.FieldType);
						obj2 = this.ExtractFieldData(fieldType, xmlElement, fieldLocator);
					}
					f.SetValue(obj, obj2);
				}
				IL_1D9:;
			}
			return obj;
		}

		// Token: 0x0600227A RID: 8826 RVA: 0x000904D8 File Offset: 0x0008E8D8
		private object ExtractFieldData(Type fieldType, XmlElement childNode, Stack<string> fieldLocator)
		{
			object result = null;
			try
			{
				result = this.ExtractFieldDataImpl(fieldType, childNode, fieldLocator);
			}
			catch (FormatException)
			{
				this.ThrowValueTypeMismatchException(fieldLocator);
			}
			catch (InvalidCastException)
			{
				this.ThrowValueTypeMismatchException(fieldLocator);
			}
			return result;
		}

		// Token: 0x0600227B RID: 8827 RVA: 0x00090530 File Offset: 0x0008E930
		private object ExtractFieldDataImpl(Type fieldType, XmlElement childNode, Stack<string> fieldLocator)
		{
			object result;
			Func<string, object> func;
			if (!fieldType.IsPrimitive && !fieldType.FullName.StartsWith("System."))
			{
				result = this.ParseRecursive(childNode, fieldType, fieldLocator);
			}
			else if (GFaceResponseParserImpl.m_typeConverters.TryGetValue(fieldType, out func))
			{
				result = func(childNode.InnerText);
			}
			else
			{
				result = ((IConvertible)childNode.InnerText).ToType(fieldType, new CultureInfo("en-us"));
			}
			return result;
		}

		// Token: 0x0600227C RID: 8828 RVA: 0x000905AA File Offset: 0x0008E9AA
		private string GetStatusString()
		{
			return this.m_root.GetAttribute("stat");
		}

		// Token: 0x0600227D RID: 8829 RVA: 0x000905BC File Offset: 0x0008E9BC
		private GFaceError ParseErrorInfo()
		{
			if (this.GetStatusString() == "ok")
			{
				return GFaceError.TheNoError;
			}
			if (this.GetStatusString() != "fail" || this.m_root.FirstChild.NodeType != XmlNodeType.Element || this.m_root.FirstChild.LocalName != "err")
			{
				this.ThrowEarlyFailureException();
			}
			XmlElement xmlElement = this.m_root.FirstChild as XmlElement;
			string errMsg = null;
			int errCode;
			if (!int.TryParse(xmlElement.GetAttribute("code"), out errCode) || (errMsg = xmlElement.GetAttribute("msg")).Length == 0)
			{
				this.ThrowEarlyFailureException();
			}
			return new GFaceError((GErrorCode)errCode, errMsg);
		}

		// Token: 0x0600227E RID: 8830 RVA: 0x00090684 File Offset: 0x0008EA84
		private static bool IsOptional(FieldInfo field)
		{
			object[] customAttributes = field.GetCustomAttributes(typeof(GRspOptionalAttribute), false);
			int num = 0;
			if (num >= customAttributes.Length)
			{
				return false;
			}
			object obj = customAttributes[num];
			return true;
		}

		// Token: 0x0600227F RID: 8831 RVA: 0x000906BE File Offset: 0x0008EABE
		private static bool TypeTemplateEqual(Type left, Type right)
		{
			return left.MetadataToken == right.MetadataToken && !(left.Module != right.Module);
		}

		// Token: 0x06002280 RID: 8832 RVA: 0x000906EA File Offset: 0x0008EAEA
		private static Type GetWrappedType(Type wrapperType)
		{
			if (wrapperType.GetGenericArguments().Length != 1)
			{
				return null;
			}
			return wrapperType.GetGenericArguments()[0];
		}

		// Token: 0x06002281 RID: 8833 RVA: 0x00090704 File Offset: 0x0008EB04
		private static XmlElement FindFirstInChildElements(XmlElement parent, string childName)
		{
			IEnumerator enumerator = parent.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode xmlNode = (XmlNode)obj;
					if (xmlNode.NodeType == XmlNodeType.Element && !(xmlNode.Name != childName))
					{
						return (XmlElement)xmlNode;
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
			return null;
		}

		// Token: 0x06002282 RID: 8834 RVA: 0x00090794 File Offset: 0x0008EB94
		private static IEnumerable<XmlElement> EnumChildElements(XmlElement parent, string childName)
		{
			IEnumerator enumerator = parent.ChildNodes.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
					XmlNode child = (XmlNode)obj;
					if (child.NodeType == XmlNodeType.Element && !(child.Name != childName))
					{
						yield return (XmlElement)child;
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
			yield break;
		}

		// Token: 0x06002283 RID: 8835 RVA: 0x000907C0 File Offset: 0x0008EBC0
		private string MakeLocateStr(Stack<string> fields)
		{
			string[] array = fields.ToArray();
			Array.Reverse(array);
			return string.Join(".", array);
		}

		// Token: 0x06002284 RID: 8836 RVA: 0x000907E8 File Offset: 0x0008EBE8
		private void ThrowEarlyFailureException()
		{
			GFaceError gfaceError = new GFaceError(GErrorCode.ParserError, "GFace response early check failed.");
			gfaceError.Rethrow();
		}

		// Token: 0x06002285 RID: 8837 RVA: 0x0009080C File Offset: 0x0008EC0C
		private void ThrowValueTypeMismatchException(Stack<string> fieldLocator)
		{
			GFaceError gfaceError = new GFaceError(GErrorCode.ParserError, "GFace response has a mismatched value type on field: " + this.MakeLocateStr(fieldLocator));
			gfaceError.Rethrow();
		}

		// Token: 0x06002286 RID: 8838 RVA: 0x0009083C File Offset: 0x0008EC3C
		private void ThrowFieldNotFoundInResponseException(Stack<string> fieldLocator)
		{
			GFaceError gfaceError = new GFaceError(GErrorCode.ParserError, "GFace response has a field missing: " + this.MakeLocateStr(fieldLocator));
			gfaceError.Rethrow();
		}

		// Token: 0x0400111F RID: 4383
		private const string ResponseTag = "rsp";

		// Token: 0x04001120 RID: 4384
		private const string StatusAttribName = "stat";

		// Token: 0x04001121 RID: 4385
		private const string OkStatusValue = "ok";

		// Token: 0x04001122 RID: 4386
		private const string FailStatusValue = "fail";

		// Token: 0x04001123 RID: 4387
		private const string ErrorTag = "err";

		// Token: 0x04001124 RID: 4388
		private const string ErrorCodeAttribName = "code";

		// Token: 0x04001125 RID: 4389
		private const string ErrorMsgAttribName = "msg";

		// Token: 0x04001126 RID: 4390
		private XmlElement m_root;

		// Token: 0x04001127 RID: 4391
		private GFaceError m_error;

		// Token: 0x04001128 RID: 4392
		private object m_response;

		// Token: 0x04001129 RID: 4393
		private Type m_expectedType;

		// Token: 0x0400112A RID: 4394
		private static readonly Dictionary<Type, Func<string, object>> m_typeConverters = new Dictionary<Type, Func<string, object>>
		{
			{
				typeof(Guid),
				(string s) => new Guid(s)
			}
		};

		// Token: 0x02000653 RID: 1619
		private class AutoGuard : IDisposable
		{
			// Token: 0x06002289 RID: 8841 RVA: 0x000908AE File Offset: 0x0008ECAE
			public AutoGuard(GFaceResponseParserImpl.AutoGuard.Deleg before, GFaceResponseParserImpl.AutoGuard.Deleg after)
			{
				this.m_after = after;
				before();
			}

			// Token: 0x0600228A RID: 8842 RVA: 0x000908C3 File Offset: 0x0008ECC3
			public void Dispose()
			{
				this.m_after();
			}

			// Token: 0x0400112B RID: 4395
			private GFaceResponseParserImpl.AutoGuard.Deleg m_after;

			// Token: 0x02000654 RID: 1620
			// (Invoke) Token: 0x0600228C RID: 8844
			public delegate void Deleg();
		}
	}
}

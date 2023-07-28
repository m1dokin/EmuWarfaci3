using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
using HK2Net;
using MasterServer.Core;

namespace MasterServer.ServerInfo
{
	// Token: 0x020006C4 RID: 1732
	[Service]
	[Singleton]
	internal class ReflectionMemoryUsageCollector : IMemoryUsageCollector
	{
		// Token: 0x06002448 RID: 9288 RVA: 0x000975FE File Offset: 0x000959FE
		public string GetMemoryUsageInfo(object o)
		{
			return this.ReflectObject(o);
		}

		// Token: 0x06002449 RID: 9289 RVA: 0x00097608 File Offset: 0x00095A08
		private string ReflectObject(object obj)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (FieldInfo fieldInfo in obj.GetType().GetFields(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
			{
				int num = -1;
				if (fieldInfo.FieldType.IsGenericType && fieldInfo.FieldType.GetGenericTypeDefinition() == typeof(CacheDictionary<, >))
				{
					object value = fieldInfo.GetValue(obj);
					num = (int)fieldInfo.FieldType.GetMethod("Count").Invoke(value, null);
				}
				IEnumerable enumerable = fieldInfo.GetValue(obj) as IEnumerable;
				if (enumerable != null && fieldInfo.FieldType.Name.ToLower() != "string" && !typeof(MarshalByRefObject).IsAssignableFrom(enumerable.GetType()))
				{
					num = enumerable.Cast<object>().Count<object>();
				}
				Delegate @delegate = fieldInfo.GetValue(obj) as Delegate;
				if (@delegate != null)
				{
					num = @delegate.GetInvocationList().Count<Delegate>();
				}
				if (num >= 0)
				{
					stringBuilder.Append(this.LogField(obj.GetType().Name, fieldInfo.Name, fieldInfo.FieldType.Name, fieldInfo.FieldType.GetGenericArguments(), num));
				}
			}
			return (stringBuilder.Length != 0) ? stringBuilder.ToString() : string.Empty;
		}

		// Token: 0x0600244A RID: 9290 RVA: 0x00097774 File Offset: 0x00095B74
		private string LogField(string className, string fieldName, string fieldType, Type[] genericsArgs, int numElements)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat(string.Format("Field name : {0}.{1}; Field type : {2}; \tField members types : ", className, fieldName, fieldType), new object[0]);
			foreach (Type type in genericsArgs)
			{
				stringBuilder.AppendFormat("\t{0}", type.Name);
			}
			stringBuilder.Append(";\t").AppendFormat("Num of elements/subscribers : {0};\n", numElements);
			return stringBuilder.ToString();
		}
	}
}

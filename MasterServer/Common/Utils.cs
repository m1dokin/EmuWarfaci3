using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using MasterServer.Core;
using MasterServer.CryOnlineNET;
using Util.Common;

namespace MasterServer.Common
{
	// Token: 0x02000162 RID: 354
	public static class Utils
	{
		// Token: 0x06000627 RID: 1575 RVA: 0x00019054 File Offset: 0x00017454
		public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
		{
			T t = (value.CompareTo(min) <= 0) ? min : value;
			return (t.CompareTo(max) >= 0) ? max : t;
		}

		// Token: 0x06000628 RID: 1576 RVA: 0x00019098 File Offset: 0x00017498
		public static T ParseEnum<T>(string value)
		{
			return (T)((object)Enum.Parse(typeof(T), value, true));
		}

		// Token: 0x06000629 RID: 1577 RVA: 0x000190B0 File Offset: 0x000174B0
		public static IEnumerable<T> GetEnumValues<T>()
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}

		// Token: 0x0600062A RID: 1578 RVA: 0x000190C6 File Offset: 0x000174C6
		public static T ValueOrDefault<T>(this T? nullable, T defaultValue) where T : struct
		{
			return (nullable == null) ? defaultValue : nullable.Value;
		}

		// Token: 0x0600062B RID: 1579 RVA: 0x000190E4 File Offset: 0x000174E4
		public static T ValueOrDefault<T>(this T? nullable) where T : struct
		{
			return nullable.ValueOrDefault(default(T));
		}

		// Token: 0x0600062C RID: 1580 RVA: 0x00019100 File Offset: 0x00017500
		public static void SafeForEach<T>(this IEnumerable<T> arr, Action<T> step)
		{
			if (Utils.<>f__mg$cache0 == null)
			{
				Utils.<>f__mg$cache0 = new Action<Exception>(Log.Error);
			}
			arr.SafeForEachEx(step, Utils.<>f__mg$cache0);
		}

		// Token: 0x0600062D RID: 1581 RVA: 0x00019128 File Offset: 0x00017528
		public static void ForEachAggregate<T>(this IEnumerable<T> arr, Action<T> step)
		{
			List<Exception> list = new List<Exception>();
			arr.SafeForEachEx(step, new Action<Exception>(list.Add));
			if (list.Any<Exception>())
			{
				throw new AggregateException(list);
			}
		}

		// Token: 0x0600062E RID: 1582 RVA: 0x00019160 File Offset: 0x00017560
		public static IEnumerable<TResult> SafeSelect<TSource, TResult>(this IEnumerable<TSource> source, Func<TSource, TResult> selector)
		{
			if (Utils.<>f__mg$cache1 == null)
			{
				Utils.<>f__mg$cache1 = new Action<Exception>(Log.Error);
			}
			return source.SafeSelectEx(selector, Utils.<>f__mg$cache1);
		}

		// Token: 0x0600062F RID: 1583 RVA: 0x00019186 File Offset: 0x00017586
		public static void Retry(Func<bool> deleg)
		{
			Utils.TryFor(2, deleg);
		}

		// Token: 0x06000630 RID: 1584 RVA: 0x00019190 File Offset: 0x00017590
		public static void TryFor(int tries, Func<bool> deleg)
		{
			for (int num = 0; num != tries; num++)
			{
				if (deleg())
				{
					return;
				}
			}
			throw new Exception("Retry count exceeded");
		}

		// Token: 0x06000631 RID: 1585 RVA: 0x000191C8 File Offset: 0x000175C8
		public static bool TryUntil<T>(this Func<bool> deleg, int tries, Func<T, bool> filter) where T : Exception
		{
			for (int i = 0; i < tries; i++)
			{
				try
				{
					return deleg();
				}
				catch (T t)
				{
					T arg = (T)((object)t);
					Log.Warning<int, int, string>("TryUntil Func failed attempt: {0} of {1} in {2}", i, tries, deleg.Method.Name);
					if (filter != null && !filter(arg))
					{
						throw;
					}
					if (i == tries - 1)
					{
						throw;
					}
				}
			}
			return false;
		}

		// Token: 0x06000632 RID: 1586 RVA: 0x0001924C File Offset: 0x0001764C
		public static void TryUntil<T>(this Action deleg, int tries, Func<T, bool> filter) where T : Exception
		{
			for (int i = 0; i < tries; i++)
			{
				try
				{
					deleg();
					break;
				}
				catch (T t)
				{
					T arg = (T)((object)t);
					Log.Warning<int, int, string>("TryUntil Action failed attempt: {0} of {1} in {2}", i, tries, deleg.Method.Name);
					if (filter != null && !filter(arg))
					{
						throw;
					}
					if (i == tries - 1)
					{
						throw;
					}
				}
			}
		}

		// Token: 0x06000633 RID: 1587 RVA: 0x000192CC File Offset: 0x000176CC
		public static bool Find<T>(IEnumerable<T> arr, Predicate<T> pred, out T val)
		{
			foreach (T t in arr)
			{
				if (pred(t))
				{
					val = t;
					return true;
				}
			}
			val = default(T);
			return false;
		}

		// Token: 0x06000634 RID: 1588 RVA: 0x00019348 File Offset: 0x00017748
		public static bool Contains<T>(IEnumerable<T> arr, T val)
		{
			foreach (T y in arr)
			{
				if (EqualityComparer<T>.Default.Equals(val, y))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000635 RID: 1589 RVA: 0x000193B0 File Offset: 0x000177B0
		public static bool Contains<T>(IEnumerable<T> arr, Predicate<T> pred)
		{
			foreach (T obj in arr)
			{
				if (pred(obj))
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06000636 RID: 1590 RVA: 0x00019414 File Offset: 0x00017814
		public static bool IsNullOrEmpty<T>(this IEnumerable<T> arr)
		{
			return arr == null || !arr.Any<T>();
		}

		// Token: 0x06000637 RID: 1591 RVA: 0x00019428 File Offset: 0x00017828
		public static T[] Copy<T>(T[] arr)
		{
			T[] array = new T[arr.Length];
			for (int num = 0; num != arr.Length; num++)
			{
				array[num] = arr[num];
			}
			return array;
		}

		// Token: 0x06000638 RID: 1592 RVA: 0x00019464 File Offset: 0x00017864
		public static IEnumerable<KeyValuePair<T, int>> GroupByCount<T>(List<T> arr)
		{
			if (arr.Count == 0)
			{
				yield break;
			}
			arr.Sort();
			T val = arr[0];
			int c = 1;
			for (int i = 1; i != arr.Count; i++)
			{
				if (val.Equals(arr[i]))
				{
					c++;
				}
				else
				{
					yield return new KeyValuePair<T, int>(val, c);
					val = arr[i];
					c = 1;
				}
			}
			yield return new KeyValuePair<T, int>(val, c);
			yield break;
		}

		// Token: 0x06000639 RID: 1593 RVA: 0x00019487 File Offset: 0x00017887
		public static IEnumerable<T> Slice<T>(List<T> list, int start)
		{
			return Utils.Slice<T>(list, start, list.Count);
		}

		// Token: 0x0600063A RID: 1594 RVA: 0x00019498 File Offset: 0x00017898
		public static IEnumerable<T> Slice<T>(List<T> list, int start, int end)
		{
			for (int i = start; i != end; i++)
			{
				yield return list[i];
			}
			yield break;
		}

		// Token: 0x0600063B RID: 1595 RVA: 0x000194CC File Offset: 0x000178CC
		public static float Diff<T>(this IEnumerable<T> seq, Func<T, float> selector)
		{
			List<float> source = seq.Select(selector).ToList<float>();
			return source.Max() - source.Min();
		}

		// Token: 0x0600063C RID: 1596 RVA: 0x000194F4 File Offset: 0x000178F4
		public static int Diff<T>(this IEnumerable<T> seq, Func<T, int> selector)
		{
			List<int> source = seq.Select(selector).ToList<int>();
			return source.Max() - source.Min();
		}

		// Token: 0x0600063D RID: 1597 RVA: 0x0001951C File Offset: 0x0001791C
		public static IEnumerable<KeyValuePair<T1, T2>> Zip<T1, T2>(IEnumerable<T1> a1, IEnumerable<T2> a2)
		{
			IEnumerator<T1> e = a1.GetEnumerator();
			IEnumerator<T2> e2 = a2.GetEnumerator();
			while (e.MoveNext() && e2.MoveNext())
			{
				yield return new KeyValuePair<T1, T2>(e.Current, e2.Current);
			}
			yield break;
		}

		// Token: 0x0600063E RID: 1598 RVA: 0x00019546 File Offset: 0x00017946
		public static string GetDelegInfo(MulticastDelegate deleg)
		{
			return (deleg == null) ? "<null>" : Utils.GetDelegInvocationList(deleg);
		}

		// Token: 0x0600063F RID: 1599 RVA: 0x00019560 File Offset: 0x00017960
		public static bool HasFlag(this Enum variable, Enum value)
		{
			if (variable == null)
			{
				return false;
			}
			if (value == null)
			{
				throw new ArgumentNullException("value");
			}
			if (!Enum.IsDefined(variable.GetType(), value))
			{
				throw new ArgumentException(string.Format("Enumeration type mismatch.  The flag is of type '{0}', was expecting '{1}'.", value.GetType(), variable.GetType()));
			}
			ulong num = Convert.ToUInt64(value);
			return (Convert.ToUInt64(variable) & num) == num;
		}

		// Token: 0x06000640 RID: 1600 RVA: 0x000195C5 File Offset: 0x000179C5
		public static bool HasAnyFlag(this Enum variable, Enum value)
		{
			return (Convert.ToUInt64(variable) & Convert.ToUInt64(value)) != 0UL;
		}

		// Token: 0x06000641 RID: 1601 RVA: 0x000195DB File Offset: 0x000179DB
		public static T ConvertToEnumFlag<T>(int id) where T : struct
		{
			if (!typeof(T).IsEnum)
			{
				throw new ArgumentException("Generic type should be enum");
			}
			return (T)((object)Enum.ToObject(typeof(T), 1 << id));
		}

		// Token: 0x06000642 RID: 1602 RVA: 0x00019616 File Offset: 0x00017A16
		public static bool TryParse<T>(string value, out T returnedValue)
		{
			return Utils.TryParse<T>(value, out returnedValue, true);
		}

		// Token: 0x06000643 RID: 1603 RVA: 0x00019620 File Offset: 0x00017A20
		public static bool TryParse<T>(string value, out T returnedValue, bool ignoreCase)
		{
			bool result;
			try
			{
				returnedValue = (T)((object)Enum.Parse(typeof(T), value, ignoreCase));
				result = true;
			}
			catch
			{
				returnedValue = default(T);
				result = false;
			}
			return result;
		}

		// Token: 0x06000644 RID: 1604 RVA: 0x00019678 File Offset: 0x00017A78
		public static TSource MinEx<TSource, TCompare>(this IEnumerable<TSource> list, Func<TSource, TCompare> func) where TCompare : IComparable<TCompare>
		{
			bool flag = false;
			TSource tsource = default(TSource);
			foreach (TSource tsource2 in list)
			{
				if (flag)
				{
					TCompare tcompare = func(tsource);
					if (tcompare.CompareTo(func(tsource2)) <= 0)
					{
						continue;
					}
				}
				tsource = tsource2;
				flag = true;
			}
			return tsource;
		}

		// Token: 0x06000645 RID: 1605 RVA: 0x00019704 File Offset: 0x00017B04
		public static TSource MaxEx<TSource, TCompare>(this IEnumerable<TSource> list, Func<TSource, TCompare> func) where TCompare : IComparable<TCompare>
		{
			bool flag = false;
			TSource tsource = default(TSource);
			foreach (TSource tsource2 in list)
			{
				if (flag)
				{
					TCompare tcompare = func(tsource);
					if (tcompare.CompareTo(func(tsource2)) >= 0)
					{
						continue;
					}
				}
				tsource = tsource2;
				flag = true;
			}
			return tsource;
		}

		// Token: 0x06000646 RID: 1606 RVA: 0x00019790 File Offset: 0x00017B90
		public static string CreateFormatedString(string[] args)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (string text in args)
			{
				bool flag = text.Contains(" ");
				stringBuilder.AppendFormat((!flag) ? "{0}" : "'{0}'", text);
				stringBuilder.Append(' ');
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000647 RID: 1607 RVA: 0x000197F8 File Offset: 0x00017BF8
		public static string SerializeCollectionToString<T>(IEnumerable<T> collection)
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (T t in collection)
			{
				stringBuilder.AppendLine(t.ToString());
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06000648 RID: 1608 RVA: 0x00019868 File Offset: 0x00017C68
		public static byte[] CreateByteArrayFromType<T>(T data)
		{
			byte[] result;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				binaryFormatter.Serialize(memoryStream, data);
				result = memoryStream.ToArray();
			}
			return result;
		}

		// Token: 0x06000649 RID: 1609 RVA: 0x000198B8 File Offset: 0x00017CB8
		public static T GetTypeFromByteArray<T>(byte[] data)
		{
			T result;
			using (MemoryStream memoryStream = new MemoryStream(data))
			{
				BinaryFormatter binaryFormatter = new BinaryFormatter();
				result = (T)((object)binaryFormatter.Deserialize(memoryStream));
			}
			return result;
		}

		// Token: 0x0600064A RID: 1610 RVA: 0x00019904 File Offset: 0x00017D04
		public static IEnumerable<T> Shuffle<T>(IList<T> list)
		{
			Random random = new Random((int)DateTime.Now.Ticks);
			for (int i = list.Count - 1; i > 0; i--)
			{
				int index = random.Next(i + 1);
				T value = list[i];
				list[i] = list[index];
				list[index] = value;
			}
			return list;
		}

		// Token: 0x0600064B RID: 1611 RVA: 0x0001996C File Offset: 0x00017D6C
		public static string ToIsoString(this DateTime dt, string format)
		{
			return ((!format.EndsWith("Z", StringComparison.Ordinal)) ? dt.ToLocalTime() : dt.ToUniversalTime()).ToString(format);
		}

		// Token: 0x0600064C RID: 1612 RVA: 0x000199A8 File Offset: 0x00017DA8
		public static string GetAttributeDefault(this XmlElement el, string name, string defaultValue)
		{
			string attribute = el.GetAttribute(name);
			return string.IsNullOrEmpty(attribute) ? defaultValue : attribute;
		}

		// Token: 0x0600064D RID: 1613 RVA: 0x000199CF File Offset: 0x00017DCF
		public static Jid MakeJid(string client, string bootstrap, string host, string resource)
		{
			return new Jid(client, string.Format("{0}.{1}", bootstrap, host), resource);
		}

		// Token: 0x0600064E RID: 1614 RVA: 0x000199E4 File Offset: 0x00017DE4
		public static Jid MakeJid(string client, string resource)
		{
			return (!Resources.BootstrapMode) ? new Jid(client, "warface", resource) : Utils.MakeJid(client, Resources.BootstrapName, "warface", resource);
		}

		// Token: 0x0600064F RID: 1615 RVA: 0x00019A12 File Offset: 0x00017E12
		public static bool IsGZipHeader(byte[] dataBytes)
		{
			return dataBytes != null && dataBytes.Length > 2 && dataBytes[0] == 31 && dataBytes[1] == 139;
		}

		// Token: 0x06000650 RID: 1616 RVA: 0x00019A3A File Offset: 0x00017E3A
		public static bool ConsistsOfDigitsOnly(this string str)
		{
			bool result;
			if (!string.IsNullOrEmpty(str))
			{
				IEnumerable<char> source = str.ToCharArray();
				if (Utils.<>f__mg$cache2 == null)
				{
					Utils.<>f__mg$cache2 = new Func<char, bool>(char.IsDigit);
				}
				result = source.All(Utils.<>f__mg$cache2);
			}
			else
			{
				result = false;
			}
			return result;
		}

		// Token: 0x06000651 RID: 1617 RVA: 0x00019A74 File Offset: 0x00017E74
		public static IEnumerable<T> OfType<T>(this IEnumerable<T> seq, Type[] types)
		{
			return (from <>__TranspIdent0 in (from type in types
			from elem in seq
			select new
			{
				type,
				elem
			}).Where(delegate(<>__TranspIdent0)
			{
				Type type = <>__TranspIdent0.type;
				T elem = <>__TranspIdent0.elem;
				return type.IsAssignableFrom(elem.GetType());
			})
			select <>__TranspIdent0.elem).ToList<T>();
		}

		// Token: 0x06000652 RID: 1618 RVA: 0x00019AD4 File Offset: 0x00017ED4
		public static TReturn GetValueOrDefault<TKey, TValue, TReturn>(this Dictionary<TKey, TValue> dict, TKey key, TReturn @default)
		{
			TValue value;
			if (dict.TryGetValue(key, out value))
			{
				try
				{
					return value.ChangeType<TValue, TReturn>();
				}
				catch (InvalidCastException)
				{
					return @default;
				}
				catch (FormatException)
				{
					return @default;
				}
				return @default;
			}
			return @default;
		}

		// Token: 0x06000653 RID: 1619 RVA: 0x00019B2C File Offset: 0x00017F2C
		public static TOut ChangeType<TIn, TOut>(this TIn value)
		{
			return (TOut)((object)Convert.ChangeType(value, typeof(TOut)));
		}

		// Token: 0x06000654 RID: 1620 RVA: 0x00019B48 File Offset: 0x00017F48
		public static double Median(this IEnumerable<double> list)
		{
			List<double> list2 = (from x in list
			orderby x
			select x).ToList<double>();
			if (!list2.Any<double>())
			{
				return 0.0;
			}
			if (list2.Count == 1)
			{
				return list2[0];
			}
			double num = (double)(list2.Count - 1) / 2.0;
			return (list2[(int)num] + list2[(int)(num + 0.5)]) / 2.0;
		}

		// Token: 0x06000655 RID: 1621 RVA: 0x00019BE0 File Offset: 0x00017FE0
		public static bool TryParseGuid(string candidate, out Guid output)
		{
			bool result = false;
			output = Guid.Empty;
			if (candidate != null)
			{
				Regex regex = new Regex("^(\\{){0,1}[0-9a-fA-F]{8}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{4}\\-[0-9a-fA-F]{12}(\\}){0,1}$");
				if (regex.IsMatch(candidate))
				{
					output = new Guid(candidate);
					result = true;
				}
			}
			return result;
		}

		// Token: 0x06000656 RID: 1622 RVA: 0x00019C21 File Offset: 0x00018021
		public static bool IsOneOf<T>(this T value, params T[] items)
		{
			return items.Contains(value);
		}

		// Token: 0x06000657 RID: 1623 RVA: 0x00019C2C File Offset: 0x0001802C
		public static void LoadWithoutComments(this XmlDocument doc, Stream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = true
			};
			XmlReader reader = XmlReader.Create(stream, settings);
			doc.Load(reader);
		}

		// Token: 0x06000658 RID: 1624 RVA: 0x00019C58 File Offset: 0x00018058
		public static void LoadWithoutComments(this XmlDocument doc, string fileName)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				IgnoreComments = true
			};
			XmlReader reader = XmlReader.Create(fileName, settings);
			doc.Load(reader);
		}

		// Token: 0x06000659 RID: 1625 RVA: 0x00019C84 File Offset: 0x00018084
		public static DateTime SafeAdd(this DateTime time, TimeSpan span)
		{
			TimeSpan t = DateTime.MaxValue - time;
			TimeSpan t2 = DateTime.MinValue - time;
			if (span > t)
			{
				return DateTime.MaxValue;
			}
			return (!(span < t2)) ? time.Add(span) : DateTime.MinValue;
		}

		// Token: 0x0600065A RID: 1626 RVA: 0x00019CDC File Offset: 0x000180DC
		private static string GetDelegInvocationList(MulticastDelegate deleg)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine(string.Format("\n\tCount: {0}", deleg.GetInvocationList().Length));
			foreach (Delegate @delegate in deleg.GetInvocationList())
			{
				stringBuilder.AppendLine(string.Format("\t\tOwnerName: {0}, FunctionName: {1}", (@delegate.Target == null) ? "<unknown>" : @delegate.Target, (!(@delegate.Method != null)) ? "<unknown>" : @delegate.Method.Name));
			}
			return stringBuilder.ToString();
		}

		// Token: 0x040003ED RID: 1005
		[CompilerGenerated]
		private static Action<Exception> <>f__mg$cache0;

		// Token: 0x040003EE RID: 1006
		[CompilerGenerated]
		private static Action<Exception> <>f__mg$cache1;

		// Token: 0x040003EF RID: 1007
		[CompilerGenerated]
		private static Func<char, bool> <>f__mg$cache2;

		// Token: 0x02000163 RID: 355
		public class RegexStringComparer : IEqualityComparer<Regex>
		{
			// Token: 0x06000660 RID: 1632 RVA: 0x00019DCF File Offset: 0x000181CF
			public bool Equals(Regex regex1, Regex regex2)
			{
				return object.ReferenceEquals(regex1, regex2) || (!object.ReferenceEquals(regex1, null) && !object.ReferenceEquals(regex2, null) && regex1.ToString() == regex2.ToString());
			}

			// Token: 0x06000661 RID: 1633 RVA: 0x00019E0A File Offset: 0x0001820A
			public int GetHashCode(Regex regex)
			{
				return (!object.ReferenceEquals(regex, null)) ? regex.ToString().GetHashCode() : 0;
			}

			// Token: 0x040003F1 RID: 1009
			public static readonly Utils.RegexStringComparer Instance = new Utils.RegexStringComparer();
		}
	}
}

using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Web;
using HK2Net;
using MasterServer.Common;

namespace MasterServer.Core
{
	// Token: 0x02000125 RID: 293
	[Service]
	[Singleton]
	public class LogServiceBuilder : ILogServiceBuilder
	{
		// Token: 0x060004C0 RID: 1216 RVA: 0x0001412C File Offset: 0x0001252C
		public Type Build(Type baseClass, Type[] ctorParams, Type _interface)
		{
			LogServiceBuilder.CheckInterface(_interface);
			LogServiceBuilder.CheckBaseClass(baseClass);
			ModuleBuilder moduleBuilder = this.CreateModule();
			string name = string.Format("{0}Implementation", _interface.Name);
			TypeBuilder typeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public, baseClass);
			typeBuilder.AddInterfaceImplementation(_interface);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, ctorParams);
			ConstructorInfo constructor = baseClass.GetConstructor(ctorParams);
			ILGenerator ilgenerator = constructorBuilder.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			for (int i = 0; i < ctorParams.Length; i++)
			{
				ilgenerator.Emit(OpCodes.Ldarg, i + 1);
			}
			ilgenerator.Emit(OpCodes.Call, constructor);
			ilgenerator.Emit(OpCodes.Ret);
			FieldInfo field = baseClass.GetField("RECORD_SEPARATOR", BindingFlags.Static | BindingFlags.Public);
			MethodInfo method = typeof(object).GetMethod("ToString", new Type[0]);
			MethodInfo method2 = typeof(StringBuilder).GetMethod("Append", new Type[]
			{
				typeof(string)
			});
			foreach (MethodInfo methodInfo in from m in _interface.GetMethods()
			where !m.IsSpecialName
			select m)
			{
				string text = LogServiceBuilder.IsLogCategorySpecified(methodInfo);
				ParameterInfo[] parameters = methodInfo.GetParameters();
				MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, methodInfo.ReturnType, parameters.Select((ParameterInfo mp) => mp.ParameterType).ToArray<Type>());
				ILGenerator ilgenerator2 = methodBuilder.GetILGenerator();
				LocalBuilder localBuilder = ilgenerator2.DeclareLocal(typeof(Exception));
				LocalBuilder localBuilder2 = ilgenerator2.DeclareLocal(typeof(StringBuilder));
				ilgenerator2.Emit(OpCodes.Newobj, typeof(StringBuilder).GetConstructor(new Type[0]));
				ilgenerator2.Emit(OpCodes.Stloc, localBuilder2);
				ilgenerator2.BeginExceptionBlock();
				LogServiceBuilder.WriteLogKey(LogKey.log_category, method, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteString(text, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteLogKey(LogKey.log_datetime, method, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteDateTimeNow(baseClass, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteLogKey(LogKey.channel_id, method, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
				LogServiceBuilder.WriteChannelType(ilgenerator2, localBuilder2);
				for (int j = 0; j < parameters.Length; j++)
				{
					ParameterInfo parameterInfo = parameters[j];
					LogParamAttribute logParamAttribute = LogServiceBuilder.IsLogKeyForMethodParameterSpecified(methodInfo, parameterInfo);
					LogKeyAttribute logKeyAttribute = LogServiceBuilder.IsTypeForLogKeySpecified(logParamAttribute);
					LogServiceBuilder.AreLogKeyAndMethodParameterTypeSame(methodInfo, logParamAttribute, parameterInfo, logKeyAttribute);
					LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
					LogServiceBuilder.WriteLogKey(logParamAttribute.Key, method, method2, ilgenerator2, localBuilder2);
					LogServiceBuilder.WriteSeparator(field, method2, ilgenerator2, localBuilder2);
					this.WriteValue(j + 1, parameterInfo.ParameterType, method, baseClass, method2, ilgenerator2, localBuilder2);
				}
				ilgenerator2.BeginCatchBlock(typeof(Exception));
				ilgenerator2.Emit(OpCodes.Stloc_S, localBuilder);
				LogServiceBuilder.LogError(ilgenerator2, "Log formatting error for {0} category.", new string[]
				{
					text
				});
				LogServiceBuilder.LogException(localBuilder, ilgenerator2);
				ilgenerator2.EndExceptionBlock();
				LogServiceBuilder.WriteToLogs(baseClass, method, ilgenerator2, localBuilder2, text);
				ilgenerator2.Emit(OpCodes.Ret);
			}
			return typeBuilder.CreateType();
		}

		// Token: 0x060004C1 RID: 1217 RVA: 0x000144E0 File Offset: 0x000128E0
		private void WriteValue(int parameterIndex, Type parameterType, MethodInfo objectToString, Type baseClass, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			if (parameterType == typeof(DateTime))
			{
				LogServiceBuilder.WriteDateTime(delegate(ILGenerator ilGenerator)
				{
					ilGenerator.Emit(OpCodes.Ldarg, parameterIndex);
				}, baseClass, stringBuilderAppend, methodIlGenerator, stringBuilder);
			}
			else if (parameterType == typeof(TimeSpan))
			{
				methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
				methodIlGenerator.Emit(OpCodes.Ldarga_S, parameterIndex);
				methodIlGenerator.EmitCall(OpCodes.Call, typeof(TimeSpan).GetMethod("get_TotalSeconds"), null);
				methodIlGenerator.Emit(OpCodes.Conv_U8);
				methodIlGenerator.EmitCall(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[]
				{
					typeof(ulong)
				}), null);
				methodIlGenerator.Emit(OpCodes.Pop);
			}
			else if (typeof(Enum).IsAssignableFrom(parameterType))
			{
				LogServiceBuilder.WriteEnumValue(parameterType, delegate(ILGenerator ilGenerator)
				{
					ilGenerator.Emit(OpCodes.Ldarg, parameterIndex);
				}, objectToString, stringBuilderAppend, methodIlGenerator, stringBuilder);
			}
			else
			{
				if (!LogServiceBuilder.AllSupportedTypes.Contains(parameterType))
				{
					throw new ApplicationException(string.Format("Type {0} is not supported in LogService", parameterType.Name));
				}
				methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
				if (LogServiceBuilder.SupportedFloatingPointTypes.Contains(parameterType))
				{
					methodIlGenerator.Emit(OpCodes.Ldarga_S, parameterIndex);
					methodIlGenerator.EmitCall(OpCodes.Call, typeof(CultureInfo).GetMethod("get_InvariantCulture", BindingFlags.Static | BindingFlags.Public), null);
					methodIlGenerator.EmitCall(OpCodes.Callvirt, parameterType.GetMethod("ToString", new Type[]
					{
						typeof(IFormatProvider)
					}), null);
					methodIlGenerator.EmitCall(OpCodes.Call, typeof(HttpUtility).GetMethod("UrlEncode", new Type[]
					{
						typeof(string)
					}), null);
					methodIlGenerator.EmitCall(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[]
					{
						typeof(string)
					}), null);
				}
				else if (parameterType == typeof(string))
				{
					methodIlGenerator.Emit(OpCodes.Ldarg, parameterIndex);
					methodIlGenerator.EmitCall(OpCodes.Call, typeof(HttpUtility).GetMethod("UrlEncode", new Type[]
					{
						typeof(string)
					}), null);
					methodIlGenerator.EmitCall(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[]
					{
						typeof(string)
					}), null);
				}
				else
				{
					methodIlGenerator.Emit(OpCodes.Ldarg, parameterIndex);
					methodIlGenerator.EmitCall(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[]
					{
						parameterType
					}), null);
				}
				methodIlGenerator.Emit(OpCodes.Pop);
			}
		}

		// Token: 0x060004C2 RID: 1218 RVA: 0x000147FC File Offset: 0x00012BFC
		private static void WriteToLogs(Type baseClass, MethodInfo objectToString, ILGenerator methodIlGenerator, LocalBuilder stringBuilder, string logCategory)
		{
			methodIlGenerator.Emit(OpCodes.Ldarg_0);
			methodIlGenerator.Emit(OpCodes.Ldstr, logCategory);
			methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, objectToString, null);
			MethodInfo method = baseClass.GetMethod("WriteToLogs", BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[]
			{
				typeof(string),
				typeof(string)
			}, null);
			methodIlGenerator.EmitCall(OpCodes.Call, method, null);
		}

		// Token: 0x060004C3 RID: 1219 RVA: 0x00014878 File Offset: 0x00012C78
		private static void LogError(ILGenerator methodIlGenerator, string message, params string[] paramList)
		{
			methodIlGenerator.Emit(OpCodes.Ldstr, message);
			foreach (string str in paramList)
			{
				methodIlGenerator.Emit(OpCodes.Ldstr, str);
			}
			methodIlGenerator.EmitCall(OpCodes.Call, typeof(Log).GetMethod("Error", new Type[]
			{
				typeof(string),
				typeof(object[])
			}), null);
		}

		// Token: 0x060004C4 RID: 1220 RVA: 0x000148F8 File Offset: 0x00012CF8
		private static void LogException(LocalBuilder exception, ILGenerator methodIlGenerator)
		{
			methodIlGenerator.Emit(OpCodes.Ldloc_S, exception);
			methodIlGenerator.EmitCall(OpCodes.Call, typeof(Log).GetMethod("Error", new Type[]
			{
				typeof(Exception)
			}), null);
		}

		// Token: 0x060004C5 RID: 1221 RVA: 0x00014944 File Offset: 0x00012D44
		private static void WriteDateTimeNow(Type baseClass, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			LogServiceBuilder.WriteDateTime(delegate(ILGenerator ilGenerator)
			{
				ilGenerator.EmitCall(OpCodes.Call, typeof(DateTime).GetMethod("get_Now", BindingFlags.Static | BindingFlags.Public), null);
			}, baseClass, stringBuilderAppend, methodIlGenerator, stringBuilder);
		}

		// Token: 0x060004C6 RID: 1222 RVA: 0x0001496C File Offset: 0x00012D6C
		private static void WriteDateTime(Action<ILGenerator> emitDateTimeValue, Type baseClass, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
			emitDateTimeValue(methodIlGenerator);
			methodIlGenerator.EmitCall(OpCodes.Call, baseClass.GetMethod("get_TimestampFormat", BindingFlags.Static | BindingFlags.NonPublic), null);
			methodIlGenerator.EmitCall(OpCodes.Call, typeof(Utils).GetMethod("ToIsoString", BindingFlags.Static | BindingFlags.Public), null);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, stringBuilderAppend, null);
			methodIlGenerator.Emit(OpCodes.Pop);
		}

		// Token: 0x060004C7 RID: 1223 RVA: 0x000149E0 File Offset: 0x00012DE0
		private static void WriteString(string str, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
			methodIlGenerator.Emit(OpCodes.Ldstr, str);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, stringBuilderAppend, null);
			methodIlGenerator.Emit(OpCodes.Pop);
		}

		// Token: 0x060004C8 RID: 1224 RVA: 0x00014A12 File Offset: 0x00012E12
		private static void WriteSeparator(FieldInfo separatorField, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
			methodIlGenerator.Emit(OpCodes.Ldsfld, separatorField);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, stringBuilderAppend, null);
			methodIlGenerator.Emit(OpCodes.Pop);
		}

		// Token: 0x060004C9 RID: 1225 RVA: 0x00014A44 File Offset: 0x00012E44
		private static void WriteLogKey(LogKey logKey, MethodInfo objectToString, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			LogServiceBuilder.WriteEnumValue<LogKey>(delegate(ILGenerator ilGenerator)
			{
				ilGenerator.Emit(OpCodes.Ldc_I4, (int)logKey);
			}, objectToString, stringBuilderAppend, methodIlGenerator, stringBuilder);
		}

		// Token: 0x060004CA RID: 1226 RVA: 0x00014A74 File Offset: 0x00012E74
		private static void WriteChannelType(ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
			methodIlGenerator.EmitCall(OpCodes.Call, typeof(Resources).GetMethod("get_Channel", BindingFlags.Static | BindingFlags.Public), null);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, typeof(StringBuilder).GetMethod("Append", new Type[]
			{
				typeof(int)
			}), null);
			methodIlGenerator.Emit(OpCodes.Pop);
		}

		// Token: 0x060004CB RID: 1227 RVA: 0x00014AED File Offset: 0x00012EED
		private static void WriteEnumValue<T>(Action<ILGenerator> emitEnumValue, MethodInfo objectToString, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder) where T : struct
		{
			LogServiceBuilder.WriteEnumValue(typeof(T), emitEnumValue, objectToString, stringBuilderAppend, methodIlGenerator, stringBuilder);
		}

		// Token: 0x060004CC RID: 1228 RVA: 0x00014B04 File Offset: 0x00012F04
		private static void WriteEnumValue(Type enumType, Action<ILGenerator> emitEnumValue, MethodInfo objectToString, MethodInfo stringBuilderAppend, ILGenerator methodIlGenerator, LocalBuilder stringBuilder)
		{
			methodIlGenerator.Emit(OpCodes.Ldloc, stringBuilder);
			emitEnumValue(methodIlGenerator);
			methodIlGenerator.Emit(OpCodes.Box, enumType);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, objectToString, null);
			methodIlGenerator.EmitCall(OpCodes.Callvirt, stringBuilderAppend, null);
			methodIlGenerator.Emit(OpCodes.Pop);
		}

		// Token: 0x060004CD RID: 1229 RVA: 0x00014B5C File Offset: 0x00012F5C
		private static void AreLogKeyAndMethodParameterTypeSame(MethodInfo method, LogParamAttribute logParamAttribute, ParameterInfo parameter, LogKeyAttribute logKeyAttribute)
		{
			if (parameter.ParameterType != logKeyAttribute.Type)
			{
				throw new ApplicationException(string.Format("Parameter {0}'s type {1} of method {2} doesn't match the log key {3}'s type {4}", new object[]
				{
					parameter.Name,
					parameter.ParameterType,
					method.Name,
					logParamAttribute.Key,
					logKeyAttribute.Type
				}));
			}
		}

		// Token: 0x060004CE RID: 1230 RVA: 0x00014BC8 File Offset: 0x00012FC8
		private static LogKeyAttribute IsTypeForLogKeySpecified(LogParamAttribute logParamAttribute)
		{
			MemberInfo memberInfo = typeof(LogKey).GetMember(logParamAttribute.Key.ToString()).FirstOrDefault<MemberInfo>();
			LogKeyAttribute logKeyAttribute = (LogKeyAttribute)memberInfo.GetCustomAttributes(typeof(LogKeyAttribute), false).FirstOrDefault<object>();
			if (logKeyAttribute == null)
			{
				throw new ApplicationException(string.Format("Log key {0} is not marked by LogKeyAttribute.", logParamAttribute.Key));
			}
			return logKeyAttribute;
		}

		// Token: 0x060004CF RID: 1231 RVA: 0x00014C3C File Offset: 0x0001303C
		private static LogParamAttribute IsLogKeyForMethodParameterSpecified(MethodInfo method, ParameterInfo parameter)
		{
			LogParamAttribute logParamAttribute = (LogParamAttribute)parameter.GetCustomAttributes(typeof(LogParamAttribute), false).FirstOrDefault<object>();
			if (logParamAttribute == null)
			{
				throw new ApplicationException(string.Format("Log key for parameter {0} of method {1} is not specified.", parameter.Name, method.Name));
			}
			return logParamAttribute;
		}

		// Token: 0x060004D0 RID: 1232 RVA: 0x00014C88 File Offset: 0x00013088
		private static string IsLogCategorySpecified(MethodInfo method)
		{
			LogCategoryAttribute logCategoryAttribute = (LogCategoryAttribute)method.GetCustomAttributes(typeof(LogCategoryAttribute), false).FirstOrDefault<object>();
			if (logCategoryAttribute == null)
			{
				throw new ApplicationException(string.Format("Log category for method {0} is not specified.", method.Name));
			}
			return logCategoryAttribute.Name;
		}

		// Token: 0x060004D1 RID: 1233 RVA: 0x00014CD4 File Offset: 0x000130D4
		private ModuleBuilder CreateModule()
		{
			AssemblyName name = new AssemblyName("LogServiceImplementation");
			AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(name, AssemblyBuilderAccess.Run);
			return assemblyBuilder.DefineDynamicModule("LogServiceImplementation", true);
		}

		// Token: 0x060004D2 RID: 1234 RVA: 0x00014D05 File Offset: 0x00013105
		private static void CheckBaseClass(Type baseClass)
		{
			if (!baseClass.IsClass)
			{
				throw new ArgumentException(string.Format("Can't create log service for with base type {0}", baseClass), "baseClass");
			}
		}

		// Token: 0x060004D3 RID: 1235 RVA: 0x00014D28 File Offset: 0x00013128
		private static void CheckInterface(Type _interface)
		{
			if (!_interface.IsInterface)
			{
				throw new ArgumentException(string.Format("Can't create log service for type {0}", _interface), "_interface");
			}
		}

		// Token: 0x04000201 RID: 513
		private static readonly Type[] SupportedTypes = new Type[]
		{
			typeof(DateTime),
			typeof(TimeSpan),
			typeof(Enum),
			typeof(bool),
			typeof(byte),
			typeof(sbyte),
			typeof(short),
			typeof(ushort),
			typeof(int),
			typeof(uint),
			typeof(long),
			typeof(ulong),
			typeof(string)
		};

		// Token: 0x04000202 RID: 514
		private static readonly Type[] SupportedFloatingPointTypes = new Type[]
		{
			typeof(float),
			typeof(double),
			typeof(decimal)
		};

		// Token: 0x04000203 RID: 515
		private static readonly Type[] AllSupportedTypes = LogServiceBuilder.SupportedFloatingPointTypes.Union(LogServiceBuilder.SupportedTypes).ToArray<Type>();
	}
}

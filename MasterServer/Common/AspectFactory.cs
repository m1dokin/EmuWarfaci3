using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace MasterServer.Common
{
	// Token: 0x02000015 RID: 21
	internal class AspectFactory
	{
		// Token: 0x06000051 RID: 81 RVA: 0x00005610 File Offset: 0x00003A10
		private static void PrepareAssembly()
		{
			object obj = AspectFactory.s_lock;
			lock (obj)
			{
				if (!(AspectFactory.module != null))
				{
					AssemblyName assemblyName = new AssemblyName();
					assemblyName.Name = "AspectWrappers";
					AppDomain currentDomain = AppDomain.CurrentDomain;
					AspectFactory.assembly = currentDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
					AspectFactory.module = AspectFactory.assembly.DefineDynamicModule("AspectWrappers", true);
				}
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x0000569C File Offset: 0x00003A9C
		public static Type CreateWrapper(Type iface)
		{
			AspectFactory.PrepareAssembly();
			string name = string.Format("{0}_wrapper", iface.Name);
			TypeBuilder typeBuilder = AspectFactory.module.DefineType(name, TypeAttributes.Public, typeof(AspectWrapperBase));
			typeBuilder.AddInterfaceImplementation(iface);
			FieldBuilder field = typeBuilder.DefineField("m_wrapped", iface, FieldAttributes.Private);
			FieldInfo field2 = typeof(AspectWrapperBase).GetField("m_aspects", BindingFlags.Instance | BindingFlags.NonPublic);
			ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[]
			{
				iface
			});
			ConstructorInfo constructor = typeof(AspectWrapperBase).GetConstructor(new Type[]
			{
				typeof(Type)
			});
			MethodInfo method = typeof(Type).GetMethod("GetTypeFromHandle", new Type[]
			{
				typeof(RuntimeTypeHandle)
			});
			ILGenerator ilgenerator = constructorBuilder.GetILGenerator();
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldtoken, iface);
			ilgenerator.Emit(OpCodes.Call, method);
			ilgenerator.Emit(OpCodes.Call, constructor);
			ilgenerator.Emit(OpCodes.Ldarg_0);
			ilgenerator.Emit(OpCodes.Ldarg_1);
			ilgenerator.Emit(OpCodes.Stfld, field);
			ilgenerator.Emit(OpCodes.Ret);
			int num = 0;
			MethodInfo method2 = typeof(Aspect).GetMethod("PreCall");
			List<Type> list = new List<Type>(iface.GetInterfaces());
			list.Insert(0, iface);
			foreach (Type type in list)
			{
				foreach (MethodInfo methodInfo in type.GetMethods())
				{
					IEnumerable<object> enumerable = from attr in methodInfo.GetCustomAttributes(false)
					where attr is Aspect
					select attr;
					Type[] array = (from param in methodInfo.GetParameters()
					select param.ParameterType).ToArray<Type>();
					MethodBuilder methodBuilder = typeBuilder.DefineMethod(methodInfo.Name, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual, methodInfo.ReturnType, array);
					ILGenerator ilgenerator2 = methodBuilder.GetILGenerator();
					LocalBuilder local = ilgenerator2.DeclareLocal(typeof(MethodInfo));
					LocalBuilder local2 = ilgenerator2.DeclareLocal(iface);
					ilgenerator2.Emit(OpCodes.Ldtoken, methodInfo);
					ilgenerator2.Emit(OpCodes.Call, typeof(MethodBase).GetMethod("GetMethodFromHandle", new Type[]
					{
						typeof(RuntimeMethodHandle)
					}));
					ilgenerator2.Emit(OpCodes.Stloc, local);
					ilgenerator2.Emit(OpCodes.Ldarg_0);
					ilgenerator2.Emit(OpCodes.Ldfld, field);
					ilgenerator2.Emit(OpCodes.Stloc, local2);
					foreach (object obj in enumerable)
					{
						ilgenerator2.Emit(OpCodes.Ldarg_0);
						ilgenerator2.Emit(OpCodes.Ldfld, field2);
						ilgenerator2.Emit(OpCodes.Ldc_I4, num);
						ilgenerator2.Emit(OpCodes.Ldelem_Ref);
						ilgenerator2.Emit(OpCodes.Ldloc, local2);
						ilgenerator2.Emit(OpCodes.Ldloc, local);
						ilgenerator2.Emit(OpCodes.Callvirt, method2);
						num++;
					}
					ilgenerator2.Emit(OpCodes.Ldloc, local2);
					for (int num2 = 1; num2 != array.Length + 1; num2++)
					{
						ilgenerator2.Emit(OpCodes.Ldarg, num2);
					}
					ilgenerator2.Emit(OpCodes.Callvirt, methodInfo);
					ilgenerator2.Emit(OpCodes.Ret);
				}
			}
			return typeBuilder.CreateType();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005A98 File Offset: 0x00003E98
		public static T CreateInstance<T>(Type wrapper, object wrapped)
		{
			return (T)((object)Activator.CreateInstance(wrapper, new object[]
			{
				wrapped
			}));
		}

		// Token: 0x0400002D RID: 45
		private static AssemblyBuilder assembly;

		// Token: 0x0400002E RID: 46
		private static ModuleBuilder module;

		// Token: 0x0400002F RID: 47
		private static object s_lock = new object();
	}
}

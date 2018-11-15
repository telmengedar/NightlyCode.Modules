using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NightlyCode.Modules.Context;

namespace NightlyCode.DynamicProxy {

    /// <summary>
    /// methods used to create proxies
    /// </summary>
    public static class Proxy {
        static readonly AssemblyName assemblyname;
        static readonly AssemblyBuilder assemblybuilder;
        static readonly ModuleBuilder modulebuilder;
        static readonly Dictionary<Type, Type> cache=new Dictionary<Type, Type>();

        static Proxy() {
            AppDomain domain = AppDomain.CurrentDomain;
            assemblyname = new AssemblyName {
                Name = "NightlyCode.Proxy"
            };
            assemblybuilder = domain.DefineDynamicAssembly(assemblyname,
#if DEBUG
                AssemblyBuilderAccess.RunAndSave, @"C:\Develop\Projects\DynamicProxy\DynamicProxy.Tests\bin\Debug"
#else
                AssemblyBuilderAccess.Run
#endif
                );

#if DEBUG
            modulebuilder = assemblybuilder.DefineDynamicModule(assemblyname.Name, assemblyname.Name + ".dll", true);
#else
            modulebuilder = assemblybuilder.DefineDynamicModule(assemblyname.Name, true);
#endif
        }

        static Type CreateInterface(Type interfacetype) {
            if (!interfacetype.IsInterface)
                throw new ArgumentException($"'{interfacetype.Name}' is not an interface");

            TypeBuilder typebuilder = modulebuilder.DefineType($"NCProxy_{interfacetype.Name}", TypeAttributes.Class | TypeAttributes.Public);
            typebuilder.AddInterfaceImplementation(interfacetype);
            FieldBuilder interceptorfield = typebuilder.DefineField("interceptor", typeof(IInterceptor), FieldAttributes.Private);
            ConstructorBuilder constructor = typebuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, new[] { typeof(IInterceptor) });

            ILGenerator generator = constructor.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, typeof(object).GetConstructor(new Type[0]));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, interceptorfield);
            generator.Emit(OpCodes.Ret);

            // implement methods
            MethodInfo methodcaller = typeof(IInterceptor).GetMethod("CallMethod");
            foreach (MethodInfo method in GetMethods(interfacetype))
            {
                Type returntype = method.ReturnType;
                Type[] parameters = method.GetParameters().Select(p => p.ParameterType).ToArray();
                MethodBuilder methodbuilder = typebuilder.DefineMethod(method.Name, MethodAttributes.Public | MethodAttributes.Virtual, returntype, parameters);
                generator = methodbuilder.GetILGenerator();


                // call interceptor method
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, interceptorfield); // call method from interceptor
                generator.Emit(OpCodes.Ldstr, method.Name); // first parameter is method name

                generator.EmitLdc(parameters.Length);
                generator.Emit(OpCodes.Newarr, typeof(object));

                for (int i = 0; i < parameters.Length; ++i)
                {
                    generator.Emit(OpCodes.Dup);
                    generator.EmitLdc(i);
                    generator.EmitLdarg(i + 1);
                    if (parameters[i].IsValueType)
                        generator.Emit(OpCodes.Box, parameters[i]);
                    generator.Emit(OpCodes.Stelem_Ref);
                }
                generator.Emit(OpCodes.Callvirt, methodcaller); // call method

                if (method.ReturnType == typeof(void))
                    generator.Emit(OpCodes.Pop);
                else
                {
                    if (method.ReturnType.IsValueType)
                        generator.Emit(OpCodes.Unbox_Any, method.ReturnType);
                }
                generator.Emit(OpCodes.Ret);
                typebuilder.DefineMethodOverride(methodbuilder, method);
            }
            Type created = typebuilder.CreateType();
#if DEBUG
            assemblybuilder.Save(assemblyname.Name + ".dll");
#endif
            return created;
        }

        static void EmitLdarg(this ILGenerator generator, int param)
        {
            switch (param)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (param < 256)
                        generator.Emit(OpCodes.Ldarg_S, (byte)param);
                    else generator.Emit(OpCodes.Ldarg, param);
                    break;
            }
        }

        static void EmitLdc(this ILGenerator generator, int param) {
            switch(param) {
                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    break;
                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    break;
                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    break;
                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    break;
                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    break;
                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    break;
                default:
                    if(param < 256)
                        generator.Emit(OpCodes.Ldc_I4_S, (byte)param);
                    else generator.Emit(OpCodes.Ldc_I4, param);
                    break;
            }
        }
        /// <summary>
        /// implements an interface dynamically using an interceptor
        /// </summary>
        /// <typeparam name="T">type of interface to implement</typeparam>
        /// <param name="interceptor">interceptor to use</param>
        /// <returns>dynamic interface implementation</returns>
        public static T ImplementInterface<T>(IInterceptor interceptor) {
            return (T)ImplementInterface(typeof(T), interceptor);
        }

        public static object ImplementInterface(Type interfacetype, IInterceptor interceptor) {
            Type implementationtype;
            if (!cache.TryGetValue(interfacetype, out implementationtype))
                cache[interfacetype] = implementationtype = CreateInterface(interfacetype);

            return Activator.CreateInstance(implementationtype, interceptor);
        }

        static IEnumerable<MethodInfo> GetMethods(Type type) {
            foreach(MethodInfo method in type.GetMethods())
                yield return method;

            foreach(Type baseinterface in type.GetInterfaces())
                foreach(MethodInfo method in GetMethods(baseinterface))
                    yield return method;
        }

        static IEnumerable<PropertyInfo> GetProperties(Type type) {
            foreach(PropertyInfo property in type.GetProperties())
                yield return property;
            foreach(Type baseinterface in type.GetInterfaces())
                foreach(PropertyInfo property in GetProperties(baseinterface))
                    yield return property;
        }
    }
}
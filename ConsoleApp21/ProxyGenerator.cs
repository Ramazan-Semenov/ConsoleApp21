using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace ConsoleApp21
{
    public static class ProxyGenerator
    {
        public static T PropertyChangedProxy<T>() where T : class, new()
        {
            var type = typeof(T);
            var assemblyName = type.FullName + "_Proxy";
            var fileName = assemblyName + ".dll";
            var name = new AssemblyName(assemblyName);
            var assembly = AssemblyBuilder.DefineDynamicAssembly(name, AssemblyBuilderAccess.RunAndSave);
            var module = assembly.DefineDynamicModule(assemblyName, fileName);
            var typeBuilder = module.DefineType(type.Name + "Proxy",
                TypeAttributes.Class | TypeAttributes.Public, type);
            typeBuilder.DefineDefaultConstructor(MethodAttributes.Public);

            var onPropertyChangedMethod = type.GetMethod("OnPropertyChanged",
                BindingFlags.Instance | BindingFlags.NonPublic);

            var propertyInfos = type.GetProperties().Where(p => p.CanRead && p.CanWrite);
            foreach (var item in propertyInfos)
            {
                var baseMethod = item.GetGetMethod();
                var getAccessor = typeBuilder.DefineMethod(baseMethod.Name, baseMethod.Attributes, item.PropertyType, null);
                var il = getAccessor.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.EmitCall(OpCodes.Call, baseMethod, null);
                il.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(getAccessor, baseMethod);
                baseMethod = item.GetSetMethod();
                var setAccessor = typeBuilder.DefineMethod(baseMethod.Name, baseMethod.Attributes, typeof(void), new[] { item.PropertyType });
                il = setAccessor.GetILGenerator();
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldarg_1);
                il.Emit(OpCodes.Call, baseMethod);
                il.Emit(OpCodes.Ldarg_0);
                il.Emit(OpCodes.Ldstr, item.Name);
                il.Emit(OpCodes.Call, onPropertyChangedMethod);
                il.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(setAccessor, baseMethod);
            }
            var t = typeBuilder.CreateType();
            assembly.Save(fileName);
            return Activator.CreateInstance(t) as T;
        }
    }

}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp21
{
    public class TypeUser
    {
        public string FieldName { get; set; }
        public Type FieldType { get; set; }
    }
    //public static class PropertyChangedInvoker
    //{
    //    public static void Invoke(INotifyPropertyChanged sender,
    //        PropertyChangedEventHandler source, string propertyName)
    //    {
    //        source?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
    //    }
    //}
    public static class MyTypeBuilder
    {
        public static object CreateNewObject(IEnumerable<TypeUser> yourListOfFields)
        {
            var myType = CompileResultType(yourListOfFields);
            return Activator.CreateInstance(myType);
        }
        private static Type CompileResultType(IEnumerable<TypeUser> yourListOfFields)
        {
            TypeBuilder tb = GetTypeBuilder();
            ConstructorBuilder constructor = tb.DefineDefaultConstructor(MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName);

            var raiseEventMethod = ImplementPropertyChanged(tb);

            foreach (var field in yourListOfFields)
            {
                CreateProperty(tb, field.FieldName, field.FieldType, raiseEventMethod);
            }

            Type objectType = tb.CreateType();

            var propertyInfos = objectType.GetProperties().Where(p => p.CanRead && p.CanWrite);

       
            return objectType;
        }
      
        private static TypeBuilder GetTypeBuilder()
        {
            var typeSignature = "MyDynamicType";
            var an = new AssemblyName(typeSignature);
            AssemblyBuilder assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(an, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder typeBuilder = moduleBuilder.DefineType(typeSignature,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    null);



            return typeBuilder;
        }

        private static void ImplementAddEvent(TypeBuilder typeBuilder, FieldBuilder field, EventBuilder eventInfo)
        {
            var ibaseMethod = typeof(INotifyPropertyChanged).GetMethod("add_PropertyChanged");
            var addMethod = typeBuilder.DefineMethod("add_PropertyChanged",
                ibaseMethod.Attributes ^ MethodAttributes.Abstract,
                ibaseMethod.CallingConvention,
                ibaseMethod.ReturnType,
                new[] { typeof(PropertyChangedEventHandler) });
            var generator = addMethod.GetILGenerator();
            var combine = typeof(Delegate).GetMethod("Combine", new[] { typeof(Delegate), typeof(Delegate) });
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, combine);
            generator.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);
            eventInfo.SetAddOnMethod(addMethod);
        }

        private static MethodBuilder ImplementOnPropertyChanged(TypeBuilder typeBuilder, FieldBuilder field,
            EventBuilder eventInfo)
        {
            var methodBuilder = typeBuilder.DefineMethod("OnPropertyChanged",
                MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig |
                MethodAttributes.NewSlot, typeof(void),
                new[] { typeof(string) });
            var generator = methodBuilder.GetILGenerator();
            var returnLabel = generator.DefineLabel();
            var propertyArgsCtor = typeof(PropertyChangedEventArgs).GetConstructor(new[] { typeof(string) });
            generator.DeclareLocal(typeof(PropertyChangedEventHandler));
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Stloc_0);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Brfalse, returnLabel);
            generator.Emit(OpCodes.Ldloc_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Newobj, propertyArgsCtor);
            generator.Emit(OpCodes.Callvirt, typeof(PropertyChangedEventHandler).GetMethod("Invoke"));
            generator.MarkLabel(returnLabel);
            generator.Emit(OpCodes.Ret);
            eventInfo.SetRaiseMethod(methodBuilder);
            return methodBuilder;
        }


        private static MethodBuilder ImplementPropertyChanged(TypeBuilder typeBuilder)
        {
            typeBuilder.AddInterfaceImplementation(typeof(INotifyPropertyChanged));
            var field = typeBuilder.DefineField("PropertyChanged", typeof(PropertyChangedEventHandler),
                FieldAttributes.Private);
            var eventInfo = typeBuilder.DefineEvent("PropertyChanged", EventAttributes.None,
                typeof(PropertyChangedEventHandler));
            var methodBuilder = ImplementOnPropertyChanged(typeBuilder, field, eventInfo);
            ImplementAddEvent(typeBuilder, field, eventInfo);
            ImplementRemoveEvent(typeBuilder, field, eventInfo);
            return methodBuilder;
        }

        private static void ImplementRemoveEvent(TypeBuilder typeBuilder, FieldBuilder field, EventBuilder eventInfo)
        {
            var ibaseMethod = typeof(INotifyPropertyChanged).GetMethod("remove_PropertyChanged");
            var removeMethod = typeBuilder.DefineMethod("remove_PropertyChanged",
                ibaseMethod.Attributes ^ MethodAttributes.Abstract,
                ibaseMethod.CallingConvention,
                ibaseMethod.ReturnType,
                new[] { typeof(PropertyChangedEventHandler) });
            var remove = typeof(Delegate).GetMethod("Remove", new[] { typeof(Delegate), typeof(Delegate) });
            var generator = removeMethod.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldfld, field);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Call, remove);
            generator.Emit(OpCodes.Castclass, typeof(PropertyChangedEventHandler));
            generator.Emit(OpCodes.Stfld, field);
            generator.Emit(OpCodes.Ret);
            eventInfo.SetRemoveOnMethod(removeMethod);
        }

        private static void emmit_inheritance(Type typeBase)
        {
            // Создание типа, который наследует "typeBase" и вызывает его конструктор по умолчанию.
            var baseType = typeBase;
            var baseConstructor = baseType.GetConstructor(BindingFlags.Public | BindingFlags.FlattenHierarchy | BindingFlags.Instance, null, new Type[0], null);

            // Создайте конструктор типов, который генерирует тип непосредственно в текущем домене приложения
            var appDomain = AppDomain.CurrentDomain;
            var assemblyName = new AssemblyName("MyDynamicAssembly");
            var assemblyBuilder = appDomain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
            var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name);

            var typeBuilder = moduleBuilder.DefineType("MyDynamicType", TypeAttributes.Class | TypeAttributes.Public, baseType);

            // Создание конструктора без параметров (по умолчанию).
            var constructor = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);

            var ilGenerator = constructor.GetILGenerator();

            // Generate constructor code
            ilGenerator.Emit(OpCodes.Ldarg_0);                // push "this" onto stack.
            ilGenerator.Emit(OpCodes.Call, baseConstructor);  // call base constructor

            ilGenerator.Emit(OpCodes.Nop);                    // C# compiler add 2 NOPS, so
            ilGenerator.Emit(OpCodes.Nop);                    // we'll add them, too.

            ilGenerator.Emit(OpCodes.Ret);                    // Return
        }
        private static void CreateProperty(TypeBuilder tb, string propertyName, Type propertyType, MethodBuilder raiseEventMethod)
        {
            FieldBuilder fieldBuilder = tb.DefineField("_" + propertyName, propertyType, FieldAttributes.Private);

            PropertyBuilder propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            MethodBuilder getPropMthdBldr = tb.DefineMethod("get_" + propertyName, System.Reflection.MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig, propertyType, Type.EmptyTypes);
            ILGenerator getIl = getPropMthdBldr.GetILGenerator();
          
            getIl.Emit(OpCodes.Ldarg_0);
            getIl.Emit(OpCodes.Ldfld, fieldBuilder);
            getIl.Emit(OpCodes.Ret);

            MethodBuilder setPropMthdBldr =
                tb.DefineMethod("set_" + propertyName,
                  MethodAttributes.Public |
                  MethodAttributes.SpecialName |
                  MethodAttributes.HideBySig,
                  null, new[] { propertyType });

            ILGenerator currSetIL = setPropMthdBldr.GetILGenerator();

            currSetIL.Emit(OpCodes.Ldarg_0);
            currSetIL.Emit(OpCodes.Ldarg_1);
            currSetIL.Emit(OpCodes.Stfld, fieldBuilder);
            currSetIL.Emit(OpCodes.Ldarg_0);
            currSetIL.Emit(OpCodes.Ldstr, propertyName);
            currSetIL.Emit(OpCodes.Call, raiseEventMethod);
            currSetIL.Emit(OpCodes.Ret);


            propertyBuilder.SetGetMethod(getPropMthdBldr);
            propertyBuilder.SetSetMethod(setPropMthdBldr);
        }

    
    }
    public class Program
    {
        public static void Main()
        {
            var l = new List<TypeUser> { new TypeUser { FieldName = "Name", FieldType = typeof(string), } };
            List<object> list = new List<object>();
            var a = MyTypeBuilder.CreateNewObject(l);
            Type type = a.GetType();
            for (int i = 0; i < 1000; i++)
            {

                list.Add(Activator.CreateInstance(type));

            }
            //foreach (var item in q.GetType().GetRuntimeProperties())
            //{
            //    Console.WriteLine(item.Name+ "     "+item.PropertyType);
            //}

            Console.WriteLine();

            foreach (var item in list)
            {
                if (item is INotifyPropertyChanged aw)
                {
                    aw.PropertyChanged += Aw_PropertyChanged;
                   // Console.WriteLine();

                }
            }
            foreach (var item in list)
            {
                item.GetType().GetProperty("Name").SetValue(item, "dsfsdf");

            }
            //Console.WriteLine(list);
            Console.Read();
        }

        private static void Aw_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
        }
    }

}

//using System;
//using System.Linq;
//using System.Reflection;
//using System.Collections.Generic;

//namespace Loxodon.Framework.Utilities
//{
//    public static class TypeUtility
//    {
//        private static Assembly[] assemblies;
//        private static Dictionary<string, Type> typeLookup;
//        static TypeUtility()
//        {
//            assemblies = AppDomain.CurrentDomain.GetAssemblies();
//            var runtimeAsms = new List<Assembly>();
//            foreach (Assembly asm in assemblies)
//            {
//                if (!asm.GetName().Name.Contains("Editor"))
//                    runtimeAsms.Add(asm);
//            }
//            assemblies = runtimeAsms.ToArray();
//            typeLookup = new Dictionary<string, Type>();

//        }

//        public static string[] GetSubTypeNames(Type baseType)
//        {
//            return GetSubTypes(baseType).Select(x => x.Name).ToArray();
//        }

//        public static Type[] GetSubTypes(Type baseType)
//        {
//            IEnumerable<Type> types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(assembly => assembly.GetTypes()).Where(type => type.IsSubclassOf(baseType));
//            return types.ToArray();
//        }

//        public static Type GetMemberType(Type type, string name)
//        {
//            FieldInfo fieldInfo = type.GetField(name);
//            if (fieldInfo != null)
//            {
//                return fieldInfo.FieldType;
//            }
//            PropertyInfo propertyInfo = type.GetProperty(name);
//            if (propertyInfo != null)
//            {
//                return propertyInfo.PropertyType;
//            }
//            return null;
//        }

//        public static Type GetType(string name)
//        {
//            Type type = null;
//            if (typeLookup.TryGetValue(name, out type))
//            {
//                return type;
//            }

//            foreach (Assembly a in assemblies)
//            {
//                Type[] assemblyTypes = a.GetTypes();
//                for (int j = 0; j < assemblyTypes.Length; j++)
//                {
//                    if (assemblyTypes[j].Name == name)
//                    {
//                        typeLookup.Add(name, assemblyTypes[j]);
//                        return assemblyTypes[j];
//                    }
//                }
//            }

//            return null;

//        }

//        public static Type[] GetTypeByName(string className)
//        {
//            List<Type> returnVal = new List<Type>();

//            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
//            {
//                Type[] assemblyTypes = a.GetTypes();
//                for (int j = 0; j < assemblyTypes.Length; j++)
//                {
//                    if (assemblyTypes[j].Name == className && !assemblyTypes[j].ToString().Contains("PlayMaker"))
//                    {
//                        returnVal.Add(assemblyTypes[j]);
//                    }
//                }
//            }
//            return returnVal.ToArray();
//        }
//    }
//}
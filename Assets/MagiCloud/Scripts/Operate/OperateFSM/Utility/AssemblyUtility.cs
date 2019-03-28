using System;
using System.Collections.Generic;
using System.Reflection;
namespace Utility
{
    public static class AssemblyUtility
    {
        private static readonly Assembly[] assemblies = null;
        private static readonly Dictionary<string,Type> allTypes = new Dictionary<string,Type>();
        static AssemblyUtility()
        {
            assemblies=AppDomain.CurrentDomain.GetAssemblies();
        }
        public static Type GetTypeByName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message",nameof(name));
            }
            Type type = null;
            if (allTypes.TryGetValue(name,out type))
            {
                return type;
            }
            type=Type.GetType(name);
            if (type!=null)
            {
                allTypes.Add(name,type);
                return type;
            }
            foreach (var item in assemblies)
            {
                type=Type.GetType(Utility.Text.Format(name,item.FullName));
                if (type!=null)
                {
                    allTypes.Add(name,type);
                    return type;
                }
            }
            return null;
        }
    }
}

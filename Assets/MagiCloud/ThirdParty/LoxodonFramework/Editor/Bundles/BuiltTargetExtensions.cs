//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEditor;

//namespace Loxodon.Framework.Bundles.Editors
//{
//    [InitializeOnLoad]
//    public static class BuiltTargetExtensions
//    {
//        static bool initialized = false;
//        static Dictionary<BuildTargetGroup, List<BuildTarget>> dict;

//        static BuiltTargetExtensions()
//        {
//            Init();
//        }

//        public static void Init()
//        {
//            if (initialized)
//                return;

//            dict = new Dictionary<BuildTargetGroup, List<BuildTarget>>();
//            foreach (int value in Enum.GetValues(typeof(BuildTarget)))
//            {
//                BuildTarget target = (BuildTarget)value;
//                BuildTargetGroup group = BuildPipeline.GetBuildTargetGroup(target);

//                List<BuildTarget> targets;
//                if (!dict.TryGetValue(group, out targets))
//                {
//                    targets = new List<BuildTarget>();
//                    dict.Add(group, targets);
//                }
//                targets.Add(target);
//            }
//            initialized = true;
//        }

//        public static BuildTarget[] GetTargets(this BuildTargetGroup group)
//        {
//            if (!initialized)
//                Init();

//            if (dict.ContainsKey(group))
//                return dict[group].ToArray();
//            return new BuildTarget[0];
//        }

//        private static MethodInfo buildTargetSupported = typeof(BuildPipeline).GetMethod("IsBuildTargetSupported", BindingFlags.Static | BindingFlags.NonPublic);
//        public static bool IsSupported(this BuildTarget target)
//        {
//            return Convert.ToBoolean(buildTargetSupported.Invoke(null, new object[] { target.GetGroup(), target }));
//        }

//        public static BuildTargetGroup GetGroup(this BuildTarget target)
//        {
//            return BuildPipeline.GetBuildTargetGroup(target);
//        }
//    }
//}

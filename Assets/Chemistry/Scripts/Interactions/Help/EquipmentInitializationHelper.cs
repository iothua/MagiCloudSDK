using Chemistry.Data;
using Chemistry.Equipments;
using MagiCloud.Equipments;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Chemistry.Help
{
    /// <summary>
    /// 仪器初始化编辑
    /// </summary>
    public class EquipmentInitializationHelper
    {
        private static List<string> equimentsAsset;

        /// <summary>
        /// 创建配置好的仪器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static GameObject CreateSuccessEquipment(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("message",nameof(name));
            }

#if UNITY_EDITOR
            Transform[] selectedObject = UnityEditor.Selection.GetTransforms(UnityEditor.SelectionMode.TopLevel |UnityEditor.SelectionMode.ExcludePrefab);
#endif


            Transform parent = EquipmentUtility.GetSeleteTransform();
            var gameObject = GetResource<GameObject>("Prefabs/Equipments/"+name,"完整预制体");// Resources.Load<GameObject>("Prefabs/Equipments/"+capName+"_"+modeName);

            return EquipmentUtility.CreateObject(parent,gameObject);
        }

        /// <summary>
        /// 加载资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path"></param>
        /// <returns></returns>
        public static T GetResource<T>(string path,string remark = default(string)) where T : UnityEngine.Object
        {
            T prefab = Resources.Load<T>(path);
            if (prefab==null)
            {
                Debug.Log("<color=#FF4040>"+"加载"+remark+"资源失败,请检查资源路径："+path+"</color>");
            }
            return prefab;
        }

        /// <summary>
        /// 获取资源名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<string> GetAssetNames(string path = "Prefabs/Equipments")
        {
            if (equimentsAsset==null)
            {
                equimentsAsset=new List<string>();
#if UNITY_EDITOR
                equimentsAsset.AddRange(GetAssetNamesByFilePath());
#endif
            }
            return equimentsAsset;
        }

        /// <summary>
        /// 从Resource路径获取资源名称
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private static List<string> GetAssetNamesByRes(string path)
        {
            List<string> temp = new List<string>();
            var e = Resources.LoadAll<GameObject>(path);
            for (int i = 0; i < e.Length; i++)
            {
                temp.Add(e[i].name);
            }
            return temp;
        }

        /// <summary>
        /// 通过文件目录路径获得资源名
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static string[] GetAssetNamesByFilePath(string path = "Assets/Chemistry/Resources/Prefabs/Equipments")
        {
            List<string> temp = new List<string>();
            if (Directory.Exists(path))
            {
                DirectoryInfo info = new DirectoryInfo(path);
                FileInfo[] files = info.GetFiles("*",SearchOption.AllDirectories);

                for (int i = 0; i < files.Length; i++)
                {
                    string name = files[i].Name;
                    if (name.EndsWith(".meta"))
                        continue;
                    name=name.Remove(name.LastIndexOf("."));
                    temp.Add(name);
                }
            }
            return temp.ToArray();
        }


    }
}

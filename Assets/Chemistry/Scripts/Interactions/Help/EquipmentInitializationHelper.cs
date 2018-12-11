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
        /// 创建或添加仪器
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public static Transform CreateOrGetEquipment(DI_EquipmentInfo info)
        {
            if (info==null)
                throw new ArgumentNullException(nameof(info));
            //实例化模型
            Transform tra = LoadModel(info.resourcesName);
            tra.gameObject.name=info.equipmentName;

            //挂载脚本
            var component = tra.AddComponentByName(info.Namespaces+"."+info.scriptName);
            
            if (component is EquipmentBase)
            {
                var equipment = component as EquipmentBase;
                equipment.OnInitializeEquipment_Editor(info.equipmentName);
                
            }

            if (info.childEquipments!=null)
            {
                for (int i = 0; i < info.childEquipments.Count; i++)
                {
                    Transform child = null;
                    string childName = info.childEquipments[i];
                    //如果子物体已经完成,直接加载完成体
                    if (equimentsAsset.Contains(childName))
                        child= CreateSuccessEquipment(childName).transform;
                    else
                    {
                        //如果已经编辑,加载编辑体
                        if (DataLoading.DicEquipmentLoadingInfo.ContainsKey(childName))
                            child= CreateOrGetEquipment(DataLoading.DicEquipmentLoadingInfo[childName]);
                    }
                    child.SetParent(tra);
                }
            }
            return tra;
        }

        #region 注释

        ///// <summary>
        ///// 创建仪器
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="containerType"></param>
        ///// <param name="capType"></param>
        ///// <param name="labelName"></param>
        //public static Transform CreatEquipment(string key,EContainerType containerType,ECapType capType,string labelName = default(string))
        //{
        //    string modelName = containerType.ToString();
        //    int modelIndex = (int)containerType;
        //    return CreatEquipment(key,modelName,modelIndex,capType,labelName);
        //}

        ///// <summary>
        ///// 创建仪器
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="capType"></param>
        ///// <param name="labelName"></param>
        ///// <param name="modelName"></param>
        ///// <param name="modelIndex"></param>
        //public static Transform CreatEquipment(string key,string modelName,int modelIndex,ECapType capType,string labelName = default(string))
        //{
        //    Transform equipment;
        //    equipment= LoadModel(modelName);        //加载模型
        //    equipment.name=key;

        //    EO_Cap cap = LoadCap(capType,equipment);               //加载盖子

        //    EquipmentBase equipmentBase = ModeAddScript(modelIndex,key,equipment,cap,labelName);

        //    LoadLabel(labelName,equipmentBase.ModelNode);                          //加载标签
        //    return equipment;
        //}
        #endregion

        /// <summary>
        /// 加载模型
        /// </summary>
        /// <param name="equipmentName"></param>
        /// <param name="equipment"></param>
        /// <param name="model"></param>
        private static Transform LoadModel(string equipmentName)
        {
            Transform equipment = null;
            Transform prefab = GetResource<Transform>("Prefabs/"+equipmentName,"模型");// Resources.Load<Transform>("Prefabs/"+containerName);
            if (prefab!=null)
            {
                equipment  = UnityEngine.Object.Instantiate(prefab);
                EquipmentUtility.CreateNode(equipment,"Model");
                EquipmentUtility.CreateNode(equipment,"Effect");
                EquipmentUtility.CreateNode(equipment,"Liquid");
            }
            return equipment;
        }

        #region 注释

        ///// <summary>
        ///// 模型挂载脚本
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="key"></param>
        ///// <param name="equipment"></param>
        ///// <param name="cap"></param>
        //private static EquipmentBase ModeAddScript(int index,string key,Transform equipment,EO_Cap cap,string labelName = null)
        //{
        //    //匹配枚举文件
        //    string name = EnumTool.IntToString(index,typeof(EContainerTypeEN));

        //    string scriptName = "Chemistry.Equipments."+name;
        //    if (!string.IsNullOrEmpty(scriptName))
        //    {
        //        //根据枚举值添加脚本
        //        var mode = equipment.AddComponentByName(scriptName) as EquipmentBase;

        //        if (!string.IsNullOrEmpty(labelName))
        //        {
        //            if (cap==null)
        //            {
        //                var container = mode as EC_ContainerBase;
        //                container.containerType=(EContainerType)index;
        //            }
        //            else
        //            {
        //                var container = mode as EC_Save;
        //                container.containerType=(EContainerType)index;
        //                container._Cap=cap;
        //            }
        //        }
        //        mode.OnInitializeEquipment_Editor(key);
        //        return mode;
        //    }
        //    return null;
        //}


        ///// <summary>
        ///// 加载盖子
        ///// </summary>
        ///// <param name="containerName"></param>
        ///// <param name="capName"></param>
        ///// <param name="equipment"></param>
        ///// <param name="cap"></param>
        ///// <returns></returns>
        //private static EO_Cap LoadCap(ECapType capType,Transform equipment)
        //{
        //    string capName = (capType== ECapType.无 ? string.Empty : capType.ToString());
        //    int capIndex = (int)capType;
        //    EO_Cap cap = LoadCap(equipment,capName,capIndex);
        //    return cap;
        //}

        //private static EO_Cap LoadCap(Transform equipment,string capName,int capIndex)
        //{
        //    EO_Cap cap = null;
        //    if (!string.IsNullOrEmpty(capName))
        //    {
        //        string capPath = capName;

        //        Transform prefab = GetResource<Transform>("Prefabs/"+capPath,"盖子");// Resources.Load<Transform>();

        //        var capTrans = GameObject.Instantiate<Transform>(prefab,equipment);

        //        //创建页面

        //        EquipmentUtility.CreateNode(capTrans,"Effect");
        //        EquipmentUtility.CreateNode(capTrans,"Liquid");
        //        cap = capTrans.gameObject.AddComponent<EO_Cap>();

        //        CapAddScript(capIndex,capName,cap,capTrans);
        //    }
        //    return cap;
        //}


        ///// <summary>
        ///// 盖子挂载脚本
        ///// </summary>
        ///// <param name="info"></param>
        ///// <param name="capName"></param>
        ///// <param name="cap"></param>
        ///// <param name="capTrans"></param>
        //private static void CapAddScript(int index,string capName,EO_Cap cap,Transform capTrans)
        //{
        //    string name = EnumTool.IntToString(index,typeof(ECapTypeEN));
        //    string scriptName = "Chemistry.Equipments."+name;
        //    if (!string.IsNullOrEmpty(scriptName))
        //    {
        //        var capScript = capTrans.AddComponentByName(scriptName) as EquipmentBase;
        //        capScript.OnInitializeEquipment_Editor(capName);
        //    }
        //    cap.IsRotate=false;
        //}
        #endregion

        /// <summary>
        /// 加载标签
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="model"></param>
        private static void LoadLabel(string labelName,Transform model)
        {
            if (!string.IsNullOrEmpty(labelName))
            {

                Transform prefab = GetResource<Transform>("Models/Bottle/Labels/"+labelName,"标签");
                if (prefab!=null)
                {
                    var labels = GameObject.Instantiate<Transform>(prefab,model);
                    labels.localPosition = new Vector3(0.002f,0.293f,-0.165f);
                }
            }
        }

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

            //string modeName = info.containerType.ToString();
            //string capName = info.capType==ECapType.无 ? string.Empty : info.capType.ToString(); //盖子名字
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

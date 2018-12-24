//using UnityEngine;
//using UnityEditor;
//using System.Collections.Generic;
//using MagiCloud.Interactive.Distance;
//using Chemistry.Interactions;
//using System.Linq;

///// <summary>
///// 倒水控制编辑器扩展
///// 阮榆皓
///// </summary>
//[CustomEditor(typeof(PourLiquidControl))]
//[CanEditMultipleObjects]
//public class PourLiquidControlEdit : Editor
//{
//    //public SerializedProperty 
//    private PourLiquidControl pourLiquidCtl;

//    public List<InteractionPourWater> interactionLst = new List<InteractionPourWater>();

//    public string pourPtName = "倒水交互";

//    public PourPointSide curPourType = PourPointSide.Left;

//    private void OnEnable()
//    {
//        pourLiquidCtl = serializedObject.targetObject as PourLiquidControl;

//        //interactionLst = new List<InteractionPourWater>();

//        for (int i = 0; i < interactionLst.Count; i++)
//        {
//            interactionLst[i].gameObject.SetActive(true);
//        }

//        Debug.Log(pourLiquidCtl.name + "调用OnEnable");
//    }

//    public override void OnInspectorGUI()
//    {
//        GUILayout.Space(20);
//        EditorGUILayout.BeginVertical("box");
//        pourPtName = EditorGUILayout.TextField("倒水交互名称:", pourPtName);
//        curPourType = (PourPointSide)EditorGUILayout.EnumPopup("倒水点类型:", curPourType);
//        GUILayout.Space(5);
//        if (GUILayout.Button("创建倒水点", GUILayout.Width(150)))
//        {
//            string tmpType = "";
//            switch (curPourType)
//            {
//                case PourPointSide.Left:
//                    tmpType = "左";
//                    break;
//                case PourPointSide.Right:
//                    tmpType = "右";
//                    break;
//                default:
//                    break;
//            }
//            GameObject pourPtObj = new GameObject("pourObj_" + pourPtName + "_" + tmpType);
//            var pourObj = pourPtObj.AddComponent<InteractionPourWater>();
//            pourPtObj.transform.SetParent(FindDistanceParent());
//            pourPtObj.transform.localPosition = Vector3.zero;
//            pourObj.distanceData.TagID = pourPtName;
//            pourObj.distanceData.interactionType = InteractionType.Pour;
//            pourObj.distanceData.detectType = InteractionDetectType.And;

//            interactionLst.Add(pourObj);
//        }
//        GUILayout.Space(5);

//        if (GUILayout.Button("刷新", GUILayout.Width(150)))
//        {
//            interactionLst.Clear();

//            interactionLst = pourLiquidCtl.transform.GetComponentsInChildren<InteractionPourWater>().ToList();
//        }

//        EditorGUILayout.EndVertical();

//    }

//    private void OnDisable()
//    {
//        for (int i = 0; i < interactionLst.Count; i++)
//        {
//            interactionLst[i].gameObject.SetActive(false);
//        }
//        Debug.Log(pourLiquidCtl.name + "调用OnDisable");
//    }

//    private void OnDestroy()
//    {
//        if (Application.isPlaying)
//        {
//            Destroy(this);
//            for (int i = 0; i < interactionLst.Count; i++)
//            {
//                Destroy(interactionLst[i].gameObject);
//            }
//        }
//        else
//        {
//            for (int i = 0; i < interactionLst.Count; i++)
//            {
//                DestroyImmediate(interactionLst[i].gameObject);
//            }
//        }

//        Debug.Log(pourLiquidCtl.name + "调用OnDestroy");
//    }

//    Transform FindDistanceParent()
//    {
//        Transform parent = pourLiquidCtl.transform.Find("distanceParent");
//        if (parent == null)
//        {
//            parent = new GameObject("distanceParent").transform;
//            parent.SetParent(pourLiquidCtl.transform);
//            parent.localPosition = Vector3.zero;
//            parent.localRotation = Quaternion.identity;
//            parent.localScale = Vector3.one;
//        }

//        return parent;
//    }


//}
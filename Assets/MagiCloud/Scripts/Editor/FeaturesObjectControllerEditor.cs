using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using MagiCloud.Interactive.Distance;
using MagiCloud.Interactive;

namespace MagiCloud.Features
{

    [CustomEditor(typeof(FeaturesObjectController))]
    [CanEditMultipleObjects]
    public class FeaturesObjectControllerEditor :Editor
    {
        private FeaturesObjectController features;
        private MCDeskLimit _deskLimit;
        private SpaceLimit _spaceLimit;
        private HighlightObject _highlight;
        // private ShadowController _shadowController;
        private LabelData _labelController;

        private MCCanGrab _cangrabController;
        private MCObjectRotation _objectRatationController;
        private MCCameraRotateAround _rotateAround;
        private MCLimitMove _limitMoveController;
        private MCustomize _customizeController;
        private MCObjectButton _objectButton;
        private MCNone _noneController;

        private string distanceName;
        private List<DistanceInteraction> distances;
        private List<List<DistanceInteraction>> distanceSums;

        public static bool ShowOpera { get; private set; }

        private void OnEnable()
        {
            features = serializedObject.targetObject as FeaturesObjectController;
            distances = new List<DistanceInteraction>();
            if (distanceSums == null)
                distanceSums = new List<List<DistanceInteraction>>();

            var parent = features.transform.Find("distanceParent");
            if (parent != null)
            {
                distances = parent.GetComponentsInChildren<DistanceInteraction>().ToList();

                Dismantling();
            }
        }

        /// <summary>
        /// 拆解
        /// </summary>
        void Dismantling()
        {

            int addValue = 3;
            int maxCount = addValue;

            for (int i = 0; i < distances.Count; i += addValue)
            {
                List<DistanceInteraction> cList = new List<DistanceInteraction>();
                cList = distances.Take(maxCount).Skip(i).ToList();
                maxCount += addValue;

                distanceSums.Add(cList);
            }
        }

        public override void OnInspectorGUI()
        {

            GUILayout.Space(10);

            EditorGUILayout.BeginVertical("box");

            //桌面吸附及限制下陷
            EditorGUILayout.BeginVertical("box");
            features.ActiveDeskLimit_ = GUILayout.Toggle(features.ActiveDeskLimit_, "  激活桌面吸附及限制下陷到桌面----------------------------------------------------");
            InspectorDeskLimit();
            EditorGUILayout.EndVertical();

            //空间限制面板
            EditorGUILayout.BeginVertical("box");
            features.ActiveSpaceLimit = GUILayout.Toggle(features.ActiveSpaceLimit,"  激活空间限制--------------------------------------------------------------");
            InspectorSpaceLimit();
            EditorGUILayout.EndVertical();

            //高亮面板
            EditorGUILayout.BeginVertical("box");
            features.ActiveHighlight = GUILayout.Toggle(features.ActiveHighlight,"  激活高亮--------------------------------------------------------------");
            InspectorHighLight();
            EditorGUILayout.EndVertical();


            //标签面板
            EditorGUILayout.BeginVertical("box");
            features.ActiveLabel = GUILayout.Toggle(features.ActiveLabel,"  激活标签--------------------------------------------------------------");
            InspectorLabel();
            EditorGUILayout.EndVertical();


            ////虚影面板
            //EditorGUILayout.BeginVertical("box");
            //features.ActiveShadow = GUILayout.Toggle(features.ActiveShadow,"  激活虚影--------------------------------------------------------------");
            //InspectorShadow();
            //EditorGUILayout.EndVertical();



            //限制移动面板
            EditorGUILayout.BeginVertical("box");
            features.ActiveLimitMove = GUILayout.Toggle(features.ActiveLimitMove,"  激活限制移动--------------------------------------------------------------");
            InspectorLimitMove();
            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginVertical("box");
            //物体操作
            features.operaType = (ObjectOperaType)EditorGUILayout.EnumPopup("物体操作类型：",features.operaType);

            OnOperaStatus();

            switch (features.operaType)
            {
                case ObjectOperaType.无:
                    InspectorNone();
                    break;
                case ObjectOperaType.能抓取:
                    InspectorCanGrab();
                    break;
                case ObjectOperaType.物体自身旋转:
                    InspectorSelfRotation();
                    break;
                case ObjectOperaType.摄像机围绕物体旋转:
                    InspectorCameraCenterObjectRotation();
                    break;

                case ObjectOperaType.自定义:
                    InspectorCustomize();
                    break;
                case ObjectOperaType.物体式按钮:

                    InspectorObjectButton();
                    break;
                default:
                    break;
            }
            DrawShowOperaBtn();
            EditorGUILayout.EndVertical();

            EditorGUILayout.EndVertical();

            //创建距离界面
            InspectorDistance();
        }

        private void DrawShowOperaBtn()
        {
            if (ShowOpera)
            {
                if (GUILayout.Button("隐藏operaObject上的脚本",GUILayout.Width(150)))
                {
                    var monos = features.OperaObject.GetComponents<MonoBehaviour>();
                    foreach (var item in monos)
                    {
                        item.hideFlags=HideFlags.HideInInspector;
                    }
                    ShowOpera=false;
                }
            }
            else
            {
                if (GUILayout.Button("显示operaObject上的脚本",GUILayout.Width(150)))
                {
                    var monos = features.OperaObject.GetComponents<MonoBehaviour>();
                    foreach (var item in monos)
                    {
                        item.hideFlags=HideFlags.None;
                    }
                    ShowOpera=true;
                }
            }
        }

        /// <summary>
        /// 操作状态
        /// </summary>
        public void OnOperaStatus()
        {
            if (features.operaType != features.beforeChange)
            {
                switch (features.beforeChange)
                {
                    case ObjectOperaType.无:
                        features.RemoveNone();
                        _noneController = null;
                        break;
                    case ObjectOperaType.能抓取:
                        features.RemoveCanGrab();
                        _cangrabController = null;
                        break;
                    case ObjectOperaType.物体自身旋转:
                        features.RemoveRotation();
                        _objectRatationController = null;
                        break;
                    case ObjectOperaType.摄像机围绕物体旋转:
                        features.RemoveCameraCenterObjectRotation();
                        _rotateAround = null;
                        break;
                    case ObjectOperaType.自定义:
                        features.RemoveCustomize();
                        _customizeController = null;
                        break;
                    case ObjectOperaType.物体式按钮:
                        features.RemoveObjectButton();
                        _objectButton = null;
                        break;
                    default:
                        break;
                }
                features.beforeChange = features.operaType;
            }

            switch (features.operaType)
            {
                case ObjectOperaType.无:
                    if (_noneController == null)
                        _noneController = features.AddNone();
                    break;
                case ObjectOperaType.能抓取:
                    if (_cangrabController == null)
                        _cangrabController = features.AddCanGrab();
                    break;
                case ObjectOperaType.物体自身旋转:
                    if (_objectRatationController == null)
                        _objectRatationController = features.AddSelfRotation();
                    break;
                case ObjectOperaType.摄像机围绕物体旋转:
                    if (_rotateAround == null)
                        _rotateAround = features.AddCameraCenterObjectRotation();
                    break;

                case ObjectOperaType.自定义:
                    if (_customizeController == null)
                    {
                        _customizeController = features.AddCustomize();
                    }
                    break;
                case ObjectOperaType.物体式按钮:
                    if (_objectButton==null)
                    {
                        _objectButton=features.AddObjectButton();
                    }
                    break;
                default:
                    break;
            }

        }

        /// <summary>
        /// 桌面吸附及限制下陷面板显示
        /// </summary>
        private void InspectorDeskLimit()
        {
            if (features.ActiveDeskLimit_)
            {
                _deskLimit = features.AddDeskLimit();
                if (_deskLimit == null) return;
                EditorGUILayout.BeginVertical();
                _deskLimit.limitObj = EditorGUILayout.ObjectField("    *被限制的物体", _deskLimit.limitObj, typeof(GameObject), true) as GameObject;
                if (_deskLimit.limitObj == null)
                    EditorGUILayout.HelpBox("请赋值被抓取物体本身", MessageType.None, false);
                _deskLimit.openAdsorption = EditorGUILayout.Toggle("    开启桌面吸附", _deskLimit.openAdsorption);
                _deskLimit.limitSink = EditorGUILayout.Toggle("    限制下陷到桌面下", _deskLimit.limitSink);
                _deskLimit.deskHeight = EditorGUILayout.FloatField("    桌面高度", _deskLimit.deskHeight);
                _deskLimit.deskDistance = EditorGUILayout.FloatField("    距离桌面多高开始吸附", _deskLimit.deskDistance);
                _deskLimit.boxCollider = EditorGUILayout.ObjectField("    用于空间查询的BoxCollider", _deskLimit.boxCollider, typeof(BoxCollider), true) as BoxCollider;
                if (_deskLimit.boxCollider == null)
                    EditorGUILayout.HelpBox("用于空间查询的BoxCollider，可以使用operaObject物体\n需包住物体的中心点。", MessageType.None, false);
                _deskLimit.autoExtremum = EditorGUILayout.Toggle("    自动计算网格极值，蒙皮网格可能不准确", _deskLimit.autoExtremum);
                if (!_deskLimit.autoExtremum)
                {
                    _deskLimit.minPoint = EditorGUILayout.ObjectField("    网格极小点", _deskLimit.minPoint, typeof(Transform), true) as Transform;
                    _deskLimit.maxPoint = EditorGUILayout.ObjectField("    网格极大点", _deskLimit.maxPoint, typeof(Transform), true) as Transform;
                }

                EditorGUILayout.EndVertical();
            }
            else
            {
                features.RemoveDeskLimit();
                _deskLimit = null;
            }
        }

        /// <summary>
        /// 空间限制面板显示
        /// </summary>
        private void InspectorSpaceLimit()
        {
            if (features.ActiveSpaceLimit)
            {
                _spaceLimit = features.AddSpaceLimit();
                if (_spaceLimit == null) return;
                EditorGUILayout.BeginVertical();

                _spaceLimit.limitObj = EditorGUILayout.ObjectField("    *被限制的物体",_spaceLimit.limitObj,typeof(GameObject),true) as GameObject;
                if (_spaceLimit.limitObj == null)
                    EditorGUILayout.HelpBox("请赋值被抓取物体本身",MessageType.None,false);
                EditorGUILayout.BeginHorizontal();
                _spaceLimit.topLimit = EditorGUILayout.Toggle("    *上边限制",_spaceLimit.topLimit);
                _spaceLimit.topOffset = EditorGUILayout.FloatField("    上偏移量",_spaceLimit.topOffset);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                _spaceLimit.bottomLimit = EditorGUILayout.Toggle("    *下边限制",_spaceLimit.bottomLimit);
                _spaceLimit.bottomOffset = EditorGUILayout.FloatField("    下偏移量",_spaceLimit.bottomOffset);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                _spaceLimit.leftLimit = EditorGUILayout.Toggle("    *左边限制",_spaceLimit.leftLimit);
                _spaceLimit.leftOffset = EditorGUILayout.FloatField("    左偏移量",_spaceLimit.leftOffset);
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                _spaceLimit.rightLimit = EditorGUILayout.Toggle("    *右边限制",_spaceLimit.rightLimit);
                _spaceLimit.rightOffset = EditorGUILayout.FloatField("    右偏移量",_spaceLimit.rightOffset);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.EndVertical();
            }
            else
            {
                features.RemoveSpaceLimit();
                _spaceLimit = null;
            }
        }

        /// <summary>
        /// 高亮面板显示
        /// </summary>
        private void InspectorHighLight()
        {
            if (features.ActiveHighlight)
            {
                _highlight = features.AddHighlight();
                _highlight.highlightType = (HighLightType)EditorGUILayout.EnumPopup("高亮类型：",_highlight.highlightType);

                if (_highlight.highlightType == HighLightType.Model)
                {
                    if (_highlight == null) return;
                    _highlight.highlightModel = EditorGUILayout.ObjectField("    *模型高亮：",_highlight.highlightModel,typeof(GameObject),true) as GameObject;
                    if (_highlight.highlightModel == null)
                        EditorGUILayout.HelpBox("高亮的物体，默认是自己...",MessageType.None);
                }
                else
                {
                    if (_highlight == null) return;

                    _highlight.highlightColor = FrameConfig.Config.highlightColor;

                    _highlight.grabColor = FrameConfig.Config.grabColor;
                }
            }
            else
            {
                features.RemoveHighlight();
                _highlight = null;
            }
        }

        /// <summary>
        /// 标签显示面板
        /// </summary>
        private void InspectorLabel()
        {
            if (features.ActiveLabel)
            {
                _labelController = features.AddLabel();
                if (_labelController == null) return;
                EditorGUILayout.BeginVertical();
                //标签参数
                _labelController.labelName = EditorGUILayout.TextField(new GUIContent("    *标签名字:","labelName"),_labelController.labelName);
                _labelController.type=(LabelType)EditorGUILayout.EnumPopup("    *标签类型:",_labelController.type);
                GUILayout.Space(10);

                GUILayout.Label("字体设置");

                _labelController.fontSize = EditorGUILayout.IntField(new GUIContent("    *大小","fontSize"),_labelController.fontSize);
                _labelController.color = EditorGUILayout.ColorField(new GUIContent("    *颜色","color"),_labelController.color);

                _labelController.fontStyle= (FontStyle)EditorGUILayout.EnumPopup(new GUIContent("    *风格","fontStyle"),_labelController.fontStyle);
                _labelController.useShadow=EditorGUILayout.Toggle(new GUIContent("    *阴影","useShadow"),_labelController.useShadow);
                _labelController.useOutline=EditorGUILayout.Toggle(new GUIContent("    *描边","useOutline"),_labelController.useOutline);
                //if (GUILayout.Button("转到标签",GUILayout.Width(60),GUILayout.Height(15)))
                //{
                //    Selection.activeObject=_labelController.label.gameObject;
                //}
                GUILayout.Space(10);
                GUILayout.Label("标签位置设置");
                //实时变化
                _labelController.labelSize = EditorGUILayout.Vector2Field(new GUIContent("    *标签的sizeDelta","labelSize"),_labelController.labelSize);

                _labelController.labelOffset = EditorGUILayout.Vector3Field(new GUIContent("    *世界坐标偏移量","labelOffset"),_labelController.labelOffset);
                _labelController.peakZreaZ = EditorGUILayout.Vector2Field(new GUIContent("    *离相机的距离范围内显示","peakZreaZ"),_labelController.peakZreaZ);

                _labelController.clearAreaZ = EditorGUILayout.Vector2Field(new GUIContent("    *在显示范围内的缩放","clearAreaZ"),_labelController.clearAreaZ);
                _labelController.label.OnUpdate();
                if (GUILayout.Button("在编辑器下更新标签位置",GUILayout.Width(200)))
                {
                    _labelController.label.LateUpdate();
                }
                EditorGUILayout.EndVertical();

            }
            else
            {
                features.RemoveLabel();
                _labelController = null;
            }
        }

        ///// <summary>
        ///// 标签显示面板
        ///// </summary>
        //private void InspectorShadow()
        //{
        //    ////if (features.ActiveShadow)
        //    ////{
        //    ////    _shadowController = features.AddShadow();
        //    ////    _shadowController.shadowType = (ShadowType)EditorGUILayout.EnumPopup("    ·虚影类型：",_shadowController.shadowType);
        //    ////    if (_shadowController.shadowType == ShadowType.Manual)
        //    ////    {
        //    ////        _shadowController.traModelNode = EditorGUILayout.ObjectField("    ·虚影模型：",_shadowController.traModelNode,typeof(Transform),true) as Transform;
        //    ////    }
        //    ////    //// //   _shadowController.Intension=EditorGUILayout.Slider("    ·虚影透明度：",_shadowController.Intension,0.1f,0.3f);
        //    ////    ////   // _shadowController.renderQueue = EditorGUILayout.IntField("    ·Shader渲染层级：",_shadowController.renderQueue);
        //    ////}
        //    ////else
        //    ////{
        //    ////    features.RemoveShadow();
        //    ////    _shadowController = null;
        //    ////}
        //}

        /// <summary>
        /// 无显示面板
        /// </summary>
        private void InspectorNone()
        {

        }
        /// <summary>
        /// 能抓取显示面板
        /// </summary>
        private void InspectorCanGrab()
        {
            if (_cangrabController == null) return;

            _cangrabController.grabObject = EditorGUILayout.ObjectField("    *被抓取物体",
                _cangrabController.grabObject,typeof(GameObject),true) as GameObject;
            if (_cangrabController.grabObject == null)
                EditorGUILayout.HelpBox("当射线照射到该物体时，赋予谁被抓取，不赋值默认为本身...",MessageType.None,false);
        }
        /// <summary>
        /// 物体自身旋转显示面板
        /// </summary>
        private void InspectorSelfRotation()
        {
            if (_objectRatationController == null) return;

            //grabObject
            _objectRatationController.grabObject = EditorGUILayout.ObjectField("    *被抓取物体",
                _objectRatationController.grabObject,typeof(GameObject),true) as GameObject;
            if (_objectRatationController.grabObject == null)
                EditorGUILayout.HelpBox("当射线照射到该物体时，赋予谁被抓取，不赋值默认为本身...",MessageType.None,false);

            EditorGUILayout.BeginVertical();
            //space
            _objectRatationController.space = (Space)EditorGUILayout.EnumPopup("    *空间坐标：",_objectRatationController.space);
            _objectRatationController.axisLimits = (AxisLimits)EditorGUILayout.EnumPopup("    *旋转轴：",_objectRatationController.axisLimits);
            //axisLimits
            //_objectRatationController.axisLimits = (AxisLimits)EditorGUILayout.EnumPopup("    *坐标系：",_objectRatationController.axisLimits);

            EditorGUILayout.EndVertical();

            EditorGUILayout.BeginHorizontal();
            //minAngle
            //maxAngle
            Vector2 minmax = EditorGUILayout.Vector2Field("    *旋转值(最小-最大):",
                new Vector2(_objectRatationController.minAngle,_objectRatationController.maxAngle));
            _objectRatationController.minAngle = minmax.x;
            _objectRatationController.maxAngle = minmax.y;


            EditorGUILayout.EndHorizontal();
            _objectRatationController.rotateSpeed=EditorGUILayout.FloatField("    *旋转速度：",_objectRatationController.rotateSpeed);

        }
        /// <summary>
        /// 摄像机围绕物体旋转显示面板
        /// </summary>
        private void InspectorCameraCenterObjectRotation()
        {
            if (_rotateAround == null) return;

            //grabObject
            _rotateAround.grabObject = EditorGUILayout.ObjectField("    *被抓取物体",
                _rotateAround.grabObject,typeof(GameObject),true) as GameObject;
            if (_rotateAround.grabObject == null)
                EditorGUILayout.HelpBox("当射线照射到该物体时，赋予谁被抓取，不赋值默认为本身...",MessageType.None,false);
            _rotateAround.speed=EditorGUILayout.FloatField("    *旋转速度：",_rotateAround.speed);
            //maxAngle
            _rotateAround.leftAndRight = EditorGUILayout.Vector2Field("    *左右旋转范围(最小-最大):",
                _rotateAround.leftAndRight);
            _rotateAround.upAndDown = EditorGUILayout.Vector2Field("    *上下旋转范围(最小-最大):",
               _rotateAround.upAndDown);
        }


        /// <summary>
        /// 物体式按钮
        /// </summary>
        private void InspectorObjectButton()
        {
            if (_objectButton==null) return;
            _objectButton.grabObject = EditorGUILayout.ObjectField("    *被抓取物体",
               _objectButton.grabObject,typeof(GameObject),true) as GameObject;
            GUILayout.Space(10);
            GUILayout.Box("需要通过FeaturesObjectController对象访问ObjectButton对象，然后注册onDown、onPress、OnFreed事件",GUILayout.Width(350));
        }



        /// <summary>
        /// 限制移动显示面板
        /// </summary>
        private void InspectorLimitMove()
        {
            if (features.ActiveLimitMove)
            {
                _limitMoveController = features.AddLimitMove();

                if (_limitMoveController == null) return;

                //grabObject
                _limitMoveController.grabObject = EditorGUILayout.ObjectField("    *被抓取物体",
                    _limitMoveController.grabObject,typeof(GameObject),true) as GameObject;
                if (_limitMoveController.grabObject == null)
                    EditorGUILayout.HelpBox("当射线照射到该物体时，赋予谁被抓取，不赋值默认为本身...",MessageType.None,false);

                _limitMoveController.type=(ProcessType)EditorGUILayout.EnumPopup("    *选择更新事件",_limitMoveController.type);

                _limitMoveController.isLocal= EditorGUILayout.Toggle("      *使用本地坐标：",_limitMoveController.isLocal);
                EditorGUILayout.Space();
                _limitMoveController.activeX = EditorGUILayout.Toggle("      *开启X轴限制：",_limitMoveController.activeX);
                if (_limitMoveController.activeX)
                {
                    _limitMoveController.xRange=EditorGUILayout.Vector2Field("  *x值的限制范围",_limitMoveController.xRange);
                }
                EditorGUILayout.Space();
                _limitMoveController.activeY = EditorGUILayout.Toggle("      *开启Y轴限制：",_limitMoveController.activeY);
                if (_limitMoveController.activeY)
                {
                    _limitMoveController.yRange     = EditorGUILayout.Vector2Field("  *初始y值的限制范围",_limitMoveController.yRange);
                    _limitMoveController.minYCurve  = EditorGUILayout.CurveField("  *在x范围內的最小Y值",_limitMoveController.minYCurve);
                    _limitMoveController.maxYCurve  = EditorGUILayout.CurveField("  *在x范围內的最大Y值",_limitMoveController.maxYCurve);
                    if (GUILayout.Button("重置曲线",GUILayout.Width(60),GUILayout.Height(20)))
                    {
                        Debug.Log("生成Y曲线");
                        _limitMoveController.minYCurve=new AnimationCurve();
                        _limitMoveController.maxYCurve=new AnimationCurve();
                        _limitMoveController.AddKeyGroup(_limitMoveController.yRange.x,_limitMoveController.yRange.y,AxisLimits.Y,0);
                        _limitMoveController.AddKeyGroup(_limitMoveController.yRange.x,_limitMoveController.yRange.y,AxisLimits.Y,1);
                    }
                }
                EditorGUILayout.Space();
                _limitMoveController.activeZ        = EditorGUILayout.Toggle("      *开启Z轴限制：",_limitMoveController.activeZ);
                if (_limitMoveController.activeZ)
                {
                    _limitMoveController.zRange     = EditorGUILayout.Vector2Field("  *初始z值的限制范围",_limitMoveController.zRange);
                    _limitMoveController.minZCurve  = EditorGUILayout.CurveField("  *在x范围內的最小z值",_limitMoveController.minZCurve);
                    _limitMoveController.maxZCurve  = EditorGUILayout.CurveField("  *在x范围內的最大z值",_limitMoveController.maxZCurve);
                    if (GUILayout.Button("重置曲线",GUILayout.Width(60),GUILayout.Height(20)))
                    {
                        Debug.Log("生成Z曲线");
                        _limitMoveController.minZCurve=new AnimationCurve();
                        _limitMoveController.maxZCurve=new AnimationCurve();
                        _limitMoveController.AddKeyGroup(_limitMoveController.zRange.x,_limitMoveController.zRange.y,AxisLimits.Z,0);
                        _limitMoveController.AddKeyGroup(_limitMoveController.zRange.x,_limitMoveController.zRange.y,AxisLimits.Z,1);
                    }
                }
            }
            else
            {
                features.RemoveLimitMove();
                _limitMoveController = null;
            }
        }

        private void InspectorCustomize()
        {
            if (_customizeController == null) return;

            //grabObject
            _customizeController.grabObject = EditorGUILayout.ObjectField("    *被抓取物体",
                _customizeController.grabObject,typeof(GameObject),true) as GameObject;

            if (_customizeController.grabObject == null)
                EditorGUILayout.HelpBox("当射线照射到该物体时，赋予谁被抓取，不赋值默认为本身...",MessageType.None,false);

            GUILayout.Space(10);

            GUILayout.Box("需要通过FeaturesObjectController对象访问Customize对象，然后注册OnOnCustomizeUpdate事件");

        }

        /// <summary>
        /// 距离创建面板
        /// </summary>
        private void InspectorDistance()
        {
            GUILayout.Space(20);
            EditorGUILayout.BeginVertical("box");

            distanceName = EditorGUILayout.TextField("距离物体名称：",distanceName);

            EditorGUILayout.BeginHorizontal();

            GUILayout.Space(5);
            if (GUILayout.Button("创建【默认距离】检测点",GUILayout.Width(150)))
            {

                CreateDistanceInteraction<DistanceInteraction>(FindDistanceParent());

                Dismantling();
            }
            GUILayout.Space(5);

            if (GUILayout.Button("创建【仪器距离】检测点",GUILayout.Width(150)))
            {
                CreateDistanceInteraction<InteractionEquipment>(FindDistanceParent());

                Dismantling();
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(5);
            GUILayout.Label("该功能控制端下的距离检测点【只针对distanceParent下的】");
            for (int i = 0; i < distanceSums.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                for (int j = 0; j < distanceSums[i].Count; j++)
                {
                    if (GUILayout.Button(distanceSums[i][j].name,GUILayout.Width(170)))
                    {
                        Selection.activeGameObject = distanceSums[i][j].gameObject;
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();
        }

        public Transform FindDistanceParent()
        {
            Transform parent = features.transform.Find("distanceParent");
            if (parent == null)
            {
                parent = new GameObject("distanceParent").transform;
                parent.SetParent(features.transform);
                parent.localPosition = Vector3.zero;
                parent.localRotation = Quaternion.identity;
                parent.localScale = Vector3.one;
            }

            return parent;
        }

        void CreateDistanceInteraction<T>(Transform parent) where T : DistanceInteraction
        {
            GameObject distanceObject = new GameObject("distanceObject_" + distanceName);
            var distance = distanceObject.AddComponent<T>();
            distanceObject.transform.SetParent(parent);
            distanceObject.transform.localPosition = Vector3.zero;

            distances.Add(distance);

            Selection.activeGameObject = distanceObject;

            distance.distanceData.TagID = distanceName;
        }
    }
}


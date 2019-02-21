using MagiCloud;
using MagiCloud.Features;
using MagiCloud.UIFrame;
using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.KGUI
{
    /// <summary>
    /// KGUI标签管理
    /// </summary>
    [RequireComponent(typeof(UI_Embed))]
    public class KGUI_LabelController :MonoBehaviour
    {
        public Color defaultTextColor;
        public Font defaultFont;
        public int defalutFontSize;
        public Sprite defaultSprite;
        [HideInInspector]
        public Dictionary<KGUI_Label,GameObject> labels;

        private static KGUI_LabelController instance;
        /// <summary>
        /// 单例
        /// </summary>
        public static KGUI_LabelController Instance
        {
            get
            {
                if (instance==null)
                {
                    KGUI_LabelController labelCon = null;

                    labelCon = FindObjectOfType<KGUI_LabelController>();// 
                    //   if (canvasTransform==null)
                    //     throw new System.Exception("请先配置KGUI");
                    if (labelCon==null)
                    {
                        GameObject obj = new GameObject("LabelController",typeof(RectTransform),typeof(KGUI_LabelController));
                        //trans.SetParent(canvasTransform);
                        //UI_Base ui_base = obj.GetComponent<UI_Base>();
                        //ui_base.type=UIType.Canvas;
                        instance = obj.GetComponent<KGUI_LabelController>();
                    }
                    else
                    {
                        instance=labelCon;
                    }
                    instance.hideFlags=HideFlags.HideInInspector;
                    instance.Initialize();
                }
                return instance;
            }
        }

        private void SetJoinUI()
        {
            Transform canvasTransform = UIManager.Instance.Canvas.transform;
            if (canvasTransform!=null)
            {
                transform.SetParent(canvasTransform);
                transform.localPosition=Vector3.zero;
                transform.localScale=Vector3.one;
            }
        }

        public static GameObject currentSelectedObj;
        private void Awake()
        {
            Initialize();
        }
     
        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            SetJoinUI();
            defaultSprite=FrameConfig.Config.labelBg;
            gameObject.GetComponent<UI_Action>().actionType=ActionType.Facede;
            gameObject.GetComponent<UI_Embed>().TagID="LabelController";
            gameObject.SetActive(true);
            // transform.localPosition=Vector3.zero;
            //transform.localScale=Vector3.one;
            //    defaultTextColor =FrameConfig.Config.initLabelColor;
            defaultFont=FrameConfig.Config.labelFont?? Font.CreateDynamicFontFromOSFont("msyh",24);
            defalutFontSize=FrameConfig.Config.initLabelFontSize;
            defaultTextColor=FrameConfig.Config.initLabelColor;
            labels =new Dictionary<KGUI_Label,GameObject>();

        }

        public void SetAllFontSize(int size = 24)
        {

            if (labels==null) return;
            foreach (var item in labels)
            {
                item.Key.SetFontSize(size);
            }
        }

        public void SetAllFontColor(Color color = default(Color))
        {
            if (labels==null) return;
            foreach (var item in labels)
            {
                item.Key.SetFontColor(color);
            }
        }

        public void SetAllLabelBG(Sprite sprite = null)
        {
            if (labels==null) return;
            foreach (var item in labels)
            {
                item.Key.SetLabelBG(sprite);
            }
        }

        /// <summary>
        /// 获取标签
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public KGUI_Label GetLabel(LabelData data)
        {
            KGUI_Label label = data.label;
            if (label==null) label=CreatLabel(data);
            if (labels.ContainsKey(data.label)&&labels[label]!=data.appertaining)
                label=CreatLabel(data);
            label.SetLabel(data);
            if (!labels.ContainsKey(label))
                labels.Add(label,data.appertaining);
            return label;
        }

        /// <summary>
        /// 检验数据是否存在于缓存中
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CheckInCache(LabelData data)
        {
            foreach (var item in labels)
            {
                if (item.Key.id==data.id)
                {
                    data.label=item.Key;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 创建标签
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private KGUI_Label CreatLabel(LabelData data)
        {
            KGUI_Label label;
            if (string.IsNullOrEmpty(data.labelName))
            {
                data.labelName=data.appertaining.name;
                DefaultLabelData(data);
            }

            GameObject obj = new GameObject(data.labelName,typeof(RectTransform));
            obj.transform.SetParent(transform);
            label=obj.AddComponent<KGUI_Label>();
            label.hideFlags=HideFlags.HideInInspector;
            data.label=label;
            label.Initialize(defaultFont,data);
            return label;
        }

        public void DefaultLabelData(LabelData data)
        {
            data.labelSize=new Vector2(170,50);// Vector2.one*100;
            data.clearAreaZ=Vector2.one;
            data.peakZreaZ=new Vector2(0,float.MaxValue);
            data.fontSize=defalutFontSize;
            data.labelBackground=defaultSprite;
            data.color=defaultTextColor;
        }

        /// <summary>
        /// 通过数据销毁标签
        /// </summary>
        /// <param name="data"></param>
        public void DestroyByLabelController(LabelData data)
        {
            if (labels == null) return;

            if (labels.ContainsKey(data.label))
            {
                KGUI_Label temp = data.label;
                labels.Remove(temp);
                data.label=null;
                GameObject tempObj = temp.gameObject;
                temp.Destroy();
                if (tempObj!=null)
                    Object.DestroyImmediate(tempObj);
            }
            if (labels.Count<1)
            {
                DestroyImmediate(this.gameObject);
            }
        }

        /// <summary>
        /// 当标签实体被删除时销毁标签
        /// </summary>
        /// <param name="label"></param>
        public void DestroyByLabel(KGUI_Label label)
        {
            if (labels.ContainsKey(label))
            {
                labels.Remove(label);
            }
        }


    }
}

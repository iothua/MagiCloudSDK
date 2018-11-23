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
        public List<KGUI_Label> labels;

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
        private void Start()
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
            defaultTextColor =FrameConfig.Config.labelColor;
            defaultFont=FrameConfig.Config.labelFont?? Font.CreateDynamicFontFromOSFont("msyh",24);
            defalutFontSize=FrameConfig.Config.labelFontSize;
            defaultTextColor=FrameConfig.Config.labelColor;
            labels =new List<KGUI_Label>();

        }

        public void SetAllFontSize(int size = 24)
        {

            if (labels==null) return;
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].SetFontSize(size);
            }
        }

        public void SetAllFontColor(Color color = default(Color))
        {
            if (labels==null) return;
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].SetFontColor(color);
            }
        }

        public void SetAllLabelBG(Sprite sprite = null)
        {
            if (labels==null) return;
            for (int i = 0; i < labels.Count; i++)
            {
                labels[i].SetLabelBG(sprite);
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

            label.SetLabel(data);

            if (!labels.Contains(label))
                labels.Add(label);
            return label;
        }

        /// <summary>
        /// 检验数据是否存在于缓存中
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool CheckInCache(LabelData data)
        {
            for (int i = 0; i < labels.Count; i++)
            {
                if (labels[i].id==data.id)
                {
                    data.label=labels[i];
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
            data.labelSize=Vector2.one*100;
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

            if (labels.Contains(data.label))
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
            if (labels.Contains(label))
            {
                labels.Remove(label);
            }
        }


    }
}

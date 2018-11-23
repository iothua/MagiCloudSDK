using UnityEngine;
using HighlightingSystem;
using System.Linq;

namespace MagiCloud.Features
{
    /// <summary>
    /// 物品高亮控制
    /// </summary>
    [RequireComponent(typeof(BoxCollider))]
    public class HighlightObject : MonoBehaviour
    {
        public HighLightType highlightType = HighLightType.Shader;                 //高亮类型

        [Header("高亮模型,如果高亮控制端是Shader则不用配置")]
        public GameObject highlightModel;                   //model 高亮模型

        private bool seeThrough = true;                     //shader 是否穿透
        public Color highlightColor = Color.cyan;           //shader 颜色
        public Color grabColor = Color.cyan;

        protected Highlighter h;                            //高亮实现
        private bool _isHighlight = true;

        
        [Header("指定该物体下的所有物体高亮，默认为父物体")]
        public Transform highLightTransform;                //高亮位置

        void Start()
        {
            OnAddHighLight();
        }

        void OnAddHighLight()
        {
            if (highLightTransform == null)
                highLightTransform = transform.parent;

            h = highLightTransform.GetComponent<Highlighter>() ?? highLightTransform.gameObject.AddComponent<Highlighter>();
        }

        public void OnRemoveHighLight()
        {
            Destroy(h);
        }

        protected virtual void OnEnable()
        {
            if (h == null) return;
            h.seeThrough = seeThrough;
        }

        private void OnDestroy()
        {

        }

        protected virtual void OnValidate()
        {
            if (h != null)
            {
                h.seeThrough = seeThrough;
            }
        }

        /// <summary>
        /// 显示高亮
        /// </summary>
        public void ShowHighLight(bool isGrab = false)
        {
            //获取到子物体下的所有高亮脚本

            var highlights = highLightTransform.GetComponentsInChildren<HighlightObject>().Where(obj => obj != this);

            switch (highlightType)
            {
                case HighLightType.Model:

                    if (highlightModel != null && highlightModel.activeSelf == false && _isHighlight)
                        highlightModel.SetActive(true);

                    //显示子物体下的所有高亮
                    foreach (var item in highlights)
                    {
                        if (item.highlightModel != null && item.highlightModel.activeSelf == false && item._isHighlight)
                            item.highlightModel.SetActive(true);
                    }

                    break;
                case HighLightType.Shader:

                    
                    //移除子物体下的所有高亮
                    foreach (var item in highlights)
                    {
                        //移除子物体
                        item.OnRemoveHighLight();
                    }

                    if (h == null)
                        OnAddHighLight();

                    //脚本重置下
                    h.enabled = false;
                    h.enabled = true;

                    //h.ConstantOnImmediate(isGrab ? grabColor : highlightColor);
                    h.ConstantOnImmediate(highlightColor);

                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// 关闭高亮
        /// </summary>
        public void HideHighLight()
        {
            var highlights = highLightTransform.GetComponentsInChildren<HighlightObject>().Where(obj => obj != this);

            switch (highlightType)
            {
                case HighLightType.Model:
                    if (highlightModel != null && highlightModel.activeSelf == true)
                        highlightModel.SetActive(false);

                    //显示子物体下的所有高亮
                    foreach (var item in highlights)
                    {
                        if (item.highlightModel != null && item.highlightModel.activeSelf)
                            item.highlightModel.SetActive(false);
                    }

                    break;
                case HighLightType.Shader:
                    h.ConstantOffImmediate();
                    break;
                default:
                    break;
            }
        }
    }

}

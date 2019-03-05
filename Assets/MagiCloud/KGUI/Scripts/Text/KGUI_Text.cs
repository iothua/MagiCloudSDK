using TMPro;
using UnityEngine;

namespace MagiCloud.KGUI
{

    [RequireComponent(typeof(TextMeshProUGUI))]
    [ExecuteInEditMode]
    public class KGUI_Text :KGUI_Base
    {
        private TextMeshProUGUI tmp;

        //public bool IsEnable
        //{
        //    get { return enabled; }
        //    set
        //    {
        //        this.enabled=value;
        //        TextMeshPro.enabled=value;
        //    }
        //}

        public TextMeshProUGUI TextMeshPro
        {
            get
            {
                return GetComponent<TextMeshProUGUI>();

            }
        }
        public TextAlignmentOptions Alignment
        {
            get { return TextMeshPro.alignment; }
            set { TextMeshPro.alignment=value; }
        }


        public TMP_FontAsset FontAsset
        {
            get { return TextMeshPro.font; }
            set { TextMeshPro.font=value; }
        }

        public Material TMPMaterial
        {
            get { return TextMeshPro.materialForRendering; }
        }

        public string Text
        {
            get { return TextMeshPro.text; }
            set
            {
                TextMeshPro.text=value;
            }
        }

        public Color MainColor
        {
            get { return TextMeshPro.color; }
            set { TextMeshPro.color=value; }
        }


        #region Font
        public FontStyles FontStyle
        {
            get { return TextMeshPro.fontStyle; }
            set { TextMeshPro.fontStyle=value; }
        }
        public bool IsAutoFontSize
        {
            get { return TextMeshPro.enableAutoSizing; }
            set { TextMeshPro.enableAutoSizing=value; }
        }
        public float FontSize
        {
            get { return TextMeshPro.fontSize; }
            set { TextMeshPro.fontSize=value; }
        }
        #endregion

        #region Outline

        public bool UseOutline
        {
            get { return TMPMaterial.IsKeywordEnabled(ShaderUtilities.Keyword_Outline); }
            set
            {
                if (value)
                    TMPMaterial.EnableKeyword(ShaderUtilities.Keyword_Outline);
                else
                    TMPMaterial.DisableKeyword(ShaderUtilities.Keyword_Outline);
            }
        }

        public Color OutlineColor
        {
            get { return TextMeshPro.outlineColor; }
            set { TextMeshPro.outlineColor=value; }
        }

        public float OutlineWidth
        {
            get { return TextMeshPro.outlineWidth; }
            set { TextMeshPro.outlineWidth=value; }
        }
        #endregion

        #region Shadow


        public bool UseShadow
        {
            get { return TMPMaterial.IsKeywordEnabled(ShaderUtilities.Keyword_Underlay); }
            set
            {
                if (value)
                    TMPMaterial.EnableKeyword(ShaderUtilities.Keyword_Underlay);
                else
                    TMPMaterial.DisableKeyword(ShaderUtilities.Keyword_Underlay);
            }
        }

        public Color ShadowColor
        {
            get { return TMPMaterial.GetColor(ShaderUtilities.ID_UnderlayColor); }
            set { TMPMaterial.SetColor(ShaderUtilities.ID_UnderlayColor,value); }
        }
        public float ShadowOffsetX
        {
            get { return TMPMaterial.GetFloat(ShaderUtilities.ID_UnderlayOffsetX); }
            set { TMPMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetX,value); }
        }

        public float ShadowOffsetY
        {
            get { return TMPMaterial.GetFloat(ShaderUtilities.ID_UnderlayOffsetY); }
            set { TMPMaterial.SetFloat(ShaderUtilities.ID_UnderlayOffsetY,value); }
        }
        public float ShadowDilate
        {
            get { return TMPMaterial.GetFloat(ShaderUtilities.ID_UnderlayDilate); }
            set { TMPMaterial.SetFloat(ShaderUtilities.ID_UnderlayDilate,value); }
        }
        public float ShadowSofrness
        {
            get { return TMPMaterial.GetFloat(ShaderUtilities.ID_UnderlaySoftness); }
            set { TMPMaterial.SetFloat(ShaderUtilities.ID_UnderlaySoftness,value); }
        }
        #endregion


        public TMP_SpriteAsset SpriteAtlas
        {
            get { return TextMeshPro.spriteAsset; }
            set
            {
                TextMeshPro.spriteAsset=value;
            }
        }


        private void OnEnable()
        {
            if (TextMeshPro!=null)
            {

                TextMeshPro.enabled=true;
                TextMeshPro.hideFlags=HideFlags.HideInInspector;
            }
            if (TMPMaterial!=null)
                TMPMaterial.hideFlags=HideFlags.HideInInspector;
        }
        private void OnDisable()
        {
            if (TextMeshPro!=null)
                TextMeshPro.enabled=false;
        }
    }
}
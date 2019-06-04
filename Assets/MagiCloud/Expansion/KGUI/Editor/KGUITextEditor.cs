using TMPro;
using TMPro.SpriteAssetUtilities;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

namespace MagiCloud.KGUI
{
    [CustomEditor(typeof(KGUI_Text),true)]
    [CanEditMultipleObjects]
    public class KGUITextEditor :Editor
    {
        private void OnEnable()
        {

        }
        public override void OnInspectorGUI()
        {
            var kguiText = target as KGUI_Text;
            //kguiText.IsEnable=EditorGUILayout.Toggle("启用",kguiText.IsEnable);
            kguiText.Order=EditorGUILayout.IntField("排序",kguiText.Order);
            EditorGUILayout.LabelField("内容：");
            kguiText.Text=EditorGUILayout.TextArea(kguiText.Text,GUILayout.MinHeight(60));
            kguiText.FontStyle=(FontStyles)EditorGUILayout.EnumFlagsField("字体风格",kguiText.FontStyle);
            kguiText.IsAutoFontSize=EditorGUILayout.Toggle("自动字体大小",kguiText.IsAutoFontSize);
            if (!kguiText.IsAutoFontSize)
                kguiText.FontSize=EditorGUILayout.FloatField("      字体大小",kguiText.FontSize);

            kguiText.MainColor=EditorGUILayout.ColorField("颜色",kguiText.MainColor);
            kguiText.Alignment=(TextAlignmentOptions)EditorGUILayout.EnumPopup("锚点",kguiText.Alignment);
            //outline
            kguiText.UseOutline=EditorGUILayout.Toggle("描边",kguiText.UseOutline);
            if (kguiText.UseOutline)
            {
                EditorGUILayout.BeginVertical("box");
                kguiText.OutlineColor=EditorGUILayout.ColorField("      描边颜色",kguiText.OutlineColor);
                kguiText.OutlineWidth=EditorGUILayout.Slider("      描边宽度",kguiText.OutlineWidth,0f,1f);
                EditorGUILayout.EndVertical();
            }

            //shadow
            kguiText.UseShadow=EditorGUILayout.Toggle("阴影",kguiText.UseShadow);
            if (kguiText.UseShadow)
            {
                EditorGUILayout.BeginVertical("box");
                kguiText.ShadowColor=EditorGUILayout.ColorField("      描边颜色",kguiText.ShadowColor);
                kguiText.ShadowOffsetX=EditorGUILayout.Slider("      x偏移量",kguiText.ShadowOffsetX,0f,1f);
                kguiText.ShadowOffsetY=EditorGUILayout.Slider("      y偏移量",kguiText.ShadowOffsetY,0f,1f);
                kguiText.ShadowDilate=EditorGUILayout.Slider("      扩张",kguiText.ShadowDilate,0f,1f);
                kguiText.ShadowSofrness=EditorGUILayout.Slider("      柔和",kguiText.ShadowSofrness,0f,1f);
                EditorGUILayout.EndVertical();
            }
            kguiText.FontAsset=EditorGUILayout.ObjectField("字符集",kguiText.FontAsset,typeof(TMP_FontAsset),false) as TMP_FontAsset;
            kguiText.SpriteAtlas=EditorGUILayout.ObjectField("图集",kguiText.SpriteAtlas,typeof(TMP_SpriteAsset),false) as TMP_SpriteAsset;
        }

    }

}
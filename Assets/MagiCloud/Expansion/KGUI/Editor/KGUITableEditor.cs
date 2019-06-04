using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

namespace MagiCloud.KGUI
{

    public enum MergeType
    {
        /// <summary>
        /// 以行为单位，合并列
        /// </summary>
        Row,
        /// <summary>
        /// 以列为单位，合并行
        /// </summary>
        Cloumn
    }

    /// <summary>
    /// KGUI表格
    /// </summary>
    [CustomEditor(typeof(KGUI_Table))]
    [CanEditMultipleObjects]
    public class KGUITableEditor : Editor
    {
        private GUIStyle style;

        private GUIStyle cellStyle;

        public SerializedProperty cellObject;
        public SerializedProperty rowObject;

        public SerializedProperty rowParent;

        public SerializedProperty Rows;
        public SerializedProperty Merges;

        public SerializedProperty cellBackground;

        private KGUI_Table table;


        private bool IsClick = false;
        private string text = string.Empty;
        private KGUI_TableCell _cell = null;

        private float columnWidth;//列宽
        private float rowHeight;

        private MergeType mergeType = MergeType.Row;
        private int value, startValue, endValue;


        private void OnEnable()
        {
            table = serializedObject.targetObject as KGUI_Table;

            cellObject = serializedObject.FindProperty("cellObject");
            rowObject = serializedObject.FindProperty("rowObject");
            rowParent = serializedObject.FindProperty("rowParent");
            Rows = serializedObject.FindProperty("Rows");
            Merges = serializedObject.FindProperty("Merges");

            cellBackground = serializedObject.FindProperty("cellBackground");
        }

        public override void OnInspectorGUI()
        {
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.name);
                style.normal.textColor = GUI.skin.label.normal.textColor;
                style.fontStyle = FontStyle.Bold;
                style.alignment = TextAnchor.UpperLeft;
            }

            if (cellStyle == null)
            {
                cellStyle = new GUIStyle(GUI.skin.box);
                cellStyle.normal.textColor = Color.white;
                cellStyle.alignment = TextAnchor.MiddleCenter;
            }

            GUILayout.Space(10);

            table.TableName = EditorGUILayout.TextField("表格名称：", table.TableName);

            table.spacing = EditorGUILayout.FloatField("表格间距：", table.spacing);
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();

            table.Ranks.x = EditorGUILayout.IntField("行数：" , table.Ranks.x);
            table.Ranks.y = EditorGUILayout.IntField("列数：", table.Ranks.y);
            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            table.cellSize.x = EditorGUILayout.FloatField("宽：", table.cellSize.x);
            table.cellSize.y = EditorGUILayout.FloatField("高：", table.cellSize.y);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("生成表格", GUILayout.Width(100), GUILayout.Height(25)))
            {
                //生成表格
                table.CreateTable();
            }

            GUILayout.Space(20);

            EditorGUILayout.EndVertical();

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("表格样式如下：");

            EditorGUILayout.BeginVertical();

            //绘制表格
            for (int i = 0; i < table.Rows.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();

                //每一行的开始，绘制一个很小的按钮
                
                //每行都绘制出来
                for (int j = 0; j < table.Rows[i].Cells.Count; j++)
                {
                    KGUI_TableCell cell = table.Rows[i].Cells[j];

                    if (cell.IsHide)
                    {
                        GUILayout.Box(cell.Position.ToString(), GUILayout.Width(100), GUILayout.Height(20));
                    }
                    else
                    {
                        if (GUILayout.Button(cell.Position.ToString(), GUILayout.Width(100), GUILayout.Height(20)))
                        {
                            KGUI_Keyboard.Keybd_event(13, 0, 0, 0);
                            columnWidth = cell.Size.x;
                            rowHeight = cell.Size.y;

                            _cell = cell;
                            IsClick = true;
                        }
                    }
                    

                    GUILayout.Space(0);
                }

                GUILayout.Space(0);

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndVertical();

            GUILayout.Space(20);

            EditorGUILayout.BeginVertical();

            if (IsClick && _cell != null)
            {

                EditorGUI.BeginChangeCheck();

                EditorGUILayout.LabelField("坐标：", _cell.Position.ToString());
                EditorGUILayout.LabelField("大小：", _cell.Size.ToString());

                text = EditorGUILayout.TextField("文本信息：", _cell.GetCell());

                _cell.SetCell(text);

                GUILayout.Space(10);

                columnWidth = EditorGUILayout.FloatField("列宽：", columnWidth);

                if (GUILayout.Button("设置列宽"))
                {
                    //设置列宽
                    table.SetColumnWidth(_cell, columnWidth);
                }

                GUILayout.Space(10);

                rowHeight = EditorGUILayout.FloatField("行高：", rowHeight);
                if (GUILayout.Button("设置行高"))
                {
                    table.SetRowHeight(_cell, rowHeight);
                }

                EditorGUI.EndChangeCheck();
            }

            GUILayout.Space(20);

            EditorGUILayout.EndVertical();

            EditorGUILayout.LabelField("合并单元格：");

            mergeType = (MergeType)EditorGUILayout.EnumPopup("合并类型：", mergeType);
            switch (mergeType)
            {
                case MergeType.Row:

                    value = EditorGUILayout.IntField("Row：", value);

                    startValue = EditorGUILayout.IntField("startCloumn：" , startValue);
                    endValue = EditorGUILayout.IntField("endCloumn：", endValue);

                    if (GUILayout.Button("合并", GUILayout.Width(100), GUILayout.Height(22)))
                    {
                        //设置列宽
                        table.MergeCloumn(value, startValue, endValue);
                    }

                    break;
                case MergeType.Cloumn:

                    value = EditorGUILayout.IntField("Cloumn：", value);

                    startValue = EditorGUILayout.IntField("startRow：", startValue);
                    endValue = EditorGUILayout.IntField("endRow：", endValue);

                    if (GUILayout.Button("合并", GUILayout.Width(150), GUILayout.Height(25)))
                    {
                        //设置列宽
                        table.MergeRow(value, startValue, endValue);
                    }
                    break;
                default:
                    break;
            }

            GUILayout.Space(20);

            table.textColor = EditorGUILayout.ColorField("单元格字体颜色：", table.textColor);
            EditorGUILayout.PropertyField(cellBackground, new GUIContent("背景纹理：", "为Null时，则单元格没有背景"), true, null);
            table.FontSize = EditorGUILayout.IntField("单元格字号：", table.FontSize);

            if (GUILayout.Button("更新单元格信息", GUILayout.Width(150), GUILayout.Height(22)))
            {
                table.UpdateCell();
            }

            GUILayout.Space(20);

            EditorGUILayout.PropertyField(Rows, true, null);
            EditorGUILayout.PropertyField(cellObject, true, null);
            EditorGUILayout.PropertyField(rowObject, true, null);
            EditorGUILayout.PropertyField(rowParent, true, null);

            serializedObject.ApplyModifiedProperties();
        }
    }
}

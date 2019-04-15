using System;
using UnityEngine;
using System.Collections.Generic;

namespace MagiCloud
{
    /// <summary>
    /// 操作模式的类型
    /// </summary>
    [Flags]
    public enum OperateModeType
    {
        /// <summary>
        /// 移动模式
        /// </summary>
        Move = 1,
        /// <summary>
        /// 旋转模式
        /// </summary>
        Rotate = 2,
        /// <summary>
        /// 缩放模式
        /// </summary>
        Zoom = 4,
        /// <summary>
        /// 工具模式
        /// </summary>
        Tool = 8
    }


    /// <summary>
    /// 模式切换管理
    /// 思路：
    /// 不同的模式开启不同的功能
    ///     1）在操作模式中，不可进行摄像机的旋转和缩放操作
    ///     2）在观察模式中，不可进行物体的抓取（高亮、标签等等），不可进行UI的操作。
    ///     3）在工具模式中，UI不可操作，物体不可抓取。只能进行图片的操作。
    /// 
    /// 
    /// </summary>
    public class MSwitchManager 
    {

        private static OperateModeType currentMode = OperateModeType.Move; //当前模式

        private static List<Action<OperateModeType>> Actions = new List<Action<OperateModeType>>();

        /// <summary>
        /// 当前模式
        /// </summary>
        public static OperateModeType CurrentMode {

            get {
                return currentMode;
            }
            set {

                if ((value & ActiveMode) == 0) return; //如果不存在这个组合中，则没必要存在了

                currentMode = value;

                SendListener(currentMode);
            }
        }

        /// <summary>
        /// 激活模式
        /// </summary>
        public static OperateModeType ActiveMode { get; private set; }

        /// <summary>
        /// 初始化激活的模式
        /// </summary>
        /// <param name="modeType"></param>
        public static void OnInitializeMode(OperateModeType modeType,OperateModeType defaultMode = OperateModeType.Move)
        {
            ActiveMode = modeType | OperateModeType.Tool; //添加工具
            CurrentMode = defaultMode;

            //UITool.UIToolManager magiCloudTool = GameObject.FindObjectOfType<UITool.UIToolManager>();
            //if (magiCloudTool == null)
            //{
            //    var toolObject = Resources.Load<GameObject>("MagiCloudTools");
            //    if (toolObject == null) throw new Exception("MagiCloudTools工具为Null，检查资源是否存在此预制物体");

            //    magiCloudTool = GameObject.Instantiate(toolObject).GetComponentInChildren<UITool.UIToolManager>();
            //}

          //  magiCloudTool.OnInitialize();
        }

        public static void AddListener(Action<OperateModeType> action)
        {
            if (IsHandler(action)) return;

            Actions.Add(action);
        }

        private static bool IsHandler(Action<OperateModeType> action)
        {
            return Actions.Contains(action);
        }

        public static void RemoveListener(Action<OperateModeType> action)
        {
            if (!IsHandler(action)) return;

            Actions.Remove(action);
        }

        public static void RemoveListenerAll()
        {
            Actions.Clear();
        }

        private static void SendListener(OperateModeType modeType)
        {

            for (int i = 0; i < Actions.Count; i++)
            {
                Actions[i].Invoke(modeType);
            }
        }
    }
}

using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace MagiCloud.UIFrame
{
    /// <summary>
    /// 动作数据
    /// </summary>
    public struct ActionData
    {
        public UI_Action UI;
        public Action action;

        public Action start, end;

        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete {
            get {
                if (UI == null) return true;

                return UI.IsComplete;
            }
        }

        public ActionData(UI_Action ui, Action action, Action start = default(Action), Action end = default(Action))
        {
            this.UI = ui;
            this.action = action;
            this.start = start;
            this.end = end;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void Excute()
        {
            UI.start = start;
            UI.end = end;

            action.Invoke();

        }
    }

    /// <summary>
    /// UI动作管理
    /// </summary>
    public class UIActionManager
    {
        public Queue<ActionData> Actions = new Queue<ActionData>();

        public ActionData? CurrentAction;

        public void AddAction(UI_Action ui, Action action, Action start = default(Action), Action end = default(Action))
        {
            Actions.Enqueue(new ActionData(ui, action, start, end));
        }

        /// <summary>
        /// 执行
        /// </summary>
        public void OnExcute()
        {
            if (Actions.Count == 0) return;

            if (CurrentAction == null)
            {
                CurrentAction = Actions.Dequeue();
                CurrentAction.Value.Excute();
            }
            else
            {
                if (CurrentAction.Value.IsComplete)
                    CurrentAction = null;
            }
        }
    }
}

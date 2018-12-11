using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

namespace Chemistry.Equipments.Actions
{

    public class EA_HandleControllerMono : MonoBehaviour
    {
        private Coroutine coroutine;

        public void OnStart()
        {
            if (coroutine != null) return;
            coroutine = StartCoroutine(EA_HandleController.OnUpdate());
        }

        public void OnStop()
        {
            if (coroutine == null) return;

            StopCoroutine(coroutine);
            coroutine = null;
        }

        public void OnDestroy()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            EA_HandleController.Instance = null;
        }
    }

    /// <summary>
    /// 动作处理控制端
    /// </summary>
    public class EA_HandleController
    {
        public static List<EA_Handle> Handles;

        private static EA_HandleController instance;
        public static EA_HandleController Instance {
            get {
                if (instance == null)
                    instance = new EA_HandleController();

                return instance;
            }
            set {
                instance = value;
            }
        }

        public EA_HandleControllerMono controllerMono;

        public EA_HandleController()
        {
            Handles = new List<EA_Handle>();

            GameObject target = new GameObject("EA_HandleController");

            controllerMono = target.AddComponent<EA_HandleControllerMono>();
        }

        public void AddHandle(EA_Handle handle)
        {
            if (Handles.Contains(handle)) return;

            Handles.Add(handle);

            controllerMono.OnStart();
        }

        public void RemoveHandle(EA_Handle handle)
        {
            if (Handles.Contains(handle))
                Handles.Remove(handle);

            if (Handles.Count == 0)
                controllerMono.OnStop();
        }

        public static void OnAddHandle(EA_Handle handle)
        {
            Instance.AddHandle(handle);
        }

        public static void OnRemoveHandle(EA_Handle handle)
        {
            Instance.RemoveHandle(handle);
        }

        public static IEnumerator OnUpdate()
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();

                foreach (var item in Handles.ToList())
                {
                    if (item == null) continue;

                    item.OnUpdate();
                }
            }
        }
    }
}

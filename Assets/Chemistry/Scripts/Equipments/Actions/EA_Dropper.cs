using System.Collections;
using UnityEngine;

namespace Chemistry.Equipments.Actions
{
    /// <summary>
    /// 胶头滴管动作
    /// </summary>
    public class EA_Dropper : MonoBehaviour
    {
        public SkinnedMeshRenderer meshRenderer;
        private Coroutine coroutineExtrusion;
        IEnumerator OnExtrusion(float start, float end)
        {
            while (true)
            {
                yield return new WaitForFixedUpdate();
                if (start >= end)
                {
                    //结束
                    start -= Time.deltaTime * 100;
                    end = 0;
                    if (start <= 0)
                        break;
                }
                else
                {
                    start += Time.deltaTime * 100;
                }
                meshRenderer.SetBlendShapeWeight(0, start);
            }

            StopCoroutine(coroutineExtrusion);
        }

        public void OnStart(float start, float end)
        {
            coroutineExtrusion = StartCoroutine(OnExtrusion(start, end));
        }
        public void OnDestroy()
        {
            if (coroutineExtrusion != null)
                StopCoroutine(coroutineExtrusion);
        }
    }
}

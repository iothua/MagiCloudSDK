using UnityEngine;

namespace MagiCloud.UISystem
{
    public static class UnityHelper
    {
        public static void SetLayer(this GameObject target,int layer)
        {
            Transform[] transforms = target.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i].gameObject.layer=layer;
            }
        }
    }
}

using System.Collections.Generic;
using UnityEngine;

namespace MagiCloud.Utility
{
    public static class Utilitys
    {
        /// <summary>
        /// 从0-指定数中随机n个不重复数
        /// </summary>
        /// <param name="total">随机总数</param>
        /// <param name="n">几个数</param>
        /// <returns></returns>
        public static int[] GetRandomSequence(int total,int n)
        {
            //随机总数组  
            int[] sequence = new int[total];
            //取到的不重复数字的数组长度  
            int[] output = new int[n];

            for (int i = 0; i < total; i++)
            {
                sequence[i] = i;
            }

            int end = total - 1;

            for (int i = 0; i < n; i++)
            {
                //随机一个数，每随机一次，随机区间-1  
                int num = Random.Range(0,end + 1);
                output[i] = sequence[num];
                //将区间最后一个数赋值到取到数上  
                sequence[num] = sequence[end];
                end--;
            }

            return output;
        }

        /// <summary>
        /// 重置transform的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="parent">父物体</param>
        /// <returns></returns>
        public static T ResetTransform<T>(this T t,Transform parent = null) where T : Component
        {
            Transform result = t.transform;
            if (parent!=null)
                result.SetParent(parent);
            result.localPosition=Vector3.zero;
            result.localRotation=Quaternion.identity;
            result.localScale=Vector3.one;
            return t;
        }

    }
}

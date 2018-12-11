using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Chemistry
{
    /// <summary>
    /// 化学工具初始化
    /// </summary>
    [ExecuteInEditMode]
    public class ChemistryInitialize : MonoBehaviour
    {

        private void Awake()
        {
            InitializeDataLoading();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public static void InitializeDataLoading()
        {
            Data.DataLoading.OnInitialize();
        }
    }
}



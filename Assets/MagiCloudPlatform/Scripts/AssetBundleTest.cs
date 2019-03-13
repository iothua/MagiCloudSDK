using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud;

public class AssetBundleTest : MonoBehaviour
{
    public string[] bundleInfos;

    private void Start()
    {
        AssetBundleManager.LoadAsset<GameObject>(bundleInfos, (target) =>
        {
            foreach (var item in target)
            {
                GameObject.Instantiate(item);
            }
        });

    }

}

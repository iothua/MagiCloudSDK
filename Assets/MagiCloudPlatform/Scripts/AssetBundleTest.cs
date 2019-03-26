using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MagiCloud;

public class AssetBundleTest : MonoBehaviour
{
    public string[] bundleInfos;

    private void Start()
    {
        AssetBundleManager.LoadAsset<GameObject>(bundleInfos, (targets) =>
        {
            foreach (var item in targets)
            {
                GameObject.Instantiate(item);
            }
        });

    }

}

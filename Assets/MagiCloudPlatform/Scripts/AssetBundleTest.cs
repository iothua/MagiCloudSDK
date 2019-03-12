using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Loxodon.Framework.Bundles;
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Contexts;

public class AssetBundleTest : MonoBehaviour
{
    private IResources resources;
    private Dictionary<string, IBundle> bundles = new Dictionary<string, IBundle>();

    // Start is called before the first frame update
    IEnumerator Start()
    {
        ApplicationContext context = Context.GetApplicationContext();
        this.resources = context.GetService<IResources>();

        yield return Preload(new string[] { "models/red", "models/green" }, 1);

        IBundle bundle = this.bundles["models/red"];
        GameObject goTemplate = bundle.LoadAsset<GameObject>("Red");
        GameObject.Instantiate(goTemplate);

        Material material = bundle.LoadAsset<Material>("Red");
        if (material != null)
        {
            Debug.Log("不为空");
        }

        GameObject goTemplate1 = bundle.LoadAsset<GameObject>("Red.prefab");
        var go1 = GameObject.Instantiate(goTemplate1);
        go1.transform.position = Vector3.one;

    }


    IEnumerator Preload(string[] bundleNames, int priority)
    {
        IProgressResult<float, IBundle[]> result = this.resources.LoadBundle(bundleNames, priority);
        yield return result.WaitForDone();

        if (result.Exception != null)
        {
            Debug.LogWarningFormat("Loads failure.Error :{0}", result.Exception);

            yield break;
        }

        foreach (var bundle in result.Result)
        {
            bundles.Add(bundle.Name, bundle);
        }
    }

    
}

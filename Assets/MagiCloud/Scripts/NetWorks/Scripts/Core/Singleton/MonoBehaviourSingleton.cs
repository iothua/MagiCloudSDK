using UnityEngine;

namespace MagiCloud.NetWorks
{
    /// <summary>
    /// Mono脚本单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class MonoBehaviourSingleton<T> :MonoBehaviour where T : MonoBehaviourSingleton<T>
    {
        protected static T instance;
        public static T Instance
        {
            get
            {
                if (instance==null)
                {
                    instance =FindObjectOfType<T>();
                    if (FindObjectsOfType<T>().Length>1)
                    {
                        return instance;
                    }
                }
                if (instance==null)
                {
                    string instanceName = typeof(T).Name;
                    GameObject obj = GameObject.Find(instanceName);
                    if (obj==null)
                    {
                        obj=new GameObject(instanceName);
                        DontDestroyOnLoad(obj);
                        instance=obj.AddComponent<T>();
                        DontDestroyOnLoad(instance);
                    }
                    else
                    {
                        instance=obj.GetComponent<T>();
                    }
                }
                return instance;
            }
        }
        protected virtual void OnDestroy()
        {
            instance=null;
        }
    }

}

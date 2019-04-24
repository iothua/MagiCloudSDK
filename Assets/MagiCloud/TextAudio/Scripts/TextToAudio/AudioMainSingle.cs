using MagiCloud.TextAudio;
using UnityEngine;
using UnityEngine.Events;
namespace MagiCloud.TextToAudio
{
    public class SingleMonoBehaviour<T> :AudioMain where T : AudioMain
    {

        private static T instance;
        public static T Instance
        {
            get
            {
           

                if (instance==null)
                {
                    instance=FindObjectOfType<T>();
                    if (instance==null)
                    {
                        instance=new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }
                return instance;
            }
        }
    }

    public class AudioMainSingle :SingleMonoBehaviour<AudioMainSingle>
    {
    }

}

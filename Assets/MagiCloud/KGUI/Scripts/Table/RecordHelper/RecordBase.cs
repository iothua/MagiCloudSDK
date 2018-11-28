using UnityEngine;
namespace MagiCloud.KGUI
{
    /// <summary>
    /// 记录者基类
    /// </summary>
    public abstract class RecordBase :MonoBehaviour, IRecord
    {

        public virtual bool CanReocrd => (true);

        public abstract string[] GetData();

    }
}
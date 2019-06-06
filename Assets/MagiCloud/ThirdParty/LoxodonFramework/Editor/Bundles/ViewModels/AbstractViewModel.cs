using Loxodon.Framework.Observables;

namespace Loxodon.Framework.Bundles.Editors
{
    [System.Serializable]
    public abstract class AbstractViewModel : ObservableObject
    {
        public virtual void OnEnable()
        {

        }

        public virtual void OnDisable()
        {

        }
    }
}

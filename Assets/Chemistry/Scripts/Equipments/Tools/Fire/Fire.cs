using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 火焰
    /// </summary>
    public class Fire :MonoBehaviour, IFire
    {

        public GameObject fireObj;

        /// <summary>
        /// 是否正在燃烧
        /// </summary>
        public bool Burning { get; private set; }


        /// <summary>
        /// 点燃
        /// </summary>
        public virtual void Ignite()
        {
            if (Burning) return;
            fireObj.SetActive(true);
            Burning=true;
        }

        /// <summary>
        /// 熄灭
        /// </summary>
        public virtual void Slake()
        {
            if (Burning)
            {
                fireObj.SetActive(false);
                Burning =false;
            }
        }

        //传递燃烧
        public virtual void PassFire(ICombustible combustible)
        {
            if (combustible==null)
            {
                throw new System.ArgumentNullException(nameof(combustible));
            }
            if (!Burning) return;
            if (combustible.CanIgnite&&!combustible.Fire.Burning)
            {
                combustible.Fire.Ignite();
            }
        }
    }

}
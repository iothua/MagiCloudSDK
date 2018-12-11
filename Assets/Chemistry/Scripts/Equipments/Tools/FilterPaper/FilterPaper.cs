using MagiCloud.Equipments;
using UnityEngine;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 滤纸
    /// </summary>
    public class FilterPaper :EquipmentBase
    {
        [Header("有动画时，初始播放")]
        public Animator animator;
        protected override void Start()
        {
            base.Start(); OnInitializeEquipment();
            if (animator!=null) animator.SetBool("play",true);
        }

        public override void OnInitializeEquipment_Editor(string name)
        {
            base.OnInitializeEquipment_Editor(name);
            var col = Collider as BoxCollider;
            col.center=new Vector3(0f,0.25f,0f);
            col.size=new Vector3(0.4f,0.3f,0.4f);
        }
    }
}
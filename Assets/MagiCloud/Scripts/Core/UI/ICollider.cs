using UnityEngine;

namespace MagiCloud.Core.UI
{
    public interface ICollider
    {
        Collider GetCollider { get; }
    }

    public interface IBoxCollider :ICollider
    {
        bool IsShake { get; set; }
        BoxCollider BoxCollider { get; }
    }
}

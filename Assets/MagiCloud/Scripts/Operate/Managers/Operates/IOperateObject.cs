using UnityEngine;
using MagiCloud.Core;

namespace MagiCloud
{
    public interface IOperateObject
    {
        GameObject GrabObject { get; set; }
        
        MInputHandStatus HandStatus { get; set; }
    }
}

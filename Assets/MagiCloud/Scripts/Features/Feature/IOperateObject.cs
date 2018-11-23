using UnityEngine;
using MagiCloud.Core;

namespace MagiCloud.Features
{
    public interface IOperateObject
    {
        GameObject GrabObject { get; set; }
        
        MInputHandStatus HandStatus { get; set; }
    }
}

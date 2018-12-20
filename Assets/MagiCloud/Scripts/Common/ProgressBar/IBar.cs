using MagiCloud.KGUI;
using UnityEngine;

namespace MagiCloud.Common
{
    public interface IBar
    {
        ButtonType BarType { get; }
        void InitRoot(Transform parent);
        void Init(Sprite bgSprite,Sprite frontSprite,Vector2 bgSize,Vector2 frontSize);
        void SetValue(float value);
        float GetValue();
        bool Remove();
    }


}
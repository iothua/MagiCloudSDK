using System;

namespace Chemistry.Equipments
{
    /// <summary>
    /// 通用处理
    /// </summary>
    public interface I_E_Handle
    {
        /// <summary>
        /// 持续时间
        /// </summary>
        float duration { get; set; }

        /// <summary>
        /// 开始
        /// </summary>
        void OnStart();
        /// <summary>
        /// 成功
        /// </summary>
        void OnSuccess();
        /// <summary>
        /// 失败
        /// </summary>
        void OnFail();

    }
}

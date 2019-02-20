/***
 * 
 *    Title: 框架类型
 *           
 *    Description: 
 *           保存框架需要用到的枚举类型
 *                  
 */

namespace MagiCloud.Kinect
{
    /// <summary>
    /// 手势激活状态
    /// </summary>
    public enum KinectHandStatus
    {
        /// <summary>
        /// 识别手势状态
        /// </summary>
        Identify,
        /// <summary>
        /// 手势启动
        /// </summary>
        Enable,
        /// <summary>
        /// 手势禁用
        /// </summary>
        Disable
    }

    /// <summary>
    /// 单双手状态
    /// </summary>
    public enum KinectActiveHandStadus
    {
        /// <summary>
        /// 不启动手
        /// </summary>
        None,
        /// <summary>
        /// 启动一只/启动左手，右手禁用。反之
        /// </summary>
        One,
        /// <summary>
        /// 双手都启动
        /// </summary>
        Two
    }
}


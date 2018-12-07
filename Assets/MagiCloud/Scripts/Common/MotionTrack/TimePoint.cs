namespace MagiCloud.Common
{
    /// <summary>
    /// 时间点
    /// </summary>
    public class TimePoint<T>
    {
        /// <summary>
        /// 起始时间
        /// </summary>
        public float startTime;
        /// <summary>
        /// 存在时间
        /// </summary>
        public float lifeTime;
        public T data;

        public TimePoint(float startTime,float lifeTime,T data)
        {
            this.startTime=startTime;
            this.lifeTime=lifeTime;
            this.data=data;
        }
    }
}

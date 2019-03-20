namespace MagiCloud.NetWorks
{
    /// <summary>
    /// 单例
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> where T : class, new()
    {
        protected static T instance;
        private static readonly object lockObj = new object();
        public static T Instance
        {
            get
            {
                if (instance==null)
                {
                    lock (lockObj)
                    {
                        if (instance==null)
                            instance=new T();
                    }
                }
                return instance;
            }
        }
    }


}

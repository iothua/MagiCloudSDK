#if UNITY_EDITOR
using Loxodon.Framework.Bundles.Archives;
using System.Collections.Generic;

namespace Loxodon.Framework.Bundles.Redundancy
{
    public class RedundancyReport
    {
        private readonly long totalSize;

        private readonly long redundantSize;

        private List<RedundancyInfo> redundancies;

        public RedundancyReport(long totalSize, long redundantSize, List<RedundancyInfo> redundancies)
        {
            this.totalSize = totalSize;
            this.redundantSize = redundantSize;
            this.redundancies = redundancies;
            this.redundancies.Sort((x, y) =>
            {
                var ret = x.Name.CompareTo(y.Name);
                if (ret != 0)
                    return ret;

                ret = x.TypeID.CompareTo(y.TypeID);
                if (ret != 0)
                    return ret;

                return x.FileSize.CompareTo(y.FileSize);
            });
        }

        public long TotalSize { get { return this.totalSize; } }

        public long RedundantSize { get { return this.redundantSize; } }

        public virtual IList<RedundancyInfo> GetAllRedundancyInfo()
        {
            return this.redundancies.AsReadOnly();
        }

        public virtual IList<RedundancyInfo> GetRedundancyInfo(TypeID typeId)
        {
            return this.redundancies.FindAll(r => r.TypeID == typeId);
        }
    }
}
#endif
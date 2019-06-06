#if UNITY_EDITOR
using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles.Redundancy
{
    public interface IRedundancyAnalyzer
    {
        IProgressResult<float, RedundancyReport> AnalyzeRedundancy();
    }
}
#endif
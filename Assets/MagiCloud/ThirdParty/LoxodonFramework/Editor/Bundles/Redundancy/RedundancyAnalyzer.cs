#if UNITY_EDITOR
using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles.Archives;
using Loxodon.Framework.Editors;
using System;
using System.Collections.Generic;

namespace Loxodon.Framework.Bundles.Redundancy
{
    public class RedundancyAnalyzer : IRedundancyAnalyzer
    {
        private readonly ArchiveContainer container;
        public RedundancyAnalyzer(ArchiveContainer container)
        {
            this.container = container;
        }

        public virtual ArchiveContainer Container { get { return this.container; } }

        public virtual IProgressResult<float, RedundancyReport> AnalyzeRedundancy()
        {
            return EditorExecutors.RunAsync<float, RedundancyReport>(promise =>
            {
                try
                {
                    long totalSize = 0;
                    long redundantSize = 0;
                    long objectTotalCount = 0;
                    List<ObjectInfo> infos = new List<ObjectInfo>();
                    foreach (var bundle in container.Bundles)
                    {
                        foreach (var archive in bundle.AssetArchives)
                        {
                            totalSize += archive.FileSize;

                            ObjectArchive objectArchive = archive as ObjectArchive;
                            if (objectArchive == null)
                                continue;

                            var objectInfos = objectArchive.GetAllObjectInfo();
                            objectTotalCount += objectInfos.Count;
                            foreach (var info in objectInfos)
                            {
                                if (promise.IsCancellationRequested)
                                    return;

                                if (info.IsPotentialRedundancy)
                                    infos.Add(info);
                            }
                        }
                    }

                    Dictionary<IFingerprint, RedundancyInfo> dict = new Dictionary<IFingerprint, RedundancyInfo>();
                    int count = infos.Count;
                    for (int i = 0; i < infos.Count; i++)
                    {
                        if (promise.IsCancellationRequested)
                            return;

                        var info = infos[i];
                        RedundancyInfo redundancyInfo = null;
                        var fingerprint = info.Fingerprint;

                        if (!dict.TryGetValue(fingerprint, out redundancyInfo))
                        {
                            redundancyInfo = new RedundancyInfo(info.Name, info.TypeID, info.Size);
                            dict.Add(fingerprint, redundancyInfo);

                            promise.UpdateProgress((float)i / count);
                        }
                        redundancyInfo.AddObjectInfo(info);
                    }

                    List<RedundancyInfo> redundancies = new List<RedundancyInfo>();
                    foreach (var redundancyInfo in dict.Values)
                    {
                        if (redundancyInfo.Count <= 1)
                            continue;

                        redundancies.Add(redundancyInfo);

                        var objectInfos = redundancyInfo.ObjectInfos;
                        for (int i = 1; i < objectInfos.Count; i++)
                        {
                            redundantSize += objectInfos[i].Size;
                        }
                    }

                    promise.SetResult(new RedundancyReport(totalSize, redundantSize, redundancies));
                }
                catch (Exception e)
                {
                    promise.SetException(e);
                }
            });
        }
    }
}
#endif
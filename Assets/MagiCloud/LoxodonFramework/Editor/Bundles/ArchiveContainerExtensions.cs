using Loxodon.Framework.Asynchronous;
using Loxodon.Framework.Bundles.Archives;
using Loxodon.Framework.Editors;
using Loxodon.Framework.Execution;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Loxodon.Framework.Bundles.Editors
{
    public static class ArchiveContainerExtensions
    {
        public static IProgressResult<float> Load(this ArchiveContainer container, DirectoryInfo dir)
        {
            return container.Load(dir, null);
        }

        public static IProgressResult<float> Load(this ArchiveContainer container, DirectoryInfo dir, IDecryptor decryptor)
        {
            try
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (FileInfo file in dir.GetFiles("*", SearchOption.TopDirectoryOnly))
                {
                    if (file.Name.EndsWith(BundleSetting.ManifestFilename))
                    {
                        files.Add(file);
                        continue;
                    }

                    if (file.Name.EndsWith(dir.Name))
                    {
                        files.Add(file);
                        continue;
                    }
                }

                if (files.Count <= 0)
                    throw new Exception("Please select the root directory of the AssetBundle");

                files.Sort((x, y) => y.LastWriteTime.CompareTo(x.LastWriteTime));

                if (files[0].Name.Equals(BundleSetting.ManifestFilename))
                {
                    BundleManifest manifest = BundleManifest.Parse(File.ReadAllText(files[0].FullName, Encoding.UTF8));
                    return container.LoadAssetBundle(dir.FullName, decryptor, manifest.GetAll());
                }
                else if (files[0].Name.Equals(dir.Name))
                {
                    List<string> filenames = new List<string>();
                    string[] bundleNames = new string[0];
                    var bundle = AssetBundleArchive.Load(files[0].FullName);
                    foreach (var archive in bundle.AssetArchives)
                    {
                        ObjectArchive oa = archive as ObjectArchive;
                        if (oa == null)
                            continue;

                        foreach (var summary in oa.GetAllObjectInfo())
                        {
                            if (summary.TypeID == TypeID.AssetBundleManifest)
                            {
                                Objects.AssetBundleManifest assetBundleManifest = summary.GetObject<Objects.AssetBundleManifest>();
                                bundleNames = assetBundleManifest.GetAllAssetBundles();
                            }
                        }
                    }

                    foreach (string bundleName in bundleNames)
                    {
                        var fullName = System.IO.Path.Combine(dir.FullName, bundleName.Replace("/", @"\"));
                        filenames.Add(fullName);
                    }
                    bundle.Dispose();

                    //UnityEngine.AssetBundle bundle = UnityEngine.AssetBundle.LoadFromFile(files[0].FullName);
                    //AssetBundleManifest manifest = bundle.LoadAsset<AssetBundleManifest>("assetbundlemanifest");
                    //bundle.Unload(false);
                    //files = new List<FileInfo>();

                    //foreach (string bundleName in manifest.GetAllAssetBundles())
                    //{
                    //    var fullName = System.IO.Path.Combine(dir.FullName, bundleName.Replace("/", @"\"));
                    //    filenames.Add(fullName);
                    //}

                    return container.LoadAssetBundle(filenames.ToArray());
                }
                else
                {
                    List<string> filenames = new List<string>();
                    foreach (FileInfo file in dir.GetFiles("*", SearchOption.AllDirectories))
                    {
                        if (file.FullName.EndsWith(BundleSetting.ManifestFilename) || file.FullName.EndsWith(".manifest"))
                            continue;

                        filenames.Add(file.FullName);
                    }
                    return container.LoadAssetBundle(filenames.ToArray());
                }
            }
            catch (Exception e)
            {
                return new ImmutableProgressResult<float>(e, 0f);
            }
        }

        public static IProgressResult<float> LoadAssetBundle(this ArchiveContainer container, string root, params BundleInfo[] bundleInfos)
        {
            return container.LoadAssetBundle(root, null, bundleInfos);
        }

        public static IProgressResult<float> LoadAssetBundle(this ArchiveContainer container, string root, IDecryptor decryptor, params BundleInfo[] bundleInfos)
        {
            return EditorExecutors.RunAsync(new Action<IProgressPromise<float>>((promise) =>
            {
                try
                {
                    int taskCount = 8;
                    int index = -1;
                    int finishedCount = 0;
                    int count = bundleInfos.Length;
                    CountFinishedEvent countFinishedEvent = new CountFinishedEvent(taskCount);
                    for (int i = 0; i < taskCount; i++)
                    {
                        EditorExecutors.RunAsyncNoReturn(() =>
                        {
                            while (true)
                            {
                                int currIndex = Interlocked.Increment(ref index);
                                if (currIndex > count - 1)
                                    break;

                                try
                                {
                                    var bundleInfo = bundleInfos[currIndex];
                                    var path = System.IO.Path.Combine(root, bundleInfo.Filename.Replace("/", @"\"));
                                    if (bundleInfo.IsEncrypted)
                                        container.LoadAssetBundle(path, decryptor);
                                    else
                                        container.LoadAssetBundle(path);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogErrorFormat("{0}", e);
                                }
                                finally
                                {
                                    Interlocked.Increment(ref finishedCount);
                                }
                            }
                            countFinishedEvent.Set();
                        });
                    }

                    while (!countFinishedEvent.Wait(100))
                    {
                        promise.UpdateProgress((float)finishedCount / count);
                    }

                    promise.SetResult();
                }
                catch (Exception e)
                {
                    promise.SetException(e);
                }
            }));
        }

        public static IProgressResult<float> LoadAssetBundle(this ArchiveContainer container, params string[] filenames)
        {
            return EditorExecutors.RunAsync(new Action<IProgressPromise<float>>((promise) =>
            {
                try
                {
                    int taskCount = 8;
                    int index = -1;
                    int finishedCount = 0;
                    int count = filenames.Length;
                    CountFinishedEvent countFinishedEvent = new CountFinishedEvent(taskCount);
                    for (int i = 0; i < taskCount; i++)
                    {
                        EditorExecutors.RunAsyncNoReturn(() =>
                        {
                            while (true)
                            {
                                int currIndex = Interlocked.Increment(ref index);
                                if (currIndex > count - 1)
                                    break;

                                try
                                {
                                    container.LoadAssetBundle(filenames[currIndex]);
                                }
                                catch (Exception e)
                                {
                                    Debug.LogErrorFormat("{0}", e);
                                }
                                finally
                                {
                                    Interlocked.Increment(ref finishedCount);
                                }
                            }
                            countFinishedEvent.Set();
                        });
                    }

                    while (!countFinishedEvent.Wait(100))
                    {
                        promise.UpdateProgress((float)finishedCount / count);
                    }

                    promise.SetResult();
                }
                catch (Exception e)
                {
                    promise.SetException(e);
                }
            }));
        }

        public static AssetBundleArchive LoadAssetBundle(this ArchiveContainer container, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                AssetBundleArchive bundle = AssetBundleArchive.Load(fileStream);
                bundle.Path = path;
                container.AddBundleArchive(bundle);
                return bundle;
            }
        }

        public static AssetBundleArchive LoadAssetBundle(this ArchiveContainer container, string path, IDecryptor decryptor)
        {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                using (var stream = decryptor.Decrypt(fileStream))
                {
                    AssetBundleArchive bundle = AssetBundleArchive.Load(stream);
                    bundle.Path = path;
                    container.AddBundleArchive(bundle);
                    return bundle;
                }
            }
        }

    }

}
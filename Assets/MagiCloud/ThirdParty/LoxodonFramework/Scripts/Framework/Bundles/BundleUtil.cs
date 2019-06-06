using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using Loxodon.Framework.Execution;

#if UNITY_ANDROID && !UNITY_EDITOR
using Ionic.Zip;
#endif

namespace Loxodon.Framework.Bundles
{
    public class BundleUtil
    {
        public readonly static string streamingAssetsPath = Application.streamingAssetsPath;
        public readonly static string persistentDataPath = Application.persistentDataPath;
        public readonly static string temporaryCachePath = Application.temporaryCachePath;

        private static string root = string.Empty;
        private static string temporaryCacheDirectory;
        private static string storableDirectory;
        private static string readOnlyDirectory;

        static BundleUtil()
        {
            Root = BundleSetting.BundleRoot;
#if UNITY_IOS
            UnityEngine.iOS.Device.SetNoBackupFlag(GetStorableDirectory());
#endif
        }

        /// <summary>
        /// The AssetBundle's root.
        /// </summary>
        private static string Root
        {
            get { return root; }
            set
            {
                root = value;
                temporaryCacheDirectory = temporaryCachePath + "/" + root + "/";
                storableDirectory = persistentDataPath + "/" + root + "/";
                readOnlyDirectory = streamingAssetsPath + "/" + root + "/";
            }
        }

        public static string GetTemporaryCacheDirectory()
        {
            if (!Root.Equals(BundleSetting.BundleRoot))
                Root = BundleSetting.BundleRoot;

            return temporaryCacheDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The storable directory.</returns>
        public static string GetStorableDirectory()
        {
            if (!Root.Equals(BundleSetting.BundleRoot))
                Root = BundleSetting.BundleRoot;

            return storableDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The read only directory.</returns>
        public static string GetReadOnlyDirectory()
        {
            if (!Root.Equals(BundleSetting.BundleRoot))
                Root = BundleSetting.BundleRoot;

            return readOnlyDirectory;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool ExistsInCache(BundleInfo bundleInfo)
        {
            if (Caching.IsVersionCached(bundleInfo.Filename, bundleInfo.Hash))
                return true;
            return false;
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        private static ZipFile zip = null;

        public static ZipFile GetAndroidAPK()
        {
            if (zip == null)
                zip = new ZipFile(Application.dataPath);
            return zip;
        }
#endif

#if !UNITY_WEBGL || UNITY_EDITOR
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool ExistsInReadOnlyDirectory(BundleInfo bundleInfo)
        {
            return ExistsInReadOnlyDirectory(bundleInfo.Filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static bool ExistsInReadOnlyDirectory(string relativePath)
        {
            string dir = GetReadOnlyDirectory();
            string fullName = System.IO.Path.Combine(dir, relativePath);
#if UNITY_ANDROID && !UNITY_EDITOR
            fullName = fullName.Substring(fullName.LastIndexOf("!") + 1);
            if (GetAndroidAPK().ContainsEntry(fullName))
                return true;
#else
            if (File.Exists(fullName))
                return true;
#endif
            return false;
        }
#endif

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool ExistsInStorableDirectory(BundleInfo bundleInfo)
        {
            return ExistsInStorableDirectory(bundleInfo.Filename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="relativePath"></param>
        /// <returns></returns>
        public static bool ExistsInStorableDirectory(string relativePath)
        {
            string dir = GetStorableDirectory();
            string fullName = System.IO.Path.Combine(dir, relativePath);
            if (File.Exists(fullName))
                return true;
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bundleInfo"></param>
        /// <returns></returns>
        public static bool Exists(BundleInfo bundleInfo)
        {
            if (ExistsInCache(bundleInfo))
                return true;

#if !UNITY_WEBGL || UNITY_EDITOR
            if (ExistsInReadOnlyDirectory(bundleInfo))
                return true;
#endif

            if (ExistsInStorableDirectory(bundleInfo))
                return true;

            return false;
        }

        private static void DeleteEmptyDirectory(DirectoryInfo directory)
        {
            try
            {
                if (directory.GetFiles().Length > 0)
                    return;

                DirectoryInfo[] arr = directory.GetDirectories();
                foreach (DirectoryInfo dir in arr)
                {
                    DeleteEmptyDirectory(dir);
                }

                arr = directory.GetDirectories();
                if (arr.Length <= 0)
                {
                    directory.Delete();
                    return;
                }
            }
            catch (Exception)
            {
            }
        }

        public static void EvictExpiredInStorableDirectory(BundleManifest manifest)
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Executors.RunAsyncNoReturn(() =>
            {
#endif
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(GetStorableDirectory());
                    if (!directory.Exists)
                        return;

                    List<string> files = new List<string>();
                    FileInfo manifestFileInfo = new FileInfo(GetStorableDirectory() + BundleSetting.ManifestFilename);
                    files.Add(manifestFileInfo.FullName);

                    BundleInfo[] bundleInfos = manifest.GetAllActivated();
                    foreach (BundleInfo bundleInfo in bundleInfos)
                    {
                        string fullname = GetStorableDirectory() + bundleInfo.Filename;
                        FileInfo info = new FileInfo(fullname);
                        if (!info.Exists)
                            continue;

                        files.Add(info.FullName);
                    }

                    foreach (FileInfo info in directory.GetFiles("*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            if (files.Contains(info.FullName))
                                continue;

                            info.Delete();
                        }
                        catch (Exception e)
                        {
                            Debug.LogErrorFormat("Delete file {0}.Error:{1}", info.FullName, e);
                        }
                    }

                    DeleteEmptyDirectory(directory);

                }
                catch (Exception ex)
                {
                    Debug.LogErrorFormat("DeleteExpiredInStorableDirectory exception.Error:{0}", ex);
                }
#if !UNITY_WEBGL || UNITY_EDITOR
            });
#endif
        }

        public static void ClearStorableDirectory()
        {
#if !UNITY_WEBGL || UNITY_EDITOR
            Executors.RunAsyncNoReturn(() =>
            {
#endif
                try
                {
                    DirectoryInfo directory = new DirectoryInfo(GetStorableDirectory());
                    if (!directory.Exists)
                        return;

                    directory.Delete(true);
                }
                catch (Exception e)
                {
#if !UNITY_WEBGL || UNITY_EDITOR
                    Debug.LogErrorFormat("Clear {0}.Error:{1}", GetStorableDirectory(), e);
#endif
                }
#if !UNITY_WEBGL || UNITY_EDITOR
            });
#endif
        }
    }
}


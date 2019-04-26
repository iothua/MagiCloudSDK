using System;
using System.Collections;
using UnityEngine;
using System.IO;
using Loxodon.Log;
#if UNITY_5_4_OR_NEWER
using UnityEngine.Networking;
#endif

using Loxodon.Framework.Asynchronous;

namespace Loxodon.Framework.Bundles
{
    public class CryptographBundleLoaderBuilder : AbstractLoaderBuilder
    {
        private IDecryptor decryptor;
        public CryptographBundleLoaderBuilder(Uri baseUri, IDecryptor decryptor) : base(baseUri)
        {
            this.decryptor = decryptor;
        }

        public override BundleLoader Create(BundleManager manager, BundleInfo bundleInfo)
        {
            return new CryptographBundleLoader(new Uri(this.BaseUri, bundleInfo.Filename), bundleInfo, manager, this.decryptor);
        }
    }

    public class CryptographBundleLoader : BundleLoader
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(CryptographBundleLoader));

        private const float WEIGHT = 0.7f;

        private IDecryptor decryptor;
        public CryptographBundleLoader(Uri uri, BundleInfo bundleInfo, BundleManager manager, IDecryptor decryptor) : base(uri, bundleInfo, manager)
        {
            this.decryptor = decryptor;
        }

        protected override IEnumerator DoLoadAssetBundle(IProgressPromise<float, AssetBundle> promise)
        {
            if (!this.decryptor.AlgorithmName.Equals(this.BundleInfo.Encoding))
            {
                promise.UpdateProgress(0f);
                promise.SetException(new Exception(string.Format("The encryption algorithm '{0}' and decryption algorithm '{1}' does not match when decrypts Assetbundle {2}. ", this.BundleInfo.Encoding, this.decryptor.AlgorithmName, this.BundleInfo.Name)));
                yield break;
            }

            byte[] chiperData = null;
            string path = this.GetAbsoluteUri();

#if UNITY_2017_1_OR_NEWER
            using (UnityWebRequest www = new UnityWebRequest(path))
            {
                www.downloadHandler = new DownloadHandlerBuffer();
#if UNITY_2018_1_OR_NEWER
                www.SendWebRequest();
#else
                www.Send();
#endif
                while (!www.isDone)
                {
                    if(www.downloadProgress >= 0)
                        promise.UpdateProgress(www.downloadProgress * WEIGHT);
                    yield return null;
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, www.error)));
                    yield break;
                }

                chiperData = www.downloadHandler.data;
            }
#elif UNITY_5_4_OR_NEWER
            if (this.Uri.Scheme.Equals("jar", StringComparison.OrdinalIgnoreCase))
            {
                using (WWW www = new WWW(path))
                {
                    while (!www.isDone)
                    {
                        if (www.progress >= 0)
                            promise.UpdateProgress(www.progress * WEIGHT);
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, www.error)));
                        yield break;
                    }

                    chiperData = www.bytes;
                }
            }
            else
            {
                using (UnityWebRequest www = new UnityWebRequest(path))
                {
                    www.downloadHandler = new DownloadHandlerBuffer();
                    www.Send();
                    while (!www.isDone)
                    {
                        if (www.downloadProgress >= 0)
                            promise.UpdateProgress(www.downloadProgress * WEIGHT);
                        yield return null;
                    }

                    if (!string.IsNullOrEmpty(www.error))
                    {
                        promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, www.error)));
                        yield break;
                    }

                    chiperData = www.downloadHandler.data;
                }
            }
#else
            using (WWW www = new WWW(path))
            {
                while (!www.isDone)
                {
                    if(www.progress >= 0)
                        promise.UpdateProgress(www.progress * WEIGHT);
                    yield return null;
                }

                if (!string.IsNullOrEmpty(www.error))
                {
                    promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, www.error)));
                    yield break;
                }

                chiperData = www.bytes;
            }
#endif

            if (this.IsRemoteUri())
            {
                string fullname = BundleUtil.GetStorableDirectory() + this.BundleInfo.Filename;
                try
                {
                    FileInfo info = new FileInfo(fullname);
                    if (info.Exists)
                        info.Delete();

                    if (!info.Directory.Exists)
                        info.Directory.Create();

                    File.WriteAllBytes(info.FullName, chiperData);
                }
                catch (Exception e)
                {
                    if (log.IsWarnEnabled)
                        log.WarnFormat("Save AssetBundle '{0}' to the directory '{1}' failed.Reason:{2}", this.BundleInfo.FullName, fullname, e);
                }
            }

            byte[] textData = null;
            try
            {
                textData = this.decryptor.Decrypt(chiperData);
            }
            catch (Exception e)
            {
                promise.SetException(new Exception(string.Format("Failed to decrypt the AssetBundle '{0}' at the address '{1}'.Error:{2}", this.BundleInfo.Name, path, e)));
                yield break;
            }

            AssetBundleCreateRequest request = AssetBundle.LoadFromMemoryAsync(textData);
            while (!request.isDone)
            {
                promise.UpdateProgress(WEIGHT + (1 - WEIGHT) * request.progress);
                yield return null;
            }

            var assetBundle = request.assetBundle;
            if (assetBundle == null)
            {
                promise.SetException(new Exception(string.Format("Failed to load the AssetBundle '{0}' at the address '{1}'.", this.BundleInfo.Name, path)));
                yield break;
            }

            promise.UpdateProgress(1f);
            promise.SetResult(assetBundle);
        }
    }
}

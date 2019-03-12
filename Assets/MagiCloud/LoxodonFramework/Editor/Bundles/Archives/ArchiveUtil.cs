#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using Hash128 = Loxodon.Framework.Bundles.Objects.Hash128;

namespace Loxodon.Framework.Bundles.Archives
{
    [InitializeOnLoad]
    public class ArchiveUtil
    {
        [ThreadStatic]
        private static MD5 md5;

        private static MD5 MD5
        {
            get
            {
                if (md5 == null)
                    md5 = new MD5CryptoServiceProvider();
                return md5;
            }
        }

        static string temporaryCachePath;

        static ArchiveUtil()
        {
            ClearTemporaryCache();
        }

        public static Hash128 Hash(byte[] data)
        {
            byte[] hash = MD5.ComputeHash(data);
            return new Hash128(hash);
        }

        public static Hash128 Hash(Stream stream)
        {
            byte[] hash = HashBytes(stream);
            return new Hash128(hash);
        }

        public static byte[] HashBytes(Stream stream)
        {
            return MD5.ComputeHash(stream);
        }

        public static void ClearTemporaryCache()
        {
            try
            {
                DirectoryInfo dir = new DirectoryInfo(GetTemporaryCachePath());
                try
                {
                    if (dir.Exists)
                        dir.Delete(true);
                }
                catch (Exception) { }

                if (!dir.Exists)
                    dir.Create();
            }
            catch (Exception)
            {
            }
        }

        public static string GetTemporaryCachePath()
        {
            if (!string.IsNullOrEmpty(temporaryCachePath))
                return temporaryCachePath;

            temporaryCachePath = Application.temporaryCachePath + @"\Bundle";
            return temporaryCachePath;
        }
    }
}
#endif
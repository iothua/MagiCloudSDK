using System;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;

namespace Loxodon.Framework.Bundles
{
#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class BundleSetting
    {
        private const string DEFAULT_ROOT_DIR_NAME = "bundles";
        private const string MANIFEST_FILENAME = "manifest.dat";

        private static string manifestFilename;
        private static string bundleRoot;

        static BundleSetting()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("BundleSetting");
            if (textAsset == null)
                return;

            using (StringReader reader = new StringReader(textAsset.text))
            {
                string line = null;
                while ((line = reader.ReadLine()) != null)
                {
                    line = line.Replace(" ", "");
                    Match m = Regex.Match(line, @"^([a-zA-Z0-9]+)=([a-zA-Z0-9/_.]+)$", RegexOptions.IgnorePatternWhitespace);
                    if (!m.Success)
                        continue;

                    string key = m.Groups[1].Value;
                    string value = m.Groups[2].Value;
                    switch (key)
                    {
                        case "manifestFilename":
                            ManifestFilename = value;
                            break;
                        case "bundleRoot":
                            BundleRoot = value;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Manifest's filename.
        /// </summary>
        public static string ManifestFilename
        {
            get { return manifestFilename ?? (manifestFilename = MANIFEST_FILENAME); }
            protected set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("ManifestFilename");

                manifestFilename = value;
            }
        }

        public static string BundleRoot
        {
            get { return bundleRoot ?? (bundleRoot = DEFAULT_ROOT_DIR_NAME); }
            protected set
            {
                if (string.IsNullOrEmpty(value))
                    throw new ArgumentNullException("BundleRoot");

                bundleRoot = value;
            }
        }
    }
}

using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Text.RegularExpressions;
using Loxodon.Framework.Attributes;

namespace Loxodon.Framework.Bundles.Editors
{
    [Serializable]
    public class BuildVM : AbstractViewModel
    {
        private const string PREFIX = "Loxodon::Framework::Bundle::";
        public const string PATH = "Assets/LoxodonFramework/Editor/AppData/Bundle/BuildSetting.json";
        public const string DEFAULT_OUTPUT = "AssetBundles";

        [SerializeField]
        private CompressOptions compression;
        [SerializeField]
        private string outputPath = DEFAULT_OUTPUT;
        [SerializeField]
        private bool usePlayerSettingVersion;
        [SerializeField]
        private string dataVersion;
        [SerializeField]
        private BuildAssetBundleOptions buildAssetBundleOptions;
        [SerializeField]
        private bool copyToStreaming;
        [SerializeField]
        private bool useHashFilename;
        [SerializeField]
        private bool encryption;
        [SerializeField]
        private Algorithm algorithm = Algorithm.AES128_CBC_PKCS7;
        [SerializeField]
        private string iv;
        [SerializeField]
        private string key;
        [SerializeField]
        private EncryptionFilterType filterType;
        [SerializeField]
        private string filterExpression;
        [SerializeField]
        private string bundleNames;

        [NonSerialized]
        private BuildTarget buildTarget;
        [NonSerialized]
        private bool advancedSettings = false;

        public BuildVM()
        {
            this.dataVersion = "1.0.0";
            this.copyToStreaming = true;
            this.useHashFilename = true;
            this.encryption = false;
        }

        public BuildTarget BuildTarget
        {
            get { return this.buildTarget; }
            set
            {
                if (this.Set<BuildTarget>(ref this.buildTarget, value, () => BuildTarget))
                    EditorPrefs.SetInt(PREFIX + "BuildTarget", (int)value);
            }
        }

        public CompressOptions Compression
        {
            get { return this.compression; }
            set
            {
                if (this.Set<CompressOptions>(ref this.compression, value, () => Compression))
                    this.Save();
            }
        }

        public string OutputPath
        {
            get { return this.outputPath; }
            set
            {
                if (this.Set<string>(ref this.outputPath, value, () => OutputPath))
                    this.Save();
            }
        }

        public bool UsePlayerSettingVersion
        {
            get { return this.usePlayerSettingVersion; }
            set
            {
                if (this.Set<bool>(ref this.usePlayerSettingVersion, value, () => UsePlayerSettingVersion))
                {
                    this.RaisePropertyChanged("DataVersion");
                    this.Save();
                }
            }
        }

        public string DataVersion
        {
            get
            {
                if (this.usePlayerSettingVersion)
                    return PlayerSettings.bundleVersion;

                return this.dataVersion;
            }
            set
            {
                if (this.usePlayerSettingVersion)
                {
                    if (string.IsNullOrEmpty(value) || value.Equals(PlayerSettings.bundleVersion))
                        return;

                    PlayerSettings.bundleVersion = value;
                    this.RaisePropertyChanged("DataVersion");
                    return;
                }

                if (this.Set<string>(ref this.dataVersion, value, () => DataVersion))
                    this.Save();
            }
        }

        public bool AdvancedSettings
        {
            get { return this.advancedSettings; }
            set
            {
                if (this.Set<bool>(ref this.advancedSettings, value, () => AdvancedSettings))
                    EditorPrefs.SetBool(PREFIX + "AdvancedSettings", value);
            }
        }

        public BuildAssetBundleOptions BuildAssetBundleOptions
        {
            get { return this.buildAssetBundleOptions; }
            set
            {
                if (this.Set<BuildAssetBundleOptions>(ref this.buildAssetBundleOptions, value, () => BuildAssetBundleOptions))
                    this.Save();
            }
        }

        public bool CopyToStreaming
        {
            get { return this.copyToStreaming; }
            set
            {
                if (this.Set<bool>(ref this.copyToStreaming, value, () => CopyToStreaming))
                    this.Save();
            }
        }

        public bool UseHashFilename
        {
            get { return this.useHashFilename; }
            set
            {
                if (this.Set<bool>(ref this.useHashFilename, value, () => UseHashFilename))
                    this.Save();
            }
        }

        public bool Encryption
        {
            get { return this.encryption; }
            set
            {
                if (this.Set<bool>(ref this.encryption, value, () => Encryption))
                    this.Save();
            }
        }

        public Algorithm Algorithm
        {
            get { return this.algorithm; }
            set
            {
                if (this.Set<Algorithm>(ref this.algorithm, value, () => Algorithm))
                    this.Save();
            }
        }

        public string IV
        {
            get { return this.iv; }
            set
            {
                if (this.Set<string>(ref this.iv, value, () => IV))
                    this.Save();
            }
        }
        public string KEY
        {
            get { return this.key; }
            set
            {
                if (this.Set<string>(ref this.key, value, () => KEY))
                    this.Save();
            }
        }

        public EncryptionFilterType FilterType
        {
            get { return this.filterType; }
            set
            {
                if (this.Set<EncryptionFilterType>(ref this.filterType, value, () => FilterType))
                    this.Save();
            }
        }
        public string FilterExpression
        {
            get { return this.filterExpression; }
            set
            {
                if (this.Set<string>(ref this.filterExpression, value, () => FilterExpression))
                    this.Save();
            }
        }
        public string BundleNames
        {
            get { return this.bundleNames; }
            set
            {
                if (this.Set<string>(ref this.bundleNames, value, () => BundleNames))
                    this.Save();
            }
        }

        public override void OnEnable()
        {
            this.Load();

            this.buildTarget = (BuildTarget)EditorPrefs.GetInt(PREFIX + "BuildTarget", (int)EditorUserBuildSettings.activeBuildTarget);
            this.advancedSettings = EditorPrefs.GetBool(PREFIX + "AdvancedSettings", this.advancedSettings);
        }

        public override void OnDisable()
        {
        }

        protected virtual void Load()
        {
            try
            {
                if (!File.Exists(PATH))
                    return;

                string json = File.ReadAllText(PATH);
                JsonUtility.FromJsonOverwrite(json, this);
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("Loads {0} failure. Error:{1}", PATH, e);
            }
        }

        protected virtual void Save()
        {
            try
            {
                if (!File.Exists(PATH))
                {
                    FileInfo info = new FileInfo(PATH);
                    if (!info.Directory.Exists)
                        info.Directory.Create();
                }

                string json = JsonUtility.ToJson(this);
                File.WriteAllText(PATH, json);

            }
            catch (System.Exception e)
            {
                Debug.LogWarningFormat("Write {0} failure. Error:{1}", PATH, e);
            }
        }

        public virtual bool VersionExists()
        {
            BundleBuilder builder = new BundleBuilder();
            string versionOutput = builder.GetVersionOutput(this.OutputPath, this.BuildTarget, this.DataVersion);
            return Directory.Exists(versionOutput);
        }

        public virtual void Build(bool forceRebuild)
        {
            string path = this.OutputPath;
            if (string.IsNullOrEmpty(path))
                BrowseOutputFolder();

            if (string.IsNullOrEmpty(path))
            {
                Debug.LogError("AssetBundle Build: No valid output path for build.");
                return;
            }

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            BuildAssetBundleOptions options = BuildAssetBundleOptions.DeterministicAssetBundle;
            switch (this.Compression)
            {
                case CompressOptions.Uncompressed:
                    options |= BuildAssetBundleOptions.UncompressedAssetBundle;
                    break;
                case CompressOptions.ChunkBasedCompression:
                    options |= BuildAssetBundleOptions.ChunkBasedCompression;
                    break;
            }

            if (forceRebuild)
                options |= BuildAssetBundleOptions.ForceRebuildAssetBundle;

            options |= this.BuildAssetBundleOptions;

            List<IBundleModifier> bundleModifierChain = this.CreateBundleModifierChain();
            BundleBuilder builder = new BundleBuilder();
            builder.Build(path, this.BuildTarget, options, this.DataVersion, bundleModifierChain);

#if UNITY_5_6_OR_NEWER
            if ((options & BuildAssetBundleOptions.DryRunBuild) > 0)
            {
                Debug.LogFormat("Dry Build OK.");
                return;
            }
#endif

            DirectoryInfo dir = new DirectoryInfo(builder.GetPlatformOutput(path, this.BuildTarget));
            try
            {
                //open the folder 
                EditorUtility.OpenWithDefaultApp(dir.FullName);
            }
            catch (Exception) { }

            Debug.LogFormat("Build OK.Please check the folder:{0}", dir.FullName);

            if (!this.CopyToStreaming)
                return;

            this.CopyToStreamingAssets();
        }

        protected virtual List<IBundleModifier> CreateBundleModifierChain()
        {
            List<IBundleModifier> bundleModifierChain = new List<IBundleModifier>();

            bundleModifierChain.Add(new PublishBundleModifier(bundleInfo => true));

            if (this.Encryption)
            {
                Func<BundleInfo, bool> filter = null;
                switch (this.FilterType)
                {
                    case EncryptionFilterType.All:
                        filter = bundle =>
                        {
                            return true;
                        };
                        break;
                    case EncryptionFilterType.RegularExpression:
                        filter = bundle =>
                        {
                            return Regex.IsMatch(bundle.FullName, this.filterExpression);
                        };
                        break;
                    case EncryptionFilterType.BundleNameList:
                        string[] bundles = Regex.Split(this.bundleNames, @"(\s)", RegexOptions.IgnorePatternWhitespace);
                        filter = bundle =>
                        {
                            return Array.IndexOf(bundles, bundle.FullName) >= 0;
                        };
                        break;
                }

                RijndaelCryptograph rijndaelCryptograph = this.GetRijndaelCryptograph();
                bundleModifierChain.Add(new CryptographBundleModifier(rijndaelCryptograph, filter));
            }

            if (this.useHashFilename)
                bundleModifierChain.Add(new HashFilenameBundleModifier());

            return bundleModifierChain;
        }

        public virtual RijndaelCryptograph GetRijndaelCryptograph()
        {
            int keySize = 128;
            switch (this.Algorithm)
            {
                case Algorithm.AES128_CBC_PKCS7:
                    keySize = 128;
                    break;
                case Algorithm.AES192_CBC_PKCS7:
                    keySize = 192;
                    break;
                case Algorithm.AES256_CBC_PKCS7:
                    keySize = 256;
                    break;
            }
            return new RijndaelCryptograph(keySize, Encoding.ASCII.GetBytes(this.KEY), Encoding.ASCII.GetBytes(this.IV));
        }

        public virtual void ClearFromStreamingAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                DirectoryInfo dir = new DirectoryInfo(BundleUtil.GetReadOnlyDirectory());
                if (dir.Exists)
                    dir.Delete(true);
                AssetDatabase.Refresh();
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public virtual void CopyToStreamingAssets()
        {
            try
            {
                AssetDatabase.StartAssetEditing();
                BundleBuilder builder = new BundleBuilder();

                DirectoryInfo src = new DirectoryInfo(builder.GetVersionOutput(this.OutputPath, this.BuildTarget, this.DataVersion));
                DirectoryInfo dest = new DirectoryInfo(BundleUtil.GetReadOnlyDirectory());

                if (dest.Exists)
                    dest.Delete(true);
                if (!dest.Exists)
                    dest.Create();

                BundleManifest manifest = builder.CopyAssetBundleAndManifest(src, dest);
                if (manifest != null)
                    Debug.LogFormat("Copy AssetBundles success.");

                AssetDatabase.Refresh();
            }
            catch (Exception e)
            {
                Debug.LogFormat("Copy AssetBundles failure. Error:{0}", e);
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
            }
        }

        public virtual string GenerateKey()
        {
            if (this.Encryption)
            {
                int keySize = 16;
                switch (this.Algorithm)
                {
                    case Algorithm.AES128_CBC_PKCS7:
                        keySize = 16;
                        break;
                    case Algorithm.AES192_CBC_PKCS7:
                        keySize = 24;
                        break;
                    case Algorithm.AES256_CBC_PKCS7:
                        keySize = 32;
                        break;
                }
                return RijndaelCryptograph.GenerateKey(keySize);
            }
            return string.Empty;
        }

        public virtual string GenerateIV()
        {
            if (this.Encryption)
                return RijndaelCryptograph.GenerateIV();
            return string.Empty;
        }

        public virtual void BrowseOutputFolder()
        {
            var path = EditorUtility.OpenFolderPanel("AssetBundle Folder", this.OutputPath, string.Empty);
            if (string.IsNullOrEmpty(path))
                return;

            var projectPath = System.IO.Path.GetFullPath(".");
            projectPath = projectPath.Replace("\\", "/");
            if (path.StartsWith(projectPath))
                path = path.Remove(0, projectPath.Length + 1);
            this.OutputPath = path;
        }
        public virtual void ResetOutputFolder()
        {
            this.OutputPath = DEFAULT_OUTPUT;
        }

        public virtual void OpenOutputFolder()
        {
            if (!Directory.Exists(this.OutputPath))
                Directory.CreateDirectory(this.OutputPath);

            EditorUtility.OpenWithDefaultApp(this.OutputPath);
        }
    }

    public enum CompressOptions
    {
        [Remark("No Compression")]
        Uncompressed = 0,

        [Remark("Standard Compression (LZMA)")]
        StandardCompression,/*LZMA*/

        [Remark("Chunk Based Compression (LZ4)")]
        ChunkBasedCompression/*LZ4*/
    }

    public enum Algorithm
    {
        [Remark("AES128 CBC PKCS7")]
        AES128_CBC_PKCS7 = 0,

        [Remark("AES192 CBC PKCS7")]
        AES192_CBC_PKCS7,

        [Remark("AES256 CBC PKCS7")]
        AES256_CBC_PKCS7
    }

    public enum EncryptionFilterType
    {
        All,
        RegularExpression,
        BundleNameList
    }
}

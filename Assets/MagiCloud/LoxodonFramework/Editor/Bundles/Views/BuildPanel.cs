using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Loxodon.Framework.Attributes;

namespace Loxodon.Framework.Bundles.Editors
{
    public class BuildPanel : Panel
    {
        private BuildVM buildVM;

        private Vector2 scrollPosition;
        private Vector2 textScrollPosition;

        private List<CheckBoxData<BuildAssetBundleOptions>> buildOptionList;
        private GUIContent buildTargetContent;
        private GUIContent compressionContent;
        private GUIContent dataVersionContent;
        private GUIContent outputPathContent;
        private GUIContent usePlaySettingVersionContent;
        private GUIContent copyToStreamingContent;
        private GUIContent useHashFilenameContent;
        private GUIContent encryptionContent;
        private GUIContent algorithmContent;
        private GUIContent ivContent;
        private GUIContent keyContent;
        private GUIContent filterTypeContent;

        public BuildPanel(EditorWindow parent, BuildVM buildVM) : base(parent)
        {
            this.buildVM = buildVM;
        }


        public override void OnEnable()
        {
            buildOptionList = new List<CheckBoxData<BuildAssetBundleOptions>>();
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "Append Hash",
                "Append the hash to the assetBundle name.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.AppendHashToAssetBundleName) > 0,
                BuildAssetBundleOptions.AppendHashToAssetBundleName));
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "Exclude Type Information",
                "Do not include type information within the asset bundle (don't write type tree).",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.DisableWriteTypeTree) > 0,
                BuildAssetBundleOptions.DisableWriteTypeTree));
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "Ignore Type Tree Changes",
                "Ignore the type tree changes when doing the incremental build check.",
               (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.IgnoreTypeTreeChanges) > 0,
                BuildAssetBundleOptions.IgnoreTypeTreeChanges));
#if UNITY_5_6_OR_NEWER
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "Strict Mode",
                "Do not allow the build to succeed if any errors are reporting during it.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.StrictMode) > 0,
                BuildAssetBundleOptions.StrictMode));
            buildOptionList.Add(new CheckBoxData<BuildAssetBundleOptions>(
                "Dry Run Build",
                "Do a dry run build.",
                (this.buildVM.BuildAssetBundleOptions & BuildAssetBundleOptions.DryRunBuild) > 0,
                BuildAssetBundleOptions.DryRunBuild));
#endif

            this.outputPathContent = new GUIContent("Output Path", "");
            this.dataVersionContent = new GUIContent("Data Version", "The AssetBundle's data version.");
            this.usePlaySettingVersionContent = new GUIContent("Use PlayerSetting Version", "Use the PlayerSetting version.");
            this.copyToStreamingContent = new GUIContent("Copy to StreamingAssets", "After build completes, will copy all build content to Assets/StreamingAssets/bundles for use in stand-alone player.");
            this.useHashFilenameContent = new GUIContent("Use Hash Filename", "");
            this.encryptionContent = new GUIContent("Encryption", "Encrypts the AssetBundle's data.");
            this.algorithmContent = new GUIContent("Algorithm", "Choose AES128_CBC_PKCS7, AES192_CBC_PKCS7 or AES256_CBC_PKCS7");
            this.filterTypeContent = new GUIContent("Bundles", "");

            this.buildTargetContent = new GUIContent("Build Target", "Choose target platform to build for.");
            this.compressionContent = new GUIContent("Compression", "Choose no compress, standard (LZMA), or chunk based (LZ4)");
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorGUILayout.Space();
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();

            //this.buildVM.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(buildTargetContent, this.buildVM.BuildTarget);

            this.buildVM.BuildTarget = EditorUserBuildSettings.activeBuildTarget;
            EditorGUILayout.LabelField(this.buildTargetContent, new GUIContent(buildVM.BuildTarget.ToString()), GUILayout.Width(300f), GUILayout.Height(20));
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Switch Platform", GUILayout.Width(200f)))
            {
                EditorWindow.GetWindow(Type.GetType("UnityEditor.BuildPlayerWindow,UnityEditor"));
                GUIUtility.ExitGUI();
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            if (this.buildVM.UsePlayerSettingVersion)
                EditorGUILayout.LabelField(this.dataVersionContent, new GUIContent(buildVM.DataVersion), GUILayout.MinWidth(230f), GUILayout.MaxWidth(250f), GUILayout.Height(20));
            else
                buildVM.DataVersion = EditorGUILayout.TextField(this.dataVersionContent, buildVM.DataVersion, GUILayout.MinWidth(230f), GUILayout.MaxWidth(250f), GUILayout.Height(20));

            this.buildVM.UsePlayerSettingVersion = EditorGUILayout.ToggleLeft(this.usePlaySettingVersionContent, this.buildVM.UsePlayerSettingVersion);

            GUILayout.FlexibleSpace();

            if (this.buildVM.UsePlayerSettingVersion)
            {
                if (GUILayout.Button("Player Setting", GUILayout.Width(200f)))
                {
                    Selection.activeObject = Unsupported.GetSerializedAssetInterfaceSingleton("PlayerSettings");
                    GUIUtility.ExitGUI();
                }
            }
            GUILayout.EndHorizontal();

            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            this.buildVM.OutputPath = EditorGUILayout.TextField(this.outputPathContent, this.buildVM.OutputPath, GUILayout.Height(20));

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Browse", GUILayout.MaxWidth(100f), GUILayout.MinHeight(25f)))
            {
                this.buildVM.BrowseOutputFolder();
                this.Repaint();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Reset", GUILayout.MaxWidth(100f), GUILayout.MinHeight(25f)))
            {
                this.buildVM.ResetOutputFolder();
                this.Repaint();
                GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Open", GUILayout.MaxWidth(100f), GUILayout.MinHeight(25f)))
            {
                this.buildVM.OpenOutputFolder();
                this.Repaint();
                GUIUtility.ExitGUI();
            }

            GUILayout.EndHorizontal();
            EditorGUILayout.Space();

            GUILayout.BeginHorizontal();
            this.buildVM.CopyToStreaming = EditorGUILayout.ToggleLeft(this.copyToStreamingContent, this.buildVM.CopyToStreaming);
            this.buildVM.UseHashFilename = EditorGUILayout.ToggleLeft(this.useHashFilenameContent, this.buildVM.UseHashFilename);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            // advanced options
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();
            this.buildVM.AdvancedSettings = EditorGUILayout.Foldout(this.buildVM.AdvancedSettings, "Advanced Settings");

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (this.buildVM.AdvancedSettings)
            {
                var indent = EditorGUI.indentLevel;
                EditorGUI.indentLevel += 1;

                GUILayout.BeginHorizontal();
                this.buildVM.Compression = (CompressOptions)EditorGUILayout.IntPopup(compressionContent, (int)this.buildVM.Compression,
                    new GUIContent[]{
                        new GUIContent(CompressOptions.Uncompressed.GetRemark()),
                        new GUIContent(CompressOptions.StandardCompression.GetRemark()),
                        new GUIContent(CompressOptions.ChunkBasedCompression.GetRemark())
                    }, new int[] { 0, 1, 2 }, GUILayout.Width(400), GUILayout.Height(20));

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                for (int i = 0; i < buildOptionList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    var buildOption = buildOptionList[i];

                    var newValue = EditorGUILayout.ToggleLeft(buildOption.Content, buildOption.Value);
                    if (newValue != buildOption.Value)
                    {
                        buildOption.Value = newValue;
                        if (buildOption.Value && (buildOption.Tag == BuildAssetBundleOptions.IgnoreTypeTreeChanges || buildOption.Tag == BuildAssetBundleOptions.DisableWriteTypeTree))
                        {
                            for (int j = 0; j < buildOptionList.Count; j++)
                            {
                                var checkBox2 = buildOptionList[j];
                                if (buildOption != checkBox2 && (checkBox2.Tag == BuildAssetBundleOptions.IgnoreTypeTreeChanges || checkBox2.Tag == BuildAssetBundleOptions.DisableWriteTypeTree) && checkBox2.Value)
                                {
                                    checkBox2.Value = false;
                                    break;
                                }
                            }
                        }

                        BuildAssetBundleOptions options = BuildAssetBundleOptions.None;
                        for (int j = 0; j < buildOptionList.Count; j++)
                        {
                            if (buildOptionList[j].Value)
                                options |= buildOptionList[j].Tag;
                        }
                        this.buildVM.BuildAssetBundleOptions = options;
                    }
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }

                EditorGUILayout.Space();
                EditorGUILayout.Space();

                GUILayout.BeginHorizontal();
                this.buildVM.Encryption = EditorGUILayout.ToggleLeft(this.encryptionContent, this.buildVM.Encryption);

                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                if (this.buildVM.Encryption)
                {
                    int indent2 = EditorGUI.indentLevel;
                    EditorGUI.indentLevel += 1;
                    this.buildVM.Algorithm = (Algorithm)EditorGUILayout.EnumPopup(this.algorithmContent, this.buildVM.Algorithm, GUILayout.Height(20));

                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    this.buildVM.IV = EditorGUILayout.TextField("IV", this.buildVM.IV, GUILayout.MinWidth(230f), GUILayout.Height(20));

                    if (GUILayout.Button("Generate IV", GUILayout.Width(100f)))
                    {
                        this.buildVM.IV = this.buildVM.GenerateIV();
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();
                    GUILayout.BeginHorizontal();
                    this.buildVM.KEY = EditorGUILayout.TextField("KEY", this.buildVM.KEY, GUILayout.MinWidth(230f), GUILayout.Height(20));
                    if (GUILayout.Button("Generate KEY", GUILayout.Width(100f)))
                    {
                        this.buildVM.KEY = this.buildVM.GenerateKey();
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.BeginHorizontal();
                    //EditorGUILayout.LabelField(new GUIContent("Bundles", "All the names of Assetbundle that need to be encrypted."));
                    this.buildVM.FilterType = (EncryptionFilterType)EditorGUILayout.EnumPopup(this.filterTypeContent, this.buildVM.FilterType, GUILayout.Height(20));
                    GUILayout.EndHorizontal();
                    EditorGUILayout.Space();

                    if (this.buildVM.FilterType == EncryptionFilterType.BundleNameList)
                    {
                        this.textScrollPosition = EditorGUILayout.BeginScrollView(textScrollPosition);
                        this.buildVM.BundleNames = EditorGUILayout.TextArea(this.buildVM.BundleNames, GUILayout.Height(rect.height - 80));
                        EditorGUILayout.EndScrollView();
                    }
                    else if (this.buildVM.FilterType == EncryptionFilterType.RegularExpression)
                    {
                        this.buildVM.FilterExpression = EditorGUILayout.TextField("", this.buildVM.FilterExpression, GUILayout.MinWidth(230f), GUILayout.Height(20));
                    }

                    EditorGUI.indentLevel = indent2;
                }

                EditorGUI.indentLevel = indent;
            }

            EditorGUILayout.Space();


            // build.
            EditorGUILayout.Space();
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Rebuild", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                if (!this.buildVM.VersionExists() || EditorUtility.DisplayDialog("Version already exist", "The version already exist!Are you sure you want to replace this version?", "Yes", "No"))
                {
                    EditorApplication.delayCall += () => this.buildVM.Build(true);
                }
            }

            if (GUILayout.Button("Build", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                if (!this.buildVM.VersionExists() || EditorUtility.DisplayDialog("Version already exist", "The version already exist!Are you sure you want to replace this version?", "Yes", "No"))
                {
                    EditorApplication.delayCall += () => this.buildVM.Build(false);
                }
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Clear from StreamingAssets", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                EditorApplication.delayCall += () => this.buildVM.ClearFromStreamingAssets();
            }

            if (GUILayout.Button("Copy to StreamingAssets", GUILayout.Width(250f), GUILayout.MinHeight(40f)))
            {
                EditorApplication.delayCall += () => this.buildVM.CopyToStreamingAssets();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        public class PropertyData<T>
        {
            protected string prefsKey;
            protected GUIContent content;
            protected T value;
            protected GUILayoutOption[] options;

            public PropertyData(string text, string tooltip, T value) : this(new GUIContent(text, tooltip), value, null)
            {
            }

            public PropertyData(GUIContent content, T value) : this(content, value, null)
            {
            }

            public PropertyData(GUIContent content, T value, GUILayoutOption[] options)
            {
                this.content = content;
                this.value = value;
                this.options = options;
                this.prefsKey = this.content.text;
                if (this.options == null)
                    this.options = new GUILayoutOption[] { };
            }

            public string PrefsKey { get { return this.prefsKey; } }

            public GUIContent Content { get { return this.content; } }

            public GUILayoutOption[] Options { get { return this.options; } }

            public T Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }

        public class CheckBoxData : PropertyData<bool>
        {
            public CheckBoxData(string text, string tooltip, bool value) : this(new GUIContent(text, tooltip), value, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value) : this(content, value, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value, GUILayoutOption[] options) : base(content, value, options)
            {
            }
        }

        public class CheckBoxData<T> : PropertyData<bool>
        {
            protected T tag;

            public CheckBoxData(string text, string tooltip, bool value, T tag) : this(new GUIContent(text, tooltip), value, tag, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value, T tag) : this(content, value, tag, null)
            {
            }

            public CheckBoxData(GUIContent content, bool value, T tag, GUILayoutOption[] options) : base(content, value, options)
            {
                this.tag = tag;
            }

            public virtual T Tag
            {
                get { return this.tag; }
                set { this.tag = value; }
            }
        }
    }
}

using Loxodon.Framework.Bundles.Archives;
using Loxodon.Framework.Bundles.Redundancy;
using Loxodon.Framework.Editors;
using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Loxodon.Framework.Bundles.Editors
{
#pragma warning disable 0414, 0219
    public class RedundancyAnalysisPanel : Panel
    {
        private const string BUNDLE_ROOT_KEY = "Loxodon::Framework::Bundle::ROOT";
        private ProgressBar progressBar;

        private BuildVM buildVM;
        private bool cancel = false;
        private Vector2 scrollPosition;

        public RedundancyAnalysisPanel(EditorWindow parent, BuildVM buildVM) : base(parent)
        {
            this.buildVM = buildVM;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.BeginArea(rect);
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
            if (GUILayout.Button("Open Folder"))
            {
                var dir = new DirectoryInfo(EditorPrefs.GetString(BUNDLE_ROOT_KEY, @".\"));
                var path = EditorUtility.OpenFolderPanel("AssetBundle Folder", dir.Parent.FullName, dir.Name);
                if (!string.IsNullOrEmpty(path))
                {
                    EditorPrefs.SetString(BUNDLE_ROOT_KEY, path);
                    dir = new DirectoryInfo(path);
                    this.progressBar = new ProgressBar();
                    EditorExecutors.RunOnCoroutine(DoTask(dir));
                }
            }
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            if (this.progressBar != null && this.progressBar.Enable)
            {
                if (EditorUtility.DisplayCancelableProgressBar(progressBar.Title, progressBar.Tip, progressBar.Progress))
                {
                    this.cancel = true;
                }
            }
            else
            {
                EditorUtility.ClearProgressBar();
                this.progressBar = null;
            }
        }

        IEnumerator DoTask(DirectoryInfo dir)
        {
            long total = 0;
            System.Diagnostics.Stopwatch w = new System.Diagnostics.Stopwatch();
            w.Start();

            var container = new ArchiveContainer();

            progressBar.Progress = 0f;
            progressBar.Enable = true;
            progressBar.Title = "Loading";
            progressBar.TipFormat = "Loading AssetBundle, please wait.Progress: {0:0.00} %";
            var loadResult = container.Load(dir, buildVM.GetRijndaelCryptograph());
            while (!loadResult.IsDone)
            {
                if (cancel)
                    loadResult.Cancel();

                progressBar.Progress = loadResult.Progress;

                yield return null;
            }
            w.Stop();
            total += w.ElapsedMilliseconds;
            Debug.LogFormat("loading time: {0} milliseconds", w.ElapsedMilliseconds);

            if (loadResult.Exception != null)
            {
                Debug.LogErrorFormat("{0}", loadResult.Exception);

                progressBar.Enable = false;
                if (container != null)
                {
                    container.Dispose();
                    container = null;
                }
                yield break;
            }

            w.Reset();
            w.Start();

            var analyzer = new RedundancyAnalyzer(container);
            progressBar.Progress = 0f;
            progressBar.Enable = true;
            progressBar.Title = "Redundancy Analysis";
            progressBar.TipFormat = "Analyzing asset redundancy,please wait.Progress: {0:0.00} %";
            var analyzeResult = analyzer.AnalyzeRedundancy();

            while (!analyzeResult.IsDone)
            {
                if (cancel)
                    analyzeResult.Cancel();

                progressBar.Progress = analyzeResult.Progress;

                yield return null;
            }

            if (analyzeResult.Exception != null)
            {
                Debug.LogErrorFormat("{0}", analyzeResult.Exception);

                progressBar.Enable = false;
                if (container != null)
                {
                    container.Dispose();
                    container = null;
                }
                yield break;
            }
            w.Stop();
            total += w.ElapsedMilliseconds;
            Debug.LogFormat("analyzing time: {0} milliseconds", w.ElapsedMilliseconds);
            Debug.LogFormat("total time: {0} milliseconds", total);

            RedundancyReport report = analyzeResult.Result;
            FileInfo fileInfo = new FileInfo(string.Format(@"{0}\RedundancyReport-{1}.csv", dir.Parent.FullName, dir.Name));
            string text = ToCSV(report);
            File.WriteAllText(fileInfo.FullName, text);

            EditorApplication.delayCall += () =>
            {
                try
                {
                    //open the folder 
                    EditorUtility.OpenWithDefaultApp(fileInfo.Directory.FullName);
                }
                catch (Exception) { }
            };

            progressBar.Enable = false;
            if (container != null)
            {
                container.Dispose();
                container = null;
            }
            analyzer = null;
        }

        /// <summary>
        /// 转为CSV文件格式.
        /// </summary>
        /// <returns>The bundleInfo to CSV.</returns>
        /// <param name="bundleInfos">List.</param>
        protected virtual string ToCSV(RedundancyReport report)
        {
            StringBuilder buf = new StringBuilder();
            buf.Append("\"Name\"").Append(",");
            buf.Append("\"TypeID\"").Append(",");
            buf.Append("\"FileSize\"").Append(",");
            buf.Append("\"Count\"").Append(",");
            buf.Append("\"Bundles\"").Append("\r\n");

            foreach (var info in report.GetAllRedundancyInfo())
            {
                buf.Append("\"").Append(info.Name).Append("\"").Append(",");
                buf.Append("\"").Append(info.TypeID).Append("\"").Append(",");
                buf.Append("\"").Append(info.FileSize).Append("\"").Append(",");
                buf.Append("\"").Append(info.Count).Append("\"").Append(",");

                buf.Append("\"");
                var bundles = info.Bundles;
                int count = bundles.Count;
                for (int i = 0; i < count; i++)
                {
                    var bundle = bundles[i];
                    if (i < count - 1)
                        buf.AppendFormat("{0}, ", bundle.Name);
                    else
                        buf.AppendFormat("{0} ", bundle.Name);
                }
                buf.Append("\"").Append("\r\n");
            }

            buf.Append("\r\n");

            buf.Append("\"").Append("Total Size").Append("\"").Append(",");
            buf.Append("\"").Append("Redundancy Size").Append("\"").Append(",");
            buf.Append("\"").Append("Redundancy Count").Append("\"").Append(",");
            buf.Append("\"").Append("Redundancy Percentage").Append("\"").Append("\r\n");

            buf.Append("\"").Append(report.TotalSize / (float)1048576).Append(" MB\"").Append(",");
            buf.Append("\"").Append(report.RedundantSize / (float)1048576).Append(" MB\"").Append(",");
            buf.Append("\"").Append(report.GetAllRedundancyInfo().Count).Append(" \"").Append(",");
            buf.Append("\"").AppendFormat("{0:0.00}", ((double)report.RedundantSize / report.TotalSize) * 100).Append(" %\"").Append("\r\n");

            return buf.ToString();
        }
    }
}
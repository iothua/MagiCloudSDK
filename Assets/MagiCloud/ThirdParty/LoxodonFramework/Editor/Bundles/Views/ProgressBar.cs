namespace Loxodon.Framework.Bundles.Editors
{
    public class ProgressBar
    {
        public string TipFormat { get; set; }

        public bool Enable { get; set; }

        public float Progress { get; set; }

        public string Title { get; set; }

        public string Tip { get { return string.Format(TipFormat, this.Progress * 100); } }

        public ProgressBar()
        {
            this.Title = "";
            this.TipFormat = "Progress: {0:0.00} %";
            this.Enable = true;
        }

        public ProgressBar(string title, string tipFormat)
        {
            this.TipFormat = tipFormat;
            this.Title = title;
            this.Enable = true;
        }
    }
}

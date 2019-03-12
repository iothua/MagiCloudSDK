namespace Loxodon.Framework.Bundles
{
    public static class Path
    {
        public static string GetExtension(string path)
        {
            int index = path.IndexOf('.');
            if (index < 0)
                return string.Empty;
            return path.Substring(index + 1);
        }

        public static string GetFilePathWithoutExtension(string path)
        {
            int index = path.IndexOf('.');
            if (index < 0)
                return path;
            return path.Substring(0, index);
        }

        public static string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public static string GetFileNameWithoutExtension(string path)
        {
            return System.IO.Path.GetFileNameWithoutExtension(path);
        }

    }
}

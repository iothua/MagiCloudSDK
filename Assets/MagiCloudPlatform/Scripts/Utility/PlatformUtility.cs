using UnityEngine;
using System.IO;

namespace MagiCloudPlatform
{
    public static class PlatformUtility
    {
        public static Texture2D GetTexture2DFromPath(string imgPath,int width,int height)
        {
            if (!File.Exists(imgPath)) return null;

            using (FileStream fs = new FileStream(imgPath, FileMode.Open, FileAccess.Read))
            {
                int byteLength = (int)fs.Length;
                byte[] imgBytes = new byte[byteLength];
                fs.Read(imgBytes, 0, byteLength);
                fs.Close();
                fs.Dispose();

                Texture2D texture = new Texture2D(width, height);
                texture.LoadImage(imgBytes);
                texture.Apply();

                return texture;
            }
        }

        public static Sprite GetSpriteFromPath(string imgPath, int width, int height)
        {
            return GetSpriteFromTexture(GetTexture2DFromPath(imgPath, width, height));
        }

        public static Sprite GetSpriteFromTexture(Texture2D texture)
        {
            if (texture == null) return null;

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            return sprite;
        }
    }
}

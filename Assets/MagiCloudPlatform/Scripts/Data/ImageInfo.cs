using System;
using UnityEngine;

namespace MagiCloudPlatform.Data
{
    public class ImageInfo
    {
        public int ID { get; set; }

        public string ImagePath { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        private Sprite spritePath;
        public Sprite SpritePath {
            get {
                if (spritePath == null)
                {
                    spritePath = PlatformUtility.GetSpriteFromPath(ImagePath, Width, Height);
                }

                return spritePath;
            }
        }

        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }
}

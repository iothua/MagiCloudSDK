using System;

namespace MagiCloud
{
    [Serializable]
    public class MagiCloudTagInfo
    {
        public string TagID;
        public string Use;
    }

    [Serializable]
    public class MagiCloudLayerInfo
    {
        public string LayerID;
        public string Use;
    }

    public enum HighlightType
    {
        Shader,
        Model
    }

    public enum AxisLimits
    {
        None,
        X,
        Y,
        Z
    }
}





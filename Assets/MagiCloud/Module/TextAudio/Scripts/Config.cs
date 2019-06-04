namespace MagiCloud.TextToAudio
{
    public class Config
    {
        public string appID;

        public Config(string appID)
        {
            this.appID=appID;
        }
        public override string ToString()
        {
            return string.Format("appid={0}",appID);
        }
    }
}

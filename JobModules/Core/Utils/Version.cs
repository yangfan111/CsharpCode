namespace Core.Utils
{
    public class Version
    {
        private static Version instance = null;

        private Version()
        {
        }

        public static Version Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new Version();
                }

                return instance;
            }
        }

        public string LocalVersion = "localTest";
        public string LocalAsset = "localTest";
        public string RemoteVersion = "remoteTest";
        public string RemoteAsset = "remoteTest";
    }
}
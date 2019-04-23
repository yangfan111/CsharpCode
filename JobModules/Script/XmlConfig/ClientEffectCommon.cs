namespace XmlConfig
{
    public class ClientEffectCommonConfig
    {
        public ClientEffectCommonConfigItem[] Items;
    }

    public class ClientEffectCommonConfigItem
    {
        public EClientEffectType Type;
        public int ServerLimit;
        public int ClientLimit;
        public int LifeTime;
    }

}

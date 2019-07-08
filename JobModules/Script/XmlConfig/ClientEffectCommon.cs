namespace XmlConfig
{
    public class ClientEffectCommonConfig
    {
        public ClientEffectCommonConfigItem[] Items;
    }

    public class ClientEffectCommonConfigItem
    {
        public EEffectObjectClassify Type;
        public int ServerLimit;
        public int ClientLimit;
        public int LifeTime;
        public float CutoffThreshold;
        public int ObjectLimit;
        public int PreLoadCfgId;
        public int Velocity;//只对移动特效
        public float Delay;

    }

}

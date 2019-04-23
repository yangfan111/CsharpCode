namespace XmlConfig
{
    public class PlayerSoundConfig
    {
        public PlayerSoundConfigItem[] Items; 
    }

    public class PlayerSoundConfigItem
    {
        public EPlayerSoundType SoundType;
        public string Id;
    }
}
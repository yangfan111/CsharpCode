namespace XmlConfig
{
    public class EnvironmentTypeConfig
    {
        public EnvironmentTypeConfigItem[] Items;
        public EnvironmentTypeFactorItem[] Factors;
    }

    public class EnvironmentTypeConfigItem
    {
        public string Name;
        public EEnvironmentType Type; 
    }

    public class EnvironmentTypeFactorItem
    {
        public EEnvironmentType Type;
        public float EnergyDecay;
        public float DamageDecay; 
    }
}

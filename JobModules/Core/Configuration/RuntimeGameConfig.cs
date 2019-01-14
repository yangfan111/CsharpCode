namespace Core.Configuration
{
    public class RuntimeGameConfig
    {
        public RuntimeGameConfig()
        {
            WeaponDropHSpeed = 5;
            WeaponDropVSpeed = -10f;
            WeaponDropOffset = 2;
            SceneWeaponLifetime = 20000;
            BagLimitTime = 30000;
        }

        public float WeaponDropHSpeed;
        public float WeaponDropVSpeed;
        public float WeaponDropOffset;
        public int SceneWeaponLifetime;
        public int BagLimitTime;
    }
}

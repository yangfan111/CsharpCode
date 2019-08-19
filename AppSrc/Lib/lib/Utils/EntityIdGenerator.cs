namespace YF.Utils
{
    public class EntityIdGenerator
    {
        public const int LocalBaseId = 0x1EFFFFFF;
        public const int GlobalBaseId = 0x1;
        public const int EquipmentBaseId = 0x0EFFFFFF;

        private int _initId;

        public EntityIdGenerator(int initId)
        {
            _initId = initId;
        }

        public int GetNextEntityId()
        {
            return _initId++;
        }
    }
}
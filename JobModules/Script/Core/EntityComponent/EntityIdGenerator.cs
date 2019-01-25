namespace App.Shared.Components
{
    public class EntityIdGenerator: IEntityIdGenerator
    {
        public const int LocalBaseId = 0x1EFFFFFF;
        public const int GlobalBaseId = 0x1;
        public const int EquipmentBaseId = 0x0EFFFFFF;

        public EntityIdGenerator(int initId)
        {
            _initId = initId;
        }

        private int _initId;
        public int GetNextEntityId()
        {
            return _initId++;
        }
    }
}
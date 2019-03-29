using UnityEngine;

namespace Core.Configuration.Equipment
{
    public interface IEquipmentInitilizerHandler
    {
        void CreateEquipmentEntity(Vector3 position, int itemId, int count);
    }
}
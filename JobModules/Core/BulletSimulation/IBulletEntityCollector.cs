using System.Collections.Generic;

namespace Core.BulletSimulation
{
    public interface IBulletEntityCollector
    {
        List<IBulletEntity> GetAllBulletEntities();
    }
}
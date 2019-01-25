using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.EntityComponent;
using Entitas;

namespace App.Shared.Components.Vehicle
{
    public struct VehicleCollisionDamage
    {
        public float VehicleDamage;
        public int VehicleDamageCount;

        public float PassagerDamage;
        public int PassagerDamageCount;


        public float AverageVehicleDamage
        {
            get
            {
                if (VehicleDamageCount > 0)
                {
                    return VehicleDamage / VehicleDamageCount;
                }

                return 0.0f;
            }
        }

        public float AveragePassagerDamage
        {
            get
            {
                if (PassagerDamageCount > 0)
                {
                    return PassagerDamage / PassagerDamageCount;
                }

                return 0.0f;
            }
        }

        public void AddVehicleDamage(float damage)
        {
            VehicleDamage += damage;
            VehicleDamageCount++;
        }

        public void AddPassagerDamage(float damage)
        {
            PassagerDamage += damage;
            PassagerDamageCount++;
        }
    }

    [Vehicle]
    public class VehicleCollisionDamageComponent : IResetableComponent, IComponent, IVehicleResetableComponent
    {
        public Dictionary<EntityKey, VehicleCollisionDamage> CollisionDamages = new Dictionary<EntityKey, VehicleCollisionDamage>(new EntityKeyComparer());

        public void Reset()
        {
            CollisionDamages.Clear();
        }
    }
}

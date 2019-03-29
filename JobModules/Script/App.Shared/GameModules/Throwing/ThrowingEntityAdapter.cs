using Core.EntityComponent;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Throwing
{
    public class ThrowingEntityAdapter
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ThrowingEntityAdapter));
        private ThrowingEntity _throwingEntity;

        public ThrowingEntityAdapter(ThrowingEntity throwingEntity)
        {
            _throwingEntity = throwingEntity;
        }

        public void SetEntity(ThrowingEntity throwingEntity)
        {
            _throwingEntity = throwingEntity;
        }

        public EntityKey OwnerEntityKey { get { return _throwingEntity.ownerId.Value; } }
        public Vector3 Origin { get { return _throwingEntity.position.Value; } set { _throwingEntity.position.Value = value; } }
        public Vector3 Velocity { get { return _throwingEntity.throwingData.Velocity; } set { _throwingEntity.throwingData.Velocity = value; } }
        public float Gravity { get { return _throwingEntity.throwingData.Config.Gravity; } }
        public float VelocityDecay { get { return _throwingEntity.throwingData.Config.VelocityDecay; } }
        public int RemainFrameTime { get { return _throwingEntity.throwingData.RemainFrameTime; } set { _throwingEntity.throwingData.RemainFrameTime = value; } }
        public int ServerTime { get { return _throwingEntity.throwingData.ServerTime; } set { _throwingEntity.throwingData.ServerTime = value; } }
        public bool IsValid { get { return !_throwingEntity.isFlagDestroy; } set { _throwingEntity.isFlagDestroy = !value; } }

        public float BaseDamage { get { return _throwingEntity.throwingData.Config.BaseDamage; } }
        public float DamageRadius { get { return _throwingEntity.throwingData.Config.DamageRadius; } }
        
    }
}
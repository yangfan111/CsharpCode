using System.Collections.Generic;
using App.Shared.Components.Common;
using App.Shared.Components.Player;
using BehaviorDesigner.Runtime.Tasks.Basic.UnityTime;
using Core.Compensation;
using Core.Components;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.HitBox;
using Core.Utils;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.GameModules.Attack
{
    public interface IHitBoxContext
    {
        Vector3         GetPosition(EntityKey entityKey);
        float           GetRadius(EntityKey entityKey);
        void            EnableHitBox(EntityKey entityKey, bool enalbe);
        List<Transform> GetCollidersTransform(EntityKey entityKey);

        void UpdateHitBox(IGameEntity gameEntity, int renderTime, int cmdSeq);
        void RecoverHitBox(IGameEntity gameEntity, int renderTime);
    }

    public class HitBoxEntityManager : IHitBoxEntityManager
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(HitBoxEntityManager));

        private IHitBoxContext[] _subManagers;

        public HitBoxEntityManager(Contexts contexts, bool isServer)
        {
            _subManagers = new IHitBoxContext[(int) EEntityType.End];
            if (isServer)
                _subManagers[(int) EEntityType.Player] = new ServerPlayerHitBoxContext(contexts.player);
            else
                _subManagers[(int) EEntityType.Player] = new PlayerHitBoxContext(contexts.player);
            _subManagers[(int) EEntityType.Vehicle] = new VehicleHitBoxContext(contexts.vehicle);
        }

        private Vector3 GetPosition(IGameEntity gameEntity)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
                return subManager.GetPosition(gameEntity.EntityKey);
            return gameEntity.Position.Value;
        }

        private float GetRadius(IGameEntity gameEntity)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
                return subManager.GetRadius(gameEntity.EntityKey);
            return 0;
        }
        /*
            _subManagers = new IHitBoxContext[(int) EEntityType.End];
            if (isServer)
                _subManagers[(int) EEntityType.Player] = new ServerPlayerHitBoxContext(contexts.player);
            else
                _subManagers[(int) EEntityType.Player] = new PlayerHitBoxContext(contexts.player);
            _subManagers[(int) EEntityType.Vehicle] = new VehicleHitBoxContext(contexts.vehicle);
        */
        public bool GetPositionAndRadius(IGameEntity gameEntity, out Vector3 position, out float radius)
        {
            var pos        = gameEntity.Position.Value;
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
            {
                //        return entity.hitBox.HitPreliminaryGeo.position;
                position = subManager.GetPosition(gameEntity.EntityKey) + pos;
                radius   = subManager.GetRadius(gameEntity.EntityKey);
                return true;
            }

            {
                position = Vector3.zero;
                radius   = 0;
                return false;
            }
        }

        private IHitBoxContext GetSubManager(int type)
        {
            if (_subManagers.Length > type)
            {
                return _subManagers[type];
            }

            return null;
        }

        public void UpdateHitBox(IGameEntity gameEntity, int renderTime, int cmdSeq)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
            {
                subManager.UpdateHitBox(gameEntity, renderTime, cmdSeq);
            }
        }
        public void RecoverHitBox(IGameEntity gameEntity, int renderTime)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
            {
                subManager.RecoverHitBox(gameEntity, renderTime);
            }
        }
        public void DrawHitBoxOnBullet(IGameEntity gameEntity)
        {
            if (DebugConfig.DrawHitBoxOnBullet)
            {
                DrawHitBox(gameEntity, DebugConfig.DrawHitBoxDuration);
            }
        }

        public void DrawHitBoxOnFrame(IGameEntity gameEntity)
        {
            if (DebugConfig.DrawHitBoxOnFrame)
            {
                DrawHitBox(gameEntity, 0);
            }
        }

        

        public void DrawHitBox(IGameEntity gameEntity, float time)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
            {
                DebugDraw.DebugWireSphere(GetPosition(gameEntity), GetRadius(gameEntity), time);
                foreach (var item in subManager.GetCollidersTransform(gameEntity.EntityKey))
                {
                    HitBoxGameObjectUpdater.DrawBoundBox(item, time);
                }
            }
        }

        public void EnableHitBox(IGameEntity gameEntity, bool enable)
        {
            var subManager = GetSubManager(gameEntity.EntityType);
            if (subManager != null)
            {
                subManager.EnableHitBox(gameEntity.EntityKey, enable);
            }
        }

        public bool Raycast(Ray rayRay, out RaycastHit hitInfo, float rayLength, int hitboxLayerMask)
        {
            return Physics.Raycast(rayRay, out hitInfo, rayLength, hitboxLayerMask);
        }

        public bool BoxCast(BoxInfo box, out RaycastHit hitInfo, int hitboxLayerMask)
        {
            return Physics.BoxCast(box.Origin, box.HalfExtens, box.Direction, out hitInfo, box.Orientation, box.Length,
            hitboxLayerMask);
        }
    }
}
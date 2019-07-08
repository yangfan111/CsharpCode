using System.Collections.Generic;
using App.Shared.Components;
using Core.Attack;
using Core.Compensation;
using Core.Components;
using Core.HitBox;
using Core.Utils;
using UnityEngine;

namespace App.Shared.GameModules.Attack
{
    public class BulletHitHandler : IBulletHitHandler
    {
        public static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(BulletHitHandler));
        private int _hitLayerMask;

        private List<AbstractHitHandler> hitTargetHandlers = new List<AbstractHitHandler>(3);


        private PlayerContext playerContext;
        public BulletHitHandler(Contexts contexts, IEntityIdGenerator entityIdGenerator, IPlayerDamager damager)
        {
            AbstractHitHandler handler = new HitHumanHandler(damager, contexts);
            hitTargetHandlers.Add(handler);
            handler = new HitVehicleHandler(contexts);
            hitTargetHandlers.Add(handler);
            handler = new HitEnvironmentHandler(contexts);
            hitTargetHandlers.Add(handler);
            playerContext = contexts.player;
        }

        public IBulletHitHandler SetHitLayerMask(int hitLayerMask)
        {
            _hitLayerMask = hitLayerMask;
            return this;
        }

        public virtual void OnHit(int cmdSeq, IBulletEntityAgent bulletEntityAgent, RaycastHit hit,
                                  CompensationWorld compensationWorld)
        {
            bulletEntityAgent.HitPoint = new PrecisionsVector3(hit.point, 3);
           
            Collider collider = hit.collider;
            if (collider == null)
            {
                _logger.ErrorFormat("[Hit{1}]bullet hit unknown collier {0}", bulletEntityAgent.OwnerEntityKey, cmdSeq);
                return;
            }
            PlayerEntity srcPlayer = playerContext.GetEntityWithEntityKey(bulletEntityAgent.OwnerEntityKey);
            if (srcPlayer == null)
            {
                _logger.WarnFormat("[Hit{3}]bullet from unkown {0} hit environment {1}, collier {2}",
                    bulletEntityAgent.OwnerEntityKey, hit.point, collider.name, cmdSeq);
                return;
            }
            var comp = hit.collider.transform.gameObject.GetComponent<HitBoxOwnerComponent>();
            foreach (var handler in hitTargetHandlers)
            {
                if (handler.ProcessHitTarget(comp, cmdSeq, compensationWorld, bulletEntityAgent, hit, srcPlayer))
                {
                    return;
                }
            }
        }
        
    }
}
﻿using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Components.Player;
using Core.Utils;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Ui.Logic
{
    public abstract class AbstractCastLogic : ICastLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractCastLogic)); 

        public virtual string Tip { get; protected set; }
        private float _maxDistance;
        private const float _groundOffset = 0.5f;
        protected PlayerContext _playerContext;

        public AbstractCastLogic(PlayerContext player, float maxDistance)
        {
            _maxDistance = maxDistance;
            _playerContext = player;
        }

        public abstract void OnAction();
        protected abstract void DoSetData(PointerData data);


        public void Action()
        {
            OnAction();
        }

        protected bool IsUseActionEnabled()
        {
            var player = _playerContext.flagSelfEntity;
            var states = player.StateInteractController().GetCurrStates();
            var manager = SingletonManager.Get<StateTransitionConfigManager>();
            foreach(var state in states)
            {
                StateTransitionConfigItem condition = manager.GetConditionByState(state);
                if (condition == null) continue;
                if(!condition.GetTransition(Transition.IsUseAction)/*IsUseAction*/)
                {
                    if(Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("use action is limited by {0}", condition.State);
                    }
                    return false;
                }
                else
                {
                    if(Logger.IsDebugEnabled)
                    {
                        Logger.DebugFormat("use action is allowed by {0}", condition.State);
                    }
                }
            }
            return true;
        }

        public void SetData(PointerData data)
        {
            Tip = "";
            var useActionEnalbed = IsUseActionEnabled();
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("IsUserActionEnabled {0}", useActionEnalbed);
            }
            if(!useActionEnalbed)
            {
                return;
            }
            var player = _playerContext.flagSelfEntity;
            if(null == player || player.gamePlay.LifeState == (int)EPlayerLifeState.Dead || player.gamePlay.TipHideStatus)
            {
                return;
            }
            DoSetData(data);
        }

        protected bool IsUntouchableOffGround(PlayerEntity player, Vector3 hitPoint, GameObject item)
        {
            bool untouchable = HasObstacle(player, hitPoint, item) || OutOfRange(player, hitPoint, item);
            return untouchable; 
        }

        private bool HasObstacle(PlayerEntity player, Vector3 targetCenter, GameObject item)
        {
            var hasObstacle = CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, targetCenter, item);
            if (hasObstacle)
            {
                Logger.DebugFormat("There must be obstacle(s) between player and {0}", item.name);
            }
            return hasObstacle; 
        }

        private bool OutOfRange(PlayerEntity player, Vector3 hitPoint, GameObject item)
        {
            var xzHitPoint = new Vector3(hitPoint.x, player.position.Value.y, hitPoint.z);
            var dis = Vector3.Distance(xzHitPoint, player.position.Value);
            var outOfrange = dis > _maxDistance;
            if (outOfrange)
            {
                Logger.DebugFormat("{0} is out of range", item.name);
            }
            return outOfrange;
        }

        public virtual void Clear()
        {
            if(_playerContext.flagSelfEntity.hasRaycastTarget)
            {
                _playerContext.flagSelfEntity.raycastTarget.Key = 0;
            }
        }
    }
}
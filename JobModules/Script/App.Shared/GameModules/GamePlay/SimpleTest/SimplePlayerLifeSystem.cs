using System.Collections.Generic;
using App.Shared.Configuration;
using App.Shared.Components.Player;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using UnityEngine;
using Random = System.Random;
using Core.Configuration.Equipment;
using Core.GameTime;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.GamePlay.SimpleTest
{
    public class SimplePlayerLifeSystem : ReactiveGamePlaySystem<PlayerEntity>, IGamePlaySystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SimplePlayerLifeSystem));
        private IGroup<PlayerEntity> _players;
        private Contexts _contexts;
        private ICurrentTime _currentTimeObject;

      
        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.GamePlay);
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }



        public SimplePlayerLifeSystem(Contexts contexts, ICurrentTime currentTimeObject) : base(contexts.player)
        {
            _players = contexts.player.GetGroup(PlayerMatcher.GamePlay);
            _currentTimeObject = currentTimeObject;
            _contexts = contexts;
        }

        public override void SingleExecute(PlayerEntity entity)
        {
           entity.gamePlay.MaxHp = 100;
                Rebirth(entity, _contexts, _currentTimeObject.CurrentTime, SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition);
        }

       

        Random r = new Random();
        public void OnGamePlay()
        {
            foreach (var playerEntity in _players)
            {
                var gp = playerEntity.gamePlay;
                var time = _currentTimeObject.CurrentTime;
                if (playerEntity.gamePlay.CurHp > 0 )
                {
                    if (!gp.IsLifeState(EPlayerLifeState.Alive))
                    {
                        _logger.InfoFormat("{0} change to alive", playerEntity.entityKey);
                        gp.ChangeLifeState(EPlayerLifeState.Alive, time);
                    }
                }
                else
                {
                    if (gp.IsLifeState(EPlayerLifeState.Alive))
                    {
                        _logger.InfoFormat("{0} change to dying", playerEntity.entityKey);
                        gp.ChangeLifeState(EPlayerLifeState.Dying, time);
                    }
                    else if (gp.IsLifeState(EPlayerLifeState.Dying) && gp.IsLifeChangeOlderThan(time - 500000))
                    {
                        if (r.Next(2) == 0)
                        {
                            _logger.InfoFormat("{0} change to dead", playerEntity.entityKey);
                            gp.ChangeLifeState(EPlayerLifeState.Dead, time);
                        }
                        else
                        {
                            
                            Rebirth(playerEntity, _contexts, time, SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition);
                        }
                    }
                    else if (gp.IsLifeState(EPlayerLifeState.Dead) && gp.IsLifeChangeOlderThan(time - 500000))
                    {
                        Rebirth(playerEntity, _contexts, time, SingletonManager.Get<MapConfigManager>().SceneParameters.PlayerBirthPosition);
                    }
                }
            }
        }

        private static void Rebirth(PlayerEntity playerEntity, Contexts contexts, int time, Vector3 pos)
        {
            _logger.InfoFormat("{0} rebirth", playerEntity.entityKey);
            playerEntity.gamePlay.ChangeLifeState(EPlayerLifeState.Alive, time);
            playerEntity.gamePlay.CurHp = 100;
            playerEntity.position.Value = pos;
            PlayerWeaponController controller = playerEntity.WeaponController();
            var configAssy = controller.HeldWeaponLogicConfigAssy;
            if (configAssy == null)
                return;
            controller.ModifyBullet(configAssy.CommonFireCfg.MagazineCapacity);

        }
    }
}
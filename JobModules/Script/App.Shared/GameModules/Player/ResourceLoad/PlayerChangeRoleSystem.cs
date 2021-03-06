﻿using App.Shared.FreeFramework.Free.player;
using App.Shared.Player;
using Core.GameModule.System;
using Core.Utils;
using Entitas;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils.AssetManager;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Shared.GameModules.Player.ResourceLoad
{
    public class PlayerChangeRoleSystem : ReactiveResourceLoadSystem<PlayerEntity>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerChangeRoleSystem));
        private readonly PlayerContext _player;
        private readonly Contexts _contexts;
        private readonly FirstPersonModelLoadHandler _p1Handler;
        private readonly ThirdPersonModelLoadHandler _p3Handler;

        private readonly Dictionary<int, bool> _animationFinished = new Dictionary<int, bool>();
        private readonly Dictionary<int, UnityObject> _p3Objs = new Dictionary<int, UnityObject>();
        private readonly Dictionary<int, UnityObject> _p1Objs = new Dictionary<int, UnityObject>();

        public PlayerChangeRoleSystem(Contexts contexts) : base(contexts.player)
        {
            _contexts = contexts;
            _player = contexts.player;
            _p1Handler = new FirstPersonModelLoadHandler(contexts);
            _p3Handler = new ThirdPersonModelLoadHandler(contexts);
        }

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.Position.Added());
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return true;
        }

        public override void SingleExecute(PlayerEntity entity)
        {
        }

        public override void OnLoadResources(IUnityAssetManager assetManager)
        {
            base.OnLoadResources(assetManager);

            foreach (var entity in _player.GetEntities())
            {
                ChangeRoleByGamePlayData(entity);
                ChangeRoleByCmd(entity);

                AssetLoadSuccess(entity);
            }
        }

        private void ChangeRoleByGamePlayData(PlayerEntity player)
        {
            if (!player.hasGamePlay) return;
            var gamePlay = player.gamePlay;

            if (!gamePlay.HasNewRoleIdChangedFlag()) return;
            ChangeRole(player, gamePlay.NewRoleId);
            gamePlay.ClearNewRoleIdChangedFlag();
        }

        private void ChangeRoleByCmd(PlayerEntity player)
        {
            if (!player.isFlagSelf || !SharedConfig.ChangeRole) return;
            ChangeRole(player, 100);
            SharedConfig.ChangeRole = false;
        }

        private void ChangeRole(PlayerEntity player, int roleId)
        {
            if(player.hasGamePlay)
            Logger.InfoFormat("NewRoleId: {0}, LastRoleId: {1}", roleId, player.gamePlay.LastNewRoleId);
            
            if (!player.hasPlayerInfo ||
                !AssetConfig.RoleIdExit(roleId)) return;

            player.playerInfo.ChangeNewRole(roleId);
            ChangeRoleInfo(player, roleId);
            ChangeCharacterControllerInfo(player);
            PlayChangeRoleAnimation(player);

            LoadModelAsset(player);
        }

        private void ChangeCharacterControllerInfo(PlayerEntity player)
        {
            if (player.isFlagSelf && player.hasCharacterInfo)
            {
                var info = player.characterInfo.CharacterInfoProviderContext;
                var controller = player.RootGo().GetComponent<CharacterController>();
                if (controller != null)
                {
                    controller.stepOffset = info.GetStepOffset();
                    controller.slopeLimit = info.GetSlopeLimit();
                }
            }
        }

        private void ChangeRoleInfo(PlayerEntity player, int roleId)
        {
            if (player.isFlagSelf && player.hasCharacterInfo)
            {
                var item = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId);
                if (item != null)
                {
                    player.characterInfo.CharacterInfoProviderContext.SetCurrentType(item.CharacterType);
                }
            }
        }

        private void PlayChangeRoleAnimation(PlayerEntity player)
        {
            if (!player.isFlagSelf || !player.hasStateInterface || !player.hasEntityKey) return;

            if (!NeedPlayChangeRoleAnimation(player))
            {
                _animationFinished[player.entityKey.Value.EntityId] = true;
                if (SharedConfig.IsServer && player.hasChangeRole)
                    player.changeRole.ChangeRoleAnimationFinished = true;
                return;
            }

            if (SharedConfig.IsServer && player.hasChangeRole)
                player.changeRole.ChangeRoleAnimationFinished = false;
            
            player.stateInterface.State.TransfigurationStart(() =>
            {
                _animationFinished[player.entityKey.Value.EntityId] = true;
                if (SharedConfig.IsServer && player.hasChangeRole)
                    player.changeRole.ChangeRoleAnimationFinished = true;
            });
        }

        private bool NeedPlayChangeRoleAnimation(PlayerEntity player)
        {
            if (!player.hasPlayerInfo) return false;
            
            var item = SingletonManager.Get<RoleConfigManager>().GetRoleItemById(player.playerInfo.RoleModelId);
            if (null == item) return false;

            return item.Unique;
        }

        private static void ClearPlayerData(PlayerEntity player)
        {
            if (!player.hasAppearanceInterface) return;

            var appearance = player.appearanceInterface.Appearance;
            appearance.ClearThirdPersonCharacter();
        }

        private static void ClearComponentData(PlayerEntity player)
        {
            if (!player.hasAppearanceInterface) return;
            
            var appearance = player.appearanceInterface.Appearance;
            appearance.Init();
            appearance.SyncPredictedTo(player.predictedAppearance);
            appearance.SyncClientTo(player.clientAppearance);

            if (!SharedConfig.IsServer) return;
            appearance.SyncLatestTo(player.latestAppearance);
        }

        private void AssetLoadSuccess(PlayerEntity player)
        {
            if (!player.hasEntityKey) return;
            var entityId = player.entityKey.Value.EntityId;

            if (_p1Objs.ContainsKey(entityId) && null != _p1Objs[entityId])
            {
                _p1Handler.OnLoadSucc(player, _p1Objs[entityId]);
                _p1Objs.Remove(entityId);
            }

            if (player.isFlagSelf)
            {
                if (_p3Objs.ContainsKey(entityId) && null != _p3Objs[entityId] &&
                    _animationFinished.ContainsKey(entityId) && _animationFinished[entityId])
                {
                    ClearPlayerData(player);
                    ClearComponentData(player);
                    _p3Handler.OnLoadSucc(player, _p3Objs[entityId]);

                    if (player.hasStateInterface && NeedPlayChangeRoleAnimation(player))
                        player.stateInterface.State.TransfigurationFinish(null);

                    _p3Objs.Remove(entityId);
                    _animationFinished.Remove(entityId);

                    IniAvatarAction.InitAvatar(_contexts, player);
                }
            }
            else
            {
                if (!player.hasChangeRole) return;
                if (_p3Objs.ContainsKey(entityId) && null != _p3Objs[entityId] &&
                    player.changeRole.ChangeRoleAnimationFinished)
                {
                    ClearPlayerData(player);
                    _p3Handler.OnLoadSucc(player, _p3Objs[entityId]);
                    _p3Objs.Remove(entityId);
                }
            }
        }

        private void LoadModelAsset(PlayerEntity player)
        {
            AssetManager.LoadAssetAsync(
                player,
                AssetConfig.GetCharacterModelAssetInfo(player.playerInfo.RoleModelId),
                P3ModelLoadSuccess);

            if (player.isFlagSelf && HasFirstPerson(player.playerInfo.RoleModelId))
            {
                AssetManager.LoadAssetAsync(
                    player,
                    AssetConfig.GetCharacterHandAssetInfo(player.playerInfo.RoleModelId),
                    P1ModelLoadSuccess);
            }

            Logger.InfoFormat("new client player entity {0}, id:{1}, avatarIds:{2}", player.entityKey,
                player.playerInfo.RoleModelId,
                string.Join(",", player.playerInfo.AvatarIds.Select(i => i.ToString()).ToArray()));
        }

        private void P3ModelLoadSuccess(PlayerEntity player, UnityObject unityObj)
        {
            if (!player.hasEntityKey) return;
            _p3Objs[player.entityKey.Value.EntityId] = unityObj;
            // 移除视野
            PlayerEntityUtility.SetVisibility(unityObj.AsGameObject, false);
        }


        private void P1ModelLoadSuccess(PlayerEntity player, UnityObject unityObj)
        {
            if (!player.hasEntityKey) return;
            _p1Objs[player.entityKey.Value.EntityId] = unityObj;
            // 移除视野
            PlayerEntityUtility.SetVisibility(unityObj.AsGameObject, false);
        }

        private static bool HasFirstPerson(int roleId)
        {
            return SingletonManager.Get<RoleConfigManager>().GetRoleItemById(roleId).HasFirstPerson;
        }
    }
}

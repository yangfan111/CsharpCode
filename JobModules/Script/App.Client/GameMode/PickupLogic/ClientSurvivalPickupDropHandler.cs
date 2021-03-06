﻿using App.Shared;
using App.Shared.GameMode;
using Core;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Free.framework;

namespace App.Client.GameMode
{
    /// <summary>
    /// Defines the <see cref="ClientSurvivalPickupDropHandler" />
    /// </summary>
    public class ClientSurvivalPickupDropHandler : SurvivalPickupDropHandler
    {

        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientSurvivalPickupDropHandler));

        private IUserCmdGenerator _userCmdGenerator;

        private Contexts _contexts;

        private PlayerEntity Player
        {
            get { return _contexts.player.flagSelfEntity; }
        }

        public ClientSurvivalPickupDropHandler(Contexts contexts, int modeId) : base(contexts, modeId)
        {
            _userCmdGenerator = contexts.session.clientSessionObjects.UserCmdGenerator;
            _contexts = contexts;
        }

        public override void SendPickup(IPlayerWeaponProcessor weaponProcessor, int entityId, int itemId, int category, int count)
        {

            var player = _contexts.player.flagSelfEntity;
            if (null == player)
            {
                return;
            }

            SimpleProto pickUp = FreePool.Allocate();
            pickUp.Key = FreeMessageConstant.PickUpItem;
            pickUp.Ins.Add(entityId);
            pickUp.Ins.Add(category);
            pickUp.Ins.Add(itemId);
            pickUp.Ins.Add(count);
            player.network.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, pickUp);
            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("send pick up message entityId {0}, category {1}, itemid {2}, count {3}", entityId, category, itemId, count);
            }
        }

        /*public override void SendAutoPickupWeapon(int entityId)
        {
            var target = _contexts.sceneObject.GetEntityWithEntityKey(new EntityKey(entityId, (short)EEntityType.SceneObject));
            if (target != null && Player.WeaponController().FilterAutoPickup(target.simpleItem.Id))
            {
                var model = target.hasUnityObject ? target.unityObject.UnityObject : target.multiUnityObject.FirstAsset;
                if (CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(Player, target.position.Value, model))
                {
                    return;
                }

                SimpleProto pickUp = FreePool.Allocate();
                pickUp.Key = FreeMessageConstant.PickUpItem;
                pickUp.Ins.Add(entityId);
                pickUp.Ins.Add(target.simpleItem.Category);
                pickUp.Ins.Add(target.simpleItem.Id);
                pickUp.Ins.Add(1);
                Player.network.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, pickUp);
            }
        }*/

        protected override void DoDropGrenade(PlayerEntity playerEntity, EWeaponSlotType slot, IUserCmd cmd)
        {
            _userCmdGenerator.SetUserCmd((userCmd) => userCmd.IsLeftAttack = true);
            playerEntity.WeaponController().AutoThrowing = true;
        }
    }
}

﻿using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.GameMode;
using Core;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Client.GameMode
{
    /// <summary>
    /// Defines the <see cref="ClientCommonPickupDropHandler" />
    /// </summary>
    public class ClientCommonPickupDropHandler : CommonPickupDropHandler
    {
        private IUserCmdGenerator _userCmdGenerator;

        private Contexts _contexts;

        private PlayerEntity Player
        {
            get { return _contexts.player.flagSelfEntity; }
        }


        public ClientCommonPickupDropHandler(Contexts contexts,int modeId) : base(contexts,modeId)
        {

            _contexts = contexts;
            _userCmdGenerator = contexts.session.clientSessionObjects.UserCmdGenerator;
        }

        public override void SendPickup(IPlayerWeaponProcessor weaponProcessor, int entityId, int itemId, int category, int count)
        {
            _userCmdGenerator.SetUserCmd((cmd) =>
            {
                cmd.IsManualPickUp = true;
                cmd.ManualPickUpEquip = entityId;
            });
        }

        protected override void DoDropGrenade(PlayerEntity playerEntity, EWeaponSlotType slot, IUserCmd cmd)
        {
            _userCmdGenerator.SetUserCmd((userCmd) => userCmd.IsLeftAttack = true);
            playerEntity.WeaponController().AutoThrowing = true;
        }

        /*public override void SendAutoPickupWeapon(int entityId)
        {
            var target = _contexts.sceneObject.GetEntityWithEntityKey(new EntityKey(entityId, (short)EEntityType.SceneObject));
            var model = target.hasUnityObject ? target.unityObject.UnityObject : target.multiUnityObject.FirstAsset;
            if (!CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(Player, target.position.Value, model))
            {
                _userCmdGenerator.SetUserCmd((cmd) =>
                {
                    if (!cmd.AutoPickUpEquip.Contains(entityId))
                        cmd.AutoPickUpEquip.Add(entityId);
                });
            }
        }*/
    }
}

using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Components.Player;
using Assets.XmlConfig;
using Core.EntityComponent;
using Core.EntityComponent;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.SessionState;
using Core.Utils;
using Free.framework;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerPickAndDropSystem : AbstractStepExecuteSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientPlayerPickAndDropSystem));

        private Contexts _contexts;
        private IUserCmdGenerator _userCmdGenerator;

        public ClientPlayerPickAndDropSystem(Contexts contexts)
        {
            _contexts = contexts;
            _userCmdGenerator = contexts.session.clientSessionObjects.UserCmdGenerator;
        }

        protected override void InternalExecute()
        {
            var player = _contexts.player.flagSelfEntity;
            var controller = player.WeaponController();
            if (!player.gamePlay.IsLifeState(EPlayerLifeState.Alive) || !player.gamePlay.CanAutoPick())
                return;
            
            Collider[] colliders = Physics.OverlapCapsule(player.position.Value, player.bones.Head.position,
                0.4f, UnityLayerManager.GetLayerMask(EUnityLayerName.UserInputRaycast));
            foreach (Collider collider in colliders)
            {
                var rcTar = collider.transform.GetComponentInParent<RayCastTarget>();
                if (rcTar == null) continue;
                if (SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(_contexts.session.commonSession.RoomInfo.ModeId) == EBagType.Chicken)
                {
                    var target = _contexts.sceneObject.GetEntityWithEntityKey(new EntityKey(rcTar.IdList[1], (short)EEntityType.SceneObject));
                    if (target.simpleItem.Category == (int) ECategory.Weapon && player.WeaponController().FilterAutoPickup(target.simpleItem.Id))
                    {
                        var model = target.hasUnityObject ? target.unityObject.UnityObject : target.multiUnityObject.FirstAsset;
                        if (CommonObjectCastUtil.HasObstacleBetweenPlayerAndItem(player, target.position.Value, model)) continue;

                        SimpleProto pickUp = FreePool.Allocate();
                        pickUp.Key = FreeMessageConstant.PickUpItem;
                        pickUp.Ins.Add(rcTar.IdList[1]);
                        pickUp.Ins.Add(target.simpleItem.Category);
                        pickUp.Ins.Add(target.simpleItem.Id);
                        pickUp.Ins.Add(1);
                        player.network.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, pickUp);
                    }
                }
                else
                {
                    _userCmdGenerator.SetUserCmd((userCmd) =>
                    {
                        if(!userCmd.AutoPickUpEquip.Contains(rcTar.IdList[1]))
                            userCmd.AutoPickUpEquip.Add(rcTar.IdList[1]);
                    });
                }
            }

            if (controller.AutoThrowing.HasValue && controller.AutoThrowing.Value)
            {
                if(null != _userCmdGenerator && controller.RelatedThrowAction.IsReady == true)
                {
                    _userCmdGenerator.SetUserCmd((userCmd) => { userCmd.IsThrowing = true; });
                    controller.AutoThrowing = false;
                }
            }
        }
    }
}

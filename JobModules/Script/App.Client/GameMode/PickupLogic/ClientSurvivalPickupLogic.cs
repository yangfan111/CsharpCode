using App.Shared;
using App.Shared.GameModeLogic.PickupLogic;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core;
using Core.Configuration;
using Core.Free;
using Core.Network;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Free.framework;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameMode.PickupLogic
{
    public class ClientSurvivalPickupLogic : SurvivalPickupLogic 
    {
        private ClientAutoPickupLogic _clientAutoPickupLogic;
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientSurvivalPickupLogic));
        private IUserCmdGenerator _cmdGenerator;
        private Contexts _contexts;
        public ClientSurvivalPickupLogic(IUserCmdGenerator cmdGenerator, Contexts contexts):base(contexts,
            contexts.session.entityFactoryObject.SceneObjectEntityFactory,contexts.session.commonSession.RuntimeGameConfig)
        {
            _cmdGenerator = cmdGenerator;
            _contexts = contexts;
            _clientAutoPickupLogic = new ClientAutoPickupLogic(_contexts, cmdGenerator);
        }

        public override void SendPickup(int entityId, int itemId, int category, int count)
        {
           
            var player = _contexts.player.flagSelfEntity;
            if(null == player)
            {
                return;
            }
            SimpleProto pickUp = FreePool.Allocate();
            _cmdGenerator.SetUserCmd((cmd) => cmd.IsManualPickUp = true);
            _cmdGenerator.SetUserCmd((cmd) => cmd.IsUseAction = true);
            pickUp.Key = FreeMessageConstant.PickUpItem;
            pickUp.Ins.Add(entityId);
            pickUp.Ins.Add(category);
            pickUp.Ins.Add(itemId);
            pickUp.Ins.Add(count);
            player.network.NetworkChannel.SendReliable((int)EClient2ServerMessage.FreeEvent, pickUp);
            if(Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("send pick up message entityId {0}, category {1}, itemid {2}, count {3}", entityId, category, itemId, count);
            }
       //    var soundManager = player.soundManager.Value;
            //switch((ECategory)category)
            //{
            //    case ECategory.Weapon:
            //        var weaponConfig = SingletonManager.Get<WeaponConfigManager>().GetConfigById(itemId);
            //        if(null != weaponConfig)
            //        {
            //            soundManager.PlayOnce(weaponConfig.PickSound);
            //        }
            //        break;
            //    case ECategory.WeaponPart:
            //        var partConfig = SingletonManager.Get<WeaponPartsConfigManager>().GetConfigById(itemId);
            //        if(null != partConfig)
            //        {
            //            soundManager.PlayOnce(partConfig.PickSound); 
            //        }
            //        break;
            //    case ECategory.GameItem:
            //        var gameItemConfig = SingletonManager.Get<GameItemConfigManager>().GetConfigById(itemId);
            //        if(null != gameItemConfig)
            //        {
            //            soundManager.PlayOnce(gameItemConfig.PickSound);
            //        }
            //        break;
            //    case ECategory.Avatar:
            //        var avatarConfig = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(itemId); 
            //        if(null != avatarConfig)
            //        {
            //            soundManager.PlayOnce(avatarConfig.PickSound);
            //        }
            //        break;
            //}
        }

        public override void SendAutoPickupWeapon(int entityId)
        {
            _clientAutoPickupLogic.SendAutoPickupWeapon(entityId);
        }
    }
}

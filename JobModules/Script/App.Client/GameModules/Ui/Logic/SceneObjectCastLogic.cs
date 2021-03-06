﻿using App.Client.CastObjectUtil;
using App.Shared;
using App.Shared.Components;
using App.Shared.Util;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using I2.Loc;
using UnityEngine;
using UserInputManager.Lib;
using Utils.Configuration;
using Utils.Singleton;

namespace App.Client.GameModules.Ui.Logic
{
    public class SceneObjectCastLogic : AbstractCastLogic 
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SceneObjectCastLogic));
        private SceneObjectContext _sceneObjectContext;
        private ClientSessionObjectsComponent _sessionObjectsComponent;
        private IUserCmdGenerator _cmdGenerator;

        public SceneObjectCastLogic(
            PlayerContext playerContext,
            SceneObjectContext sceneObjectContext,
            ClientSessionObjectsComponent sessionObjectsComponent,
            IUserCmdGenerator cmdGenerator,
            float maxDistance):base(playerContext, maxDistance)
        {
            _sceneObjectContext = sceneObjectContext;
            _sessionObjectsComponent = sessionObjectsComponent;
            _cmdGenerator = cmdGenerator;
        }

        private PointerData _pointerData;

        public override void OnAction()
        {
            var entityId = SceneObjCastData.EntityId(_pointerData.IdList);
            var category = SceneObjCastData.Category(_pointerData.IdList);
            var itemId = SceneObjCastData.ItemId(_pointerData.IdList);
            var count = SceneObjCastData.Count(_pointerData.IdList);
            var player = _playerContext.flagSelfEntity;
            if (null == player)
            {
                return;
            }
            Logger.Info("[Tmp]Interrupt");
            player.stateInterface.State.InterruptAction();
            player.stateInterface.State.PickUp();
            player.ModeController().SendPickup(player.WeaponController(),entityId, itemId, category, count);
        }

        protected override void DoSetData(PointerData data)
        {
            _pointerData = data;
            var entityId = SceneObjCastData.EntityId(_pointerData.IdList);
            var sceneObj = _sceneObjectContext.GetEntityWithEntityKey(new Core.EntityComponent.EntityKey(entityId, (short)EEntityType.SceneObject));
            if(null != sceneObj)
            {
                if (!sceneObj.hasPosition)
                {
                    Logger.Error("sceneobject has no postion");
                    return;
                }
                var player = _playerContext.flagSelfEntity;
                if(!player.hasPosition)
                {
                    Logger.Error("player has no position");
                    return;
                }
                if(IsUntouchable(sceneObj, player))
                {
                    return;
                }
                if(!sceneObj.IsCanPickUpByPlayer(player))
                {
                    return;
                }
                if (player.hasGamePlay && !player.gamePlay.CanAutoPick())
                {
                    return;
                }
            }
            else
            {
                return;
            }
            var itemId = SceneObjCastData.ItemId(_pointerData.IdList);
            var count = SceneObjCastData.Count(_pointerData.IdList);
            var category = SceneObjCastData.Category(_pointerData.IdList);
            
            switch ((ECategory)category)
            {
                case ECategory.Weapon:
                    var itemCfg = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(itemId);
                    if(null != itemCfg)
                    {
                        if (itemCfg.Type == (int) EWeaponType_Config.ThrowWeapon)
                        {
                            Tip = string.Format(ScriptLocalization.client_actiontip.pickup, itemCfg.Name, string.Format("({0})",count));
                        }
                        else
                        {
                            Tip = string.Format(ScriptLocalization.client_actiontip.pickandequip, itemCfg.Name);
                        }
                    }
                    else
                    {
                        Tip = string.Format("weapon {0} doesn't exist ", itemId);
                    }
                    break;
                case ECategory.Avatar:
                    var equip = SingletonManager.Get<RoleAvatarConfigManager>().GetConfigById(itemId);
                    if(null != equip)
                    {
                        Tip = string.Format(ScriptLocalization.client_actiontip.pickandequip, equip.Name);
                    }
                    else
                    {
                        Tip = string.Format("avatar {0} doesn't exist ", itemId);
                    }
                    break;
                case ECategory.WeaponPart:
                    var part = SingletonManager.Get<WeaponPartSurvivalConfigManager>().FindConfigBySetId(itemId);
                    if(null != part)
                    {
                        Tip = string.Format(ScriptLocalization.client_actiontip.pickup, part.Name, "");
                    }
                    else
                    {
                        Tip = string.Format("part {0} doesn't exist", itemId); 
                    }
                    break;
                case ECategory.GameItem:
                    var item = SingletonManager.Get<GameItemConfigManager>().GetConfigById(itemId);
                    if(null != item)
                    {
                        Tip = string.Format(ScriptLocalization.client_actiontip.pickup, item.Name, string.Format("({0})",count));
                    }
                    else
                    {
                        Tip = string.Format("item {0} doesn't exist" , itemId); 
                    }
                    break;
                default:
                    Tip = string.Format("EntityId {0} ItemId {1} do not exist in config ", entityId, itemId);
                    break;
            }
        }

        private bool IsUntouchable(SceneObjectEntity sceneObj, PlayerEntity player)
        {
            if (sceneObj.hasUnityObject)
            {
                if(IsUntouchableOffGround(player, sceneObj.position.Value, sceneObj.unityObject.UnityObject))
                {
                    return true;
                }
            }
            else if (sceneObj.hasMultiUnityObject)
            {
                if(null != sceneObj.multiUnityObject.FirstAsset)
                {
                    if(IsUntouchableOffGround(player, sceneObj.position.Value, sceneObj.multiUnityObject.FirstAsset as GameObject))
                    {
                        return true;
                    }
                }
                else
                {
                    Logger.Error("no first asset in multiUnityObject");
                }
            }
            else
            {
                Logger.Error("secneobj has no gameobject");
                return true;
            }
            return false;
        }
    }
}
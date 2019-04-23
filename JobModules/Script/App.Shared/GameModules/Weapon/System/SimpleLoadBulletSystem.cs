using System.Collections;
using System.Collections.Generic;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.@event;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.GameModules.Weapon;
using Assets.Utils.Configuration;
using Assets.XmlConfig;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Common;
using Core;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;
using App.Shared.Components;

namespace App.Shared.GameModules.Weapon
{
    //TODO 移到firelogic
    /// <summary>
    /// Defines the <see cref="SimpleLoadBulletSystem" />
    /// </summary>
    public class SimpleLoadBulletSystem : IUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SimpleLoadBulletSystem));

        private bool _reloading;

        private IFilteredInput _filteredResult;

        private Contexts _contexts;

        private ICommonSessionObjects _sessonObjects;

        public SimpleLoadBulletSystem(Contexts contexts, ICommonSessionObjects sessionObjects)
        {
            this._contexts = contexts;
            this._sessonObjects = sessionObjects;
        }

        private IEnumerator DelayReload()
        {
            yield return new WaitForSeconds(0.1f);
        }

        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            if (cmd.IsLeftAttack == true) return; //按住鼠标左键时不可换弹
            WeaponSystemImplStruct? implStruct =
                WeaponUtil.FilterWeaponSystemCmd(owner, cmd, WeaponSystemCmdUtil.FilterWeaponReload);
            WeaponSystemImplStruct impStructVal = new WeaponSystemImplStruct();

            if (implStruct.HasValue)
            {
                impStructVal = implStruct.Value;
            }
            else
            {
                if(player.WeaponController().HeldWeaponAgent.BaseComponent.Bullet > 0 
                   || GameRules.IsChicken(_contexts.session.commonSession.RoomInfo.ModeId) 
                   || player.WeaponController().HeldWeaponAgent.BaseComponent.ReservedBullet == 0)
                {
                    return;
                }
                else
                {
                    impStructVal = new WeaponSystemImplStruct();
                    impStructVal.playerEntity = player;
                    impStructVal.weaponController = player.WeaponController();
                }
            }
 
            //检查配置
            var heldAgent = impStructVal.weaponController.HeldWeaponAgent;
            if (heldAgent.WeaponConfigAssy.CanotReloadBullet || heldAgent.CommonFireCfg == null)
                return;
            //检查背弹
            var reservedBullet = impStructVal.weaponController.GetReservedBullet();
            if (reservedBullet < 1)
            {
                if (SharedConfig.CurrentGameMode == EGameMode.Normal)
                {
                    impStructVal.weaponController.ShowTip(ETipType.BulletRunout);
                }
                else
                {
                    impStructVal.weaponController.ShowTip(ETipType.NoBulletInPackage);
                }

                return;
            }

            //检查总弹量
            int maxSizeBulletCount = heldAgent.WeaponConfigAssy.PropertyCfg.Bullet;
            if (heldAgent.BaseComponent.Magazine > 0)
            {
                maxSizeBulletCount += SingletonManager
                                      .Get<WeaponPartsConfigManager>()
                                      .GetConfigById(heldAgent.BaseComponent.Magazine).Bullet;
            }

            if (heldAgent.BaseComponent.Bullet >= maxSizeBulletCount)
            {
                return;
            }


            if (!_reloading)
            {
                //impStructVal.playerEntity.PlayWeaponSound(EWeaponSoundType.ClipDrop);
                _reloading = true;
            }

            var reloadSpeed = Mathf.Max(0.1f, heldAgent.ReloadSpeed);
            impStructVal.playerEntity.autoMoveInterface.PlayerAutoMove.StopAutoMove();
            impStructVal.playerEntity.animatorClip.ClipManager.SetReloadSpeedBuff(reloadSpeed);
           // impStructVal.playerEntity.ModeController().CallBeforeAction(impStructVal.weaponController, EPlayerActionType.Reload);
            //走特殊换弹逻辑
            var specialReloadCount = heldAgent.CommonFireCfg.SpecialReloadCount;
            if (heldAgent.CommonFireCfg.SpecialReloadCount > 0)
            {
                var reloadedBulletCount = maxSizeBulletCount - heldAgent.BaseComponent.Bullet;
                reloadedBulletCount = Mathf.Min(reloadedBulletCount, reservedBullet);
                reloadedBulletCount = Mathf.CeilToInt((float)reloadedBulletCount / specialReloadCount);
                reloadedBulletCount = Mathf.Max(1, reloadedBulletCount);
                //  impStructVal.playerEntity.AudioController().SetReloadBulletAudioCount(reloadedBulletCount);
                impStructVal.playerEntity.stateInterface.State.SpecialReload(
                    () => { SpecialReload(impStructVal.playerEntity, impStructVal.weaponController, maxSizeBulletCount); },
                    reloadedBulletCount,
                    () =>
                    {
                        //换弹结束回调
                        impStructVal.playerEntity.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        //    impStructVal.playerEntity.AudioController().SetReloadBulletAudioCount(0);
                    });
            }
            else
            {
                if (heldAgent.BaseComponent.Bullet > 0 && !heldAgent.IsWeaponEmptyReload)
                {
                    var needActionDeal = CheckNeedActionDeal(impStructVal.weaponController, ActionDealEnum.Reload);
                    if (needActionDeal)
                    {
                        impStructVal.playerEntity.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                    }
                    if(impStructVal.playerEntity.AudioController()!= null)
                    impStructVal.playerEntity.AudioController().PlayReloadAudio(impStructVal.playerEntity.WeaponController().HeldConfigId, false);
                    impStructVal.playerEntity.stateInterface.State.Reload(() =>
                    {
                        if (needActionDeal)
                        {
                            impStructVal.playerEntity.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                        }

                        Reload(impStructVal.playerEntity, impStructVal.weaponController, maxSizeBulletCount);
                        impStructVal.playerEntity.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        _reloading = false;
                    });
                }
                else
                {
                    var needActionDeal = CheckNeedActionDeal(impStructVal.weaponController, ActionDealEnum.ReloadEmpty);
                    if (needActionDeal)
                    {
                        impStructVal.playerEntity.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                    }
                if(impStructVal.playerEntity.AudioController() != null)
                    impStructVal.playerEntity.AudioController().PlayReloadAudio(impStructVal.playerEntity.WeaponController().HeldConfigId, true);
                    impStructVal.playerEntity.stateInterface.State.ReloadEmpty(() =>
                    {
                        if (needActionDeal)
                        {
                            impStructVal.playerEntity.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                        }

                        Reload(impStructVal.playerEntity, impStructVal.weaponController, maxSizeBulletCount);
                        impStructVal.playerEntity.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        _reloading = false;
                    });
                }
            }
        }




        private void SpecialReload(PlayerEntity playerEntity, PlayerWeaponController controller, int clipSize)
        {
            var cfg = controller.HeldWeaponAgent.CommonFireCfg;
            var loadCount = cfg.SpecialReloadCount;
            var target = clipSize - controller.HeldWeaponAgent.BaseComponent.Bullet;
            loadCount = Mathf.Min(loadCount, target);
            DoRealod(playerEntity, controller, loadCount);
            if(playerEntity.AudioController() != null)
                playerEntity.AudioController().PlayReloadBulletAudio(controller.HeldConfigId);
        }

        private void Reload(PlayerEntity playerEntity, PlayerWeaponController controller, int clipSize)
        {
            var target = clipSize - controller.HeldWeaponAgent.BaseComponent.Bullet;
            target = Mathf.Max(0, target);
            DoRealod(playerEntity, controller, target);
        }

        private void DoRealod(PlayerEntity playerEntity, PlayerWeaponController controller, int target)
        {
            //PlayerWeaponController controller = playerEntity.WeaponController();
            //var configAssy = controller.HeldWeaponLogicConfigAssy;
            var cfg = controller.HeldWeaponAgent.CommonFireCfg;
            var lastReservedBullet = controller.GetReservedBullet();
            target = Mathf.Min(target, lastReservedBullet);
            controller.HeldWeaponAgent.BaseComponent.Bullet += target;
            //DebugUtil.MyLog("Bullet reload" + controller.HeldWeaponAgent.BaseComponent.Bullet, DebugUtil.DebugColor.Black);
            controller.SetReservedBullet(lastReservedBullet - target);

            IEventArgs args = (IEventArgs)(_sessonObjects).FreeArgs;

            if (!args.Triggers.IsEmpty((int)EGameEvent.WeaponState))
            {
                //TODO Implement
                SimpleParaList dama = new SimpleParaList();
                //    dama.AddFields(new ObjectFields(weaponState));
                dama.AddPara(new IntPara("CarryClip", lastReservedBullet - target));
                dama.AddPara(new IntPara("Clip", controller.HeldWeaponAgent.BaseComponent.Bullet));
                dama.AddPara(new IntPara("ClipType", (int)controller.HeldWeaponAgent.Caliber));
                dama.AddPara(new IntPara("id", (int)controller.HeldConfigId));
                SimpleParable sp = new SimpleParable(dama);
                dama.AddFields(new ObjectFields(playerEntity));
                args.Trigger((int)EGameEvent.WeaponState,
                    new TempUnit[]
                        {new TempUnit("state", sp), new TempUnit("current", (FreeData) playerEntity.freeData.FreeData)});
            }
        }

        private bool CheckNeedActionDeal(PlayerWeaponController sharedApi, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponResourceConfigManager>()
                                   .NeedActionDeal(sharedApi.HeldWeaponAgent.ConfigId, action);
        }
    }
}
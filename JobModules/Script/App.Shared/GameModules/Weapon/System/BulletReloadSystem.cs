using System.Collections;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.@event;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.Util;
using Assets.Utils.Configuration;
using com.wd.free.@event;
using com.wd.free.para;
using Core;
using Core.Free;
using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Shared.GameModules.Weapon
{
    /// <summary>
    /// Defines the <see cref="BulletReloadSystem" />
    /// </summary>
    public class BulletReloadSystem : AbstractUserCmdExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(BulletReloadSystem));

        private bool _reloading;


        private ICommonSessionObjects _sessonObjects;

        public BulletReloadSystem( ICommonSessionObjects sessionObjects)
        {
            this._sessonObjects = sessionObjects;
        }
        protected override bool Filter(PlayerEntity playerEntity)
        {
            return !playerEntity.playerMove.IsAutoRun;
        }


        protected override void ExecuteUserCmd(PlayerEntity player, IUserCmd cmd)
        {

            if ((!cmd.IsAutoReload && !cmd.FilteredInput.IsInput(EPlayerInput.IsReload)) ||
            cmd.FilteredInput.IsInput(EPlayerInput.IsLeftAttack)) return; //按住鼠标左键时不可换弹
            //   if (!player.ModeController().CanModeAutoReload) return;
            var weaponController = player.WeaponController();
            var heldWeaponAgent  = weaponController.HeldWeaponAgent;

            heldWeaponAgent.RunTimeComponent.NeedAutoReload = false;
            if (!heldWeaponAgent.IsValid() //heldWeaponAgent.BaseComponent.Bullet > 0
            || heldWeaponAgent.WeaponConfigAssy.CanotReloadBullet || heldWeaponAgent.CommonFireCfg == null) return;
            var reservedBullet = weaponController.GetReservedBullet();
            if (reservedBullet < 1)
            {
                if (SharedConfig.CurrentGameMode == EGameMode.Normal)
                    player.tip.TipType = ETipType.BulletRunout;
                else
                    player.tip.TipType = ETipType.NoBulletInPackage;
                return;
            }

            //检查总弹量
            int maxSizeBulletCount = heldWeaponAgent.WeaponConfigAssy.PropertyCfg.Bullet;
            if (heldWeaponAgent.BaseComponent.Magazine > 0)
            {
                maxSizeBulletCount += SingletonManager.Get<WeaponPartsConfigManager>()
                                .GetConfigById(heldWeaponAgent.BaseComponent.Magazine).Bullet;
            }

            if (heldWeaponAgent.BaseComponent.Bullet >= maxSizeBulletCount)
            {
                return;
            }


            if (!_reloading)
            {
                //player.PlayWeaponSound(EWeaponSoundType.ClipDrop);
                _reloading = true;
            }


            var reloadSpeed = Mathf.Max(0.1f, heldWeaponAgent.ReloadSpeed);
            player.autoMoveInterface.PlayerAutoMove.StopAutoMove();
            player.animatorClip.ClipManager.SetReloadSpeedBuff(reloadSpeed);
            
            //走特殊换弹逻辑
            var specialReloadCount = heldWeaponAgent.CommonFireCfg.SpecialReloadCount;
            if (specialReloadCount > 0)
            {
                var reloadedBulletCount = maxSizeBulletCount - heldWeaponAgent.BaseComponent.Bullet;
                reloadedBulletCount = Mathf.Min(reloadedBulletCount, reservedBullet);
                reloadedBulletCount = Mathf.CeilToInt((float) reloadedBulletCount / specialReloadCount);
                reloadedBulletCount = Mathf.Max(1, reloadedBulletCount);
                //  player.AudioController().SetReloadBulletAudioCount(reloadedBulletCount);
                player.stateInterface.State.SpecialReload(
                () => { SpecialReload(player, weaponController, maxSizeBulletCount); }, reloadedBulletCount, () =>
                {
                    //换弹结束回调
                    player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                });
            }
            else
            {
                var reloadAudioParams = (reloadSpeed-1)*100;
                if (heldWeaponAgent.BaseComponent.Bullet > 0 && !heldWeaponAgent.IsWeaponEmptyReload)
                {
                    var needActionDeal = CheckNeedActionDeal(weaponController, ActionDealEnum.Reload);
                    if (needActionDeal)
                    {
                        player.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                    }

                 
                    player.AudioController().PlayReloadAudio(heldWeaponAgent.ConfigId, false,reloadAudioParams);
                    player.stateInterface.State.Reload(() =>
                    {
                        if (needActionDeal)
                        {
                            player.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                        }

                        Reload(player, weaponController, maxSizeBulletCount);
                        player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        _reloading = false;
                    });
                }
                else
                {
                    var needActionDeal = CheckNeedActionDeal(weaponController, ActionDealEnum.ReloadEmpty);
                    if (needActionDeal)
                    {
                        player.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                    }

                    player.AudioController().PlayReloadAudio(heldWeaponAgent.ConfigId, true,reloadAudioParams);
                    player.stateInterface.State.ReloadEmpty(() =>
                    {
                        if (needActionDeal)
                        {
                            player.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                        }

                        Reload(player, weaponController, maxSizeBulletCount);
                        player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        _reloading = false;
                    });
                }
            }
        }


        private void SpecialReload(PlayerEntity playerEntity, PlayerWeaponController controller, int clipSize)
        {
            var cfg       = controller.HeldWeaponAgent.CommonFireCfg;
            var loadCount = cfg.SpecialReloadCount;
            var target    = clipSize - controller.HeldWeaponAgent.BaseComponent.Bullet;
            loadCount = Mathf.Min(loadCount, target);
            DoRealod(playerEntity, controller, loadCount);
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
            var cfg                = controller.HeldWeaponAgent.CommonFireCfg;
            var lastReservedBullet = controller.GetReservedBullet();
            target                                                     =  Mathf.Min(target, lastReservedBullet);
            controller.HeldWeaponAgent.BaseComponent.Bullet            += target;
            controller.HeldWeaponAgent.RunTimeComponent.PullBoltFinish =  true;
            //DebugUtil.MyLog("Bullet reload" + controller.HeldWeaponAgent.BaseComponent.Bullet, DebugUtil.DebugColor.Black);
            controller.SetReservedBullet(lastReservedBullet - target);

            IEventArgs args = (IEventArgs) (_sessonObjects).FreeArgs;

            if (!args.Triggers.IsEmpty(FreeTriggerConstant.WEAPON_STATE))
            {
                SimpleParaList dama = new SimpleParaList();
                dama.AddPara(new IntPara("CarryClip", lastReservedBullet - target));
                dama.AddPara(new IntPara("Clip", controller.HeldWeaponAgent.BaseComponent.Bullet));
                dama.AddPara(new IntPara("ClipType", (int) controller.HeldWeaponAgent.Caliber));
                dama.AddPara(new IntPara("id", controller.HeldConfigId));
                SimpleParable sp = new SimpleParable(dama);
                dama.AddFields(new ObjectFields(playerEntity));
                args.Trigger(FreeTriggerConstant.WEAPON_STATE, new TempUnit[]{new TempUnit("state", sp), new TempUnit("current", (FreeData) playerEntity.freeData.FreeData)});
            }
        }

        private bool CheckNeedActionDeal(PlayerWeaponController sharedApi, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponResourceConfigManager>()
                            .NeedActionDeal(sharedApi.HeldWeaponAgent.ConfigId, action);
        }
    }
}
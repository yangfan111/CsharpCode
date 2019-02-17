using Core.GameModule.Interface;
using Core.GameModule.System;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using Entitas;
using XmlConfig;
using Core.WeaponLogic;
using UnityEngine;
using Core;
using WeaponConfigNs;
using Assets.Utils.Configuration;
using Core.GameInputFilter;
using Assets.XmlConfig;
using Core.Common;
using com.wd.free.@event;
using App.Shared.FreeFramework.framework.@event;
using Utils.Singleton;
using App.Shared.GameModules.Weapon;

namespace App.Shared.GameModules.GamePlay.SimpleTest
{
    //TODO 移到firelogic
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


        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            ISharedPlayerWeaponComponentGetter sharedWeaponAPI = player.WeaponController();
            if (cmd.IsReload)// && cmd.FilteredInput.IsInput(EPlayerInput.IsReload))
            {
                if (!cmd.FilteredInput.IsInput(EPlayerInput.IsReload) && !player.playerMove.IsAutoRun)
                {
                    return;
                }


                WeaponScanStruct? currWeapon = sharedWeaponAPI.HeldWeaponScan;
                if (!currWeapon.HasValue) return;
                var config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(currWeapon.Value.ConfigId);
                if (NoReloadAction(config))
                {
                    return;
                }
                PlayerWeaponController controller = player.WeaponController();
                var configAssy = controller.HeldWeaponLogicConfigAssy;
                if (null == configAssy)
                {
                    return;
                }
                if (currWeapon.Value.Bullet >= configAssy.CommonFireCfg.MagazineCapacity)
                {
                    return;
                }
                if (HasNoReservedBullet(sharedWeaponAPI, player))
                {
                    return;
                }
                if (!_reloading)
                {
                    player.PlayWeaponSound(EWeaponSoundType.ClipDrop);
                    _reloading = true;
                }

                var weaponData = sharedWeaponAPI.HeldWeaponScan.Value;
                var commonfireConfig = configAssy.CommonFireCfg;
                var reloadSpeed = configAssy.GetReloadSpeed();
                reloadSpeed = Mathf.Max(0.1f, reloadSpeed);

                player.playerMove.InterruptAutoRun();

                player.animatorClip.ClipManager.SetReloadSpeedBuff(reloadSpeed);
                if (commonfireConfig.SpecialReloadCount > 0)
                {
                    var target = commonfireConfig.MagazineCapacity - weaponData.Bullet;
                    target = Mathf.Min(target, player.WeaponController().GetReservedBullet());
                    var count = Mathf.CeilToInt((float)target / commonfireConfig.SpecialReloadCount);
                    count = Mathf.Max(1, count);
                    player.stateInterface.State.SpecialReload(
                        () =>
                        {
                            SpecialReload(_contexts, player);
                        },
                        count,
                        () =>
                        {//换弹结束回调
                            player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        });
                }
                else
                {
                    if (weaponData.Bullet > 0 && !sharedWeaponAPI.IsHeldWeaponEmptyReload())
                    {
                        var needActionDeal = CheckNeedActionDeal(sharedWeaponAPI, ActionDealEnum.Reload);
                        if (needActionDeal)
                        {
                            player.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                        }
                        player.stateInterface.State.Reload(() =>
                        {
                            if (needActionDeal)
                            {
                                player.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                            }
                            Reload(_contexts, player);
                            player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                            _reloading = false;
                        });
                    }
                    else
                    {
                        var needActionDeal = CheckNeedActionDeal(sharedWeaponAPI, ActionDealEnum.ReloadEmpty);
                        if (needActionDeal)
                        {
                            player.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
                        }
                        player.stateInterface.State.ReloadEmpty(() =>
                        {
                            if (needActionDeal)
                            {
                                player.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                            }
                            Reload(_contexts, player);
                            player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                            _reloading = false;
                        });
                    }
                }
            }
        }

        private bool NoReloadAction(NewWeaponConfigItem config)
        {
            if (null == config)
            {
                return true;
            }
            return config.Type == (int)EWeaponType.ThrowWeapon || config.Type == (int)EWeaponType.MeleeWeapon;
        }


        private bool HasNoReservedBullet(ISharedPlayerWeaponComponentGetter agent, PlayerEntity playerEntity)
        {
            if (agent.GetReservedBullet() < 1)
            {
                if (SharedConfig.CurrentGameMode == Components.GameMode.Normal)
                {
                    playerEntity.tip.TipType = ETipType.BulletRunout;
                }
                else
                {
                    playerEntity.tip.TipType = ETipType.NoBulletInPackage;
                }
                return true;
            }
            return false;
        }

        private void SpecialReload(Contexts contexts, PlayerEntity playerEntity)
        {
            PlayerWeaponController controller = playerEntity.WeaponController();
            var configAssy = controller.HeldWeaponLogicConfigAssy;
            if (configAssy == null) return;
            WeaponScanStruct weaponScan = controller.HeldWeaponScan.Value;
            var commonFireConfig = configAssy.CommonFireCfg;
            var loadCount = commonFireConfig.SpecialReloadCount;
            var target = commonFireConfig.MagazineCapacity - weaponScan.Bullet;
            loadCount = Mathf.Min(loadCount, target);
            DoRealod(contexts, playerEntity, loadCount);
        }

        private void Reload(Contexts contexts, PlayerEntity playerEntity)
        {
            PlayerWeaponController controller = playerEntity.WeaponController();
            var configAssy = controller.HeldWeaponLogicConfigAssy;
            if (configAssy == null) return;
            var weaponData = controller.HeldWeaponScan.Value;

            var commonFireConfig = configAssy.CommonFireCfg;
            var target = commonFireConfig.MagazineCapacity - weaponData.Bullet;
            target = Mathf.Max(0, target);
            DoRealod(contexts, playerEntity, target);
        }

        private void DoRealod(Contexts contexts, PlayerEntity playerEntity, int target)
        {
            //PlayerWeaponController controller = playerEntity.WeaponController();
            //var configAssy = controller.HeldWeaponLogicConfigAssy;
            var weaponData = playerEntity.WeaponController().HeldWeaponScan;
            if (!weaponData.HasValue) return;
            var lastReservedBullet = playerEntity.WeaponController().GetReservedBullet();
            target = Mathf.Min(target, lastReservedBullet);
            //  weaponData.Value.Bullet += target;
            playerEntity.WeaponController().SetReservedBullet(lastReservedBullet - target);

            IEventArgs args = (IEventArgs)(_sessonObjects).FreeArgs;

            if (!args.Triggers.IsEmpty((int)EGameEvent.WeaponState))
            {
                //TODO Implement
                //SimpleParaList dama = new SimpleParaList();
                //dama.AddFields(new ObjectFields(weaponState));
                //dama.AddPara(new IntPara("CarryClip", weaponState.ReservedBulletCount));
                //dama.AddPara(new IntPara("Clip", weaponState.LoadedBulletCount));
                //dama.AddPara(new IntPara("ClipType", (int)weaponState.Caliber));
                //dama.AddPara(new IntPara("id", (int)weaponState.CurrentWeapon));
                //SimpleParable sp = new SimpleParable(dama);

                //args.Trigger((int)EGameEvent.WeaponState, new TempUnit[] { new TempUnit("state", sp), new TempUnit("current", (FreeData)((PlayerEntity)weaponState.Owner).freeData.FreeData) });
            }
        }

        private bool CheckNeedActionDeal(ISharedPlayerWeaponComponentGetter sharedApi, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponConfigManager>().NeedActionDeal(sharedApi.HeldWeaponConfigId.Value, action);
        }
    }
}
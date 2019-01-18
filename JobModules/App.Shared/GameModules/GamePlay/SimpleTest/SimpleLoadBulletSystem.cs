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
using com.wd.free.para;
using com.wd.free.@event;
using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.framework.trigger;
using App.Shared.FreeFramework.framework.@event;
using Utils.Singleton;
<<<<<<< HEAD
using App.Shared.GameModules.Weapon;
=======
using App.Shared.WeaponLogic;
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
using App.Shared.Util;

namespace App.Shared.GameModules.GamePlay.SimpleTest
{
    public class SimpleLoadBulletSystem : ReactiveGamePlaySystem<PlayerEntity>, IUserCmdExecuteSystem, IOnGuiSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(SimpleLoadBulletSystem));

        private bool _reloading;
        private IFilteredInput _filteredResult;
        private Contexts _contexts;
        private ICommonSessionObjects _sessonObjects;

        protected override ICollector<PlayerEntity> GetTrigger(IContext<PlayerEntity> context)
        {
            return context.CreateCollector(PlayerMatcher.AllOf(PlayerMatcher.GamePlay, PlayerMatcher.PlayerWeaponState));
        }

        protected override bool Filter(PlayerEntity entity)
        {
            return entity.hasGamePlay && entity.hasPlayerWeaponState && entity.isFlagSelf && entity.hasWeaponLogic;
        }

        public SimpleLoadBulletSystem(Contexts contexts, ICommonSessionObjects sessionObjects) : base(contexts.player)
        {
            this._contexts = contexts;
            this._sessonObjects = sessionObjects;
        }

        public override void SingleExecute(PlayerEntity entity)
        {
            entity.weaponLogic.State.LoadedBulletCount = entity.weaponLogic.State.BulletCountLimit;
        }




        public void ExecuteUserCmd(IUserCmdOwner owner, IUserCmd cmd)
        {
            PlayerEntity player = (PlayerEntity)owner.OwnerEntity;
            if (cmd.IsReload)// && cmd.FilteredInput.IsInput(EPlayerInput.IsReload))
            {
                if (!cmd.FilteredInput.IsInput(EPlayerInput.IsReload) && !player.playerMove.IsAutoRun)
                {
                    return;
                }
                if (!player.hasWeaponComponentAgent)
                    return;
<<<<<<< HEAD
                ISharedPlayerWeaponComponentGetter sharedAPI = player.GetController<PlayerWeaponController>();
            
                WeaponInfo currWeapon = sharedAPI.CurrSlotWeaponInfo;
                var config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(currWeapon.Id);
=======
                }
                IPlayerWeaponComponentArchive weaponAgent = player.GetWeaponAchive();
            
                WeaponInfo heldWeapon = weaponAgent.HeldSlotWeaponInfo;
                var config = SingletonManager.Get<WeaponConfigManager>().GetConfigById(heldWeapon.Id);
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
                if (NoReloadAction(config))
                {
                    return;
                }
<<<<<<< HEAD
                if (MagazineIsFull(player.weaponLogic.State, currWeapon.Bullet))
                {
                    return;
                }
                if (HasNoReservedBullet(sharedAPI, player))
=======
                if (MagazineIsFull(player.weaponLogic.State, heldWeapon.Bullet))
                {
                    return;
                }
                if (HasNoReservedBullet(weaponAgent, player))
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
                {
                    return;
                }
                if (!_reloading)
                {
                    player.weaponLogic.WeaponSound.PlaySound(EWeaponSoundType.ClipDrop);
                    _reloading = true;
                }
                var weaponState = player.weaponLogic.State;
                var reloadSpeed = weaponState.ReloadSpeed;
                reloadSpeed = Mathf.Max(0.1f, reloadSpeed);

                player.playerMove.InterruptAutoRun();

                player.animatorClip.ClipManager.SetReloadSpeedBuff(reloadSpeed);
                if (weaponState.SpecialReloadCount > 0)
                {
                    var target = weaponState.BulletCountLimit - weaponState.LoadedBulletCount;
                    target = Mathf.Min(target, weaponState.ReservedBulletCount);
                    var count = Mathf.CeilToInt((float)target / weaponState.SpecialReloadCount);
                    count = Mathf.Max(1, count);
                    player.stateInterface.State.SpecialReload(
                        () =>
                        {
                            SpecialReload(weaponState);
                        },
                        count,
                        () =>
                        {//换弹结束回调
                            player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                        });
                }
                else
                {
                    if (weaponState.LoadedBulletCount > 0 && !weaponState.IsAlwaysEmptyReload)
                    {
<<<<<<< HEAD
                        var needActionDeal = CheckNeedActionDeal(sharedAPI, ActionDealEnum.Reload);
=======
                        var needActionDeal = CheckNeedActionDeal(weaponAgent, ActionDealEnum.Reload);
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
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
                            Reload(weaponState);
                            player.animatorClip.ClipManager.ResetReloadSpeedBuff();
                            _reloading = false;
                        });
                    }
                    else
                    {
<<<<<<< HEAD
                        var needActionDeal = CheckNeedActionDeal(sharedAPI, ActionDealEnum.ReloadEmpty);
=======
                        var needActionDeal = CheckNeedActionDeal(weaponAgent, ActionDealEnum.ReloadEmpty);
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
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
                            Reload(weaponState);
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

        private bool MagazineIsFull(IPlayerWeaponState weaponState, int bulletCount)
        {
            return bulletCount >= weaponState.BulletCountLimit;
        }

<<<<<<< HEAD
        private bool HasNoReservedBullet(ISharedPlayerWeaponComponentGetter agent, PlayerEntity playerEntity)
=======
        private bool HasNoReservedBullet(IPlayerWeaponComponentArchive agent, PlayerEntity playerEntity)
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
        {
            if (agent.GetReservedBullet() < 1)
            {
                _elapse = 0;
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

        private void SpecialReload(IPlayerWeaponState weaponState)
        {
            var loadCount = weaponState.SpecialReloadCount;
            var target = weaponState.BulletCountLimit - weaponState.LoadedBulletCount;
            loadCount = Mathf.Min(loadCount, target);
            DoRealod(weaponState, loadCount);
        }

        private void Reload(IPlayerWeaponState weaponState)
        {
            var target = weaponState.BulletCountLimit - weaponState.LoadedBulletCount;
            target = Mathf.Max(0, target);
            DoRealod(weaponState, target);
        }

        private void DoRealod(IPlayerWeaponState weaponState, int target)
        {
            target = Mathf.Min(target, weaponState.ReservedBulletCount);
            weaponState.LoadedBulletCount += target;
            weaponState.ReservedBulletCount -= target;
           
            IEventArgs args = (IEventArgs)(_sessonObjects).FreeArgs;

            if (!args.Triggers.IsEmpty((int)EGameEvent.WeaponState))
            {
                SimpleParaList dama = new SimpleParaList();
                dama.AddFields(new ObjectFields(weaponState));
                dama.AddPara(new IntPara("CarryClip", weaponState.ReservedBulletCount));
                dama.AddPara(new IntPara("Clip", weaponState.LoadedBulletCount));
                dama.AddPara(new IntPara("ClipType", (int)weaponState.Caliber));
                dama.AddPara(new IntPara("id", (int)weaponState.CurrentWeapon));
                SimpleParable sp = new SimpleParable(dama);

                args.Trigger((int)EGameEvent.WeaponState, new TempUnit[] { new TempUnit("state", sp), new TempUnit("current", (FreeData)((PlayerEntity)weaponState.Owner).freeData.FreeData) });
            }

            if (weaponState.LoadedBulletCount >= weaponState.BulletCountLimit || weaponState.ReservedBulletCount < 1)
            {
                //如果前置弹夹已满，或者后备弹夹已空，这时应该执行了最后一次换弹，应附带有拉栓动作
                weaponState.IsBolted = true;
            }
        }

<<<<<<< HEAD
        private bool CheckNeedActionDeal(ISharedPlayerWeaponComponentGetter sharedApi, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponConfigManager>().NeedActionDeal(sharedApi.CurrSlotWeaponId, action);
=======
        private bool CheckNeedActionDeal(IPlayerWeaponComponentArchive archive, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponConfigManager>().NeedActionDeal(archive.HeldSlotWeaponId, action);
>>>>>>> 6213b9d866f8e5766fe02025e06c786a8fc53841
        }

        // 临时代码
        //TODO 使用正式UI资源
        private float _showTime = 2f;
        private float _elapse;
        private string _tip = "";
        public void OnGUI()
        {
            _elapse += Time.deltaTime;
            if (_elapse < _showTime)
            {
                GUI.Label(new Rect(Screen.width / 2, Screen.height / 2, Screen.width, Screen.height), _tip);
            }
        }
    }
}
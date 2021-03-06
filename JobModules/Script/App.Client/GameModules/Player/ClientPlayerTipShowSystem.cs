﻿using App.Client.GameModules.GamePlay.Free.App;
using App.Shared;
using App.Shared.Components.Player;
using Assets.Utils.Configuration;
using Core;
using Core.GameModule.Interface;
using Core.Utils;
using Entitas;
using I2.Loc;
using Utils.Singleton;
using TipLocation = App.Shared.Components.Player.TipComponent.TipLocation;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerTipShowSystem : IRenderSystem
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(ClientPlayerTipShowSystem));
        private IGroup<PlayerEntity> _selfPlayerGroup;
        private PlayerEntity _playerEntity;
        private Contexts _contexts;

        public ClientPlayerTipShowSystem(Contexts contexts)
        {
            _contexts = contexts;
            _selfPlayerGroup = contexts.player.GetGroup(PlayerMatcher.FlagSelf);
        }

        public void OnRender()
        {
            foreach(var player in _selfPlayerGroup)
            {
                if(!player.hasTip || player.gamePlay.TipHideStatus)
                {
                    continue;
                }
                _playerEntity = player;
                if (!string.IsNullOrEmpty(player.tip.Content))
                {
                    if (player.tip.ForTest)
                    {
                        ShowBottomTip(player.tip.Content);
                        return;
                    }
                    switch(player.tip.Location)
                    {
                        default:
                        case TipLocation.Bottom:
                            ShowBottomTip(player.tip.Content);
                            break;
                        case TipLocation.Top:
                            ShowTopTip(player.tip.Content);
                            break;
                    }
                    player.tip.Content = string.Empty;
                    player.tip.Location = TipLocation.Bottom;
                }
                if(player.tip.TipType != ETipType.None)
                {
                    switch(player.tip.Location)
                    {
                        default:
                        case TipLocation.Bottom:
                            ShowBottomTip(player.tip.TipType);
                            break;
                        case TipLocation.Top:
                            ShowTopTip(player.tip.TipType);
                            break;
                    }
                    player.tip.TipType = ETipType.None;
                    player.tip.Location = TipLocation.Bottom;
                }
            }
            _playerEntity = null;
        }

        public int CurrentWeaponId { get; set; }

        private void ShowBottomTip(string content)
        {
            ChickenUIUtil.ShowBottomTip(content);
        }

        private void ShowTopTip(string content)
        {
            ChickenUIUtil.ShowTopTip(content);
        }

        private void ShowBottomTip(ETipType type)
        {
            //提示内容的异常可以吞掉不处理，不影响后续逻辑
            try
            {
                ChickenUIUtil.ShowBottomTip(TypeToContent(type, GetWeaponName(), GetReloadKey()));
            }
            catch (System.Exception e)
            {
                Logger.ErrorFormat("ShowBottomTip Failed {0}", e);
            }
        }

        private void ShowTopTip(ETipType type)
        {
            ChickenUIUtil.ShowTopTip(TypeToContent(type, GetWeaponName(), GetReloadKey()));
        }

        private string TypeToContent(ETipType type, string weaponName, string reloadKey)
        {
            switch (type)
            {
                case ETipType.BulletRunout:
                    return ScriptLocalization.client_commontip.bulletrunout;
                case ETipType.NoBulletInPackage:
                    return ScriptLocalization.client_commontip.nobulletinpackage;
                case ETipType.CanNotRescure:
                    return ScriptLocalization.client_actiontip.canNotResucre;
                case ETipType.CanNotCrouch:
                    return ScriptLocalization.client_actiontip.canNotCrouch;
                case ETipType.CanNotProne:
                    return ScriptLocalization.client_actiontip.canNotProne;
                case ETipType.CanNotStand:
                    return ScriptLocalization.client_actiontip.canNotStand;
                case ETipType.CanNotToCrouch:
                    return ScriptLocalization.client_actiontip.canNotToCrouch;
                case ETipType.OutOfOxygen:
                    return ScriptLocalization.client_actiontip.outOfOxygen;
                case ETipType.FireModeLocked:
                    return string.Format(ScriptLocalization.client_commontip.firemodelocked, weaponName);
                case ETipType.FireModeToAuto:
                    return string.Format(ScriptLocalization.client_commontip.firemodetoauto, weaponName);
                case ETipType.FireModeToBurst:
                    return string.Format(ScriptLocalization.client_commontip.firemodetoburst, weaponName);
                case ETipType.FireModeToManual:
                    return string.Format(ScriptLocalization.client_commontip.firemodetomanual, weaponName);
                case ETipType.FireWithNoBullet:
                    if (weaponName.Equals("空手")) return "";
                    return string.Format(ScriptLocalization.client_commontip.firewithnobullet, weaponName,reloadKey);
                case ETipType.NoWeaponInSlot:
                    if (_playerEntity != null && _playerEntity.gamePlay.JobAttribute != (int) EJobAttribute.EJob_EveryMan) return "";
                    return ScriptLocalization.client_commontip.noweaponinslot;
                case ETipType.CantSwithGrenade:
                    return ScriptLocalization.client_commontip.cannotswitchgrenade;
                case ETipType.EnterNumError:
                    return ScriptLocalization.client_commontip.enternumerror;

                default:
                    return "";
            }
        }

        private string GetReloadKey()
        {
            return "R";
        }

        private string GetWeaponName()
        {
            if (null != _playerEntity)
            {
                var weaponId = _playerEntity.WeaponController().HeldWeaponAgent.ConfigId;
                var weaponConfig = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
                if (null == weaponConfig)
                {
                    return "";
                }
                return weaponConfig.Name;
            }
            return "";
        }
    }
}

using System;
using App.Shared;
using App.Shared.Components.Weapon;
using App.Shared.GameModules.Weapon;
using Core;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public class CrossHairUiAdapter : UIAdapter, ICrossHairUiAdapter
    {
        private Contexts _contexts;
        private PlayerContext _playerContext;
        private UiContext _uiContext;

        public CrossHairUiAdapter(Contexts contexts)
        {
            _contexts = contexts;
            _playerContext = contexts.player;
            _uiContext = contexts.ui;
        }

        public override bool IsReady()
        {
            return Player != null;
        }

        private PlayerEntity Player
        {
            get { return _uiContext.uI.Player; }
        }

        private WeaponBaseAgent HeldWeaponAgent
        {
            get
            {
                var controller = Player.WeaponController();
                return controller == null ? null : controller.HeldWeaponAgent;
            }
        }

        //准心组
        private CrossHairType type;
        private bool isOpenCrossHairMotion = true; //常态类型准心 是否开启了准心运动

        public CrossHairType Type //准心类型
        {
            get
            {
                var type = CrossHairType.Normal;
                var player = Player;
                if (CrossShouldBeHidden(player))
                {
                    type = CrossHairType.Novisible;
                }
                else if (FocusTargetIsTeammate())
                {
                    type = CrossHairType.AddBlood;
                }

                return type;
            }
        }

        private bool CrossShouldBeHidden(PlayerEntity player)
        {
            if (!player.hasCameraStateNew)
            {
                return false;
            }

            switch ((ECameraViewMode) player.cameraStateNew.ViewNowMode)
            {
                case ECameraViewMode.GunSight:
                    return true;
                default:
                    break;
            }

            switch ((ECameraPoseMode) player.cameraStateNew.MainNowMode)
            {
                case ECameraPoseMode.AirPlane:
                case ECameraPoseMode.DriveCar:
                case ECameraPoseMode.Dead:
                case ECameraPoseMode.Dying:
                case ECameraPoseMode.Gliding:
                case ECameraPoseMode.Parachuting:
                    return true;
                default:
                    break;
            }

            return false;
        }

        private bool FocusTargetIsTeammate()
        {
            return false;
        }


//        public CrossHairNormalTypeStatue Statue //常态类型准心 当前状态
//        {
//            get
//            {
//                XSpread = HeldWeaponAgent.RunTimeComponent.LastSpreadX;
//                YSpread = HeldWeaponAgent.RunTimeComponent.LastSpreadY;
////                var player = _playerContext.flagSelfEntity;
////
////                if (null == player)
////                {
////                    return CrossHairNormalTypeStatue.None;
////                }
////
////                if (PlayerIsFiring(player))
////                {
////                    return CrossHairNormalTypeStatue.Shot;
////                }
////                else if (PlayerIsMoving(player))
////                {
////                    return CrossHairNormalTypeStatue.Move;
////                }
////                else
////                {
////                    return CrossHairNormalTypeStatue.StopShot;
////                }
//  //                       }
//        }

        public float XSpread
        {
            get
            {
                var agent = HeldWeaponAgent;
                if (agent == null) return 0;
                return agent.RunTimeComponent.LastSpreadX;
            }
        }

        public float YSpread
        {
            get
            {
                var agent = HeldWeaponAgent;
                if (agent == null) return 0;
                return agent.RunTimeComponent.LastSpreadY;
            }
        }

        public float SpreadDuration
        {
            get
            {
                float duration = 1f;
                var controller = Player.StateInteractController();
                if (controller == null) return duration;
                var currStates = controller.GetCurrStates();
                if (currStates.Contains(EPlayerState.Firing))
                {
                    duration = GlobalConst.FireSpreadDuration;
                }

                if (currStates.Contains(EPlayerState.Move))
                {
                    duration *= GlobalConst.MotionSpreadDuration;
                }

                return duration;
            }
        }


//        private bool PlayerIsMoving(PlayerEntity player)
//        {
//            if (!player.hasPlayerMove)
//            {
//                return false;
//            }
//
//            var velocity = player.playerMove.Velocity;
//            return Math.Abs(velocity.y) > 0.15f ||
//                   velocity.x * velocity.x + velocity.z * velocity.z > 0.01f;
//        }


        //攻击组
        public int ShootNum //常态类型准心 射击状态 当前发射的子弹数
        {
            get
            {

                var agent = HeldWeaponAgent;
                return agent == null ? 0 : agent.RunTimeComponent.ContinuesShootCount;
            }
        }

        public WeaponRuntimeDataComponent RuntimeDataComponent
        {
            get { return HeldWeaponAgent.RunTimeComponent; }
        }

        public float AttackNum
        {
            get
            {
                var player = Player;
                if (null == player)
                {
                    return 0;
                }

                if (!player.hasAttackDamage)
                {
                    return 0;
                }

                var damage = player.attackDamage.GetAndResetDamage();
                return damage;
            }
        }

        private bool isBurstHeart; //爆头

        public bool IsBurstHeart
        {
            get
            {
                var player = Player;
                if (null == player)
                {
                    return false;
                }

                if (!player.hasAttackDamage)
                {
                    return false;
                }

                var part = player.attackDamage.GetAndResetHitPart();
                var critical = (EBodyPart) part == EBodyPart.Head;
                return critical;
            }
        }

        public bool IsOpenCrossHairMotion
        {
            get { return isOpenCrossHairMotion; }

            set { isOpenCrossHairMotion = value; }
        }

        public int WeaponAvatarId
        {
            get
            {
                var player = Player;
                if (player.hasPlayerWeaponBagSet)
                {
                    var agent = HeldWeaponAgent;
                    if (agent == null) return SingletonManager.Get<WeaponAvatarConfigManager>().GetEmptyHandId();
                    return agent.WeaponConfigAssy.NewWeaponCfg.AvatorId;
                    /*var weaponInfo = agent.BaseComponent;
                    if (weaponInfo != null)
                    {
                        var avatarId = weaponInfo.WeaponAvatarId;
                        if (weaponInfo.WeaponAvatarId > 0)
                        {
                            return weaponInfo.WeaponAvatarId;
                        }

                        var defaultAvatar = weaponInfo.ConfigId;
                        if (defaultAvatar > 0)
                        {
                            return defaultAvatar;
                        }
                    }*/
                }

                return SingletonManager.Get<WeaponAvatarConfigManager>().GetEmptyHandId();
            }
        }

        public bool IsShowCrossHair
        {
            get { return _uiContext.uI.IsShowCrossHair; }
        }
    }
}
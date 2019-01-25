using App.Shared.WeaponLogic;
using Assets.Utils.Configuration;
using Core.CameraControl.NewMotor;
using Core.Utils;
using Utils.Singleton;
using WeaponConfigNs;
using XmlConfig;

namespace Core.WeaponLogic.FireAciton
{
    public abstract class AbstractFireActionLogic<T1> : IFireActionLogic
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(AbstractFireActionLogic<T1>));
        protected Contexts _contexts;
        public AbstractFireActionLogic(Contexts contexts)
        {
            _contexts = contexts;
        }

        public virtual void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            var weaponData = playerEntity.GetCurrentWeaponInfo(_contexts);
            var currentWeapon = weaponData.Id;
            var needActionDeal = CheckNeedActionDeal(currentWeapon, ActionDealEnum.Fire);
            OnAfterFire(playerEntity, weaponEntity, needActionDeal);
        }

        protected abstract void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, bool needActionDeal);
        public abstract void OnIdle(PlayerEntity playerWeapon, WeaponEntity weaponEntity, IWeaponCmd cmd);

        protected bool CheckNeedActionDeal(int weaponId, ActionDealEnum action)
        {
            return SingletonManager.Get<WeaponConfigManager>().NeedActionDeal(weaponId, ActionDealEnum.Reload);
        }
        
        protected void SpecialFire(PlayerEntity playerEntity, bool needActionDeal)
        {
            if (playerEntity.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                if(playerEntity.hasStateInterface)
                {
                    playerEntity.stateInterface.State.SpecialSightsFire(() =>
                    {
                        if (needActionDeal)
                        {
                            playerEntity.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                        }
                    });
                }
                else
                {
                    LogError("playerEntity has no stateInterface");
                }
            }
            else
            {
                if(playerEntity.hasStateInterface)
                {
                    playerEntity.stateInterface.State.SpecialFire(() =>
                    {
                        if (needActionDeal)
                        {
                            playerEntity.appearanceInterface.Appearance.RemountWeaponOnRightHand();
                        }
                    });
                }
                else
                {
                    LogError("playerEntity has no stateInterface");
                }
            }
            playerEntity.PlayWeaponSound(EWeaponSoundType.LeftFire1);
            if (needActionDeal)
            {
                playerEntity.appearanceInterface.Appearance.MountWeaponOnAlternativeLocator();
            }
        }

        protected void DefaultFire(PlayerEntity playerEntity)
        {
            if (playerEntity.hasCameraStateNew && playerEntity.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight)
            {
                if(playerEntity.hasStateInterface)
                {
                    playerEntity.stateInterface.State.SightsFire();
                }
                else
                {
                    LogError("player has no stateInterface");
                }
            }
            else
            {
                if(playerEntity.hasStateInterface)
                {
                    playerEntity.stateInterface.State.Fire();
                }
                else
                {
                    LogError("player has no stateInterface");
                }
            }
            playerEntity.PlayWeaponSound(EWeaponSoundType.LeftFire1);
        }

        private void LogError(string msg)
        {
            Logger.Error(msg);
            System.Console.WriteLine(msg);
        }
    }
}

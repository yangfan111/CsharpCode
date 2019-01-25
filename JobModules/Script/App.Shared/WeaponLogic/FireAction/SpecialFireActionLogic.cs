using App.Shared;
using App.Shared.WeaponLogic;
using Core.CameraControl.NewMotor;
using WeaponConfigNs;
using XmlConfig;

namespace Core.WeaponLogic.FireAciton
{
    public class SpecialFireActionLogic : AbstractFireActionLogic<CommonFireConfig>
    {
        public SpecialFireActionLogic(Contexts contexts):base(contexts)
        {

        }

        public override void OnIdle(PlayerEntity playerEntity, WeaponEntity weaponEntity, IWeaponCmd cmd)
        {
            if(cmd.IsFire)
            {
                return;
            }
            var weaponState = weaponEntity.weaponData;
            if(weaponState.PullBolting && !IsFireEnd(playerEntity) && !IsFireHold(playerEntity))
            {
                SetPullBolting(playerEntity, weaponEntity, false);
            }
            if(IsFireHold(playerEntity))
            {
                EndSpecialFire(playerEntity);
                SetPullBolting(playerEntity, weaponEntity, true);
            }
        }

        protected override void OnAfterFire(PlayerEntity playerEntity, WeaponEntity weaponEntity, bool needActionDeal)
        {
            var weaponData = weaponEntity.weaponData;
            if(weaponData.Bullet > 0)
            {
                SpecialFire(playerEntity, needActionDeal);
            }
            else
            {
                DefaultFire(playerEntity);
            }
        }

        private void SetPullBolting(PlayerEntity playerEntity, WeaponEntity weaponEntity, bool value)
        {
            var weaponData = weaponEntity.weaponData;
            if (value)
            {
                var gunSight = playerEntity.cameraStateNew.ViewNowMode == (int)ECameraViewMode.GunSight;
                weaponData.GunSightBeforePullBolting = gunSight;
                weaponData.ForceChangeGunSight = gunSight;
            }
            else
            {
                if (weaponData.GunSightBeforePullBolting)
                {
                    weaponData.ForceChangeGunSight = true;
                    weaponData.GunSightBeforePullBolting = false;
                }
            }
            weaponData.PullBolting = value;
        }

        private bool IsFireEnd(PlayerEntity playerEntity)
        {
            var state = playerEntity.stateInterface.State;
            return state.GetActionState() == ActionInConfig.SpecialFireEnd;
        }

        private bool IsFireHold(PlayerEntity playerEntity)
        {
            return playerEntity.stateInterface.State.GetActionState() == ActionInConfig.SpecialFireHold;
        }

        private void EndSpecialFire(PlayerEntity playerEntity)
        {
            playerEntity.stateInterface.State.SpecialFireEnd();
        }

    }
}

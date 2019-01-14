using App.Server.GameModules.GamePlay.free.player;
using App.Shared.FreeFramework.Free.Weapon;
using com.wd.free.@event;
using com.wd.free.skill;
using Core.Configuration;
using Core.Free;
using Core.Prediction.UserPrediction.Cmd;
using Core.WeaponLogic;
using Core.WeaponLogic.Attachment;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.WeaponLogic.Tactic
{
    public class TacticWeaponLogic : IWeaponLogic
    {
        private WeaponConfig _weaponConfig;
        private UnitSkill _unitSkill;
        private ISkillArgs _freeArgs;
        public TacticWeaponLogic(int weaponId, IFreeArgs freeArgs)
        {
            _weaponConfig = SingletonManager.Get<WeaponDataConfigManager>().GetConfigById(weaponId);

            if (SharedConfig.IsServer)
            {
                _unitSkill = WeaponSkillFactory.GetSkill(weaponId);
                _freeArgs = freeArgs as ISkillArgs;
            }
        }

        public bool EmptyHand
        {
            get;set;
        }

        public bool CanCameraFocus()
        {
            return false; 
        }

        public float GetBaseSpeed()
        {
            var moveCfg = _weaponConfig.WeaponLogic.MoveSpeedLogic as DefaultMoveSpeedLogicConfig;
            if(null != moveCfg)
            {
                return moveCfg.MaxSpeed;
            }
            return 6;
        }

        public float GetBreathFactor()
        {
            return 1;
        }

        public int GetBulletLimit()
        {
            return 1;
        }

        public float GetFocusSpeed()
        {
            return 1;
        }

        public float GetFov()
        {
            return 90;
        }

        public float GetReloadSpeed()
        {
            return 1;
        }

        public int GetSpecialReloadCount()
        {
            return 0;
        }

        public bool IsFovModified()
        {
            return false;
        }

        public void Reset()
        {
        }

        public void SetAttachment(WeaponPartsStruct attachments)
        {
            //no Attachment for tactic weapon;
        }

        public void SetVisualConfig(ref VisualConfigGroup config)
        {
        }

        public void Update(IPlayerWeaponState playerWeapon, IUserCmd cmd)
        {
            if (SharedConfig.IsServer)
            {
                if (!_unitSkill.IsEmtpy())
                {
                    _freeArgs.GetInput().SetUserCmd(cmd);

                    _freeArgs.TempUse("current", (FreeData)playerWeapon.FreeData);

                    _unitSkill.Frame(_freeArgs);

                    _freeArgs.Resume("current");
                }
            }
        }
    }
}

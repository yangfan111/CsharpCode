using App.Shared.GameModules.Weapon;
using Core.WeaponLogic.Accuracy;
using Core.WeaponLogic.Bullet;
using Core.WeaponLogic.Kickback;
using System.Collections.Generic;
using Utils.Configuration;
using Utils.Utils;
using XmlConfig;


namespace Core.WeaponLogic.Attachment
{
    public class AttachmentManager : IAttachmentManager
    {
       

        IAttachmentConfigManager _attachConfigManager;

        public AttachmentManager(IAttachmentConfigManager configManager)
        {
            _attachConfigManager = configManager;
        }

        public Dictionary<WeaponAttributeType, float> _attachAttributeDic =
            new Dictionary<WeaponAttributeType, float>(CommonEnumEqualityComparer<WeaponAttributeType>.Instance);

        private List<int> _attachmentList = new List<int>();

        public void Prepare(WeaponPartsStruct attachments)
        {
            Reset();
            _attachmentList.Add(attachments.UpperRail);
            _attachmentList.Add(attachments.LowerRail);
            _attachmentList.Add(attachments.Muzzle);
            _attachmentList.Add(attachments.Magazine);
            _attachmentList.Add(attachments.Stock);
            for (var i = 0; i < _attachmentList.Count; i++)
            {
                if (_attachmentList[i] < 1)
                {
                    continue;
                }

                var modifiedInfos = _attachConfigManager.GetModifyInfos(_attachmentList[i]);
                if (null == modifiedInfos)
                {
                    continue;
                }

                foreach (var info in modifiedInfos)
                {
                    _attachAttributeDic[info.Type] += info.Val;
                }
            }
        }

        private void Reset()
        {
            _attachmentList.Clear();
            for (var type = WeaponAttributeType.Bullet; type < WeaponAttributeType.Length; type++)
            {
                _attachAttributeDic[type] = 0;
            }
        }

        #region apply spread modifier

        private void ApplyAttachment(FixedSpreadLogic logic)
        {
            logic.ApplyModifier( _attachAttributeDic[WeaponAttributeType.Spread]);
        }

        private void ApplyAttachment(PistolSpreadLogic logic)
        {
            logic.ApplyModifier( _attachAttributeDic[WeaponAttributeType.Spread]);
        }

        private void ApplyAttachment(RifleSpreadLogic logic)
        {
            logic.ApplyModifier( 0);
        }

        private void ApplyAttachment(SniperSpreadLogic logic)
        {
            logic.ApplyModifier( 0);
        }

        #endregion

        #region apply kickback modifier

        private void ApplyAttachment(RifleKickbackLogic logic)
        {
            var arg = new RifleKickbackModifierArg()
            {
                BasicWidth = _attachAttributeDic[WeaponAttributeType.HBaseKickback],
                MaxWidth = _attachAttributeDic[WeaponAttributeType.HMaxKickback],
                ContinusWidth = _attachAttributeDic[WeaponAttributeType.HCKickback],
                BasicHeight = _attachAttributeDic[WeaponAttributeType.VBaseKickback],
                MaxHeight = _attachAttributeDic[WeaponAttributeType.VMaxKickback],
                ContinusHeight = _attachAttributeDic[WeaponAttributeType.VCKickback],
                Turnback = _attachAttributeDic[WeaponAttributeType.Turnback],
            };

            logic.ApplyModifier(arg);
        }

        private void ApplyAttachment(FixedKickbackLogic logic)
        {
            logic.ApplyModifier(_attachAttributeDic[WeaponAttributeType.VBaseKickback]);
        }

        #endregion

        #region apply accuracy modifier

        public void ApplyAttachment(IAccuracyLogic logic)
        {
            var fixedLogic = logic as BaseAccuracyLogic;
            if (null != fixedLogic)
            {
                ApplyAttachment(fixedLogic);
                return;
            }

            var pistolLogic = logic as PistolAccuracyLogic;
            if (null != pistolLogic)
            {
                ApplyAttachment(pistolLogic);
                return;
            }
        }

        public void ApplyAttachment(IKickbackLogic logic)
        {
            var fixedlogic = logic as FixedKickbackLogic;
            if (null != fixedlogic)
            {
                ApplyAttachment(fixedlogic);
                return;
            }

            var rifleLogic = logic as RifleKickbackLogic;
            if (null != rifleLogic)
            {
                ApplyAttachment(rifleLogic);
                return;
            }
        }

        public void ApplyAttachment(ISpreadLogic logic)
        {
            var pistolLogic = logic as PistolSpreadLogic;
            if (null != pistolLogic)
            {
                ApplyAttachment(pistolLogic);
                return;
            }

            var fixedLogic = logic as FixedSpreadLogic;
            if (null != fixedLogic)
            {
                ApplyAttachment(fixedLogic);
                return;
            }

            var rifleLogic = logic as RifleSpreadLogic;
            if (null != rifleLogic)
            {
                ApplyAttachment(rifleLogic);
                return;
            }

            var sniperLogic = logic as SniperSpreadLogic;
            if (null != sniperLogic)
            {
                ApplyAttachment(sniperLogic);
                return;
            }
        }

        private void ApplyAttachment(BaseAccuracyLogic logic)
        {
            logic.ApplyModifier(0);
        }

        private void ApplyAttachment(PistolAccuracyLogic logic)
        {
            logic.ApplyModifier(0);
        }

        #endregion

        #region apply sound modifier

        public void ApplyAttachment(IWeaponSoundLogic logic)
        {
            var defaultLogic = logic as DefaultWeaponSoundLogic;
            if (null != defaultLogic)
            {
                ApplyAttachment(defaultLogic);
            }
        }

        public void ApplyAttachment(DefaultWeaponSoundLogic logic)
        {
            logic.ApplyModifier((int) _attachAttributeDic[WeaponAttributeType.FireSound]);
        }

        #endregion

        public void ApplyAttachment(IWeaponLogic logic)
        {
            var defaultLogic = logic as DefaultWeaponLogic;
            if (null != defaultLogic)
            {
                ApplyAttachment(defaultLogic);
            }
        }

        public void ApplyAttachment(DefaultWeaponLogic logic)
        {
            logic.ApplyModifier(0);
        }

        public void ApplyAttachment(IFireLogic logic)
        {
            var defaultLogic = logic as DefaultFireLogic;
            if (null != defaultLogic)
            {
                ApplyAttachment(defaultLogic);
            }
        }

        public void ApplyAttachment(DefaultFireLogic logic)
        {
            logic.ApplyModifier(new DefaultFireModifierArg
            {
                Fov = _attachAttributeDic[WeaponAttributeType.Fov],
                FocusSpeed = _attachAttributeDic[WeaponAttributeType.FocusSpeed],
                ReloadSpeed = _attachAttributeDic[WeaponAttributeType.ReloadSpeed]
            });
        }

        public void ApplyAttachment(IWeaponEffectLogic logic)
        {
            var defaultLogic = logic as DefaultWeaponEffectLogic;
            if (null != defaultLogic)
            {
                ApplyAttachment(defaultLogic);
            }
        }

        public void ApplyAttachment(IBulletContainer logic)
        {
            var bulletLogic = logic as BaseWeaponBulletLogic;
            if (null != bulletLogic)
            {
                ApplyAttachment(bulletLogic);
            }
        }

        public void ApplyAttachment(BaseWeaponBulletLogic logic)
        {
            logic.ApplyModifier((int) _attachAttributeDic[WeaponAttributeType.Bullet]);
        }

        public void ApplyAttachment(DefaultWeaponEffectLogic logic)
        {
            logic.ApplyModifier((int) _attachAttributeDic[WeaponAttributeType.Spark]);
        }

        public void ApplyAttachment(IBulletFactory logic)
        {
            var factory = logic as WeaponLogicComponentsFactory.BulletFactory;
            factory.ApplyModifier(_attachAttributeDic[WeaponAttributeType.Speed]);
        }
    }
}
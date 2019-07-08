using Assets.Utils.Configuration;
using Assets.XmlConfig;
using Core.Enums;
using Core.Free;
using UnityEngine;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Attack
{
    public struct PlayerDamageInfo : IRule
    {
        public float damage;
        public int type;    //EUIDeadType
        public int part;    //EBodyPart
        public int weaponId;
        public bool IsOverWall;
        public bool IsKnife;

        //击杀信息（多状态|） EUIKillType
        public int KillType;
        //击杀反馈（多状态|） EUIKillFeedbackType
        public int KillFeedbackType;
        public bool InstantDeath;

        public Vector3 HitPoint;
        public Vector3 HitDirection;

        public PlayerDamageInfo(float damage, int type, int part, int weaponId, bool isOverWall = false, bool isKnife = false, bool instantDeath = false,
            Vector3 hitPoint = default(Vector3), Vector3 hitDirection = default(Vector3))
        {
            this.damage = damage;
            this.type = type;
            this.part = part;
            this.weaponId = weaponId;
            IsOverWall = isOverWall;
            KillType = 0;
            KillFeedbackType = 0;
            InstantDeath = instantDeath;

            HitPoint = hitPoint;
            HitDirection = hitDirection;

            WeaponResConfigItem weapon = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
            if(weapon != null)
                IsKnife = weapon.Type == (int)EWeaponType_Config.MeleeWeapon;
            else
                IsKnife = false;
        }

        public EWeaponSubType WeaponType
        {
            get
            {
                if (type == (int)EUIDeadType.Weapon)
                {
                    var config = SingletonManager.Get<WeaponResourceConfigManager>().GetConfigById(weaponId);
                    if (null != config)
                    {
                        return (EWeaponSubType) config.SubType;
                    }
                }
                return EWeaponSubType.None;
            }
        }

        public int GetRuleID()
        {
            return (int)ERuleIds.PlayerDamageInfo;
        }
    }
}

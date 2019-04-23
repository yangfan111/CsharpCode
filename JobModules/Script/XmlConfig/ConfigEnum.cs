using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.XmlConfig
{
    public enum ECategory
    {
        /// <summary>
        /// 游戏资源 货币经验积分战绩
        /// </summary>
        GameRes = 1,
        /// <summary>
        /// 武器
        /// </summary>
        Weapon = 2,
        /// <summary>
        /// 武器皮肤
        /// </summary>
        WeaponSurface = 3,
        /// <summary>
        /// 角色
        /// </summary>
        Role = 4,
        /// <summary>
        /// 武器配件
        /// </summary>
        WeaponPart = 5,
        /// <summary>
        /// 载具
        /// </summary>
        Vehicle = 6,
        /// <summary>
        /// 载具皮肤
        /// </summary>
        VehicleSurface = 7,
        /// <summary>
        /// 角色avatar
        /// </summary>
        Avatar = 9,
        /// <summary>
        /// 道具
        /// </summary>
        Item = 10,
        /// <summary>
        /// 礼包
        /// </summary>
        Gift = 11,
        /// <summary>
        /// 段位
        /// </summary>
        Rank = 12,
        /// <summary>
        /// 局内道具
        /// </summary>
        GameItem = 13,
    }

    public enum EWeaponType_Config
    {
        None,
        PrimeWeapon,
        SubWeapon,
        MeleeWeapon,
        ThrowWeapon,
        TacticWeapon,
    }

    public enum EWeaponSubType
    {
        None = 0,
        Rifle,
        Sniper,
        SubMachineGun,
        MachineGun,
        ShotGun,
        Pistol,
        Throw,
        Melee,
        Grenade,
        FlashBomb,
        FogBomb,
        BurnBomb,
        Hand,
        AccurateRifle,
        C4,
    }
}

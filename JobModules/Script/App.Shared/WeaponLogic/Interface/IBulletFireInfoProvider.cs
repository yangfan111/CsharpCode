using UnityEngine;

namespace Core.WeaponLogic.Bullet
{
    public interface IBulletFireInfo
    {
        /// <summary>
        /// 视线方向的射出位置 
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireViewPosition(PlayerEntity playerEntity, Contexts contexts);
        /// <summary>
        /// 枪口方向的射出位置
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireEmitPosition(PlayerEntity playerEntity, Contexts contexts);
        /// <summary>
        /// 射出方向
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireDir(int seed, PlayerEntity playerEntity, WeaponEntity weaponEntity, Contexts contexts);
    }
}

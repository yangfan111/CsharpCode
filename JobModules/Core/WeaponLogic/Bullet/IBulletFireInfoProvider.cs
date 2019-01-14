using UnityEngine;

namespace Core.WeaponLogic.Bullet
{
    public interface IBulletFireInfoProviderDispatcher : IBulletFireInfoProvider
    {
        /// <summary>
        /// 初始化
        /// </summary>
        void Prepare();  
    }

    public interface IBulletFireInfoProvider
    {
        /// <summary>
        /// 视线方向的射出位置 
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireViewPosition();
        /// <summary>
        /// 枪口方向的射出位置
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireEmitPosition();
        /// <summary>
        /// 射出方向
        /// </summary>
        /// <returns></returns>
        Vector3 GetFireDir(int seed);
    }
}

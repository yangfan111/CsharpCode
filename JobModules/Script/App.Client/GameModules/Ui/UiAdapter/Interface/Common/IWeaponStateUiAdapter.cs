using System.Collections.Generic;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using Utils.AssetManager;

namespace App.Client.GameModules.Ui.UiAdapter
{
    public interface IWeaponStateUiAdapter : IAbstractUiAdapter
    {
        /// <summary>
        /// 当前持有武器对应的武器槽位索引
        /// </summary>
        int HoldWeaponSlotIndex { get;  }
        /// <summary>
        /// 返回武器槽位索引对应的武器Id
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int WeaponIdByIndex(int index);
        /// <summary>
        /// 返回武器Id对应的ab
        /// </summary>
        /// <param name="weaponId"></param>
        /// <returns></returns>
        AssetInfo GetAssetInfoById(int weaponId);
        /// <summary>
        /// 返回武器槽索引对应的武器攻击模式
        /// 1：全自动，2：半自动，3：连射
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int WeaponAtkModeByIndex(int index);
        /// <summary>
        /// 返回武器槽索引对应的武器子弹数量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int WeaponBulletCountByIndex(int index);
        /// <summary>
        /// 返回武器槽索引对应的弹夹数量
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int WeaponReservedBulletByIndex(int index);
        /// <summary>
        /// 返回索引对应的武器槽是否拥有武器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        bool HasWeaponByIndex(int index);
        /// <summary>
        /// 返回武器槽索引对应的武器类别
        /// 1：一般枪类武器，载弹量显示为N/M
        /// 2：一般投掷类武器，没有弹夹，载弹量显示为N
        /// 3：一般近战武器，无消耗，载弹量不显示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int WeaponTypeByIndex(int index);
        /// <summary>
        /// 返回武器槽索引对应的武器攻击模式数量，攻击模式数量为1时，隐藏攻击模式显示
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        int WeaponAtkModeNumberByIndex(int index);
        /// <summary>
        /// 返回是否处于切枪状态
        /// </summary>
        bool IsSwitchWeapon { get; }
	    /// <summary>
	    /// 返回当前装备的手雷索引，从1到4
	    /// </summary>
	    /// <returns></returns>
	    int CurrentGrenadeIndex { get; }
		/// <summary>
		/// 返回当前手雷索引是否有对应的手雷
		/// </summary>
		/// <returns></returns>
		bool HasGrenadByIndex(int grenadeIndex);
        /// <summary>
        /// 返回手雷槽索引对应的手雷Id
        /// </summary>
        /// <param name="grenadeIndex"></param>
        /// <returns></returns>
        int GrenadeIdByIndex(int grenadeIndex);
        /// <summary>
        /// 槽位序号列表(武器hud的从上至下对应list的从头到尾)
        /// </summary>
        List<int> SlotIndexList { get; }
	
	}
}
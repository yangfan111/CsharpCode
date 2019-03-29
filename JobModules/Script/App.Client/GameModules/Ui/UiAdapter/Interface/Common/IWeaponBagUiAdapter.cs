using App.Shared.GameModules.Weapon;
using Assets.App.Client.GameModules.Ui.UiAdapter.Interface;
using UserInputManager.Lib;
using Utils.AssetManager;
using XmlConfig;

namespace App.Shared.Components.Ui.UiAdapter
{
    /// <summary>
    /// 主武器，副武器，近战武器，战术武器，投掷武器列表
    /// </summary>
    public interface IWeaponBagInfo
    {
        IWeaponBagItemInfo PrimeWeapon{ get; }
        IWeaponBagItemInfo SubWeapon { get; }
        IWeaponBagItemInfo MeleeWeapon { get; }
        IWeaponBagItemInfo TacticalWeapon { get; }
        /// <summary>
        /// 手雷槽索引(从左到右1,2,3）对应的手雷信息
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IWeaponBagItemInfo GetGrenadeInfoByIndex(int index);
    }

    /// <summary>
    /// 武器图标AB，武器名称，武器配件AB列表
    /// </summary>
    public interface IWeaponBagItemInfo
    {
        AssetInfo WeaponIconAssetInfo{ get; }
        string WeaponName { get; }
        /// <summary>
        /// 每种配件类型对应的配件，如果没有配件则返回空的AssetInfo
        /// </summary>
        /// <param name="weaponPartType"></param>
        /// <returns></returns>
        AssetInfo GetWeaponPartInfoByWeaponPartType(EWeaponPartType weaponPartType);
    }

    public interface IWeaponBagUiAdapter : IAbstractUiAdapter
    {
        PlayerWeaponController Controller
        {
            get;
            set;
        }
        /// <summary>
        /// 返回背包索引对应的背包信息，如果对应背包为空背包返回null,背包索引从1开始
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        IWeaponBagInfo GetWeaponBagInfoByBagIndex(int index);

        /// <summary>
        /// 当前选择的背包索引
        /// </summary>
        int CurBagIndex
        {
            get;set;
        }


        /// <summary>
        /// 切换至对应的背包
        /// </summary>
        /// <param name="index"></param>
        void SwitchBagByBagIndex(int index);

        /// <summary>
        /// 控制武器背包界面的显示
        /// </summary>


        /// <summary>
        /// 背包剩余可操作时间
        /// </summary>
        int RemainOperating
        {
            get;
        }

        void SetCrossVisible(bool isVisible);

        bool CanOpenBag
        {
            get;
        }
    }
}

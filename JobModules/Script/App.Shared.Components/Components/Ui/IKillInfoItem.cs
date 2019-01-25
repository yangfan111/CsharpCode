using Core.Enums;
using Utils.AssetManager;

namespace App.Shared.Components.Ui
{
    public interface IKillInfoItem
    {
        int createTime { get; set; }
        string killerName { get; set; }
        long killerTeamId { get; set; }
        int killerWeaponId { get; set; }
        int killType { get; set; }
        string deadName { get; set; }
        long deadTeamId { get; set; }
        EUIDeadType deathType { get; set; }
        AssetInfo weaponAsset { get; set; }
    }
}
using Core.Room;
using Core.Utils;
using Utils.Configuration;
using Utils.Singleton;
using XmlConfig;

namespace App.Shared.GameMode
{
    /// <summary>
    /// Defines the <see cref="ModeUtil" />
    /// </summary>
    public static class ModeUtil
    {
        public static GameModeControllerBase CreateSharedPlayerMode(Contexts contexts, int modeId)
        {
            var bagType = SingletonManager.Get<GameModeConfigManager>().GetBagTypeById(modeId);
            GameModeControllerBase newController;
            switch (bagType)
            {
                case EBagType.Chicken:
                    newController = new GameSurvivalModeController();
                    DebugUtil.MyLog("Create chicken mode",DebugUtil.DebugColor.Black);
                    break;
                //case EBagType.Group:
                default:
                    newController = new GameCommonModeController();
                    DebugUtil.MyLog("Create Common mode", DebugUtil.DebugColor.Black);

                    break;
            }
            newController.Initialize(contexts, modeId);
            DebugUtil.MyLog("Create mode finish", DebugUtil.DebugColor.Black);
            return newController;
        }
        public static int RoomWeaponCompareCmd(PlayerWeaponBagData left, PlayerWeaponBagData right)
        {
            return left.BagIndex - right.BagIndex;
        }
    }
}

using App.Shared.GameMode;
using Core;
using Core.Utils;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="PlayerEntityModeBaseExt" />
    /// </summary>
    public static class PlayerEntityModeBaseExt
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityModeBaseExt));

        public static GameModeControllerBase ModeController(this PlayerEntity player)
        {
            return GameModuleManagement.Get<GameModeControllerBase>(player.entityKey.Value.EntityId);
        }

        /// <summary>
        /// 添加武器组件控制器相关 
        /// </summary>
        /// <param name="player"></param>
        public static void AttachModeController(this PlayerEntity player, ISessionMode playerMode)
        {
            GameModuleManagement.ForceCache(player.entityKey.Value.EntityId, (GameModeControllerBase)playerMode );
        }
    }
}

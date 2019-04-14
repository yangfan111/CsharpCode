using App.Shared.GameMode;
using App.Shared.Player;
using Core;
using Core.Utils;

namespace App.Shared
{
    /// <summary>
    /// Defines the <see cref="PlayerEntityModeBaseExt" />
    /// </summary>
    public static class PlayerEntityStateInteractBaseExt
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(PlayerEntityModeBaseExt));

        public static PlayerStateInteractController StateInteractController(this PlayerEntity player)
        {
            return GameModuleManagement.Get<PlayerStateInteractController>(player.entityKey.Value.EntityId);
        }

    }
}

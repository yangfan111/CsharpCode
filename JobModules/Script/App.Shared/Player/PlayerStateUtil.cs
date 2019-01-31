using App.Shared.Components.Player;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.Player
{
    public static class PlayerStateUtil
    {
        public static bool HasUIState(EPlayerUIState state, GamePlayComponent player)
        {
            return (player.UIState & (1 << (int)state)) > 0;
        }

        public static void AddUIState(EPlayerUIState state, GamePlayComponent player)
        {
            player.UIState |= 1 << (int)state;
        }

        public static void RemoveUIState(EPlayerUIState state, GamePlayComponent player)
        {
            player.UIState &= ~(1 << (int)state);
        }

        public static bool HasPlayerState(EPlayerGameState state, GamePlayComponent player)
        {
            return (player.PlayerState & (1 << (int)state)) > 0;
        }

        public static void AddPlayerState(EPlayerGameState state, GamePlayComponent player)
        {
            player.PlayerState |= 1 << (int)state;
        }

        public static void RemoveGameState(EPlayerGameState state, GamePlayComponent player)
        {
            player.PlayerState &= ~(1 << (int)state);
        }

        public static bool HasCastState(EPlayerCastState state, GamePlayComponent player)
        {
            return (player.CastState & (1 << (int)state)) > 0;
        }

        public static void AddCastState(EPlayerCastState state, GamePlayComponent player)
        {
            player.CastState |= 1 << (int)state;
        }

        public static void RemoveCastState(EPlayerCastState state, GamePlayComponent player)
        {
            player.CastState &= ~(1 << (int)state);
        }
    }

    public enum EPlayerUIState
    {
        BagOpen = 1,
        MapOpen = 2
    }

    public enum EPlayerGameState
    {
        SpeedUp = 1,
        CanJump = 2,
        HasArmor = 3,
        HasHelmet = 4,
        DivingChok = 5,
        NotMove = 6,
        InterruptItem=7,
        
        PlayerReborn,
        PlayerRevive,
        PlayerDead,
        PlayerDying,
        TurnOver,
        TurnStart
    }

    public enum EPlayerCastState
    {
        C4Pickable = 1,
        C4Defuse = 2,
    }
}

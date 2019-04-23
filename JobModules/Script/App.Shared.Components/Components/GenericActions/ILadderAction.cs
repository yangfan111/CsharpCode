using System;
using Core.Fsm;

namespace App.Shared.Components.GenericActions
{
    public interface ILadderAction
    {
        void Update(PlayerEntity player, float speed);
        void Execute(PlayerEntity player, Action<FsmOutput> addOutput);
        void PlayerReborn(PlayerEntity player);
        void PlayerDead(PlayerEntity player);
    }
}

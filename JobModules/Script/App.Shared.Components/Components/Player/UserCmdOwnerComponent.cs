using Core.Prediction.UserPrediction.Cmd;
using Entitas;

namespace App.Shared.Components.Player
{
    [Player]
    public class UserCmdOwnerComponent : IComponent
    {
        public IUserCmdOwner OwnerAdater;
    }
}
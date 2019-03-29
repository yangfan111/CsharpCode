using Core.SessionState;
using Entitas;

namespace App.Client.GameModules.ClientInit
{
    public class CheckPreparingPlayerSystem : IExecuteSystem
    {
        private readonly ISessionState _sessionState;


        private readonly Contexts _contexts;

        public CheckPreparingPlayerSystem(Contexts contexts, ISessionState sessionState)
        {
            _sessionState = sessionState;
            _contexts = contexts;
            _sessionState.CreateExitCondition(typeof(CheckPreparingPlayerSystem));
        }

        public void Execute()
        {
            var player = _contexts.player.flagSelfEntity;
            if (player != null)
            {
                if (player.hasThirdPersonModel && player.thirdPersonModel.Value != null &&
                    player.hasFirstPersonModel && player.firstPersonModel.Value != null
                )
                {
                    _sessionState.FullfillExitCondition(typeof(CheckPreparingPlayerSystem));
                }
            }
        }
    }
}
using Core.GameModule.Interface;

namespace App.Client.GameModules.Player
{
    public abstract class AbstractSelfPlayerGamePlaySystem:IGamePlaySystem
    {

        public readonly PlayerContext _playerContext;

        protected AbstractSelfPlayerGamePlaySystem(Contexts contexts)
        {
            _playerContext = contexts.player;
        }

        protected virtual bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity != null;
        }

        public  void OnGamePlay()
        {
            if (Filter(_playerContext.flagSelfEntity))
            {
                DoGamePlay(_playerContext.flagSelfEntity);
            }
        }

        protected abstract void DoGamePlay(PlayerEntity selfEntity);
    }
}
using App.Shared.Components.Player;
using App.Shared.Player;
using App.Shared.PlayerAutoMove;

namespace App.Shared.GameModules.Player.Move
{
    public class PlayerAutoMove : IPlayerAutoMove
    {
        private readonly GamePlayComponent _gamePlay;
        private readonly PlayerMoveComponent _playerMove;
        public PlayerAutoMove(PlayerEntity player)
        {
            if(null == player) return;
            _gamePlay = player.gamePlay;
            _playerMove = player.playerMove;
        }
        
        public void StartAutoMove()
        {
            if (null != _playerMove)
                _playerMove.IsAutoRun = true;
            if(null != _gamePlay)
                PlayerStateUtil.AddPlayerState(EPlayerGameState.InterruptItem, _gamePlay);
        }

        public void StopAutoMove()
        {
            if (null != _playerMove)
                _playerMove.IsAutoRun = false;
            if(null != _gamePlay)
                PlayerStateUtil.RemoveGameState(EPlayerGameState.InterruptItem, _gamePlay);
        }
    }
}
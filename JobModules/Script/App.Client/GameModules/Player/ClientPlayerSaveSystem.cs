using Core.Utils;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerSaveSystem : AbstractSelfPlayerRenderSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(ClientPlayerSaveSystem));

        private bool _isShow;
        private float _saveTime = 10;
        private Contexts _contexts;

        public ClientPlayerSaveSystem(Contexts contexts) : base(contexts)
        {
            _contexts = contexts;
        }

        protected override bool Filter(PlayerEntity playerEntity)
        {
            return playerEntity.hasGamePlay && _contexts.ui.uI != null;
        }

        public override void OnRender(PlayerEntity playerEntity)
        {
            var ui = _contexts.ui.uI;
            if (playerEntity.gamePlay.TipHideStatus)
            {
                ui.CountingDown = false;
                ui.CountDownNum = 0;
                ui.HaveCompletedCountDown = true;
                return;
            }

            /*if (playerEntity.gamePlay.IsSave || playerEntity.gamePlay.IsBeSave)
            {
                if (!_isShow)
                {
                    _isShow = true;
                    ui.CountingDown = true;
                    ui.CountDownNum = _saveTime;
                }
            }
            else
            {
                if (_isShow)
                {
                    _isShow = false;
                    ui.CountingDown = false;
                    ui.CountDownNum = 0;
                    ui.HaveCompletedCountDown = !playerEntity.gamePlay.IsInteruptSave;
                }
            }*/
        }
    }
}
using Core.GameModule.Interface;

namespace App.Client.GameModules.Player
{
    public class ClientPlayerSaveSystem : AbstractSelfPlayerRenderSystem
    {
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
            if (playerEntity.gamePlay.IsSave || playerEntity.gamePlay.IsBeSave)
                {
                    if (!_isShow)
                    {
                        _isShow = true;
                        _contexts.ui.uI.CountingDown = true;
                        _contexts.ui.uI.CountDownNum = _saveTime;
                    }
                }
                else if (_isShow)
                {
                    _isShow = false;
                    _contexts.ui.uI.CountingDown = false;
                    _contexts.ui.uI.CountDownNum = 0;
                }
            }
        }
}
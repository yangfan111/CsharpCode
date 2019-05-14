using com.wd.free.unit;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

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
            var gamePlay = playerEntity.gamePlay;
            var ui = _contexts.ui.uI;
            if (gamePlay.IsInteruptSave)
            {
                _logger.InfoFormat("save action interrupted. {0} {1}", gamePlay.IsSave, gamePlay.IsBeSave);
                /*if (_isShow)
                {*/
                    gamePlay.IsInteruptSave = false;
                    _isShow = false;
                    ui.CountingDown = false;
                    ui.CountDownNum = 0;
                //}      
            }
            else
            {
                if ((gamePlay.IsBeSave || gamePlay.IsSave))
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
                    _isShow = false;
                }
            } 
            
        }
    }
}
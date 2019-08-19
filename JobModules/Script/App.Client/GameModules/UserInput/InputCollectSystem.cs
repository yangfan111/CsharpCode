using Core.GameModule.Interface;
using Core.SessionState;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace App.Client.GameModules.UserInput
{
    public class InputCollectSystem : AbstractStepExecuteSystem 
    {
        private readonly UserInputContext _userInputContext;
        private readonly PlayerContext _playerContext;

        public InputCollectSystem(Contexts contexts)
        {
            _userInputContext = contexts.userInput;
            _playerContext = contexts.player;
        }

        protected override void InternalExecute()
        {
            if (Camera.main != null)
            {
                var lastLayer = ProcessIgnoreLayers();
                _userInputContext.userInputManager.Mgr.Dispatch();
                ResumeIgnoreLayers(lastLayer);
            }
        }
        
        private int? ProcessIgnoreLayers()
        {
            int? lastLayer = null;
            var mySelf = _playerContext.flagSelfEntity;
            if(null != mySelf && mySelf.hasCharacterContoller)
            {
                var characterController = mySelf.characterContoller.Value.gameObject;
                lastLayer = characterController.layer;
                characterController.layer = UnityLayerManager.GetLayerIndex(EUnityLayerName.User);
            }
            return lastLayer;
        }        

        private void ResumeIgnoreLayers(int? lastLayer)
        {
            if(!lastLayer.HasValue)
            {
                return;
            }
            var mySelf = _playerContext.flagSelfEntity;
            if(null != mySelf && mySelf.hasCharacterContoller)
            {
                var characterController = mySelf.characterContoller.Value.gameObject;
                characterController.layer = lastLayer.Value;
            }
        }
    }
}

using System.Collections.Generic;
using Core.EntityComponent;
using Core.GameInputFilter;
using Core.Prediction.UserPrediction.Cmd;
using Core.UpdateLatest;
using Core.Utils;

namespace App.Shared.GameModules.Player
{
    public class UserCmdOwnerAdapter : IUserCmdOwner
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(UserCmdOwnerAdapter));
        private PlayerEntity _playerEntity;

        public UserCmdOwnerAdapter(PlayerEntity playerEntity)
        {
            _playerEntity = playerEntity;
        }


        public List<IUserCmd> UserCmdList
        {
            get
            {
                if (!_playerEntity.hasUserCmd)
                    _playerEntity.AddUserCmd();
                int lastSeq = _playerEntity.userCmdSeq.LastCmdSeq;

                return _playerEntity.userCmd.GetLargerThan(lastSeq);
                
            }

        }
        
        public List<UpdateLatestPacakge> UpdateList {  get
        {
            if (!_playerEntity.hasUpdateMessagePool)
                _playerEntity.AddUpdateMessagePool();;
            return _playerEntity.updateMessagePool.UpdateMessagePool.GetPackagesLargeThan(_playerEntity
                .updateMessagePool.LastestExecuteUserCmdSeq);
        } }

        public int LastestExecuteUserCmdSeq
        {
            get { return _playerEntity
                .updateMessagePool.LastestExecuteUserCmdSeq; }
            set { _playerEntity
                .updateMessagePool.LastestExecuteUserCmdSeq = value; }
        }
        public int LastCmdSeq
        {
            get { return _playerEntity.userCmdSeq.LastCmdSeq; }
            set { _playerEntity.userCmdSeq.LastCmdSeq = value; }
        }

        public object OwnerEntity
        {
            get { return _playerEntity; }
        }

        public EntityKey OwnerEntityKey { get { return _playerEntity.entityKey.Value; } }
        public IUserCmd LastTempCmd
        {
            get
            {
                if (!_playerEntity.hasUserCmd)
                    _playerEntity.AddUserCmd();
                return _playerEntity.userCmd.LastTemp;
            }
        }

        public IGameInputProcessor UserInputProcessor
        {
            get
            {
                if(!_playerEntity.hasUserCmdFilter)
                {
                    return null;
                }
                return _playerEntity.userCmdFilter.GameInputProcessor;
            }
        }

        public IFilteredInput Filter(IUserCmd userCmd)
        {
            var inputProcessor = _playerEntity.userCmdFilter.GameInputProcessor;
            if(!_playerEntity.isEnabled)
            {
                Logger.Error("player is destroyed");
                return inputProcessor.DummyInput(); 
            }
            if(_playerEntity.isFlagDestroy)
            {
                return inputProcessor.DummyInput(); 
            }
            if(null != inputProcessor)
            {
                inputProcessor.Init();
                inputProcessor.SetUserCmd(userCmd);
                return inputProcessor.Filter();
            }
            else
            {
                Logger.Error("no GameInputProcessor attached to player ");
                return inputProcessor.DummyInput(); 
            }
        }
    }
}
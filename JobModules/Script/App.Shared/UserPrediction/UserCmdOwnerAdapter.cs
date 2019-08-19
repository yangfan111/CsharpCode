using System.Collections.Generic;
using Core.EntityComponent;
using Core;
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

        public List<UpdateLatestPacakge> UpdateList
        {
            get
            {
                if (!_playerEntity.hasUpdateMessagePool)
                    _playerEntity.AddUpdateMessagePool();
                ;
                return _playerEntity.updateMessagePool.UpdateMessagePool.GetPackagesLargeThan(_playerEntity
                    .updateMessagePool
                    .LastestExecuteUserCmdSeq);
            }
        }

        public int LastestExecuteUserCmdSeq
        {
            get
            {
                return _playerEntity
                    .updateMessagePool.LastestExecuteUserCmdSeq;
            }
            set
            {
                _playerEntity
                    .updateMessagePool.LastestExecuteUserCmdSeq = value;
            }
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

        public EntityKey OwnerEntityKey
        {
            get { return _playerEntity.entityKey.Value; }
        }

     

        /// <summary>
        /// 获得过滤后的input命令
        /// </summary>
        /// <param name="userCmd"></param>
        /// <returns></returns>
        public IFilteredInput GetFiltedInput(IUserCmd userCmd)
        {
            var interactController = _playerEntity.StateInteractController();
            if (!_playerEntity.isEnabled || _playerEntity.isFlagDestroy)
            {
                Logger.Error("player is destroyed");
                return interactController.EmptyInput;
            }

            return interactController.ApplyUserCmd(userCmd, _playerEntity.playerWeaponDebug.DebugAutoMove);
        }

        public bool IsEnable()
        {
            if (!_playerEntity.isEnabled || _playerEntity.isFlagDestroy)
            {
                return false;
            }

            return true;
        }
    }
}
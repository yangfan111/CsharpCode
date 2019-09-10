using System.Collections.Generic;
using App.Shared;
using App.Shared.GameModules;
using App.Shared.GameModules.Player;
using Core;
using Core.EntityComponent;
using Core.Prediction.UserPrediction.Cmd;
using Core.UpdateLatest;
using Core.Utils;
public static class PlayerEntityUserCmdExt
{
    public static PlayerUserCmdController UserCmdController(this PlayerEntity playerEntity)
    {
        return GameModuleManagement.Get<PlayerUserCmdController>(playerEntity.entityKey.Value.EntityId);
    }

}
namespace App.Shared.GameModules
{
    public class ServerPlayerUserCmdController : PlayerUserCmdController
    {
        public override void Initialize(PlayerEntity player)
        {
           base.Initialize(player);
           player.AddUpdateMessagePool();
        }

        
        public new List<UpdateLatestPacakge> UpdateList
        {
            get
            {
                return playerEntity.updateMessagePool.Value.GetPackagesLargeThan(LastestExecuteUserCmdSeq);
            }
        }
        
        public new int LastestExecuteUserCmdSeq
        {
            get { return playerEntity.updateMessagePool.LastestExecuteUserCmdSeq; }
            set { playerEntity.updateMessagePool.LastestExecuteUserCmdSeq = value; }
        }
    }
   
    public class PlayerUserCmdController : ModuleLogicActivator<PlayerUserCmdController>,IPlayerUserCmdGetter
    {
        private LoggerAdapter _loggerAdapter = new LoggerAdapter("PlayerUserCmdController");
        protected PlayerEntity playerEntity;

        public List<IUserCmd> UserCmdList
        {
            get
            {
                int lastSeq = playerEntity.userCmdSeq.LastCmdSeq;
                return playerEntity.userCmd.GetLargerThan(lastSeq);
            }
        }

        public List<UpdateLatestPacakge> UpdateList
        {
            get { return null; }
        }


        public int LastCmdSeq
        {
            get { return playerEntity.userCmdSeq.LastCmdSeq; }
            set { playerEntity.userCmdSeq.LastCmdSeq = value; }
        }

        public int LastestExecuteUserCmdSeq { get; set; }
        

        public object OwnerEntity
        {
            get { return playerEntity; }
        }

        public EntityKey OwnerEntityKey
        {
            get { return playerEntity.entityKey.Value; }
        }


        public virtual void Initialize(PlayerEntity player)
        {
            playerEntity = player;
            playerEntity.AddUserCmd();
            playerEntity.AddUserCmdSeq(0);
            playerEntity.AddLatestAdjustCmd(-1, -1);
            playerEntity.AddUpdateMessagePool();
        }

        /// <summary>
        ///     获得过滤后的input命令
        /// </summary>
        /// <param name="userCmd"></param>
        /// <returns></returns>
        public IFilteredInput GetFiltedInput(IUserCmd userCmd)
        {
            var interactController = playerEntity.StateInteractController();
            if (!playerEntity.isEnabled || playerEntity.isFlagDestroy)
            {
                _loggerAdapter.Error("player is destroyed");
                return interactController.EmptyInput;
            }

            return interactController.ApplyUserCmd(userCmd, playerEntity.playerWeaponDebug.DebugAutoMove);
        }

        public bool IsEnable()
        {
            if (!playerEntity.isEnabled || playerEntity.isFlagDestroy)
            {
                return false;
            }

            return true;
        }
    }
}
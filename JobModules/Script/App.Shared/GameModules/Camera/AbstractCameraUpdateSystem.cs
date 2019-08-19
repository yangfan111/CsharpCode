using App.Shared.GameModules.Camera.Utils;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Camera
{
    public abstract class AbstractCameraUpdateSystem
    {   
        protected abstract void ExecWhenObserving(PlayerEntity player, IUserCmd cmd);
        
        protected abstract void ExecWhenNormal(PlayerEntity player, IUserCmd cmd);
        
        protected virtual void FinalExec(PlayerEntity player, IUserCmd cmd) {}        
        protected virtual void BeforeExec(PlayerEntity player, IUserCmd cmd) {}

        protected Contexts _contexts;
        protected bool LockView;
        
        public AbstractCameraUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
            LockView = !contexts.session.commonSession.RoomInfo.MultiAngleStatus;
        }
        
        public void CommonUpdate(PlayerEntity player, IUserCmd cmd)
        {
            BeforeExec(player, cmd);
            if (player.gamePlay.IsObserving())
            {
                ExecWhenObserving(player,cmd);
            }
            else
            {
                ExecWhenNormal(player, cmd);
            }
            FinalExec(player,cmd);
        }
    }

}
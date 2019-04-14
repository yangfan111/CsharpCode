using App.Shared.GameModules.Camera.Utils;
using Core.Prediction.UserPrediction.Cmd;

namespace App.Shared.GameModules.Camera
{
    public abstract class AbstractCameraUpdateSystem
    {   
        protected abstract void ExecWhenObserving(PlayerEntity player, IUserCmd cmd);
        
        //temporary: observed player calcu twice and upload data(final position and rotation)
        //           observing player copy the data
        //should be: observed player upload states, observing player calcu with states 
        protected abstract void ExecWhenBeingObserved(PlayerEntity player, IUserCmd cmd);
        protected abstract void ExecWhenNormal(PlayerEntity player, IUserCmd cmd);
        
        protected virtual void FinalExec(PlayerEntity player, IUserCmd cmd) {}        
        protected virtual void BeforeExec(PlayerEntity player, IUserCmd cmd) {}
        protected virtual bool CanPlayAsNormal(PlayerEntity player)
        {
            return true;
        }

        protected Contexts _contexts;
        
        public AbstractCameraUpdateSystem(Contexts contexts)
        {
            _contexts = contexts;
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
                if (CanPlayAsNormal(player))
                {
                    ExecWhenNormal(player,cmd);
                }
            
                if (player.gamePlay.BeingObserved)
                {
                    ExecWhenBeingObserved(player,cmd);
                }
            }
            FinalExec(player,cmd);
        }
    }

}
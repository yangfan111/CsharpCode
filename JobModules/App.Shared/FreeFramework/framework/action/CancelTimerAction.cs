using App.Server.GameModules.GamePlay;
using Sharpen;
using com.wd.free.@event;
using com.wd.free.action;
using com.wd.free.util;

namespace com.wd.free.action
{
    [System.Serializable]
    public class CancelTimerAction : AbstractGameAction
    {
        private const long serialVersionUID = -3126561548984555432L;

        private string name;

        public override void DoAction(IEventArgs args)
        {
            args.FreeContext.TimerTask.Stop(FreeUtil.ReplaceVar(name, args));
        }
    }
}

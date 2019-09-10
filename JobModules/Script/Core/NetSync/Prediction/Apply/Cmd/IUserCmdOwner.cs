using System.Collections.Generic;
using Core.EntityComponent;
using Core;
using Core.UpdateLatest;

namespace Core.Prediction.UserPrediction.Cmd
{
    public interface IPlayerUserCmdGetter
    {
        //GetLargerThan(lastSeq)
        List<IUserCmd> UserCmdList { get; }
        int LastCmdSeq { set; get; }
        //只给服务器端使用
        List<UpdateLatestPacakge> UpdateList { get; }
        //只给服务器端使用
        int LastestExecuteUserCmdSeq { set; get; }
        object OwnerEntity { get; }
        EntityKey OwnerEntityKey { get; }
      
        IFilteredInput GetFiltedInput(IUserCmd userCmd);
        bool IsEnable();
    }
}
using System.Collections.Generic;
using Core.EntityComponent;
using Core.GameInputFilter;
using Core.UpdateLatest;

namespace Core.Prediction.UserPrediction.Cmd
{
    public interface IUserCmdOwner
    {
        List<IUserCmd> UserCmdList { get; }
        List<UpdateLatestPacakge> UpdateList { get; }
        int LastCmdSeq { set; get; }
        int LastestExecuteUserCmdSeq { set; get; }
        object OwnerEntity { get; }
        EntityKey OwnerEntityKey { get; }
        IUserCmd LastTempCmd { get;  }
        IFilteredInput Filter(IUserCmd userCmd);
    }
}
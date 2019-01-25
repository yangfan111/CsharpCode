using Core.Prediction.UserPrediction.Cmd;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.FreeFramework.framework.ai.player
{
    public interface IPlayerCmdHandler
    {
        bool CanHandle(Contexts contexts, PlayerEntity player, IUserCmd cmd);
        void Handle(Contexts contexts, PlayerEntity player, IUserCmd cmd);
    }
}

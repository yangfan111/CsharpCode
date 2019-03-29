using System;
using Core.EntityComponent;
using Core.Prediction;
using Core.Prediction.UserPrediction.Cmd;
using Core.Replicaton;

namespace Core.UpdateLatest
{
    public interface IUpdateLatestManager
    {
        UpdateLatestPacakge CreateUpdateSnapshot(EntityKey selfKey);
    }
    public class UpdateLatestManager:IUpdateLatestManager
    {
        private IUpdateLatestHandler _handler;

        public UpdateLatestManager(IUpdateLatestHandler handler)
        {
            _handler = handler;
        }

        public UpdateLatestPacakge CreateUpdateSnapshot(EntityKey selfKey)
        {
            return _handler.GetUpdateLatestPacakge(selfKey);
        }
    }
}
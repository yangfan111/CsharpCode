using System;
using Core.SessionState;
using Core.Utils;
using Entitas;

namespace Core.Prediction
{
    public class PredictionInitSystem : AbstractStepExecuteSystem
    {
        private  static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PredictionInitSystem));
        private PredictionManager mgr;

        public PredictionInitSystem(PredictionManager mgr)
        {
            this.mgr = mgr;
        }


        protected override void InternalExecute()
        {
            try
            {
                mgr.PredictionInitUpdate();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("error executing {0}",  e);
            }
        }

       
    }
}

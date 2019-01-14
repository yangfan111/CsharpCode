using System;
using Core.SessionState;
using Core.Utils;
using Entitas;

namespace Core.Prediction
{
    public class PredictionInitSystem : AbstractStepExecuteSystem
    {
        private  static readonly LoggerAdapter _logger = new LoggerAdapter(typeof(PredictionInitSystem));
        private IPredictionInitManager _predictionInitManager;

        public PredictionInitSystem(IPredictionInitManager predictionInitManager)
        {
            _predictionInitManager = predictionInitManager;
        }


        protected override void InternalExecute()
        {
            try
            {
                _predictionInitManager.PredictionInit();
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("error executing {0}",  e);
            }
        }

       
    }
}

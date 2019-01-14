using System;
using Core.GameModule.Step;
using Core.Utils;
using Entitas;
using UnityEngine;

namespace Core.SessionState
{
    public abstract class AbstractStepExecuteSystem:IExecuteSystem
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(AbstractStepExecuteSystem));
        protected abstract  void InternalExecute();
        public IExecuteSystem WithExecFrameStep(EEcecuteStep step)
        {
            _execFrameStep = step;
            return this;
        }

        protected float Interval = 0;
        
        private EEcecuteStep _execFrameStep = EEcecuteStep.NormalFrameStep;
        public void Execute()
        {
            try
            {
                Interval += Time.deltaTime;
                if (!StepExecuteManager.Instance.IsStepExecute(_execFrameStep)) return;
                
                InternalExecute();
                Interval = 0;
            }
            catch (Exception e)
            {
                _logger.ErrorFormat("Exception ;{0} {1}",this.GetType().Name, e);
            }
          
        }
    }
}
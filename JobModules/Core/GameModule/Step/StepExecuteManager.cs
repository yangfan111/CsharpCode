using System;
using UnityEngine;

namespace Core.GameModule.Step
{
    public interface IStepExecuteConfig
    {
        bool IsExecute(int frameCount);
        void Tick(float delta);
        void UpdateStep(float fps, float timepassed);
        float RealFps { get; }
        float TargetFps { get; set; }
    }

    public class DummyStepExecuteConfig : IStepExecuteConfig
    {
        public bool IsExecute(int frameCount)
        {
            return true;
        }

        public void Tick(float delta)
        {
            
        }

        public void UpdateStep(float fps, float timepassed)
        {
            
        }

        public float RealFps { get; private set; }
        public float TargetFps { get; set; }
    }
    public class StepExecuteConfig: IStepExecuteConfig
    {
        public float TargetFps { get; set; }


        public int Count = 0;
        public float RealFps { get; private set; }
        private bool _isExecute;

        public StepExecuteConfig(int targetFps)
        {
            TargetFps = targetFps;
        }

        public void UpdateStep(float fps, float timepassed)
        {
            RealFps = Count / timepassed;
            Count = 0;
        }

        public bool IsExecute(int frameCount)
        {
            return _isExecute;
        }

        private float _totalDelta = 0;

        public void Tick(float delta)
        {
            float needDeltaTime = 1 / TargetFps;
            _totalDelta += delta > needDeltaTime ? needDeltaTime : delta;
            if (_totalDelta >= needDeltaTime)
            {
                _isExecute = true;
                _totalDelta -= needDeltaTime;
                Count++;
            }
            else
            {
                _isExecute = false;
            }
        }
    }

    public class StepExecuteManager
    {
        private static StepExecuteManager _instance = null;

        private StepExecuteManager()
        {
        }

        public static StepExecuteManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new StepExecuteManager();
                }

                return _instance;
            }
        }

        private int _frameCount;
        private int _addFrameCount;
        private float _lastTime;
        private float _timePassed;
        private IStepExecuteConfig _cmdFrameStepConfig = new DummyStepExecuteConfig();
        private IStepExecuteConfig _uiFrameStepConfig = new StepExecuteConfig(20);

        public void Update()
        {
            var delta = Time.deltaTime;
            _timePassed += Time.deltaTime;
            _addFrameCount++;
            if (Time.time - _lastTime > 5)
            {
                _lastTime = Time.time;
                var fps = _addFrameCount / _timePassed;
                _cmdFrameStepConfig.UpdateStep(fps, _timePassed);
                _uiFrameStepConfig.UpdateStep(fps, _timePassed);

                _addFrameCount = 0;

                _timePassed = 0;
            }

            _frameCount = Time.frameCount;

            _cmdFrameStepConfig.Tick(delta);
            _uiFrameStepConfig.Tick(delta);
        }

        public bool IsStepExecute(EEcecuteStep step)
        {
            switch (step)
            {
                case EEcecuteStep.NormalFrameStep:
                    return true;
                case EEcecuteStep.CmdFrameStep:
                    return _cmdFrameStepConfig.IsExecute(_frameCount);
                case EEcecuteStep.UIFrameStep:
                    return _uiFrameStepConfig.IsExecute(_frameCount);
                default:
                    throw new ArgumentOutOfRangeException("step", step, null);
            }

            return true;
        }

        public string FpsString()
        {
            return string.Format("fps rate;  cmd:{0:G2} ui: {1:G2}", _cmdFrameStepConfig.RealFps, _uiFrameStepConfig.RealFps);
        }

        public void SetFps(EEcecuteStep step, int fps)
        {
            switch (step)
            {
                case EEcecuteStep.CmdFrameStep:
                    _cmdFrameStepConfig.TargetFps = fps;
                    break;
                case EEcecuteStep.UIFrameStep:
                    _uiFrameStepConfig.TargetFps = fps;
                    break;
            }

            return;
        }

        public void UpdateCmdTargetFps(int fps)
        {
            _cmdFrameStepConfig.TargetFps = fps;
        }
    }
}
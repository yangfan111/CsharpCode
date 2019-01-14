using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Singleton;

namespace Core.SessionState
{

    public class SubProgressBlackBoard : Singleton<SubProgressBlackBoard>
    {
        internal bool Enabled;
        private float _totalNum;
        private float _currentNum;

        public void Reset()
        {
            _totalNum = _currentNum = 0;
        }

        public void Add(uint num = 1)
        {
            if(Enabled)
                _totalNum += num;
        }

        public void Step(uint num = 1)
        {
            if(Enabled)
                _currentNum += num;
        }

        public float ProgressRate
        {
            get
            {
                if (Enabled && _totalNum > 0)
                {
                    return Math.Min(_currentNum / _totalNum, 1.0f);
                }

                return 0.0f;
            }
        }
    }

    public abstract class SessionStateProgress : ISessionStateMachineMonitor
    {
        private int _totalProgressNum;
        private int _loadedProgress;

        private bool _finished;

        private ISessionState _currentState;
        private int _currentProgressNum;

        public SessionStateProgress()
        {
            SingletonManager.Get<SubProgressBlackBoard>().Enabled = true;
        }

        public void AddState(ISessionState state)
        {
            _totalProgressNum += state.LoadingProgressNum;
        }

        public void ChangeState(ISessionState state)
        {
            if (_currentState != state)
            {
                if (_currentState != null)
                {
                    _loadedProgress += _currentProgressNum;
                }

                if (_loadedProgress >= _totalProgressNum)
                {
                    _finished = true;
                    SingletonManager.Get<SubProgressBlackBoard>().Enabled = false;
                }

                _currentState = state;
                _currentProgressNum = _currentState.LoadingProgressNum;
                SingletonManager.Get<SubProgressBlackBoard>().Reset();
            }
        }

        public void LateUpdate()
        {
            if (_currentState != null)
            {
                if (_finished)
                {
                    OnProgressUpdated(float.MaxValue, "");
                    _currentState = null;
                }
                else
                {
                    var progress = _currentProgressNum  * SingletonManager.Get<SubProgressBlackBoard>().ProgressRate;
                    progress = Math.Min(Math.Max(progress, 0), _currentState.LoadingProgressNum);
                    OnProgressUpdated((_loadedProgress + progress) / (float)_totalProgressNum, _currentState.LoadingTip);
                }
            }

        }


        protected abstract void OnProgressUpdated(float progressRate, string tip);
    }
}

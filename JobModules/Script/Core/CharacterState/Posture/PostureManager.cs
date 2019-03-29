using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Fsm;
using Utils.Appearance;
using XmlConfig;

namespace Core.CharacterState.Posture
{
    class PostureManager : FsmManager, IFsmUpdate, ICharacterPostureInConfig, IGetPostureState
    {
        private readonly PostureFsm _commonFsm;
        private readonly PostureFsm _leanFsm;
        private FsmUpdateType _commonUpdateType = FsmUpdateType.ResponseToInput | FsmUpdateType.ResponseToAnimation;
        private FsmUpdateType _leanUpdateType = FsmUpdateType.ResponseToInput;

        public PostureManager(IFsmTransitionHelper infoProvider)
        {
            _commonFsm = new PostureFsm("StandCrouchProne");
            _commonFsm.InitAsCommonState(infoProvider);
            AddFsm(_commonFsm);

            _leanFsm = new PostureFsm("Lean");
            _leanFsm.InitAsLeanState(infoProvider);
            AddFsm(_leanFsm);
        }

        public void Reset(Action<FsmOutput> addOutput)
        {
            _commonFsm.Reset(addOutput);
            _leanFsm.Reset(addOutput);
        }

        public void SetName(string name)
        {
            _commonFsm.Name = name;
            _leanFsm.Name = name;
        }

        #region IFsmUpdate

        public void Update(IAdaptiveContainer<IFsmInputCommand> commands,
                           int frameInterval,
                           Action<FsmOutput> addOutput,
                           FsmUpdateType updateType)
        {
            if ((_commonUpdateType & updateType) != 0)
            {
                _commonFsm.Update(commands, frameInterval, addOutput);
            }
            if ((_leanUpdateType & updateType) != 0)
            {
                _leanFsm.Update(commands, frameInterval, addOutput);
            }
        }

        #endregion

        #region ICharacterPosture

        public PostureInConfig GetCurrentPostureState()
        {
            return _commonFsm.GetCurrentPostureState();
        }

        public PostureInConfig GetNextPostureState()
        {
            return _commonFsm.GetNextPostureState();
        }

        public LeanInConfig GetCurrentLeanState()
        {
            return _leanFsm.GetCurrentLeanState();
        }

        public LeanInConfig GetNextLeanState()
        {
            return _leanFsm.GetNextLeanState();
        }

        public bool IsNeedJumpSpeed()
        {
            return _commonFsm.IsNeedJumpSpeed();
        }

        public bool IsNeedJumpForSync
        {
            get
            {
                return _commonFsm.IsNeedJumpForSync;
            }
            set { _commonFsm.IsNeedJumpForSync = value; }
        }
		
        public bool IsJumpStart()
        {
            return _commonFsm.IsJumpStart();
        }

        #endregion

        #region ICharacterPostureInConfig

        public bool InTransition()
        {
            return _commonFsm.InTransition();
        }

        public float TransitionRemainTime()
        {
            return _commonFsm.TransitionRemainTime();
        }

        public float TransitionTime()
        {
            return _commonFsm.TransitionTime();
        }

        public PostureInConfig CurrentPosture()
        {
            return GetCurrentOrNextState(true);
        }

        public PostureInConfig NextPosture()
        {
            return GetCurrentOrNextState(false);
        }

        private PostureInConfig GetCurrentOrNextState(bool getCurrent)
        {
            var ret = PostureInConfig.Null;
            ret = getCurrent ? _commonFsm.GetCurrentPostureState():_commonFsm.GetNextPostureState();
            return ret;
        }

        #endregion
    }
}

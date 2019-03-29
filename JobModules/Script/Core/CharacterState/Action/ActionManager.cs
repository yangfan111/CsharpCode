using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState.Posture;
using Core.Fsm;
using Utils.Appearance;
using XmlConfig;

namespace Core.CharacterState.Action
{
    class ActionManager : FsmManager, IFsmUpdate, IGetActionState
    {
        private readonly ActionFsm _commonFsm;
        private readonly ActionFsm _keepFsm;

        private FsmUpdateType _commonUpdateType = FsmUpdateType.ResponseToInput | FsmUpdateType.ResponseToAnimation;
        private FsmUpdateType _keepUpdateType = FsmUpdateType.ResponseToInput;

        public ActionManager(IFsmTransitionHelper infoProvider)
        {
            _commonFsm = new ActionFsm("CommonAction");
            _commonFsm.InitCommon(infoProvider);
            AddFsm(_commonFsm);

            _keepFsm = new ActionFsm("KeepAction");
            _keepFsm.InitKeep(infoProvider);
            AddFsm(_keepFsm);
        }

        public void Reset(Action<FsmOutput> addOutput)
        {
            _commonFsm.Reset(addOutput);
            _keepFsm.Reset(addOutput);
        }

        public void SetName(string name)
        {
            _commonFsm.Name = name;
            _keepFsm.Name = name;
        }

        #region IFsmUpdate

        public void Update(IAdaptiveContainer<IFsmInputCommand> commands,
                           int frameInterval,
                           Action<FsmOutput> addOutput,
                           FsmUpdateType updateType)
        {
            if ((updateType & _commonUpdateType) != 0)
            {
                _commonFsm.Update(commands, frameInterval, addOutput);
            }

            if ((updateType & _keepUpdateType) != 0)
            {
                _keepFsm.Update(commands, frameInterval, addOutput);
            }
        }

        #endregion

        public bool CanFire()
        {
            return _commonFsm.CanFire();
        }

        public ActionInConfig GetActionState()
        {
            return _commonFsm.GetActionState();
        }

        public ActionInConfig GetNextActionState()
        {
            return _commonFsm.GetNextActionState();
        }

        public ActionKeepInConfig GetActionKeepState()
        {
            return _keepFsm.GetActionKeepState();
        }

        public ActionKeepInConfig GetNextActionKeepState()
        {
            return _keepFsm.GetNextActionKeepState();
        }
    }
}

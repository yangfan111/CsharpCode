using Utils.AssetManager;
using System.Collections.Generic;
using System;
using Core.Utils;
using Utils.Compare;
using Utils.Utils;
using XmlConfig;

namespace Utils.Appearance.Weapon.WeaponShowPackage
{
    public abstract class WeaponDataAdapter
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(WeaponDataAdapter));

        private class Command
        {
            private WeaponInPackage _weaponPosition;
            private int _id;
            private int _attachment;
            private WeaponPartLocation _attachmentLocation;

            private Action _changeOverrideController;
            private Action<WeaponInPackage> _changeWeaponInHand;
            private Action<WeaponInPackage, int> _changeWeaponInPackage;
            private Action<WeaponInPackage, WeaponPartLocation, int> _changeWeaponAttachment;

            public void SetOverrideController(Action action)
            {
                _changeOverrideController = action;
            }

            public void SetChangeWeaponInHand(Action<WeaponInPackage> action, WeaponInPackage pos)
            {
                _changeWeaponInHand = action;
                _weaponPosition = pos;
            }

            public void SetChangeWeaponInPackage(Action<WeaponInPackage, int> action, WeaponInPackage pos, int id)
            {
                _changeWeaponInPackage = action;
                _weaponPosition = pos;
                _id = id;
            }
            
            public void SetChangeAttachment(Action<WeaponInPackage, WeaponPartLocation, int> action,
                WeaponInPackage pos,
                WeaponPartLocation location,
                int id)
            {
                _changeWeaponAttachment = action;
                _weaponPosition = pos;
                _attachmentLocation = location;
                _attachment = id;
            }

            public void Execute()
            {
                if (_changeOverrideController != null)
                {
                    _changeOverrideController.Invoke();
                    _changeOverrideController = null;
                }
                
                if (_changeWeaponInHand != null)
                {
                    _changeWeaponInHand.Invoke(_weaponPosition);
                    _changeWeaponInHand = null;
                }

                if (_changeWeaponInPackage != null)
                {
                    _changeWeaponInPackage.Invoke(_weaponPosition, _id);
                    _changeWeaponInPackage = null;
                }
                
                if (_changeWeaponAttachment != null)
                {
                    _changeWeaponAttachment.Invoke(_weaponPosition, _attachmentLocation, _attachment);
                    _changeWeaponAttachment = null;
                }
            }
        }

        #region ILatestWeaponState

        private readonly int[] _latestWeaponValue = new int[(int) LatestWeaponStateIndex.EndOfTheWorld];

        protected int GetLatestWeaponValue(LatestWeaponStateIndex type)
        {
            return _latestWeaponValue[(int) type];
        }

        protected void SetLatestWeaponValue(LatestWeaponStateIndex type, int value)
        {
            var index = (int) type;
            _latestWeaponValue[index] = value;
        }

        #endregion

        #region IPredictedWeaponState

        private readonly int[] _predictedWeaponValue = new int[(int) PredictedWeaponStateIndex.EndOfTheWorld];

        protected int GetPredictedWeaponValue(PredictedWeaponStateIndex type)
        {
            return _predictedWeaponValue[(int) type];
        }

        protected void SetPredictedWeaponValue(PredictedWeaponStateIndex type, int value)
        {
            var index = (int) type;
            _predictedWeaponValue[index] = value;
        }

        #endregion

        #region IClientWeaponState

        private readonly int[] _clientWeaponValue = new int[(int) ClientWeaponStateIndex.EndOfTheWorld];
        private readonly bool[] _clientWeaponRewindFlag = new bool[(int) ClientWeaponStateIndex.EndOfTheWorld];

        protected int GetClientWeaponValue(ClientWeaponStateIndex type)
        {
            return _clientWeaponValue[(int) type];
        }

        protected void SetClientWeaponValue(ClientWeaponStateIndex type, int value)
        {
            var index = (int) type;
            _rewindFlag =
                (_clientWeaponRewindFlag[index] = _clientWeaponRewindFlag[index] ||
                                                  !CompareUtility.IsApproximatelyEqual(_clientWeaponValue[index], value)
                ) || _rewindFlag;
            _clientWeaponValue[index] = value;
        }

        #endregion

        private bool _rewindFlag;

        private readonly List<Command> _outerCommand = new List<Command>();
        private int _currentCommandIndex;

        protected WeaponDataAdapter()
        {
            Init();
        }

        #region handle outer command

        public void MountWeaponToPackage(WeaponInPackage pos, int id)
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInPackage(MountWeaponInPackageImpl, pos, id);
        }

        public void UnmountWeaponInPackage(WeaponInPackage pos)
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInPackage(UnmountWeaponInPackageImpl, pos, UniversalConsts.InvalidIntId);
        }

        public void MountWeaponToHand(WeaponInPackage pos)
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInHand(MountWeaponToHandImpl, pos);
            Logger.DebugFormat("MountWeaponToHand: {0}", pos);
        }

        public void UnmountWeaponFromHand()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInHand(UnMountWeaponFromHandImpl, WeaponInPackage.EmptyHanded);
            Logger.DebugFormat("UnmountWeaponFromHand");
        }

        public void UnmountWeaponFromHandAtOnce()
        {
            UnMountWeaponFromHandImpl(WeaponInPackage.EmptyHanded);
        }
        
        public void JustUnMountWeaponFromHand()
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeWeaponInHand(JustUnMountWeaponFromHandImpl, WeaponInPackage.EmptyHanded);
            Logger.InfoFormat("JustUnMountWeaponFromHand");
        }
        
        public void JustClearOverrideController()
        {
            var cmd = GetAvailableCommand();
            cmd.SetOverrideController(ClearOverrideController);
            Logger.InfoFormat("JustClearOverrideController");
        }
        
        public void MountAttachment(WeaponInPackage pos, WeaponPartLocation location, int id)
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeAttachment(MountAttachmentImpl, pos, location, id);
        }

        public void UnmountAttachment(WeaponInPackage pos, WeaponPartLocation location)
        {
            var cmd = GetAvailableCommand();
            cmd.SetChangeAttachment(UnmountAttachmentImpl, pos, location, UniversalConsts.InvalidIntId);
        }

        #endregion


        public void Execute()
        {
            for (int i = 0; i < _currentCommandIndex; i++)
            {
                _outerCommand[i].Execute();
            }

            _currentCommandIndex = 0;
        }

        public void Init()
        {
            for (var i = 0; i < (int) LatestWeaponStateIndex.EndOfTheWorld; ++i)
            {
                _latestWeaponValue[i] = UniversalConsts.InvalidIntId;
            }

            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int)WeaponInPackage.EmptyHanded;
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                (int) OverrideControllerState.Null;

//            for (var i = 0; i < (int) ClientWeaponStateIndex.EndOfTheWorld; ++i)
//            {
//                _clientWeaponValue[i] = UniversalConsts.InvalidIntId;
//            }
        }

        private void MountWeaponInPackageImpl(WeaponInPackage pos, int id)
        {
            UpdateWeaponIdInPackage(pos, id);
            if (_predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] == (int) pos)
                _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                    (int) OverrideControllerState.Null;
        }

        private void UnmountWeaponInPackageImpl(WeaponInPackage pos, int id)
        {
            if (_predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] == (int) pos)
            {
                _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int)WeaponInPackage.EmptyHanded;
                _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                    (int) OverrideControllerState.Null;
            }
            UpdateWeaponIdInPackage(pos, id);
        }
        
        private void UpdateWeaponIdInPackage(WeaponInPackage pos, int id)
        {
            var index = GetIndexFromPos(pos);
            if (index >= 0 && index < _latestWeaponValue.Length)
                _latestWeaponValue[index] = id;
        }

        protected abstract AbstractLoadRequest CreateLoadRequest(AssetInfo assetInfo, ILoadedHandler loadedHandler);

        private void MountWeaponToHandImpl(WeaponInPackage pos)
        {
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int) pos;
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                (int) OverrideControllerState.Null;
        }

        private void UnMountWeaponFromHandImpl(WeaponInPackage pos)
        {
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int) WeaponInPackage.EmptyHanded;
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                (int) OverrideControllerState.Null;
        }

        private void JustUnMountWeaponFromHandImpl(WeaponInPackage pos)
        {
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.WeaponInHand] = (int) WeaponInPackage.EmptyHanded;
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                (int) OverrideControllerState.Transition;
        }
        
        // 恢复空手状态机
        private void ClearOverrideController()
        {
            _predictedWeaponValue[(int) PredictedWeaponStateIndex.OverrideControllerState] =
                (int) OverrideControllerState.NoTransition;
        }

        private void MountAttachmentImpl(WeaponInPackage pos, WeaponPartLocation location, int attachmentId)
        {
            UpdateAttachmentType(pos, location, attachmentId);
        }

        private void UnmountAttachmentImpl(WeaponInPackage pos, WeaponPartLocation location, int type)
        {
            UpdateAttachmentType(pos, location, type);
        }
        
        private void UpdateAttachmentType(WeaponInPackage pos, WeaponPartLocation location, int type)
        {
            if (WeaponInPackage.PrimaryWeaponOne != pos &&
                WeaponInPackage.PrimaryWeaponTwo != pos &&
                WeaponInPackage.SideArm != pos) return;
            
            AssertUtility.Assert(
                (int)WeaponPartLocation.EndOfTheWorld * 3 + 
                (int)WeaponInPackage.EndOfTheWorld - 1 == _latestWeaponValue.Length - 1, 
                "Add New WeaponPartLocation Or WeaponInPackage Enum");

            var index = GetIndexFromPos(pos) + (int) location + 1;
            if (index < 0 || index >= _latestWeaponValue.Length) return;
            _latestWeaponValue[index] = type;
        }

        private Command GetAvailableCommand()
        {
            if (_currentCommandIndex >= _outerCommand.Count)
            {
                _outerCommand.Add(new Command());
            }

            return _outerCommand[_currentCommandIndex++];
        }

        private static int GetIndexFromPos(WeaponInPackage pos)
        {
            switch (pos)
            {
                case WeaponInPackage.EmptyHanded:
                    return (int) LatestWeaponStateIndex.EmptyHand;
                case WeaponInPackage.PrimaryWeaponOne:
                    return (int) LatestWeaponStateIndex.PrimaryWeaponOne;
                case WeaponInPackage.PrimaryWeaponTwo:
                    return (int) LatestWeaponStateIndex.PrimaryWeaponTwo;
                case WeaponInPackage.SideArm:
                    return (int) LatestWeaponStateIndex.SideArm;
                case WeaponInPackage.MeleeWeapon:
                    return (int) LatestWeaponStateIndex.MeleeWeapon;
                case WeaponInPackage.ThrownWeapon:
                    return (int) LatestWeaponStateIndex.ThrownWeapon;
                case WeaponInPackage.TacticWeapon:
                    return (int) LatestWeaponStateIndex.TacticWeapon;
            }

            throw new Exception(string.Format("Unexpected Exception: {0}", pos));
        }
    }
}

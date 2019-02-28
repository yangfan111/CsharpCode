using System;
using System.Collections.Generic;
using Assets.App.Shared.GameModules.Camera;
using Core.CameraControl.NewMotor;
using Core.Utils;
using UnityEngine;
using XmlConfig;

namespace App.Shared.GameModules.Camera.Motor.Peek
{
    class PeekOffMotor : AbstractCameraMotor
    {
        private short _modeId = (short) ECameraFreeMode.Off;
       
        private float transitionTime = 100f;

        private Vector3 _finalPostOffset;

        public override int Order
        {
            get { return 0; }
        }

        public PeekOffMotor(
            float transitionTime
        )
        {
            this.transitionTime = transitionTime;
        }

        public override short ModeId
        {
            get { return _modeId; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return true;
        }


        public override Vector3 FinalPostOffset
        {
            get { return _finalPostOffset; }
        }


        private static LoggerAdapter _logger = new LoggerAdapter(typeof(PeekOffMotor));
        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
            var elapsedPercent = ElapsedPercent( clientTime,subState.ModeTime,transitionTime ) + 1-Math.Abs(state.LastPeekPercent);
            if (elapsedPercent >= 1)
            {
                elapsedPercent = 1;
                state.LastPeekPercent = 0;
            }
            output.PostOffset = Vector3.Lerp(last.FinalPostOffset, FinalPostOffset, elapsedPercent);
            
            if (elapsedPercent < 1)
            {
                _logger.DebugFormat("output.PostOffset:{0}",output.PostOffset);
            }
        

        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
           
        }

        
        
        public override HashSet<short> ExcludeNextMotor()
        {
            return EmptyHashSet;
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
           
        }
    }
}
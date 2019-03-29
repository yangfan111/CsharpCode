using System.Collections.Generic;
using App.Shared.GameModules.Camera.Utils;
using Core.CameraControl.NewMotor;
using Core.Interpolate;
using UnityEngine;
using XmlConfig;

namespace Assets.App.Shared.GameModules.Camera.Motor.Free
{
    class FreeOffMotor : AbstractCameraMotor
    {
      
        private float _transitionTime ;
        private SubCameraMotorType _motorType;


        public FreeOffMotor(float transitionTime
        )
        {
            _transitionTime = transitionTime;
            _motorType = SubCameraMotorType.View;
        }

        public override int Order
        {
            get { return 0; }
        }
        public override short ModeId
        {
            get { return  (short) ECameraFreeMode.Off; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return true;
        }


        public override void CalcOutput(PlayerEntity player, 
            ICameraMotorInput input, 
            ICameraMotorState state,
            SubCameraMotorState subState,
            DummyCameraMotorOutput output, 
            ICameraNewMotor last,
            int clientTime)
        {
            output.ArchorEulerAngle = Vector3.zero;
            if (last.ModeId == (short)ECameraFreeMode.On)
            {
                var elapsedPercent = ElapsedPercent( clientTime,subState.ModeTime,_transitionTime );

                if (elapsedPercent < 1)
                {
                    output.EulerAngle = Vector3.Lerp(new Vector3(state.LastFreePitch, state.LastFreeYaw, 0),Vector3.zero, elapsedPercent);
                    state.FreeYaw = output.ArchorEulerAngle.y;
                    state.FreePitch= output.ArchorEulerAngle.x;
                   
//                    output.ArchorPostOffset =
//                        Vector3.Lerp( -state.GetMainConfig().ScreenOffset,Vector3.zero, elapsedPercent);
                }
                else
                {
                    output.EulerAngle = Vector3.zero;
                    state.FreeYaw = output.ArchorEulerAngle.y;
                    state.FreePitch= output.ArchorEulerAngle.x;
                
                }

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
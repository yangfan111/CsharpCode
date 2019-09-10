using System.Collections.Generic;
using App.Shared.GameModules.Camera.Utils;
using App.Shared.Player;
using Assets.App.Shared.GameModules.Camera;
using UnityEngine;
using Utils.Appearance.Effects;
using XmlConfig;

namespace Core.CameraControl.NewMotor.View
{
    public class ThirdViewMotor:AbstractCameraMotor
    {
        public ThirdViewMotor(Motors m):base(m)
        {

            _motors.ActionManager.AddAction(CameraActionType.Enter, SubCameraMotorType.View, (int)ModeId, 
                (player, state) =>
            {
                if (player.hasAppearanceInterface && player.appearanceInterface.Appearance.IsFirstPerson)
                {
                    player.appearanceInterface.Appearance.SetThridPerson();
                    player.characterBoneInterface.CharacterBone.SetThridPerson();
                    player.UpdateCameraArchorPostion();
                    var playerUtils = EffectUtility.GetEffect(player.RootGo(), "PlayerUtils");
                    if (playerUtils != null)
                        playerUtils.SetParam("ThirdSightBegin", (object)player.RootGo().gameObject);
                }
            });
        }
        public override int Order
        {
            get
            {
                return 1;
            }
        }
        public override short ModeId
        {
            get { return (short)ECameraViewMode.ThirdPerson; }
        }

        public override bool IsActive(ICameraMotorInput input, ICameraMotorState state)
        {
            return true;
        }

        public override void CalcOutput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state, SubCameraMotorState subState,
            DummyCameraMotorOutput output, ICameraNewMotor last, int clientTime)
        {
        }

        public override HashSet<short> ExcludeNextMotor()
        {
            return EmptyHashSet;
        }

        public override void PreProcessInput(PlayerEntity player, ICameraMotorInput input, ICameraMotorState state)
        {
          
        }

        public override void UpdatePlayerRotation(ICameraMotorInput input, ICameraMotorState state, PlayerEntity player)
        {
            
        }
    }
}
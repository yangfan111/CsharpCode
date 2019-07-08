using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using Core.Components;
using Core.Prediction.UserPrediction.Cmd;
using Core.Utils;
using UnityEngine;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public static class CharacterBoneSynchronizer
    {
        public static void SyncToFirePositionComponent(FirePosition component, PlayerEntity playerEntity, IUserCmd cmd)
        {
            SyncSightFirePos(component, playerEntity);
            SyncMuzzleP3Pos(component, playerEntity);
        //    DebugUtil.MyLog( "[seq:{1}]MuzzleP3Position before:{0}",component.MuzzleP3Position,cmd.Seq);
        }

        private static void SyncMuzzleP3Pos(FirePosition component, PlayerEntity playerEntity)
        {
            var pos = GetMuzzleP3Pos(playerEntity);
            if (pos == null)
            {
                component.MuzzleP3Valid = false;
                component.MuzzleP3Position = FixedVector3.zero;
            }
            else
            {
                component.MuzzleP3Valid = true;
                component.MuzzleP3Position = pos.position.ShiftedToFixedVector3();
            }
        }

        private static void SyncSightFirePos(FirePosition component, PlayerEntity playerEntity)
        {
            var fireTrans = GetSightFirePos(playerEntity);
            if (fireTrans == null)
            {
                component.SightValid = false;
                component.SightPosition = FixedVector3.zero;
            }
            else
            {
                component.SightValid = true;
                component.SightPosition =fireTrans.position.ShiftedToFixedVector3();
            }
        }

        public static Transform GetMuzzleP3Pos(PlayerEntity playerEntity)
        {
            Transform ret = null;
            if (!playerEntity.hasCharacterBoneInterface || !playerEntity.stateInterface.State.CanFire() || !playerEntity.IsCameraCanFire())
            {
                return ret;
            }
            ret = playerEntity.characterBoneInterface.CharacterBone.GetLocation(Utils.CharacterState.SpecialLocation.MuzzleEffectPosition, Utils.CharacterState.CharacterView.FirstPerson);
            return ret;
        }

        private static Transform GetSightFirePos(PlayerEntity playerEntity)
        {
            Transform ret = null;
            if (!playerEntity.hasFirstPersonModel || !playerEntity.IsCameraCanFire() || !playerEntity.stateInterface.State.CanFire())
            {
                return ret;
            }
            ret = playerEntity.characterBoneInterface.CharacterBone.GetLocation(Utils.CharacterState.SpecialLocation.SightsLocatorPosition, Utils.CharacterState.CharacterView.FirstPerson);
            return ret;
        }
    }
}
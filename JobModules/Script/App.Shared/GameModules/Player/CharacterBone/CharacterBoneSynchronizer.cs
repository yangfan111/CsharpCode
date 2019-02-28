using App.Shared.Components.Player;
using App.Shared.GameModules.Camera.Utils;
using UnityEngine;
using Utils.Appearance;

namespace App.Shared.GameModules.Player.CharacterBone
{
    public static class CharacterBoneSynchronizer
    {
        public static void SyncToFirePositionComponent(FirePosition component, PlayerEntity playerEntity)
        {
            SyncSightFirePos(component, playerEntity);
            SyncMuzzleP3Pos(component, playerEntity);
        }

        private static void SyncMuzzleP3Pos(FirePosition component, PlayerEntity playerEntity)
        {
            var pos = GetMuzzleP3Pos(playerEntity);
            if (pos == null)
            {
                component.MuzzleP3Valid = false;
                component.MuzzleP3Position = Vector3.zero;
            }
            else
            {
                component.MuzzleP3Valid = true;
                component.MuzzleP3Position = pos.position;
            }
        }

        private static void SyncSightFirePos(FirePosition component, PlayerEntity playerEntity)
        {
            var fireTrans = GetSightFirePos(playerEntity);
            if (fireTrans == null)
            {
                component.SightValid = false;
                component.SightPosition = Vector3.zero;
            }
            else
            {
                component.SightValid = true;
                component.SightPosition = fireTrans.position;
            }
        }

        private static Transform GetMuzzleP3Pos(PlayerEntity playerEntity)
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
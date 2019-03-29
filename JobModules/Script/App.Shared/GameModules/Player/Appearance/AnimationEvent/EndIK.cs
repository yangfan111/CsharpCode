using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Utils.Appearance;
using XmlConfig;

namespace App.Shared.GameModules.Player.Appearance.AnimationEvent
{
    public class EndIK : IAnimationEventCallback
    {
        public void AnimationEventCallback(PlayerEntity player, string param, UnityEngine.AnimationEvent eventParam)
        {
            player.characterBoneInterface.CharacterBone.EndIK();
            //player.appearanceInterface.Appearance.EndIK();
        }
    }
}

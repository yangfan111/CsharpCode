using System;
using Core.CharacterController;
using Entitas;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    [Serializable]
    public class CharacterContollerComponent : IComponent
    {
        public ICharacterControllerContext Value;
    }
}
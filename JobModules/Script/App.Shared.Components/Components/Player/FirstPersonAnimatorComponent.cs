using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class FirstPersonAnimatorComponent : IComponent
    {
        private Animator _unityAnimator;
        public Animator UnityAnimator
        {
            get { return _unityAnimator; }
            set
            {
                _unityAnimator = value;
                _unityAnimator.enabled = false;
            }
        }
    }
}

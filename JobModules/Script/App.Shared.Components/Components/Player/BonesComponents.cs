using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class BonesComponent : IComponent
    {

        public Transform Head;
        
        public Transform Spine;

        public Transform FirstPersonCamera;
        
    }
}

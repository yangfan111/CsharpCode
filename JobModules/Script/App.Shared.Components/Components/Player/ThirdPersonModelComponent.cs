using Core.Components;
using Entitas.CodeGeneration.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class ThirdPersonModelComponent : IComponent
    {
        public GameObject Value;
    }
}

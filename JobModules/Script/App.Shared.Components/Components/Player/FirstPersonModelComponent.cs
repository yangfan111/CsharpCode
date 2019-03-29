using Core.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using UnityEngine;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    [Player]
    public class FirstPersonModelComponent : IComponent
    {
        public GameObject Value;
    }
}

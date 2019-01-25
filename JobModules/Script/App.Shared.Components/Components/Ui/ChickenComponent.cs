using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class ChickenComponent : IComponent
    {
        [DontInitilize] public Vector3 Test;
    }
}
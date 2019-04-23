using Entitas;
using UnityEngine;
using Utils.AssetManager;

namespace App.Shared.Components.Player
{
    [Player]
    public class FirstPersonModelComponent : IComponent
    {
        public GameObject Value;
        public UnityObject UnityObjectValue;
    }
}

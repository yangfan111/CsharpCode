using System.Collections;
using System.Collections.Generic;
using Core.Components;
using Core.EntityComponent;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player]
    public class RaycastTestComponent:IGameComponent
    {
        [DontInitilize] public int Num;
        public float Distance;
        public List<GameObject> MapObjects;
        
        public int GetComponentId()
        {
            return (int)EComponentIds.RaycastTest;
        }

        public void clear()
        {
            Num = 0;
            MapObjects.Clear();
        }

    }
}
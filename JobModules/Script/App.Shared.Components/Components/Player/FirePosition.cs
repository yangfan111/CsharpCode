using Core.Components;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Core.UpdateLatest;
using Entitas.CodeGeneration.Attributes;
using UnityEngine;

namespace App.Shared.Components.Player
{
    [Player, ]
    public class FirePosition:IUpdateComponent
    {
        [DontInitilize][NetworkProperty] public bool SightValid;
        [DontInitilize][NetworkProperty] public Vector3 SightPosition;
        [DontInitilize][NetworkProperty] public bool MuzzleP3Valid;
        [DontInitilize][NetworkProperty] public Vector3 MuzzleP3Position;

        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as FirePosition;
            SightValid = right.SightValid;
            SightPosition = right.SightPosition;
            MuzzleP3Valid = right.MuzzleP3Valid;
            MuzzleP3Position = right.MuzzleP3Position;
        }

        public int GetComponentId()
        {
            return (int) EComponentIds.PlayerFirePos;
        }
    }
}
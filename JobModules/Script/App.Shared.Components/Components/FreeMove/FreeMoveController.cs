using System;
using Core.Components;
using Core.Interpolate;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.FreeMove
{
    public enum EFreeMoveControllType
    {
        DontControll,
        FixFocusPos,
        End
    }
    
    [FreeMove]
    [Serializable]
    public class FreeMoveController:IPlaybackComponent
    {
        [NetworkProperty][DontInitilize()] public Byte ControllType;
        [NetworkProperty(SyncFieldScale.Position)][DontInitilize()] public FixedVector3 FocusOnPosition;
        
        public int GetComponentId()
        {
            return (int) EComponentIds.FreeMoveController;
        }

        public void CopyFrom(object rightComponent)
        {
            var r = rightComponent as FreeMoveController;
            FocusOnPosition = r.FocusOnPosition;
            ControllType = r.ControllType;
        }

        public void Interpolate(object left, object right, IInterpolationInfo interpolationInfo)
        {
            var l = left as FreeMoveController;
            var r = right as FreeMoveController;
            FocusOnPosition = InterpolateUtility.Interpolate(l.FocusOnPosition, r.FocusOnPosition, interpolationInfo);
            ControllType = r.ControllType;
        }

        public bool IsInterpolateEveryFrame()
        {
            return true;
        }
    }
}
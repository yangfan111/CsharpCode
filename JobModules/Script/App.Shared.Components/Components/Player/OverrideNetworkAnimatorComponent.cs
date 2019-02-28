using Core.Components;
using Core.Playback;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;

namespace App.Shared.Components.Player
{
    /// <summary>
    /// 覆盖NetworkAnimator的某一层，比如强制进入受伤状态
    /// </summary>
    [Player]
    public class OverrideNetworkAnimatorComponent : IComponent
    {
        [DontInitilize]
        public bool IsInjuryAnimatorActive;

        [DontInitilize] public int InjuryTriigerTime;
        
        [DontInitilize]
        public float InjuryNormalizeTime;
        
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.CharacterState;
using Core.Compare;
using Core.Prediction.UserPrediction;
using Core.SnapshotReplication.Serialization.NetworkObject;
using Core.SnapshotReplication.Serialization.NetworkProperty;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using Core.Animation;

namespace App.Shared.Components.Player
{
    [Player]
    public class AnimatorClipComponent : IComponent
    {
        public AnimatorClipManager ClipManager;
    }
}

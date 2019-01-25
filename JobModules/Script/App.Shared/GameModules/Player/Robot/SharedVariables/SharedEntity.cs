using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Player.Robot.SharedVariables;
using BehaviorDesigner.Runtime;
using Core.EntityComponent;
using UnityEngine;

namespace Assets.App.Shared.GameModules.Player.Robot.SharedVariables
{
   
    [System.Serializable]
    public class SharedPlayerEntity : SharedVariable<PlayerEntity>
    {
        public static implicit operator SharedPlayerEntity(PlayerEntity value) { return new SharedPlayerEntity { mValue = value }; }
    }

    public class SharedSceneObjectEntity : SharedVariable<SceneObjectEntity>
    {
        public static implicit operator SharedSceneObjectEntity(SceneObjectEntity value) { return new SharedSceneObjectEntity { mValue = value }; }
    }
    
}

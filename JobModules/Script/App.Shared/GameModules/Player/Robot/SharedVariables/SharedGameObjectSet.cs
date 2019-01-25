using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.SharedVariables
{
    [System.Serializable]
    public class SharedGameObjectSet : SharedVariable<HashSet<GameObject>>
    {
        public SharedGameObjectSet() { mValue = new HashSet<GameObject>(); }
        public static implicit operator SharedGameObjectSet(HashSet<GameObject> value) { return new SharedGameObjectSet { mValue = value }; }
    }
}

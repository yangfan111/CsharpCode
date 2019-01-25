using Utils.AssetManager;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System.Collections.Generic;
using UnityEngine;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class WaitToBeLoadedComponent : IComponent
    {
        public List<AssetInfo> LoadInfos;
        public List<GameObject> RecycleObjs;
    }
}

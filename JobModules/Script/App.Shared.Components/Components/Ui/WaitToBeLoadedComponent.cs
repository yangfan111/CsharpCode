using System;
using Utils.AssetManager;
using Entitas;
using Entitas.CodeGeneration.Attributes;
using System.Collections.Generic;

namespace App.Shared.Components.Ui
{
    [Ui, Unique]
    public class WaitToBeLoadedComponent : IComponent
    {
        public List<AssetInfo> LoadInfos;
        public List<Object> RecycleObjs;
    }
}

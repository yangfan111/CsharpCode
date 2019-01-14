using System;
using Core.EntityComponent;
using Entitas.CodeGeneration.Attributes;

namespace Core.Components
{
    
    public class LifeTimeComponent : IGameComponent
    {
        public DateTime CreateTime;
        public int LifeTime;
      

        [DontInitilize]
        public bool IsExpired
        {
            get { return (DateTime.Now - CreateTime).TotalMilliseconds > LifeTime; }
        }

        public int GetComponentId() { { return (int)ECoreComponentIds.LifeTime; } }
    }

}
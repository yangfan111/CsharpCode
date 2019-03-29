using System;
using Entitas;

namespace Core.EntityComponent
{
    public class ComponentInfo<TComponent> where TComponent : IGameComponent
    {
        private static int _componentId = -1;
        public static int ComponentId {
            get
            {
                if (_componentId == -1)
                {
                    var t = (IGameComponent)Activator.CreateInstance(typeof(TComponent));
                    _componentId = t.GetComponentId();
                }
                return _componentId;
            }
        }
    }
    public interface IGameComponentInfo
    {
        IGameComponent Allocate(int componentId);
        void Free(IGameComponent component);
        void Free(int componentId, IGameComponent component);
        int MaxComponentId { get; }
    }
    public interface IGameComponent : IComponent
    {
        int GetComponentId();
    }
}
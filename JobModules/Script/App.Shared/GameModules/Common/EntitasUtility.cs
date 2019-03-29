using System;
using Entitas;

namespace App.Shared.GameModules.Common
{
    internal static class EntitasUtility
    {
        public static object GetValueComponent(Entity entity, int compId, Type compType)
        {
            var component = entity.GetComponent(compId);
            var field = compType.GetField("value");
            var o = field.GetValue(component);
            return o;
        }

        public static void AddValueComponent(Entity entity, int compId, Type compType, object value)
        {
            var component = entity.CreateComponent(compId, compType);
            var field = compType.GetField("value");
            field.SetValue(component, value);
            entity.AddComponent(compId, component);
        }
    }
}
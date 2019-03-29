using System.IO;
using Core.EntityComponent;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class EntityKeySerializer :IFieldSerializer<EntityKey>
    {
        public void Write(EntityKey data, Core.Utils.MyBinaryWriter writer)
        {
            writer.Write(data.EntityId);
            writer.Write(data.EntityType);
        }

        public EntityKey Read(BinaryReader reader)
        {
            
            var entityId = reader.ReadInt32();
            var entityType = reader.ReadInt16();
            return  new EntityKey(entityId, entityType);
        }
    }
}

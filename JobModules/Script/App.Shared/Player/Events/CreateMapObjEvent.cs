using System.IO;
using App.Shared.Components.Serializer;
using Core.Event;
using Core.Utils;

namespace App.Shared.Player.Events
{
    public class CreateMapObjEvent:IEvent
    {
        public EEventType EventType
        {
            get { return EEventType.CreateMapObj; }
        }

        public int Type;
        public int Id;
        
        public bool IsRemote { get; set; }
        public void ReadBody(BinaryReader reader)
        {
            Type = FieldSerializeUtil.Deserialize(Type, reader);
            Id = FieldSerializeUtil.Deserialize(Id, reader);
        }

        public void WriteBody(MyBinaryWriter writer)
        {
            FieldSerializeUtil.Serialize(Type, writer);
            FieldSerializeUtil.Serialize(Id, writer);
        }

        public void RewindTo(IEvent value)
        {
            var right = value as CreateMapObjEvent;
            if (right == null) return;
            Type = right.Type;
            Id = right.Id;
        }
    }
}
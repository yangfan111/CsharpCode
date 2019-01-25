using System.Collections.Generic;
using System.IO;
using App.Shared.Components.Player;
using Core.Event;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    internal class EventsSerializer
    {
        public void Write(PlayerEvents data, MyBinaryWriter writer)
        {
           
            data.Write(writer);
        }

        public PlayerEvents Read(BinaryReader reader,  PlayerEvents events)
        {
            events.Read(reader);
            return events;
        }
    }
}
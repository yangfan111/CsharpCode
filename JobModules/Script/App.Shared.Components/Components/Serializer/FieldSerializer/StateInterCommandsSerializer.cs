using System.IO;
using Core.CharacterState;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class StateInterCommandsSerializer:IFieldSerializer<StateInterCommands>
    {
        public void Write(StateInterCommands data, MyBinaryWriter writer)
        {
            data.Write(writer);
        }

        public StateInterCommands Read(BinaryReader reader)
        {
            StateInterCommands ret = new StateInterCommands();
            ret.Read(reader);
            return ret;
        }
    }
}
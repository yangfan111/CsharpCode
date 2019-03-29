using System.IO;
using Core.CharacterState.Posture;
using Core.Utils;

namespace App.Shared.Components.Serializer.FieldSerializer
{
    public class UnityAnimationEventCommandsSerializer : IFieldSerializer<UnityAnimationEventCommands>
    {
        public void Write(UnityAnimationEventCommands data, MyBinaryWriter writer)
        {
            data.Write(writer);
        }

        public UnityAnimationEventCommands Read(BinaryReader reader)
        {
            UnityAnimationEventCommands ret = new UnityAnimationEventCommands();
            ret.Read(reader);
            return ret;
        }
    }
}
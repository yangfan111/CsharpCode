using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Text;
using Core.Animation;
using Core.Compare;
using Core.Fsm;
using Core.Prediction;
using Core.Utils;
using Utils.Utils.Buildin;

namespace Core.CharacterState
{
    public class StateInterCommands : ICloneableComponent
    {
        public List<KeyValuePair<short, float>> Commands = new List<KeyValuePair<short, float>>();

        public void Write(MyBinaryWriter writer)
        {
            writer.Write(Commands.Count);
            for (int i = 0; i < Commands.Count; ++i)
            {
                writer.Write(Commands[i].Key);
                writer.Write(Commands[i].Value);
            }
            
        }

        public void Read(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                
                var type = reader.ReadInt16();
                var value = reader.ReadSingle();
                Commands.Add(new KeyValuePair<short, float>(type, value));
            }
        }
        
        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as StateInterCommands;
            InitFields(right);
        }

        private void InitFields(StateInterCommands right)
        {
            if (Commands == null)
            {
                Commands = new List<KeyValuePair<short, float>>(32);
            }
            Commands.Clear();
            if (right.Commands != null)
            {
                Commands.AddRange(right.Commands);
            }
        }

        public StateInterCommands Clone()
        {
            StateInterCommands ret = new StateInterCommands();
            ret.Commands = new List<KeyValuePair<short, float>>(Commands);
            return ret;
        }

        public void Reset()
        {
            Commands.Clear();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Commands count:{0}\n", Commands.Count);
            foreach (KeyValuePair<short,float> keyValuePair in Commands)
            {
                sb.AppendFormat("key:{0}, value:{1}\t", keyValuePair.Key, keyValuePair.Value);
            }

            return sb.ToString();
        }


    }
}
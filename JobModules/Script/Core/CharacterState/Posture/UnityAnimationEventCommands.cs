using System.Collections.Generic;
using System.IO;
using System.Text;
using Core.Prediction;
using Core.Prediction.UserPrediction;
using Core.Utils;
using UnityEngine;

namespace Core.CharacterState.Posture
{
    public struct AnimationEventParam
    {
        public string EventParam;
        public int IntParameter;
        public float FloatParameter;
        public string StringParameter;

        public override string ToString()
        {
            return string.Format("EventParam: {0}, IntParameter: {1}, FloatParameter: {2}, StringParameter: {3}", EventParam, IntParameter, FloatParameter, StringParameter);
        }

        public UnityEngine.AnimationEvent ToAnimationEvent()
        {
            UnityEngine.AnimationEvent ret = new AnimationEvent();
            ret.intParameter = IntParameter;
            ret.floatParameter = FloatParameter;
            ret.stringParameter = StringParameter;
            return ret;
        }
    }
    
    public class UnityAnimationEventCommands: ICloneableComponent
    {
        public List<KeyValuePair<short, AnimationEventParam>> Commands = new List<KeyValuePair<short, AnimationEventParam>>();

        public void Write(MyBinaryWriter writer)
        {
            writer.Write(Commands.Count);
            for (int i = 0; i < Commands.Count; ++i)
            {
                writer.Write(Commands[i].Key);
                writer.Write(Commands[i].Value.EventParam);
                writer.Write(Commands[i].Value.IntParameter);
                writer.Write(Commands[i].Value.FloatParameter);
                writer.Write(Commands[i].Value.StringParameter);
            }
            
        }
        
        public void Read(BinaryReader reader)
        {
            int count = reader.ReadInt32();
            for (int i = 0; i < count; ++i)
            {
                
                var type = reader.ReadInt16();
                var eventParam = reader.ReadString();
                var intParam = reader.ReadInt32();
                var floatParam = reader.ReadSingle();
                var stringParam = reader.ReadString();
                Commands.Add(new KeyValuePair<short, AnimationEventParam>(type, new AnimationEventParam
                {
                    EventParam = eventParam,
                    IntParameter = intParam,
                    FloatParameter = floatParam,
                    StringParameter = stringParam
                }));
            }
        }
        
        public void CopyFrom(object rightComponent)
        {
            var right = rightComponent as UnityAnimationEventCommands;
            InitFields(right);
        }

        private void InitFields(UnityAnimationEventCommands right)
        {
            if (Commands == null)
            {
                Commands = new List<KeyValuePair<short, AnimationEventParam>>(32);
            }
            Commands.Clear();
            if (right.Commands != null)
            {
                Commands.AddRange(right.Commands);
            }
        }
        
        public UnityAnimationEventCommands Clone()
        {
            UnityAnimationEventCommands ret = new UnityAnimationEventCommands();
            ret.Commands = new List<KeyValuePair<short, AnimationEventParam>>(Commands);
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
            foreach (KeyValuePair<short,AnimationEventParam> keyValuePair in Commands)
            {
                sb.AppendFormat("key:{0}, value:{1}\t", keyValuePair.Key, keyValuePair.Value);
            }

            return sb.ToString();
        }
    }
}
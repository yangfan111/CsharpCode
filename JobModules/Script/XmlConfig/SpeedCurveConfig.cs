using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace XmlConfig
{
    [Serializable]
    public class SerializableCurve
    {
        public SerializableKeyframe[] keys;
        public string postWrapMode;
        public string preWrapMode;

        [Serializable]
        public class SerializableKeyframe
        {
            public Single inTangent;
            public Single outTangent;
            public Int32 tangentMode;
            public Single time;
            public Single value;

            public SerializableKeyframe()
            {
            }

            public SerializableKeyframe(Keyframe original)
            {
                inTangent = original.inTangent;
                outTangent = original.outTangent;
                tangentMode = original.tangentMode;
                time = original.time;
                value = original.value;
            }
        }

        public SerializableCurve(AnimationCurve original)
        {
            postWrapMode = getWrapModeAsString(original.postWrapMode);
            preWrapMode = getWrapModeAsString(original.preWrapMode);
            keys = new SerializableKeyframe[original.length];
            for (int i = 0; i < original.keys.Length; i++)
            {
                keys[i] = new SerializableKeyframe(original.keys[i]);
            }
        }
        
        private static AnimationCurve AireMoveCurve = new AnimationCurve(
            new Keyframe[3]
            {
                new Keyframe(0f, 1f),
                new Keyframe(1.0f, 0.95f),
                new Keyframe(1.5f, 0.5f)
            });

        public SerializableCurve():this(AireMoveCurve)
        {
            
        }

        public AnimationCurve toCurve()
        {
            AnimationCurve res = new AnimationCurve();
            res.postWrapMode = getWrapMode(postWrapMode);
            res.preWrapMode = getWrapMode(preWrapMode);
            Keyframe[] newKeys = new Keyframe[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                SerializableKeyframe aux = keys[i];
                Keyframe newK = new Keyframe();
                newK.inTangent = aux.inTangent;
                newK.outTangent = aux.outTangent;
                newK.tangentMode = aux.tangentMode;
                newK.time = aux.time;
                newK.value = aux.value;
                newKeys[i] = newK;
            }

            res.keys = newKeys;
            return res;
        }

        private WrapMode getWrapMode(String mode)
        {
            if (mode.Equals("Clamp"))
            {
                return WrapMode.Clamp;
            }

            if (mode.Equals("ClampForever"))
            {
                return WrapMode.ClampForever;
            }

            if (mode.Equals("Default"))
            {
                return WrapMode.Default;
            }

            if (mode.Equals("Loop"))
            {
                return WrapMode.Loop;
            }

            if (mode.Equals("Once"))
            {
                return WrapMode.Once;
            }

            if (mode.Equals("PingPong"))
            {
                return WrapMode.PingPong;
            }

            Debug.LogError("Wat is this wrap mode???");
            return WrapMode.Default;
        }

        private string getWrapModeAsString(WrapMode mode)
        {
            if (mode.Equals(WrapMode.Clamp))
            {
                return "Clamp";
            }

            if (mode.Equals(WrapMode.ClampForever))
            {
                return "ClampForever";
            }

            if (mode.Equals(WrapMode.Default))
            {
                return "Default";
            }

            if (mode.Equals(WrapMode.Loop))
            {
                return "Loop";
            }

            if (mode.Equals(WrapMode.Once))
            {
                return "Once";
            }

            if (mode.Equals(WrapMode.PingPong))
            {
                return "PingPong";
            }

            Debug.LogError("Wat is this wrap mode???");
            return "Default";
        }
    }
    
    public class CurveSerializerTool
    {
        /// <summary>
        /// 生成xml
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="path">like ../Assets/Foo/Foo.xml</param>
        /// <param name="cfg"></param>
        public static void GenerateConfig<T>(string path, T cfg)
        {
            var xmlPath = path;
            System.Xml.Serialization.XmlSerializer writer =
                new System.Xml.Serialization.XmlSerializer(typeof(T));

            var file = System.IO.File.Create(xmlPath);
            writer.Serialize(file, cfg);
            file.Close();
        }
        
        public static T Load<T>(string filename) where T : class
        {
            if (File.Exists(filename))
            {
                try
                {
                    using (Stream stream = File.OpenRead(filename))
                    {
                        BinaryFormatter formatter = new BinaryFormatter();
                        return formatter.Deserialize(stream) as T;
                    }
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }

            return default(T);
        }

        public static void Save<T>(string filename, T data) where T : class
        {
            using (Stream stream = File.OpenWrite(filename))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, data);
            }
        }
    }
    
    [Serializable]
    public class SerializablePostureCurveInfo
    {
        public SerializablePostureCurveInfo()
        {
        }

        public PostureInConfig StateOne;
        public PostureInConfig StateTwo;
        public SerializableCurve ScaleCurve;

        public PostureCurveInfo ToPostureCurveInfo()
        {
            PostureCurveInfo ret = new PostureCurveInfo
            {
                StateOne = StateOne,
                StateTwo = StateTwo,
                ScaleCurve = ScaleCurve.toCurve()
            };
            return ret;
        }
    }
    
    [Serializable]
    public class SerializableMovementCurveInfo
    {
        public SerializableMovementCurveInfo()
        {
        }

        public MovementInConfig StateOne;
        public MovementInConfig StateTwo;
        public SerializableCurve ScaleCurve;

        public MovementCurveInfo ToMovementCurveInfo()
        {
            MovementCurveInfo ret = new MovementCurveInfo
            {
                StateOne = StateOne,
                StateTwo = StateTwo,
                ScaleCurve = ScaleCurve.toCurve()
            };
            return ret;
        }
    }

    [Serializable]
    public class PostureCurveInfo
    {
        public PostureCurveInfo()
        {
            StateOne = PostureInConfig.End;
            StateTwo = PostureInConfig.End;
            ScaleCurve = new AnimationCurve(
                new Keyframe[3]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(0.5f, 1),
                    new Keyframe(1.0f, 1)
                });
        }
        
        public SerializablePostureCurveInfo ToSerializablePostureCurveInfo()
        {
            SerializablePostureCurveInfo ret = new SerializablePostureCurveInfo
            {
                StateOne = StateOne,
                StateTwo = StateTwo,
                ScaleCurve = new SerializableCurve(ScaleCurve)
            };
            return ret;
        }

        public PostureInConfig StateOne;
        public PostureInConfig StateTwo;
        public AnimationCurve ScaleCurve;
    }
    
    [Serializable]
    public class MovementCurveInfo
    {
        public MovementCurveInfo()
        {
            StateOne = MovementInConfig.End;
            StateTwo = MovementInConfig.End;
            ScaleCurve = new AnimationCurve(
                new Keyframe[3]
                {
                    new Keyframe(0f, 1f),
                    new Keyframe(0.5f, 1),
                    new Keyframe(1.0f, 1)
                });
        }
        
        public SerializableMovementCurveInfo ToSerializableMovementCurveInfo()
        {
            SerializableMovementCurveInfo ret = new SerializableMovementCurveInfo
            {
                StateOne = StateOne,
                StateTwo = StateTwo,
                ScaleCurve = new SerializableCurve(ScaleCurve)
            };
            return ret;
        }

        public MovementInConfig StateOne;
        public MovementInConfig StateTwo;
        public AnimationCurve ScaleCurve;
    }
    
    [Serializable]
    public class SpeedCurveConfig
    {
        public SpeedCurveConfig()
        {
        }

        public SerializableCurve AirMoveCurve;
        public List<SerializableMovementCurveInfo> MovementCurveInfos;
        public List<SerializablePostureCurveInfo> PostureCurveInfos;

        public SpeedCurveConfig(AnimationCurve airMoveCurve, List<MovementCurveInfo> transitionCurve, List<PostureCurveInfo> postureCurve)
        {
            AirMoveCurve = new SerializableCurve(airMoveCurve);
            MovementCurveInfos = new List<SerializableMovementCurveInfo>();
            foreach (MovementCurveInfo info in transitionCurve)
            {
                MovementCurveInfos.Add(info.ToSerializableMovementCurveInfo());
            }
            PostureCurveInfos = new List<SerializablePostureCurveInfo>();
            foreach (var info in postureCurve)
            {
                PostureCurveInfos.Add(info.ToSerializablePostureCurveInfo());
            }
        }
    }
}
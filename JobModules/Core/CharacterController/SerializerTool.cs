using System;
using System.IO;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;


namespace Core.CharacterController
{
    public class SerializerTool
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
}
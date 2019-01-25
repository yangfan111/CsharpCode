using System;
using System.Collections.Generic;
using App.Shared.Components.Bag;
using Core.Animation;
using Core.CameraControl;
using Core.CharacterState;
using Core.EntityComponent;
using UnityEngine;

namespace App.Shared.Components.Serializer
{
    public class FieldCloneUtil
    {
        public static int Clone(int data)
        {
            return data;
        }

        public static long Clone(long data)
        {
            return data;
        }

        public static string Clone(string data)
        {
            return data;
        }

        public static float Clone(float data)
        {
            return data;
        }

        public static double Clone(double data)
        {
            return data;
        }

        public static Vector2 Clone(Vector2 data)
        {
            return data;
        }

        public static Vector3 Clone(Vector3 data)
        {
            return data;
        }

        public static Quaternion Clone(Quaternion data)
        {
            return data;
        }

        public static bool Clone(bool data)
        {
            return data;
        }

        public static EntityKey Clone(EntityKey data)
        {
            return data;
        }

        public static T CloneListElem<T>(T data)where T : class,IPatchClass<T>, new()
        {
            return data!=null?data.Clone():null;
        }

        

        public static StateInterCommands Clone(StateInterCommands data)
        {
            return data != null ? data.Clone():null;
        }
        
        public static List<T> Clone<T>(List<T> data)
        {
            List<T> list = new List<T>(data.Count);
            if (typeof(T) == typeof(NetworkAnimatorLayer))
            {
                
                foreach (var elem in data)
                {
                    object obj = CloneListElem(elem as NetworkAnimatorLayer);
                    list.Add((T)obj);
                }
                
            }
            else if (typeof(T) == typeof(NetworkAnimatorParameter))
            {
                foreach (var elem in data)
                {
                    object obj = CloneListElem(elem as NetworkAnimatorParameter);
                    list.Add((T) obj);
                }

            }
            else
            {
                string info = string.Format("list of type {0} is not supported", typeof(T));
                throw new Exception(info);
            }

            
            return list;
        }


    }
}

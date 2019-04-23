using System.Xml.Serialization;
using UnityEngine;

namespace Core.HitBox
{
    [XmlRoot]
    [XmlInclude(typeof(BoxHitBox))]
    public class HitBox
    {
        [XmlElement]
        public string Name;
        [XmlElement]
        public string Parent;

        [XmlElement]
        public Vector3 Position;

        [XmlElement]
        public Vector3 Scale;

        [XmlElement]
        public Vector3 Rotation;

    }

   

    public class BoxHitBox : HitBox
    {
        [XmlElement]
        public Vector3 Size;
        [XmlElement]
        public Vector3 Center;
    }
}
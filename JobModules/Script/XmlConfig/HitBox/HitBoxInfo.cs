using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;

namespace Core.HitBox
{
    /// <summary>
    /// 碰撞帮助信息
    /// </summary>
    [XmlRoot]
    public class HitBoxInfo
    {

        /// <summary>
        /// 碰撞盒
        /// </summary>
        [XmlElement("Node")]
        public List<HitBox> HitBoxList = new List<HitBox>();

        /// <summary>
        /// 碰撞球体
        /// </summary>
        [XmlElement("Bound")]
        public BoundingSphere HitPreliminaryGeo;

        public string Root;
    }
}
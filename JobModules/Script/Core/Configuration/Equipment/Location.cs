using System;
using System.Collections.Generic;
using Core.Configuration.Utils;
using UnityEngine;

namespace Core.Configuration.Equipment
{
    public class Location : XmlNodeBase
    {
        [XmlNode("Id")]
        private int _id;
        [XmlNode("BasePosition")]
        private Vector3 _basePosition;
        [XmlNode("CombinationId")]
        private int _combinationId;

        private static Dictionary<string, Action<object, string>> _handler = new Dictionary<string, Action<object, string>>();

        static Location()
        {
            Init(typeof(Location), _handler);
        }

        public int Id { get { return _id; } set { _id = value; } }
        public Vector3 BasePosition { get { return _basePosition; } set { _basePosition = value; } }
        public int CombinationId { get { return _combinationId; } set { _combinationId = value; } }

        protected override Dictionary<string, Action<object, string>> GetHandler()
        {
            return _handler;
        }
    }
}

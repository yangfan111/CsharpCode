using System;
using System.Collections.Generic;
using Utils.AssetManager;
using XmlConfig;
using UnityEngine;
using Utils.Utils;

namespace Core.Geography
{
    public class ZoneController
    {
        struct Zone
        {
            public Vector3 Center;
            public Vector3 HalfSize;
            public float Radius;
            public Quaternion InverseRotation;
        }

        private Dictionary<SpecialZone, List<Zone>> _zones = new Dictionary<SpecialZone, List<Zone>>(CommonIntEnumEqualityComparer<SpecialZone>.Instance);

        public void AddZone(IList<SpecialZoneScope> zones)
        {
            foreach (var v in zones)
            {
                AddZone(v.Type, v.MaxPoint, v.MinPoint, v.Rotation);
            }
        }

        public bool InZone(SpecialZone type, Vector3 position)
        {
            bool ret = false;

            if (_zones.ContainsKey(type))
            {
                var zones = _zones[type];

                foreach (var zone in zones)
                {
                    if (InZone(zone, position))
                    {
                        ret = true;
                        break;
                    }
                }
            }

            return ret;
        }

        public float DistanceInsideUpperBorder(SpecialZone type, Vector3 position)
        {
            float ret = float.NaN;
            List<Zone> zones;
            if(_zones.TryGetValue(type, out zones))
            {
                foreach (var zone in zones)
                {
                    if (InZone(zone, position))
                    {
                        ret = zone.Center.y + zone.HalfSize.y;
                        break;
                    }
                }

            }

            return ret;
        }

        public float DistanceOutsideUpperBorder(SpecialZone type, Vector3 position)
        {
            float ret = 0;

            if (_zones.ContainsKey(type))
            {
                var zones = _zones[type];

                foreach (var zone in zones)
                {
                    if (!InZone(zone, position) && ProjectionInZone(zone, position))
                    {
                        ret = position.y - zone.Center.y - zone.HalfSize.y;
                        break;
                    }
                }
            }

            return ret;
        }

        public float GetHeightOfUpperBorder(SpecialZone type, Vector3 position)
        {
            float ret = float.NaN;
            if (_zones.ContainsKey(type))
            {
                var zones = _zones[type];

                foreach (var zone in zones)
                {
                    if (ProjectionInZone(zone, position))
                    {
                        ret = zone.Center.y + zone.HalfSize.y;
                        break;
                    }
                }
            }

            return ret;
        }

        private void AddZone(SpecialZone type, Vector3 max, Vector3 min, Vector3 rotation)
        {
            if (!_zones.ContainsKey(type))
            {
                _zones.Add(type, new List<Zone>());
            }
            var newZone = new Zone
            {
                Center = (max + min) * 0.5f,
                HalfSize = (max - min) * 0.5f,
                InverseRotation = Quaternion.Euler(rotation),
            };
            newZone.Radius = newZone.HalfSize.sqrMagnitude;
            newZone.InverseRotation.w = -newZone.InverseRotation.w;
            _zones[type].Add(newZone);
        }

        private bool InZone(Zone zone, Vector3 position)
        {
            var distance = position - zone.Center;
            if (distance.sqrMagnitude <= zone.Radius)
            {
                var rotatedDistance = zone.InverseRotation * distance;

                return Math.Abs(rotatedDistance.x) < zone.HalfSize.x
                    && Math.Abs(rotatedDistance.y) < zone.HalfSize.y
                    && Math.Abs(rotatedDistance.z) < zone.HalfSize.z;
            }
            return false;
        }

        private bool ProjectionInZone(Zone zone, Vector3 position)
        {
            var distance = position - zone.Center;
            var rotatedDistance = zone.InverseRotation * distance;

            return Math.Abs(rotatedDistance.x) < zone.HalfSize.x
                && Math.Abs(rotatedDistance.z) < zone.HalfSize.z;
        }

        public void CreateZoneTriggers(SpecialZone type, int layer)
        {
            var baseName = String.Format("Zone_{0}_Trigger", type);
            if (_zones.ContainsKey(type))
            {
                var zones = _zones[type];

                int count = zones.Count;
                for(int i = 0; i < count; ++i)
                {
                    var name = String.Format("{0}_{1}", baseName, i);
                    var zoneGo = DefaultGo.CreateGameObject(name);
                    zoneGo.layer = layer;
                    var box = zoneGo.AddComponent<BoxCollider>();
                    var zone = zones[i];
                    box.center = zone.Center;
                    box.size = zone.HalfSize * 2;
                    box.isTrigger = true;
                }

            }
        }
    }
}

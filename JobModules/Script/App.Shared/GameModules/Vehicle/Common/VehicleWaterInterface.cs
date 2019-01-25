using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Configuration;
using UnityEngine;
using Utils.Singleton;
using VehicleCommon;

namespace App.Shared.GameModules.Vehicle.Common
{
    public class VehicleWaterInterface : Singleton<VehicleWaterInterface>, IWaterInterface
    {
        private float _cachedHeight;
        private bool _isHeightCached = false;

        public float GetWaterHeightAtPosition(Vector3 position)
        {
            if (!_isHeightCached)
            {
                var cachedHeight = SingletonManager.Get<MapConfigManager>().GetHeightOfWater(position);
                if (float.IsNaN(cachedHeight))
                {
                    return 0.0f;
                }

                _cachedHeight = cachedHeight;
                _isHeightCached = true;
            }

            return _cachedHeight;
        }
    }
}

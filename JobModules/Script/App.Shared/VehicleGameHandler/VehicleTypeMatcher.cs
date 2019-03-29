using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.Components.Vehicle;
using XmlConfig;

namespace App.Shared.VehicleGameHandler
{
    public class VehicleTypeMatcher
    {
        private bool _matchAll;
        private EVehicleType _matchType;
        public VehicleTypeMatcher()
        {
            _matchAll = true;
        }

        public VehicleTypeMatcher(EVehicleType type)
        {
            _matchAll = false;
            _matchType = type;
        }

        public bool IsMatched(EVehicleType type)
        {
            return _matchAll || _matchType == type;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entitas;
using XmlConfig;

namespace App.Shared.Components.Vehicle
{
   

    [Vehicle]
    public class VehicleTypeComponent : IComponent
    {
        public EVehicleType VType;
    }
}

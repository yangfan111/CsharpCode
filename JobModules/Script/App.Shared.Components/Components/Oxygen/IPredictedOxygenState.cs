using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace App.Shared.Components.Oxygen
{
    public interface IPredictedOxygenState
    {
        float CurrentOxygen { get; set; }
    }
}

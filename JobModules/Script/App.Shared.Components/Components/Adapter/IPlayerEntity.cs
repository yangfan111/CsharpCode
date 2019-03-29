using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.Components.Adapter
{
    public interface IPlayerEntity
    {
        bool IsGround { get; }
        Vector3 Velocity { get; }
        Vector3 Position { get; }
    }
}

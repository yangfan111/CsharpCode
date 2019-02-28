using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    public interface IMoveSpeed
    {
        float GetSpeed(IEventArgs args, int startTime);
        Vector3 GetDirection(IEventArgs args);
    }
}

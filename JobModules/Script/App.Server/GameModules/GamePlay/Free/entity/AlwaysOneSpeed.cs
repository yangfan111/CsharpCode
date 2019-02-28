using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.wd.free.@event;
using UnityEngine;

namespace App.Server.GameModules.GamePlay.Free.entity
{
    public class AlwaysOneSpeed : IMoveSpeed
    {
        private float speed;

        public AlwaysOneSpeed()
        {

        }

        public AlwaysOneSpeed(float speed) : base()
        {
            this.speed = speed;
        }

        public Vector3 GetDirection(IEventArgs args)
        {
            return Vector3.up;
        }

        public float GetSpeed(IEventArgs args, int startTime)
        {
            return speed;
        }
    }
}

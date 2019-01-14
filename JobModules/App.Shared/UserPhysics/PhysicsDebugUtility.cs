using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.UserPhysics
{
    public static class PhysicsDebugUtility
    {
        public static String FullString(this Vector3 v)
        {
            return v.ToString("f3");
        }

        public static String FullString(this Quaternion q)
        {
            return q.ToString("f3");
        }
    }
}

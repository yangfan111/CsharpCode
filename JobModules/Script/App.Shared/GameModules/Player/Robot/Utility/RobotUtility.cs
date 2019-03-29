using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace App.Shared.GameModules.Player.Robot.Utility
{
    public class RobotUtility
    {
        // Internal variables
        private static Dictionary<string, Type> s_TypeLookup = new Dictionary<string, Type>();
        private static Dictionary<GameObject, Dictionary<Type, Component>> s_GameObjectComponentMap = new Dictionary<GameObject, Dictionary<Type, Component>>();
        private static List<string> s_LoadedAssemblies = null;
        public static T GetComponentForType<T>(GameObject target) where T : Component
        {
            return GetComponentForType<T>(target, false);
        }
        public static T GetComponentForType<T>(GameObject target, bool allowParentComponents) where T : Component
        {
            Dictionary<Type, Component> typeComponentMap;
            Component targetComponent;
            if (s_GameObjectComponentMap.TryGetValue(target, out typeComponentMap)) {
                if (typeComponentMap.TryGetValue(typeof(T), out targetComponent)) {
                    return targetComponent as T;
                }
            } else {
                typeComponentMap = new Dictionary<Type, Component>();
                s_GameObjectComponentMap.Add(target, typeComponentMap);
            }

            if (allowParentComponents) {
                targetComponent = target.GetComponentInParent(typeof(T));
            } else {
                targetComponent = target.GetComponent(typeof(T));
            }
            typeComponentMap.Add(typeof(T), targetComponent);
            return targetComponent as T;
        }
        public static Type GetType(string name)
        {
            Type type;
            // Cache the results for quick repeated lookup.
            if (s_TypeLookup.TryGetValue(name, out type)) {
                return type;
            }

            type = Type.GetType(name);
            // Look in the loaded assemblies.
            if (type == null) {
                if (s_LoadedAssemblies == null) {
#if NETFX_CORE && !UNITY_EDITOR
                    s_LoadedAssemblies = GetStorageFileAssemblies(typeName).Result;
#else
                    s_LoadedAssemblies = new List<string>();
                    var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                    for (int i = 0; i < assemblies.Length; ++i) {
                        s_LoadedAssemblies.Add(assemblies[i].FullName);
                    }
#endif
                }
                // Continue until the type is found.
                for (int i = 0; i < s_LoadedAssemblies.Count; ++i) {
                    type = Type.GetType(name + "," + s_LoadedAssemblies[i]);
                    if (type != null) {
                        break;
                    }
                }
            }
            if (type != null) {
                s_TypeLookup.Add(name, type);
            }
            return type;
        }
          /// <summary>
        /// Restricts the angle between -360 and 360 degrees.
        /// </summary>
        /// <param name="angle">The angle to restrict.</param>
        /// <returns>An angle between -360 and 360 degrees.</returns>
        public static float RestrictAngle(float angle)
        {
            if (angle < -360) {
                angle += 360;
            }
            if (angle > 360) {
                angle -= 360;
            }
            return angle;
        }

        /// <summary>
        /// Restricts the angle between -180 and 180 degrees.
        /// </summary>
        /// <param name="angle">The angle to restrict.</param>
        /// <returns>An angle between -180 and 180 degrees.</returns>
        public static float RestrictInnerAngle(float angle)
        {
            if (angle < -180) {
                angle += 360;
            }
            if (angle > 180) {
                angle -= 360;
            }
            return angle;
        }

        /// <summary>
        /// Restricts the angle between the firstAmount and secondAmount.
        /// </summary>
        /// <param name="angle">The angle to restrict.</param>
        /// <param name="firstAmount">The first amount to restrict the angle by.</param>
        /// <param name="secondAmount">The second amount to restrict the angle by.</param>
        /// <returns></returns>
        public static float RestrictAngleBetween(float currentAngle, float angle, float firstAmount, float secondAmount)
        {
            var lowerAngle = RestrictInnerAngle(currentAngle + firstAmount);
            var upperAngle = RestrictInnerAngle(currentAngle + secondAmount);
            if (upperAngle < lowerAngle) {
                upperAngle += 360;
            }
            // Keep the angle in the same restricted angle to ease the smoothing.
            if (angle < upperAngle - 360) {
                angle += 360;
            } else if (angle > lowerAngle + 360) {
                angle -= 360;
            }
            return Mathf.Clamp(angle, lowerAngle, upperAngle);
        }

        /// <summary>
        /// Clamp the angle between the min and max angle values.
        /// </summary>
        /// <param name="angle">The angle to be clamped.</param>
        /// <param name="min">The minimum angle value.</param>
        /// <param name="max">The maximum angle value.</param>
        /// <returns></returns>
        public static float ClampAngle(float angle, float min, float max)
        {
            return Mathf.Clamp(RestrictAngle(angle), min, max);
        }
    }
}

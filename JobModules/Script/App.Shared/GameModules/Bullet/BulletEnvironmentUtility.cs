using Core.BulletSimulation;
using Core.Utils;
using UnityEngine;
using Utils.Configuration;
using Utils.Singleton;
using WeaponConfigNs;

namespace App.Shared.GameModules.Bullet
{
    public struct EnvironmentInfo
    {
        public EEnvironmentType Type;
        public float Thickness;
        public int LayerCount;

        public override string ToString()
        {
            return string.Format("Type: {0}, Thickness: {1}, LayerCount: {2}", Type, Thickness, LayerCount);
        }
    }

    public class BulletEnvironmentUtility
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(BulletEnvironmentUtility));

        public static EnvironmentInfo GetEnvironmentInfoByHitBoxName(RaycastHit hit, Vector3 velocity, out ThicknessInfo thicknessInfo)
        {
            var go = hit.transform.gameObject;

            if(go.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.Water) || go.layer == UnityLayerManager.GetLayerIndex(EUnityLayerName.WaterTrigger))
            {
                thicknessInfo = new ThicknessInfo
                {
                    Normal = Vector3.up,
                    OutPoint = hit.point,
                    Thickness = float.MaxValue,
                };
                return new EnvironmentInfo
                {
                    LayerCount = 1,
                    Thickness = float.MaxValue,
                    Type = EEnvironmentType.Water,
                };
            }
            else
            {
                var name = RaycastUtility.GetMaterialByHit(hit);
                var envType = SingletonManager.Get<EnvironmentTypeConfigManager>().GetEnvironmentTypeByMatName(name);

                if(RaycastUtility.GetColliderThickness(hit, velocity, out thicknessInfo))
                {
                    var info = new EnvironmentInfo()
                    {
                        Type = envType,
                        Thickness = thicknessInfo.Thickness,
                        LayerCount = 1
                    };
                    return info;
                }
                return new EnvironmentInfo
                {
                    Type = envType,
                    Thickness = envType == EEnvironmentType.Glass ? 0.1f : float.MaxValue,
                    LayerCount = 1
                };
            }
        }
    }
}
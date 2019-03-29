using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Vehicle;
using Core.Components;
using Core.EntityComponent;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace App.Shared.DebugSystem
{
    public struct RigidbodyInfo
    {
        public string Name;
        public bool IsActive;
        public bool IsKinematic;
        public bool IsSleeping;
        public Vector3 Position;
        public Vector3 Velocity;
        public EntityKey EntityKey;
    }

    public struct VehiclesInfo
    {
        public int ActiveUpdateRate;
        public int ActiveCount;
        public int DeactiveCount;

    }

    public class RigidBodyDebugInfo
    {
        public List<RigidbodyInfo> RigidBodyInfoList;
        public VehiclesInfo VehcilesInfo;

    }

    public class RigidbodyDebugInfoSystem : AbstractDebugInfoSystem<RigidbodyDebugInfoSystem, RigidBodyDebugInfo>
    {
        private static LoggerAdapter _logger = new LoggerAdapter(typeof(RigidbodyDebugInfoSystem));

        private Contexts _contexts;
        public RigidbodyDebugInfoSystem(Contexts contexts)
        {
            _contexts = contexts;
        }

        protected override RigidBodyDebugInfo GetDebugInfo(object param)
        {
            var debugInfo = new RigidBodyDebugInfo();;

            var vhInfo = new VehiclesInfo();
            vhInfo.ActiveUpdateRate = SharedConfig.VehicleActiveUpdateRate;

            var vehicles = _contexts.vehicle.GetEntities();
            foreach (var v in vehicles)
            {
                if (v.IsActiveSelf())
                {
                    vhInfo.ActiveCount++;
                }
                else
                {
                    vhInfo.DeactiveCount++;
                }
            }
            debugInfo.VehcilesInfo = vhInfo;

            var rbDebugInfo = new List<RigidbodyInfo>();
            debugInfo.RigidBodyInfoList = rbDebugInfo;
            int sceneCount = SceneManager.sceneCount;
            for (int si = 0; si < sceneCount; ++si)
            {
                try
                {
                    var scene = SceneManager.GetSceneAt(si);
                    var gos = scene.GetRootGameObjects();
                    int goCount = gos.Length;
                    for (int gi = 0; gi < goCount; ++gi)
                    {
                        var go = gos[gi];
                        var rbs = go.GetComponentsInChildren<Rigidbody>();
                        var rbCount = rbs.Length;
                        for (int bi = 0; bi < rbCount; ++bi)
                        {
                            var rb = rbs[bi];
                            var info = new RigidbodyInfo()
                            {
                                Name = GetFullName(rb),
                                IsActive = rb.gameObject.activeSelf,
                                IsKinematic = rb.isKinematic,
                                IsSleeping = rb.IsSleeping(),
                                Position = rb.position,
                                Velocity = rb.velocity,
                                EntityKey = EntityKey.Default
                            };

                            var entityReference = rb.GetComponent<EntityReference>();
                            if (entityReference != null)
                            {
                                info.EntityKey = entityReference.EntityKey;
                            }

                            rbDebugInfo.Add(info);
                        }
                    }
                }
                catch (Exception e)
                {
                    _logger.ErrorFormat("{0}", e);
                }

            }

            return debugInfo;
        }


        private String GetFullName(Rigidbody rb)
        {
            var transform = rb.transform;
            var name = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                name = transform.name + "/" + name;
            }

            return name;
        }
    }
}

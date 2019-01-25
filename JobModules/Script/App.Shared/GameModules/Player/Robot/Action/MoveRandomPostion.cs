using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using App.Shared.GameModules.Common;
using App.Shared.GameModules.Player.Robot.Action;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Core.Utils;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Assets.App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager/NavMeshAgent")]
    [TaskDescription("Apply relative movement to the current position. Returns Success.")]
    class MoveRandomPostion : NavMeshMovementAgent
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("随机移动半径")]
        public SharedFloat MaxRadius;

        public SharedFloat MinRadius;
        public SharedVector3 Dest;
        private PlayerEntity mEntity;

        public override void OnAwake()
        {
            base.OnAwake();

            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
        }

        public override void OnStart()
        {
            base.OnStart();
            var center = transform.position;
            Vector3 dest = Vector3.zero;
            RaycastHit hit;
            var ray = new Ray(dest, new Vector3(0, -1, 0));
            NavMeshHit navhit;
            for (int i = 0; i < 10; i++)
            {
                var r = Random.Range(MinRadius.Value, MaxRadius.Value);
                var d = Random.insideUnitCircle;
                dest.x = center.x +
                         d.x * r;
                dest.z = center.z +
                         d.y * r;
                dest.y = 1000;
                ray.origin = dest;
                ray.direction = new Vector3(0, -1, 0);
                if (Physics.Raycast(ray, out hit, 1200, UnityLayers.SceneCollidableLayerMask))
                {
                    if (SamplePosition(hit.point))
                    {
                        SetDestination(hit.point);
                        Dest.Value = hit.point;
                        return;
                    }
                }
            }
        }

        public override void OnReset()
        {
            base.OnReset();
        }

        public override TaskStatus OnUpdate()
        {
            base.OnUpdate();
            if (HasArrived())
            {
                return TaskStatus.Success;
            }
           

            return TaskStatus.Running;
        }
    }
}
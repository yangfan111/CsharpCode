

using App.Shared.GameModules.Common;
using Assets.App.Shared.GameModules.Player.Robot.SharedVariables;

using BehaviorDesigner.Runtime.Tasks;
using Entitas;
using UnityEngine;
using UnityEngine.AI;

namespace App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("随机搜索敌人(作弊方式,透视)")]
    class SearchPlayer : BehaviorDesigner.Runtime.Tasks.Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "计划攻击的敌人")]
        public SharedPlayerEntity mPlayerEntity;

        private PlayerEntity mEntity;

    
        private PlayerContext playerContext;
        private IGroup<PlayerEntity> playerGroup;
        private Contexts _contexts;

        public override void OnStart()
        {
            base.OnStart();
            _contexts = mEntity.robot.Wapper.GameContexts as Contexts;
          
            playerContext = _contexts.player;
            playerGroup =
                playerContext.GetGroup(PlayerMatcher.AllOf(PlayerMatcher.Position, PlayerMatcher.ThirdPersonModel));

        }

        public override void OnAwake()
        {
            base.OnAwake();

            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;
        }

        private NavMeshHit hit;

        public override TaskStatus OnUpdate()
        {
            float i = 1;
            mPlayerEntity.Value = null;
            PlayerEntity temp = null;
            foreach (var entity in playerGroup)
            {
              
                if (NavMesh.SamplePosition(entity.position.Value, out hit, 1, NavMesh.AllAreas))
                {
                    i = i + 1;
                    if (Random.Range(0f, 1) < 1f / i)
                    {
                        temp = entity;
                    }

                   
                }


            }

            if (temp != null)
            {
                mPlayerEntity.Value = temp;
                return TaskStatus.Success;
            }

            return TaskStatus.Failure;
        }

        public override void OnReset()
        {
            base.OnReset();
        }
    }
}
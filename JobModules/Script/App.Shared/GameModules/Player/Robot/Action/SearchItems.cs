

using App.Shared.GameModules.Common;
using Assets.App.Shared.GameModules.Player.Robot.SharedVariables;

using BehaviorDesigner.Runtime.Tasks;
using Entitas;
using UnityEngine;
using UnityEngine.AI;

namespace App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("搜寻装备,判断是否需要捡取")]
    class SearchItems : BehaviorDesigner.Runtime.Tasks.Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "计划捡起的物品Entity")]
        public SharedSceneObjectEntity ItemEntity;

        private PlayerEntity mEntity;

    
        private SceneObjectContext sceneObjectContext;
        private IGroup<SceneObjectEntity> weaponGroup;
        private Contexts _contexts;

        public override void OnStart()
        {
            base.OnStart();
            _contexts = mEntity.robot.Wapper.GameContexts as Contexts;
          //  bagContext = (mEntity.robot.Wapper.GameContexts as Contexts).bag;
            sceneObjectContext = _contexts.sceneObject;
            weaponGroup = sceneObjectContext.GetGroup(
                SceneObjectMatcher.AnyOf(SceneObjectMatcher.SimpleEquipment)
            );
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
            ItemEntity.Value = null;
            SceneObjectEntity temp = null;
            foreach (var sceneObjectEntity in weaponGroup)
            {
                if (!sceneObjectEntity.hasPosition) continue;
                if (!sceneObjectEntity.hasUnityObject && !sceneObjectEntity.hasMultiUnityObject) continue;

                if (NavMesh.SamplePosition(sceneObjectEntity.position.Value, out hit, 1, NavMesh.AllAreas))
                {
                    i = i + 1;
                    if (Random.Range(0f, 1) < 1f / i)
                    {
                        temp = sceneObjectEntity;
                    }

                   
                }


            }

            if (temp != null)
            {
                ItemEntity.Value = temp;
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
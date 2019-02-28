using App.Shared.GameModules.Common;
using Assets.App.Shared.GameModules.Player.Robot.SharedVariables;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using BehaviorDesigner.Runtime.Tasks.Movement;
using Core.Configuration;
using Core.Free;
using Free.framework;
using UnityEngine;
using UserInputManager.Lib;

namespace App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("拾起武器")]
    public class PickUpItem : NavMeshMovementAgent
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip(
            "计划捡起的物品Entity")]
        public SharedSceneObjectEntity ItemEntity;

        public SharedGameObject ItemGameObject;
        private PlayerEntity mEntity;
        private Vector3 dst;

        public override void OnStart()
        {
            base.OnStart();
            if (SamplePosition(ItemEntity.Value.position.Value,out dst, 0.3f))
            {
                SetDestination(ItemEntity.Value.position.Value);
               
            }

            if (ItemEntity.Value.hasMultiUnityObject)
                ItemGameObject.Value = ItemEntity.Value.multiUnityObject.FirstAsset as GameObject;
            if (ItemEntity.Value.hasUnityObject)
                ItemGameObject.Value = ItemEntity.Value.unityObject.UnityObject;
           
        }

        public override void OnAwake()
        {
            base.OnAwake();

            mEntity = GetComponent<EntityReference>().Reference as PlayerEntity;

            
        }

        public override TaskStatus OnUpdate()
        {
            base.OnUpdate();
            if (ItemEntity == null || ItemEntity.Value == null) return TaskStatus.Failure;
            
            if (!HasArrived())
            {
                SetDestination(ItemEntity.Value.position.Value);
                mEntity.robot.Wapper.RobotUserCmdProvider.LookAt =Quaternion.LookRotation( navMeshAgent.desiredVelocity);
                return TaskStatus.Running;
            }
            else
            {
                if(!ItemEntity.Value.hasPosition)  return TaskStatus.Failure;
                var position = ItemEntity.Value.position.Value;
              

                mEntity.robot.Wapper.RobotUserCmdProvider.LookAt =Quaternion.LookRotation( mEntity.position.Value - position);
                var entityId = ItemEntity.Value.entityKey.Value.EntityId;
                var itemId = 0;
                var itemCount = 0;
                if (ItemEntity.Value.hasSimpleEquipment)
                {
                    var equip = ItemEntity.Value.simpleEquipment;
                    itemId = equip.Id;
                    itemCount = equip.Count;
                }

                mEntity.robot.Wapper.UserCmdGenerator.SetUserCmd((cmd) =>
                {
                    cmd.PickUpEquip = entityId;
                    cmd.PickUpEquipItemId = itemId;
                    cmd.PickUpEquipItemCount = itemCount;
                });

                ItemEntity.Value = null;
                return TaskStatus.Success;
            }

           
        }

        public override void OnReset()
        {
            base.OnReset();
        }
    }
}
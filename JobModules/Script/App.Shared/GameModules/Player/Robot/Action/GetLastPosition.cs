using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BehaviorDesigner.Runtime;
using BehaviorDesigner.Runtime.Tasks;
using Core.Utils;
using UnityEngine;

namespace Assets.App.Shared.GameModules.Player.Robot.Action
{
    [TaskCategory("Voyager")]
    [TaskDescription("Stores the position of the Transform.")]
   
    public class GetLastPosition : BehaviorDesigner.Runtime.Tasks.Action
    {
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The GameObject that the task operates on. If null the task GameObject is used.")]
        public SharedGameObject targetGameObject;
        [BehaviorDesigner.Runtime.Tasks.Tooltip("The position of the Transform")]
        [RequiredField] public SharedVector3 m_Position;

        // Internal variables
        private RaycastHit m_RaycastHit;
        private Transform targetTransform;
        private GameObject prevGameObject;

        /// <summary>
        /// Cache the target transform.
        /// </summary>
        public override void OnStart()
        {
            var currentGameObject = GetDefaultGameObject(targetGameObject.Value);
            if (currentGameObject != prevGameObject) {
                targetTransform = currentGameObject.GetComponent<Transform>();
                prevGameObject = currentGameObject;
            }
        }

        /// <summary>
        /// Stores the position of the Transform.
        /// </summary>
        /// <returns>Always returns Success.</returns>
        public override TaskStatus OnUpdate()
        {
            if (targetTransform == null) {
                Debug.LogWarning("Transform is null");
                return TaskStatus.Failure;
            }

            // The character may not be on the ground so fire a raycast from the character's position down so it will hit the ground.
            if (Physics.Raycast(targetTransform.position, Vector3.down, out m_RaycastHit, float.MaxValue, UnityLayers.SceneCollidableLayerMask)) {
                m_Position.Value = m_RaycastHit.point;
            } else {
                m_Position.Value = targetTransform.position;
            }

            return TaskStatus.Success;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinematicCharacterController
{
    /// <summary>
    /// BaseMoverController is the class you inherit from in order to create custom physics mover controllers (moving platforms). It’s a component that receives callbacks from PhysicsMover, and tells it how to behave through these callbacks.
    /// </summary>
    public abstract class BaseMoverController : MonoBehaviour
    {
        /// <summary>
        /// The PhysicsMover that will be assigned to this MoverController via the inspector
        /// </summary>
        public PhysicsMover Mover { get; private set; }

        /// <summary>
        /// This is called by the PhysicsMover in its Awake to setup references
        /// </summary>
        public void SetupMover(PhysicsMover mover)
        {
            Mover = mover;
        }

        /// <summary>
        /// Asks for the new position and rotation that the mover should have on this update
        /// </summary>
        public abstract void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime);
    }
}
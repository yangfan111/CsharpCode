using UnityEngine;

namespace ECM.Components
{
    public interface ICapsuleCollider
    {
        float radius { get;  }
        Vector3 center { get;  }
        float height { get;  }
        Collider collider { get; }
    }

    public class CharacterControllerCollider:ICapsuleCollider
    {
        public CharacterControllerCollider(CharacterController characterController)
        {
            _characterController = characterController;
        }

        private CharacterController _characterController;
        
        #region ICapsuleCollider
        public float radius {
            get { return _characterController.radius; }
        }

        public Vector3 center
        {
            get { return _characterController.center; }
        }

        public float height
        {
            get { return _characterController.height; }
        }

        public Collider collider
        {
            get { return _characterController.GetComponent<Collider>(); }
        }

        #endregion
    }
}
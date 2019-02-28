using UnityEngine;

namespace ECM.Components
{
    public class CharacterControllerAdapter
    {
        public CharacterControllerAdapter(GameObject obj, CharacterController controller)
        {
            _obj = obj;
            _capsuleCollider = new CharacterControllerCollider(controller);
        }

        private GameObject _obj;

        private ICapsuleCollider _capsuleCollider;

        public GameObject gameObject
        {
            get { return _obj; }
        }

        public Transform transform
        {
            get { return _obj.transform; }
        }

        public ICapsuleCollider CapsuleCollider
        {
            get { return _capsuleCollider; }
        }
    }

    
}
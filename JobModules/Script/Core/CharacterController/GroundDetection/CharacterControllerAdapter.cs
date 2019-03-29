using Core.CharacterController;
using Core.Utils;
using UnityEngine;

namespace ECM.Components
{
    public class CharacterControllerAdapter
    {
        public CharacterControllerAdapter(GameObject obj, CharacterController controller)
        {
            _obj = obj;
            _capsuleCollider = new CharacterControllerCollider(controller);
            _playerScript = controller.GetComponent<PlayerScript>();
        }

        private GameObject _obj;
        private PlayerScript _playerScript;

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


        public ControllerHitInfo GetHitInfo()
        {
            AssertUtility.Assert(_playerScript != null);
            return _playerScript.GetHitInfo(HitType.Down);
        }
        
    }

    
}
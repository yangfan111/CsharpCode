using UnityEngine;
using UserInputManager.Lib;

namespace UserInputManager.Utility
{
    public class RayCastTargetUtil
    {
        public static RayCastTarget AddRayCastTarget(GameObject obj)
        {
           var target = obj.GetComponent<RayCastTarget>();
            if (target == null)
            {
                target = obj.AddComponentUncheckRequireAndDisallowMulti<RayCastTarget>();
            }
            else
            {
                target.IdList.Clear();
                target.KeyList.Clear();
                target.Data = null;
            }

            return target;
        }
    }
}
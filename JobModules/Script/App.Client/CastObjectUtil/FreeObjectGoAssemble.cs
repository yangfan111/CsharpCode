using UnityEngine;
using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    public static class FreeObjectGoAssemble
    {
        public static RayCastTarget Assemble(GameObject model, FreeMoveEntity entity)
        {
            var root = BaseGoAssemble.Assemble(model, GetLegalPosition(entity.position.Value), entity.entityKey.Value.ToString());
            return root;
        }

        private static Vector3 _offset = new Vector3(0f, 0.01f, 0f);
        private static Vector3 GetLegalPosition(Vector3 srcPos)
        {
            RaycastHit hit;
            if(Physics.Raycast(srcPos, Vector3.down, out hit))
            {
                return hit.point + _offset;
            }
            return srcPos; 
        }
    }
}

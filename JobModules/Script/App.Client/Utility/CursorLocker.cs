using UnityEngine;

namespace App.Client.Utility
{
    public static class CursorLocker
    {
        public static bool SystemUnlock;
        public static int SystemBlockKeyId;
        public static int SystemBlockPointerId;

        public static void LockCursor()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public static void UnLockCursor()
        {
            Cursor.visible = true;
            if (Screen.fullScreen)
            {
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
            }
            
            
        }
    }
}

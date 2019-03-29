using UserInputManager.Lib;

namespace App.Client.CastObjectUtil
{
    public static class BaseCastData
    {
        public static void Make(RayCastTarget target, ECastDataType type)
        {
            target.KeyList.Clear();
            target.IdList.Clear();
            target.KeyList.Add(UserInputKey.PickUpTip);
            target.IdList.Add((int)type);
        }
    } 
}


namespace Core.CameraControl
{
    public static class TransitionMotroUtility
    {
        /// <summary>
        /// 根据初始和目标状态计算一个切换状态ID
        /// </summary>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns></returns>
        public static int GetTransitionId(int from, int to)
        {
            return (from << 16) + (to << 8);
        }

        public static int GetMotroFrom(int transitionId)
        {
            return transitionId >> 16;
        }

        public static int GetMotorTo(int transitionId)
        {
            return (transitionId >> 8) & 0x000FF;
        }
    }
}

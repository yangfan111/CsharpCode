namespace Core.Playback
{
    /// <summary>
    /// 支持差值的component
    /// </summary>
    public interface IInterpolatableComponent
    {
        /// <summary>
        /// 差值接口，提供差值的前后两帧的数据，和当前的时间
        /// </summary>
        /// <param name="left">差值的左值</param>
        /// <param name="right">差值的右值</param>
        /// <param name="interpolationInfo">时间信息，可以计算出差值的比例</param>
        void Interpolate(object left, object right, IInterpolationInfo interpolationInfo);
        /// <summary>
        /// 该component是否需要每一帧进行差值，像<see cref="Core.Components.PositionComponent"/>就需要每一帧进行差值
        /// </summary>
        /// <returns>true：每一帧都需要差值;false：只需要差值赋值一次</returns>
        bool IsInterpolateEveryFrame();
    }
    
   
}
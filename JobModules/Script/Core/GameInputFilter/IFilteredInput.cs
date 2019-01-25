using XmlConfig;

namespace Core.GameInputFilter
{
    public interface IFilteredInput 
    {
        bool IsInput(EPlayerInput input);
        /// <summary>
        /// 设置输入值
        /// </summary>
        /// <param name="input"></param>
        /// <param name="val"></param>
        void SetInput(EPlayerInput input, bool val);
        /// <summary>
        /// 屏蔽输入值
        /// </summary>
        /// <param name="input"></param>
        void BlockInput(EPlayerInput input);
        bool IsInputBlocked(EPlayerInput input);
        void Reset();
    }
}

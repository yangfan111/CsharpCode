using XmlConfig;

namespace Core
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
    }
}

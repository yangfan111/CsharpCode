namespace UserInputManager.Lib
{
    /// <summary>
    /// 越靠前越早执行
    /// </summary>
    public enum EInputLayer
    {
        Top = 0,
        System = 32,
        Ui = 64,
        Env = 128,
    }
}
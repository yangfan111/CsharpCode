namespace UserInputManager.Lib
{
    public enum BlockType
    {
        /// <summary>
        /// 不遮挡
        /// </summary>
        None,
        /// <summary>
        /// 可见的部分遮挡, 仅针对鼠标事件，键盘不存在这个情况
        /// </summary>
        [System.Obsolete]Visible,
        /// <summary>
        /// 全屏遮挡
        /// </summary>
        All,
    }
}

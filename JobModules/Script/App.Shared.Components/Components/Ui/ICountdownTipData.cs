namespace App.Shared.Components.Ui
{
    public interface ITipData 
    {
        /// <summary>
        /// 倒计时标题
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 持续时间，毫秒
        /// </summary>
        long DurationTime { get; set; }
        /// <summary>
        /// 事件对应的表Id
        /// </summary>
        int Id { get; set; }
    }
}

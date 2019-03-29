namespace App.Shared.Components.Ui
{
    public interface ITaskTipData
    {
        /// <summary>
        /// 倒计时标题
        /// </summary>
        string Title { get; set; }
        /// <summary>
        /// 任务进度原值
        /// </summary>
        int OldProgress { get; set; }
        /// <summary>
        /// 任务进度新值
        /// </summary>
        int NewProgress { get; set; }
        /// <summary>
        /// 任务总进度
        /// </summary>
        int TotalProgress { get; set; }
    }
}

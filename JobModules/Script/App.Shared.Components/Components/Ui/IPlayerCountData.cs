namespace App.Shared.Components.Ui
{
    public interface IPlayerCountData
    {
        /// <summary>
        /// 参与玩家数量
        /// </summary>
        int PlayerCount { get; set; }
        /// <summary>
        /// 死亡玩家数量
        /// </summary>
        int DeadPlayerCount { get; set; }

    }
}

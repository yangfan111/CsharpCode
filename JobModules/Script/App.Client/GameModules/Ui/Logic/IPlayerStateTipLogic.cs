namespace App.Client.GameModules.Ui.Logic
{
    interface IPlayerStatTipLogic
    {
        bool HasTipState();
        string StateTip { get; }
        void Action();
    }
}
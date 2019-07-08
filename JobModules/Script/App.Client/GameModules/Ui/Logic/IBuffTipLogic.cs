namespace App.Client.GameModules.Ui.Logic
{
    interface IBuffTipLogic
    {
        bool HasTipState();
        string StateTip { get; }
        void Action();
    }
}
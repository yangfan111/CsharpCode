using UserInputManager.Lib;

namespace App.Client.GameModules.Ui.Logic
{
    public interface ICastLogic
    {
        void SetData(PointerData data);
        string Tip { get; }
        void Action();
        void Clear();
    }
}

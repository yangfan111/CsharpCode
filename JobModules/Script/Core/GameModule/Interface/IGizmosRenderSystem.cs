namespace Core.GameModule.Interface
{
    public interface IGizmosRenderSystem:IUserSystem
    {
        void OnGizmosRender();
    }

    public interface IOnGuiSystem:IUserSystem
    {
        void OnGUI();
    }

    public interface IGamePlaySystem:IUserSystem
    {
        void OnGamePlay();
    }
}
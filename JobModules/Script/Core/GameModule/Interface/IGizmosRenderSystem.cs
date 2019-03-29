namespace Core.GameModule.Interface
{
    public interface IGizmosRenderSystem
    {
        void OnGizmosRender();
    }

    public interface IOnGuiSystem
    {
        void OnGUI();
    }

    public interface IGamePlaySystem
    {
        void OnGamePlay();
    }
}
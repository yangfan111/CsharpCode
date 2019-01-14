
namespace Core.GameModule.System
{
    public interface ICustomProfiler
    {
        void Start(string name);
        void Pause(string name);
        void Stop(string name);
    }
}

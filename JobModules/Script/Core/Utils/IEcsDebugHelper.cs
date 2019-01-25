using Core.SessionState;

namespace Core.Utils
{
    public interface IEcsDebugHelper
    {
        SessionStateMachine GetSessionStateMachine();
    }
}
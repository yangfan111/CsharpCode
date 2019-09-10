using Core.EntityComponent;
using Core.Playback;
using Core.Prediction;

namespace Core.Compensation
{
    public interface ICompensationComponent : IGameComponent, IInterpolatableComponent, IRewindableComponent,ICloneableComponent
    {
        
    }
}
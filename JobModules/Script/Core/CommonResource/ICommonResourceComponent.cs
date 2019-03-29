using Core.EntityComponent;
using Entitas;

namespace Core.CommonResource
{
    public interface ICommonResourceComponent : IGameComponent, IResetableComponent
    {
        AssetStatus[] Resources { get; }
    }
}
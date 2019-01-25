using System.Collections.Generic;

namespace Core.Attack
{
    public interface IHitMaskController
    {
        List<int> BulletExcludeTargetList { get; }
        List<int> MeleeExcludeTargetList { get; }
        List<int> ThrowingExcludeTargetList { get; }
    }
}

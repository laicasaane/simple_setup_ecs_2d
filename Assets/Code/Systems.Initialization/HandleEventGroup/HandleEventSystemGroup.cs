using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InitializeSystemGroup))]
    [UpdateBefore(typeof(DestroySystemGroup))]
    public sealed partial class HandleEventSystemGroup : ComponentSystemGroup
    {
    }
}

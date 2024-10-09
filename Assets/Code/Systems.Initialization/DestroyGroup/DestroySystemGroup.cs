using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InitializeSystemGroup))]
    public sealed partial class DestroySystemGroup : ComponentSystemGroup
    {

    }
}

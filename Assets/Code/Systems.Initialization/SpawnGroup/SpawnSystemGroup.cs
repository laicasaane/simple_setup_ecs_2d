using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(DestroySystemGroup))]
    public sealed partial class SpawnSystemGroup : ComponentSystemGroup
    {

    }
}

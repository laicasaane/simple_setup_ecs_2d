using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public sealed partial class InitializeSystemGroup : ComponentSystemGroup
    {

    }
}

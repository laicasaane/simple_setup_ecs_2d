using Unity.Burst;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    public partial struct SetNeedsDestroyJob : IJobEntity
    {
        private void Execute(EnabledRefRW<NeedsDestroyTag> tag)
        {
            tag.ValueRW = true;
        }
    }
}

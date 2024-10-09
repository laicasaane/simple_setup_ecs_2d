using Unity.Burst;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    public partial struct InitializeRandomAnimationSystem : ISystem
    {
        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var randomAnim = new RandomAnimation {
                value = false,
            };

            state.EntityManager.CreateSingleton(randomAnim, nameof(RandomAnimation));
            state.Enabled = false;
        }
    }
}

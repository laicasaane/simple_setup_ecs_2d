using Unity.Burst;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(DestroySystemGroup))]
    public partial struct DestroySpriteEntitySystem : ISystem
    {
        private EntityQuery _commandQuery;
        private EntityQuery _spriteQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _commandQuery = SystemAPI.QueryBuilder()
                .WithDisabledRW<NeedsDestroyTag>()
                .WithAll<DestroySpriteEntityCommandTag>()
                .Build();

            _spriteQuery = SystemAPI.QueryBuilder()
                .WithDisabledRW<NeedsDestroyTag>()
                .WithAll<Version2Tag>()
                .Build();

            state.RequireForUpdate(_commandQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Dependency = new SetNeedsDestroyJob().ScheduleParallel(_spriteQuery, state.Dependency);
            state.Dependency = new SetNeedsDestroyJob().ScheduleParallel(_commandQuery, state.Dependency);
        }
    }
}

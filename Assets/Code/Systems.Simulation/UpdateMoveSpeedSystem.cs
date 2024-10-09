using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct UpdateMoveSpeedSystem : ISystem
    {
        private EntityQuery _configQuery;
        private EntityQuery _commandQuery;
        private EntityQuery _entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _configQuery = SystemAPI.QueryBuilder()
                .WithAll<MoveSpeedConfig>()
                .Build();

            _commandQuery = SystemAPI.QueryBuilder()
                .WithAll<UpdateMoveSpeedCommandTag>()
                .WithDisabledRW<NeedsDestroyTag>()
                .Build();

            _entityQuery = SystemAPI.QueryBuilder()
                .WithAllRW<MoveSpeed>()
                .WithDisabled<NeedsInitComponentsTag>()
                .WithDisabled<NeedsDestroyTag>()
                .Build();

            state.RequireForUpdate(_configQuery);
            state.RequireForUpdate(_commandQuery);
            state.RequireForUpdate(_entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var config = _configQuery.GetSingleton<MoveSpeedConfig>();

            var updateJob = new UpdateMoveSpeedJob {
                config = config.value,
            };

            state.Dependency = updateJob.ScheduleParallel(_entityQuery, state.Dependency);
            state.Dependency = new SetNeedsDestroyJob().ScheduleParallel(_commandQuery, state.Dependency);
        }

        [BurstCompile]
        private partial struct UpdateMoveSpeedJob : IJobEntity
        {
            public float2 config;

            private void Execute(ref MoveSpeed moveSpeed)
            {
                moveSpeed.value = config.x;
            }
        }
    }
}

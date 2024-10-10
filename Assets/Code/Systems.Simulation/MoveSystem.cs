using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct MoveSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder()
                .WithAll<MoveSpeed>()
                .WithAll<FaceDirection>()
                .WithAllRW<LocalTransform>()
                .WithAll<CanMoveTag>()
                .WithDisabled<NeedsInitComponentsTag>()
                .WithDisabled<NeedsDestroyTag>()
                .Build();

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new MoveJob {
                deltaTime = SystemAPI.Time.DeltaTime,
            };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }

        [BurstCompile]
        private partial struct MoveJob : IJobEntity
        {
            public float deltaTime;

            private void Execute(
                  in MoveSpeed moveSpeed
                , in FaceDirection faceDirection
                , ref LocalTransform transform
            )
            {
                transform.Position += new float3(moveSpeed.value * deltaTime, 0f, 0f) * faceDirection.GetFace();
            }
        }
    }
}

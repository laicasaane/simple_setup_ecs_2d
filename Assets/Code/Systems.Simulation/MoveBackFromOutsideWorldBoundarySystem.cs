using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using Unity.Transforms;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    [UpdateAfter(typeof(MoveSystem))]
    public partial struct MoveBackFromOutsideWorldBoundarySystem : ISystem
    {
        private EntityQuery _boundaryQuery;
        private EntityQuery _entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _boundaryQuery = SystemAPI.QueryBuilder()
                .WithAll<WorldBoundary>()
                .Build();

            _entityQuery = SystemAPI.QueryBuilder()
                .WithAll<FaceDirection>()
                .WithAllRW<LocalTransform>()
                .WithAll<CanMoveTag>()
                .WithDisabled<NeedsInitComponentsTag>()
                .WithDisabled<NeedsDestroyTag>()
                .Build();

            state.RequireForUpdate(_boundaryQuery);
            state.RequireForUpdate(_entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var boundary = _boundaryQuery.GetSingleton<WorldBoundary>();

            var job = new MoveBackFromOutsideWorldBoundaryJob {
                boundary = boundary,
                padding = 1f,
            };

            state.Dependency = job.ScheduleParallel(_entityQuery, state.Dependency);
        }

        [BurstCompile]
        private partial struct MoveBackFromOutsideWorldBoundaryJob : IJobEntity
        {
            public WorldBoundary boundary;
            public float padding;

            private void Execute(in FaceDirection faceDirection, ref LocalTransform transform)
            {
                var position = transform.Position;
                var min = boundary.aabb.Min;
                var max = boundary.aabb.Max;
                var xToTheRight = math.select(position.x, min.x - padding, position.x > max.x + padding);
                var xToTheLeft = math.select(position.x, max.x + padding, position.x < min.x - padding);
                position.x = math.select(xToTheLeft, xToTheRight, faceDirection.GetFace() > 0);

                transform.Position = position;
            }
        }
    }
}

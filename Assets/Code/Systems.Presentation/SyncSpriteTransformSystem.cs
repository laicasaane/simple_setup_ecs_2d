using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine.Jobs;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public partial struct SyncSpriteTransformSystem : ISystem
    {
        private EntityQuery _poolerQuery;
        private EntityQuery _entityQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _poolerQuery = SystemAPI.QueryBuilder()
                .WithAll<SpritePresenterPoolerRef>()
                .Build();

            _entityQuery = SystemAPI.QueryBuilder()
                .WithAll<GameObjectInfo, LocalToWorld>()
                .WithDisabled<NeedsInitComponentsTag>()
                .WithDisabled<NeedsInitPresenterTag>()
                .WithDisabled<NeedsDestroyTag>()
                .Build();

            state.RequireForUpdate(_poolerQuery);
            state.RequireForUpdate(_entityQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var pooler = _poolerQuery.GetSingleton<SpritePresenterPoolerRef>();
            var transformArray = pooler.transformArray;
            var positions = pooler.positions.ToArray(state.WorldUpdateAllocator);

            var fetchJob = new FetchTransformPositionJob {
                positions = positions,
            };

            state.Dependency = fetchJob.ScheduleParallel(_entityQuery, state.Dependency);

            var syncJob = new SyncPositionToTransformJob {
                positions = positions.AsReadOnly(),
            };

            state.Dependency = syncJob.Schedule(transformArray, state.Dependency);
        }

        [BurstCompile]
        private partial struct FetchTransformPositionJob : IJobEntity
        {
            [NativeDisableParallelForRestriction] public NativeArray<float3> positions;

            private void Execute(in GameObjectInfo info, in LocalToWorld ltw)
            {
                positions[info.value.transformArrayIndex] = ltw.Position;
            }
        }

        [BurstCompile]
        private partial struct SyncPositionToTransformJob : IJobParallelForTransform
        {
            [ReadOnly] public NativeArray<float3>.ReadOnly positions;

            [BurstCompile]
            public void Execute(int index, TransformAccess transform)
            {
                transform.position = positions[index];
            }
        }
    }
}

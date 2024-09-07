using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct AnimateSpriteSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder()
                .WithAllRW<SpriteIndex>()
                .WithAllRW<SpriteIndexPrevious>()
                .WithAllRW<SpriteElapsedTime>()
                .WithAll<SpriteInterval, SpriteSheetInfo>()
                .Build();

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var job = new AnimateSpriteJob {
                deltaTime = SystemAPI.Time.DeltaTime,
            };

            state.Dependency = job.ScheduleParallel(_query, state.Dependency);
        }

        [BurstCompile]
        private partial struct AnimateSpriteJob : IJobEntity
        {
            public float deltaTime;

            private void Execute(
                  ref SpriteIndex spriteIndex
                , ref SpriteIndexPrevious spriteIndexPrev
                , ref SpriteElapsedTime spriteElapsedTime
                , in SpriteInterval spriteInterval
                , in SpriteSheetInfo sheetInfo
            )
            {
                var elapsedTime = spriteElapsedTime.value;
                elapsedTime += deltaTime;

                var canUpdate = elapsedTime >= spriteInterval.value;
                var length = sheetInfo.length;
                var index = spriteIndex.value;
                var indexPrev = spriteIndexPrev.value;

                index = math.select(index, (index + 1) % length, canUpdate);
                indexPrev = math.select(indexPrev, (indexPrev + 1) % length, canUpdate);

                spriteElapsedTime.value = math.select(elapsedTime, 0f, canUpdate);
                spriteIndexPrev.value = indexPrev;
                spriteIndex.value = index;
            }
        }
    }
}

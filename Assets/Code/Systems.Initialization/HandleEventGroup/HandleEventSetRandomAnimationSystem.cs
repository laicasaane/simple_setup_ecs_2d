using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SimpleSetupEcs2d
{
    using Random = Unity.Mathematics.Random;

    [UpdateInGroup(typeof(HandleEventSystemGroup))]
    public sealed partial class HandleEventSetRandomAnimationSystem : SystemBase
    {
        private bool? _value;
        private EntityQuery _vaultQuery;
        private EntityQuery _randomAnimQuery;
        private EntityQuery _randomizerQuery;
        private EntityQuery _spriteQuery;

        protected override void OnCreate()
        {
            EventVault.OnSetRandomAnimation += EventVault_OnSetRandomAnimation;

            _vaultQuery = SystemAPI.QueryBuilder()
                .WithAll<NativeSpriteSheetVault>()
                .Build();

            _randomAnimQuery = SystemAPI.QueryBuilder()
                .WithAllRW<RandomAnimation>()
                .Build();

            _randomizerQuery = SystemAPI.QueryBuilder()
                .WithAll<Randomizer>()
                .Build();

            _spriteQuery = SystemAPI.QueryBuilder()
                .WithAll<CanChangeSpriteSheetTag>()
                .WithAllRW<SpriteSheetInfo>()
                .WithAllRW<SpriteInterval>()
                .WithAllRW<SpriteElapsedTime>()
                .WithAllRW<SpriteIndex>()
                .WithAllRW<SpriteIndexPrevious>()
                .WithPresentRW<CanMoveTag>()
                .Build();

            RequireForUpdate(_vaultQuery);
            RequireForUpdate(_randomAnimQuery);
            RequireForUpdate(_spriteQuery);
        }

        protected override void OnUpdate()
        {
            if (_value.HasValue == false)
            {
                return;
            }

            // Before accessing the GlobalSettings via GetSingletonRW
            // it is required to complete the Dependency
            // then reset it.
            Dependency.Complete();
            Dependency = default;

            var value = _value.Value;
            _value = default;

            var randomAnimRW = _randomAnimQuery.GetSingletonRW<RandomAnimation>();
            randomAnimRW.ValueRW.value = value;

            if (value == false)
            {
                return;
            }

            var random = _randomizerQuery.GetSingleton<Randomizer>().value;
            var vault = _vaultQuery.GetSingleton<NativeSpriteSheetVault>();

            var job = new RandomizeSpriteSheetJob {
                random = Random.CreateFromIndex(random.NextUInt(0, 100)),
                sheetIds = vault.sheetIds,
                assetIdToSheetIdRangeMap = vault.assetIdToSheetIdRangeMap,
                idToSheetMap = vault.sheetIdToSheetMap,
            };

            Dependency = job.ScheduleParallel(_spriteQuery, Dependency);
        }

        private void EventVault_OnSetRandomAnimation(bool value)
        {
            _value = value;
        }

        [BurstCompile]
        private partial struct RandomizeSpriteSheetJob : IJobEntity
        {
            public Random random;

            [ReadOnly] public NativeArray<SpriteSheetId> sheetIds;
            [ReadOnly] public NativeHashMap<ushort, Range> assetIdToSheetIdRangeMap;
            [ReadOnly] public NativeHashMap<SpriteSheetId, NativeSpriteSheet> idToSheetMap;

            private void Execute(
                  ref SpriteSheetInfo info
                , ref SpriteInterval interval
                , ref SpriteElapsedTime elapsedTime
                , ref SpriteIndex index
                , ref SpriteIndexPrevious indexPrev
                , EnabledRefRW<CanMoveTag> canMoveTag
            )
            {
                if (assetIdToSheetIdRangeMap.TryGetValue(info.id.AssetId, out var sheetIdRange) == false)
                {
                    return;
                }

                var (startIndex, length) = sheetIdRange.GetOffsetAndLength(sheetIds.Length);
                var sheetId = random.NextInt(0, startIndex + length);
                var id = new SpriteSheetId(info.id.AssetId, (ushort)math.max(0, sheetId));

                if (idToSheetMap.TryGetValue(id, out var sheet) == false)
                {
                    return;
                }

                info.id = id;
                info.length = sheet.length;
                interval.value = sheet.spriteInterval;
                elapsedTime.value = 0f;
                index.value = 0;
                indexPrev.value = -1;
                canMoveTag.ValueRW = SpriteSheetAPI.IsMovable(id);
            }
        }
    }
}

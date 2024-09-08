using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed partial class ChangeSpriteSheetSystem : SystemBase
    {
        private int? _sheetId;
        private EntityQuery _vaultQuery;
        private EntityQuery _spriteQuery;

        protected override void OnCreate()
        {
            EventVault.OnChangeSpriteSheet += EventVault_OnChangeSpriteSheet;

            _vaultQuery = SystemAPI.QueryBuilder()
                .WithAll<NativeSpriteSheetVault>()
                .Build();

            _spriteQuery = SystemAPI.QueryBuilder()
                .WithAll<CanChangeSpriteSheetTag>()
                .WithAllRW<SpriteSheetInfo>()
                .WithAllRW<SpriteInterval>()
                .WithAllRW<SpriteElapsedTime>()
                .WithAllRW<SpriteIndex>()
                .WithAllRW<SpriteIndexPrevious>()
                .Build();

            RequireForUpdate(_vaultQuery);
            RequireForUpdate(_spriteQuery);
        }

        protected override void OnUpdate()
        {
            if (_sheetId.HasValue == false)
            {
                return;
            }

            var sheetId = _sheetId.Value;
            _sheetId = default;

            var vault = _vaultQuery.GetSingleton<NativeSpriteSheetVault>().map;

            var job = new ChangeSpriteSheetJob {
                sheetId = sheetId,
                map = vault,
            };

            Dependency = job.ScheduleParallel(_spriteQuery, Dependency);
        }

        private void EventVault_OnChangeSpriteSheet(int sheetId)
        {
            _sheetId = sheetId;
        }

        [BurstCompile]
        private partial struct ChangeSpriteSheetJob : IJobEntity
        {
            public int sheetId;

            [ReadOnly] public NativeHashMap<SpriteSheetId, NativeSpriteSheet> map;

            private void Execute(
                  ref SpriteSheetInfo info
                , ref SpriteInterval interval
                , ref SpriteElapsedTime elapsedTime
                , ref SpriteIndex index
                , ref SpriteIndexPrevious indexPrev
            )
            {
                var id = new SpriteSheetId(info.id.AssetId, (ushort)math.max(0, sheetId));

                if (map.TryGetValue(id, out var sheet) == false)
                {
                    return;
                }

                info.id = id;
                info.length = sheet.length;
                interval.value = sheet.spriteInterval;
                elapsedTime.value = 0f;
                index.value = 0;
                indexPrev.value = -1;
            }
        }
    }
}

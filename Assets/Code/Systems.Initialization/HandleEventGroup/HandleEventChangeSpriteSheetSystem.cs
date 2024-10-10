using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(HandleEventSystemGroup))]
    public sealed partial class HandleEventChangeSpriteSheetSystem : SystemBase
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
                .WithPresentRW<CanMoveTag>()
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

            var vault = _vaultQuery.GetSingleton<NativeSpriteSheetVault>().sheetIdToSheetMap;

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
                , EnabledRefRW<CanMoveTag> canMoveTag
            )
            {
                var id = new SpriteSheetId(info.id.AssetId, (ushort)math.max(0, sheetId));

                if (map.TryGetValue(id, out var sheet) == false)
                {
                    return;
                }

                SpriteSheetAPI.Initialize(
                      id
                    , sheet
                    , ref info
                    , ref interval
                    , ref elapsedTime
                    , ref index
                    , ref indexPrev
                    , canMoveTag
                );
            }
        }
    }
}

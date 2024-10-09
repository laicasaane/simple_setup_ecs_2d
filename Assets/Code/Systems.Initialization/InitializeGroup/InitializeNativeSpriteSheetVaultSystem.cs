using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public sealed partial class InitializeNativeSpriteSheetVaultSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithNone<NativeSpriteSheetVault>()
                .Build();

            RequireForUpdate(query);
        }

        protected override void OnDestroy()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<NativeSpriteSheetVault>()
                .Build();

            if (query.TryGetSingleton<NativeSpriteSheetVault>(out var vault))
            {
                vault.assetIdToSheetIdRangeMap.Dispose();
                vault.sheetIds.Dispose();
                vault.sheetIdToSheetMap.Dispose();
            }
        }

        protected override void OnUpdate()
        {
            var indicator = Object.FindFirstObjectByType<SceneLoadedIndicator>();

            if (indicator == false || indicator.Loaded == false || SpriteSheetVault.IsReady == false)
            {
                return;
            }

            var idToSheet = SpriteSheetVault.IdToSheet;
            var count = idToSheet.Count;

            var vault = new NativeSpriteSheetVault {
                assetIdToSheetIdRangeMap = new(count, Allocator.Persistent),
                sheetIds = new(count, Allocator.Persistent),
                sheetIdToSheetMap = new(count, Allocator.Persistent),
            };

            var assetIds = new NativeHashSet<ushort>(count, Allocator.Temp);
            var assetIdToSheetIdsMap = new NativeParallelMultiHashMap<ushort, ushort>(count, Allocator.Temp);

            foreach (var (id, sheet) in idToSheet)
            {
                assetIds.Add(id.AssetId);
                assetIdToSheetIdsMap.Add(id.AssetId, id.SheetId);
                vault.sheetIdToSheetMap.TryAdd(id, new NativeSpriteSheet {
                    id = sheet.Id,
                    length = sheet.Sprites.Length,
                    spriteInterval = 1f / math.max(sheet.Fps, 1),
                });
            }

            var sheetIdIndex = 0;

            foreach (var assetId in assetIds)
            {
                var start = sheetIdIndex;

                foreach (var sheetId in assetIdToSheetIdsMap.GetValuesForKey(assetId))
                {
                    vault.sheetIds[sheetIdIndex] = new(assetId, sheetId);
                    sheetIdIndex++;
                }

                vault.assetIdToSheetIdRangeMap.TryAdd(assetId, new(start, sheetIdIndex));
            }

            EntityManager.CreateSingleton(vault, nameof(NativeSpriteSheetVault));
            CheckedStateRef.Enabled = false;
        }
    }
}

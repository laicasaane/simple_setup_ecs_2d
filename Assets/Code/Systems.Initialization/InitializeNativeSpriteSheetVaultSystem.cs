using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed partial class InitializeNativeSpriteSheetVaultSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithNone<NativeSpriteSheetVault>()
                .Build();

            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var indicator = Object.FindFirstObjectByType<SceneLoadedIndicator>();

            if (indicator == false || indicator.Loaded == false || SpriteSheetVault.IsReady == false)
            {
                return;
            }

            var idToSheet = SpriteSheetVault.IdToSheet;
            var vault = new NativeSpriteSheetVault {
                map = new(idToSheet.Count, Allocator.Persistent)
            };

            foreach (var (id, sheet) in idToSheet)
            {
                vault.map.TryAdd(id, new NativeSpriteSheet {
                    id = sheet.Id,
                    length = sheet.Sprites.Length,
                    spriteInterval = 1f / math.max(sheet.Fps, 1),
                });
            }

            EntityManager.CreateSingleton(vault, "NativeSpriteSheetVault");
            CheckedStateRef.Enabled = false;
        }
    }
}

using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public partial struct InstantiateSpriteCharacterSystem : ISystem
    {
        private EntityQuery _vaultQuery;
        private EntityQuery _spawnQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _vaultQuery = SystemAPI.QueryBuilder()
                .WithAll<NativeSpriteSheetVault>()
                .Build();

            _spawnQuery = SystemAPI.QueryBuilder()
                .WithAll<SpriteSpawnInfo>()
                .Build();

            state.RequireForUpdate(_vaultQuery);
            state.RequireForUpdate(_spawnQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var vault = _vaultQuery.GetSingleton<NativeSpriteSheetVault>().map;
            var spawnInfoArray = _spawnQuery.ToComponentDataArray<SpriteSpawnInfo>(state.WorldUpdateAllocator);
            
            foreach (var spawnInfo in spawnInfoArray)
            {
                var amount = spawnInfo.amount;

                if (amount < 1) continue;

                var spriteSheetLength = 0;
                var spriteInterval = 0f;

                if (vault.TryGetValue(spawnInfo.id, out var sheet))
                {
                    spriteSheetLength = sheet.length;
                    spriteInterval = sheet.spriteInterval;
                }

                var entities = em.Instantiate(spawnInfo.prefab, amount, Allocator.Temp);

                for (var i = 0; i < amount; i++)
                {
                    var entity = entities[i];

                    em.SetComponentData(entity, new SpriteSheetInfo { id = spawnInfo.id, length = spriteSheetLength });
                    em.SetComponentData(entity, new SpriteIndexPrevious { value = -1 });
                    em.SetComponentData(entity, new SpriteInterval { value = spriteInterval });
                    em.SetComponentEnabled<CanChangeSpriteSheetTag>(entity, spawnInfo.canChangeSpriteSheet);
                    em.SetComponentEnabled<NeedsInitComponentsTag>(entity, false);
                }
            }

            em.DestroyEntity(_spawnQuery);
        }
    }
}

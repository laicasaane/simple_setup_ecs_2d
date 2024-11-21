using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    public partial struct SpawnSpriteCharacterSystem : ISystem
    {
        private EntityQuery _spawnRangeQuery;
        private EntityQuery _randomizerQuery;
        private EntityQuery _randomAnimQuery;
        private EntityQuery _moveSpeedConfigQuery;
        private EntityQuery _vaultQuery;
        private EntityQuery _spawnQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _spawnRangeQuery = SystemAPI.QueryBuilder()
                .WithAll<SpawnRange>()
                .Build();

            _randomizerQuery = SystemAPI.QueryBuilder()
                .WithAll<Randomizer>()
                .Build();

            _randomAnimQuery = SystemAPI.QueryBuilder()
                .WithAll<RandomAnimation>()
                .Build();

            _moveSpeedConfigQuery = SystemAPI.QueryBuilder()
                .WithAll<MoveSpeedConfig>()
                .Build();

            _vaultQuery = SystemAPI.QueryBuilder()
                .WithAll<NativeSpriteSheetVault>()
                .Build();

            _spawnQuery = SystemAPI.QueryBuilder()
                .WithAll<SpriteSpawnInfo>()
                .Build();

            state.RequireForUpdate(_randomizerQuery);
            state.RequireForUpdate(_randomAnimQuery);
            state.RequireForUpdate(_moveSpeedConfigQuery);
            state.RequireForUpdate(_vaultQuery);
            state.RequireForUpdate(_spawnQuery);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var em = state.EntityManager;
            var spawnRange = _spawnRangeQuery.GetSingleton<SpawnRange>();
            var random = _randomizerQuery.GetSingleton<Randomizer>().value;
            var randomAnim = _randomAnimQuery.GetSingleton<RandomAnimation>().value;
            var moveSpeedConfig = _moveSpeedConfigQuery.GetSingleton<MoveSpeedConfig>().value;
            var vault = _vaultQuery.GetSingleton<NativeSpriteSheetVault>();
            var spawnInfoArray = _spawnQuery.ToComponentDataArray<SpriteSpawnInfo>(Allocator.Temp);
            em.DestroyEntity(_spawnQuery);

            foreach (var spawnInfo in spawnInfoArray)
            {
                var amount = spawnInfo.amount;
                var randomPosition = spawnInfo.randomPosition;

                if (amount < 1
                    || vault.assetIdToSheetIdRangeMap.TryGetValue(spawnInfo.sheetId.AssetId, out var range) == false
                )
                {
                    continue;
                }

                var (startIndex, length) = range.GetOffsetAndLength(vault.sheetIds.Length);
                var spawnRandom = Random.CreateFromIndex(random.NextUInt(0, 100));
                var entities = em.Instantiate(spawnInfo.prefab, amount, Allocator.Temp);

                for (var i = 0; i < amount; i++)
                {
                    var entity = entities[i];
                    var sheetId = randomAnim
                        ? vault.sheetIds[spawnRandom.NextInt(startIndex, startIndex + length)]
                        : spawnInfo.sheetId;

                    var sheetResult = vault.sheetIdToSheetMap.TryGetValue(sheetId, out var sheet);
                    var sheetLength = math.select(0, sheet.length, sheetResult);
                    var spriteInterval = math.select(0f, sheet.spriteInterval, sheetResult);
                    var faceDirection = math.select(0, spawnRandom.NextBool() ? 1 : -1, randomPosition);
                    var transform = em.GetComponentData<LocalTransform>(entity);
                    var position = spawnRandom.NextFloat3(spawnRange.min, spawnRange.max);
                    transform.Position = math.select(float3.zero, position, randomPosition);

                    em.SetComponentData(entity, transform);
                    em.SetComponentData(entity, new SpriteSheetInfo { id = sheetId, length = sheetLength });
                    em.SetComponentData(entity, new SpriteIndexPrevious { value = -1 });
                    em.SetComponentData(entity, new SpriteInterval { value = spriteInterval });
                    em.SetComponentData(entity, new FaceDirection { value = (sbyte)faceDirection });
                    em.SetComponentData(entity, new MoveSpeed { value = moveSpeedConfig.x });
                    em.SetComponentEnabled<CanMoveTag>(entity, SpriteSheetAPI.IsMovable(sheetId));
                    em.SetComponentEnabled<CanChangeSpriteSheetTag>(entity, spawnInfo.canChangeSpriteSheet);
                    em.SetComponentEnabled<NeedsInitComponentsTag>(entity, false);
                }
            }

            // Dependency has been completed by _spawnQuery.ToComponentDataArray
            // so we have to reset it here.
            state.Dependency = default;
        }
    }
}

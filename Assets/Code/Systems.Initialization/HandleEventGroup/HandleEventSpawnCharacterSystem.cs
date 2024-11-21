using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(HandleEventSystemGroup))]
    public sealed partial class HandleEventSpawnCharacterSystem : SystemBase
    {
        private SpawnInfo? _spawnInfo;
        private SpawnInfo? _prevSpawnInfo;
        private EntityArchetype _archetype;
        private EntityQuery _prefabVaultQuery;
        private byte _version = VersionAPI.VERSION_1;

        protected override void OnCreate()
        {
            var types = new NativeArray<ComponentType>(1, Allocator.Temp) {
                [0] = ComponentType.ReadWrite<SpriteSpawnInfo>(),
            };

            _archetype = EntityManager.CreateArchetype(types);

            _prefabVaultQuery = SystemAPI.QueryBuilder()
                .WithAll<EntityPrefabVault>()
                .Build();

            RequireForUpdate(_prefabVaultQuery);

            EventVault.OnSpawnCharacters += EventVault_OnSpawnCharacters;
            EventVault.OnSetVersion2 += EventVault_OnSetVersion2;
        }

        protected override void OnUpdate()
        {
            if (_spawnInfo.HasValue == false)
            {
                return;
            }

            var spawnInfo = _spawnInfo.Value;
            _prevSpawnInfo = spawnInfo;
            _spawnInfo = default;

            if (spawnInfo.amount < 1)
            {
                return;
            }

            if (_prefabVaultQuery.TryGetSingleton<EntityPrefabVault>(out var prefabVault) == false)
            {
                return;
            }

            if (prefabVault.value.TryGetValue(spawnInfo.prefabId, out var prefab) == false)
            {
                return;
            }

            var entity = EntityManager.CreateEntity(_archetype);

            EntityManager.SetComponentData(entity, new SpriteSpawnInfo {
                prefab = prefab,
                sheetId = new(spawnInfo.assetId, 0),
                amount = spawnInfo.amount,
                canChangeSpriteSheet = true,
                randomPosition = spawnInfo.randomPosition,
            });
        }

        private void EventVault_OnSpawnCharacters(int assetId, int amount, bool randomPosition)
        {
            _spawnInfo = new SpawnInfo {
                prefabId = new((uint)assetId, _version),
                assetId = (ushort)assetId,
                amount = amount,
                randomPosition = randomPosition,
            };
        }

        private void EventVault_OnSetVersion2(bool value)
        {
            _version = VersionAPI.GetVersion(value);

            if (_prevSpawnInfo.HasValue == false)
            {
                return;
            }

            var spawnInfo = _prevSpawnInfo.Value;
            spawnInfo.prefabId = new(spawnInfo.assetId, _version);
            _spawnInfo = spawnInfo;
        }

        private struct SpawnInfo
        {
            public EntityPrefabId prefabId;
            public int amount;
            public ushort assetId;
            public bool randomPosition;
        }
    }
}

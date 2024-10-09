using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(HandleEventSystemGroup))]
    public sealed partial class HandleEventSpawnCharacterSystem : SystemBase
    {
        private SpawnInfo? _spawnInfo;
        private EntityArchetype _archetype;

        protected override void OnCreate()
        {
            var types = new NativeArray<ComponentType>(1, Allocator.Temp) {
                [0] = ComponentType.ReadWrite<SpriteSpawnInfo>(),
            };

            _archetype = EntityManager.CreateArchetype(types);

            EventVault.OnSpawnCharacters += EventVault_OnSpawnCharacters;
        }

        protected override void OnUpdate()
        {
            if (_spawnInfo.HasValue == false)
            {
                return;
            }

            var spawnInfo = _spawnInfo.Value;
            _spawnInfo = default;

            if (spawnInfo.amount < 1)
            {
                return;
            }

            var entity = EntityManager.CreateEntity(_archetype);

            EntityManager.SetComponentData(entity, new SpriteSpawnInfo {
                id = new(spawnInfo.assetId, 0),
                amount = spawnInfo.amount,
                canChangeSpriteSheet = true,
                randomPosition = spawnInfo.randomPosition,
            });
        }

        private void EventVault_OnSpawnCharacters(int assetId, int amount, bool randomPosition)
        {
            _spawnInfo = new SpawnInfo {
                assetId = (ushort)assetId,
                amount = amount,
                randomPosition = randomPosition,
            };
        }

        private struct SpawnInfo
        {
            public int amount;
            public ushort assetId;
            public bool randomPosition;
        }
    }
}

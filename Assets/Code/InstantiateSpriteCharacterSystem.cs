using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed partial class InstantiateSpriteCharacterSystem : SystemBase
    {
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder()
                .WithAll<SpriteSpawnInfo>()
                .Build();

            RequireForUpdate(_query);
        }

        protected override void OnUpdate()
        {
            var indicator = Object.FindFirstObjectByType<SceneLoadedIndicator>();

            if (indicator == false || indicator.Loaded == false || SpriteSheetVault.IsReady == false)
            {
                return;
            }

            var em = EntityManager;
            var spawnInfoArray = _query.ToComponentDataArray<SpriteSpawnInfo>(WorldUpdateAllocator);
            var gameObjectPrefab = new GameObject("prefab-sprite-character", typeof(SpriteRenderer));
            gameObjectPrefab.SetActive(false);

            foreach (var spawnInfo in spawnInfoArray)
            {
                var amount = spawnInfo.amount;

                if (amount < 1) continue;

                var spriteSheetId = new SpriteSheetId(0, 0);
                var spriteSheetLength = 0;
                var spriteInterval = 0f;

                if (SpriteSheetVault.TryGetSheet(spriteSheetId, out var sheet))
                {
                    spriteSheetLength = sheet.Sprites.Length;
                    spriteInterval = 1f / math.max(sheet.Fps, 1);
                }

                var instanceIds = new NativeArray<int>(amount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var transformIds = new NativeArray<int>(amount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
                var entities = em.Instantiate(spawnInfo.prefab, amount, Allocator.Temp);

                GameObject.InstantiateGameObjects(gameObjectPrefab.GetInstanceID(), amount, instanceIds, transformIds);
                GameObject.SetGameObjectsActive(instanceIds, true);

                for (var i = 0; i < amount; i++)
                {
                    var entity = entities[i];
                    var transform = Resources.InstanceIDToObject(transformIds[i]) as Transform;
                    var renderer = transform.GetComponent<SpriteRenderer>();
                    transform.name = entity.ToString();

                    em.SetComponentData(entity, new TransformRef { value = transform });
                    em.SetComponentData(entity, new SpriteRendererRef { value = renderer });
                    em.SetComponentData(entity, new SpriteSheetInfo { id = spriteSheetId, length = spriteSheetLength });
                    em.SetComponentData(entity, new SpriteIndexPrevious { value = -1 });
                    em.SetComponentData(entity, new SpriteInterval { value = spriteInterval });
                }
            }

            CheckedStateRef.Enabled = false;
        }
    }
}

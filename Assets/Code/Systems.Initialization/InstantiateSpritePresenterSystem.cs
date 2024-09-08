using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InstantiateSpriteCharacterSystem))]
    public sealed partial class InstantiateSpritePresentertSystem : SystemBase
    {
        private int? _prefabInstanceID;
        private EntityQuery _query;

        protected override void OnCreate()
        {
            _query = SystemAPI.QueryBuilder()
                .WithAllRW<NeedsInitPresenterTag>()
                .WithAllRW<TransformRef>()
                .WithAllRW<SpriteRendererRef>()
                .Build();

            RequireForUpdate(_query);
        }

        protected override void OnUpdate()
        {
            if (_prefabInstanceID.HasValue == false)
            {
                var prefab = new GameObject("prefab-sprite-presenter", typeof(SpriteRenderer));
                prefab.SetActive(false);
                _prefabInstanceID = prefab.GetInstanceID();
            }

            var entities = _query.ToEntityArray(WorldUpdateAllocator);
            var length = entities.Length;
            var em = EntityManager;

            var instanceIds = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var transformIds = new NativeArray<int>(length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            GameObject.InstantiateGameObjects(_prefabInstanceID.Value, length, instanceIds, transformIds);
            GameObject.SetGameObjectsActive(instanceIds, true);

            for (var i = 0; i < length; i++)
            {
                var entity = entities[i];
                var transform = Resources.InstanceIDToObject(transformIds[i]) as Transform;
                var renderer = transform.GetComponent<SpriteRenderer>();
                transform.name = entity.ToString();

                em.SetComponentData(entity, new TransformRef { value = transform });
                em.SetComponentData(entity, new SpriteRendererRef { value = renderer });
                em.SetComponentEnabled<NeedsInitPresenterTag>(entity, false);
            }
        }
    }
}

using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    [UpdateAfter(typeof(InstantiateSpriteCharacterSystem))]
    public sealed partial class InstantiateSpritePresentertSystem : SystemBase
    {
        private EntityQuery _poolerQuery;
        private EntityQuery _presenterQuery;

        protected override void OnCreate()
        {
            _poolerQuery = SystemAPI.QueryBuilder()
                .WithAll<SpritePresenterPoolerRef>()
                .Build();

            _presenterQuery = SystemAPI.QueryBuilder()
                .WithAllRW<NeedsInitPresenterTag>()
                .WithAllRW<GameObjectInfo>()
                .WithAllRW<SpriteRendererRef>()
                .Build();

            RequireForUpdate(_poolerQuery);
            RequireForUpdate(_presenterQuery);
        }

        protected override void OnUpdate()
        {
            var pool = _poolerQuery.GetSingleton<SpritePresenterPoolerRef>().poolerRef.Value.Pool;
            var entities = _presenterQuery.ToEntityArray(WorldUpdateAllocator);
            var length = entities.Length;
            var pooledObjects = new NativeList<PooledGameObject>(length, WorldUpdateAllocator);

            if (pool.TryRent(length, ref pooledObjects, true) == false)
            {
                return;
            }

            var em = EntityManager;

            for (var i = 0; i < length; i++)
            {
                var entity = entities[i];
                ref readonly var obj = ref pooledObjects.ElementAt(i);

                var go = Resources.InstanceIDToObject(obj.instanceId) as GameObject;
                var renderer = go.GetComponent<SpriteRenderer>();
                go.name = entity.ToString();

                em.SetComponentData(entity, new GameObjectInfo { value = obj });
                em.SetComponentData(entity, new SpriteRendererRef { value = renderer });
                em.SetComponentEnabled<NeedsInitPresenterTag>(entity, false);
            }
        }
    }
}

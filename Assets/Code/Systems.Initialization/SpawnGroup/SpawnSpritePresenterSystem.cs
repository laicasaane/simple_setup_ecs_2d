using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(SpawnSystemGroup))]
    [UpdateAfter(typeof(SpawnSpriteCharacterSystem))]
    public sealed partial class SpawnSpritePresenterSystem : SystemBase
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
            var entities = _presenterQuery.ToEntityArray(Allocator.Temp);
            var length = entities.Length;
            var pooledObjects = new NativeList<PooledGameObject>(length, Allocator.Temp);

            if (pool.TryRent(length, ref pooledObjects, true))
            {
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

            // Dependency has been completed by _presenterQuery.ToEntityArray
            // so we have to reset it here.
            Dependency = default;
        }
    }
}

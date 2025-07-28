using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    public partial struct InitializeEntityPrefabVaultSystem : ISystem
    {
        private EntityQuery _prefabQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _prefabQuery = SystemAPI.QueryBuilder()
                .WithAll<EntityPrefab>()
                .Build();

            state.RequireForUpdate(_prefabQuery);
        }

        [BurstCompile]
        public void OnDestroy(ref SystemState state)
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<EntityPrefabVault>()
                .Build();

            if (query.TryGetSingleton<EntityPrefabVault>(out var vault))
            {
                vault.value.Dispose();
            }
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefabs = _prefabQuery.ToComponentDataArray<EntityPrefab>(Allocator.Temp);

            var vault = new EntityPrefabVault {
                value = new(prefabs.Length, Allocator.Persistent),
            };

            for (var i = 0; i < prefabs.Length; i++)
            {
                var prefab = prefabs[i];
                var id = new EntityPrefabId(prefab.id, prefab.version);
                vault.value.Add(id, prefab.prefab);
            }

            state.EntityManager.CreateSingleton(vault, nameof(EntityPrefabVault));
            state.EntityManager.DestroyEntity(_prefabQuery);
        }
    }
}

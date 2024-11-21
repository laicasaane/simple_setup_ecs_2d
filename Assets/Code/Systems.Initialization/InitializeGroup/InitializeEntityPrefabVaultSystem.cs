using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    public partial struct InitializeEntityPrefabVaultSystem : ISystem
    {
        private EntityQuery _vaultQuery;
        private EntityQuery _prefabQuery;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            _vaultQuery = SystemAPI.QueryBuilder()
                .WithNone<EntityPrefabVault>()
                .Build();

            _prefabQuery = SystemAPI.QueryBuilder()
                .WithAll<EntityPrefab>()
                .Build();

            state.RequireForUpdate(_vaultQuery);
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

            if (prefabs.Length < 1)
            {
                return;
            }

            var vault = new EntityPrefabVault {
                value = new(prefabs.Length, Allocator.Persistent),
            };

            var arr = new NativeArray<Entity>(prefabs.Length, Allocator.Temp);

            for (var i = 0; i < prefabs.Length; i++)
            {
                var prefab = prefabs[i];
                var id = new EntityPrefabId(prefab.id, prefab.version);
                vault.value.Add(id, prefab.prefab);
                arr[i] = prefab.prefab;
            }

            state.EntityManager.AddComponent<Prefab>(arr);

            state.EntityManager.CreateSingleton(vault, nameof(EntityPrefabVault));
            state.EntityManager.DestroyEntity(_prefabQuery);
        }
    }
}

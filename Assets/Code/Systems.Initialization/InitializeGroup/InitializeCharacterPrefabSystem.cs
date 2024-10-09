using Unity.Burst;
using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    public partial struct InitializeCharacterPrefabSystem : ISystem
    {
        private EntityQuery _query;

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            var settingQuery = SystemAPI.QueryBuilder()
                .WithNone<CharacterPrefab>()
                .Build();

            _query = SystemAPI.QueryBuilder()
                .WithOptions(EntityQueryOptions.IncludePrefab)
                .WithAll<SpriteIndex>()
                .Build();

            state.RequireForUpdate(settingQuery);
            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            var prefabs = _query.ToEntityArray(Allocator.Temp);

            if (prefabs.Length > 0)
            {
                var prefab = new CharacterPrefab {
                    value = prefabs[0],
                };

                state.EntityManager.CreateSingleton(prefab, nameof(CharacterPrefab));
                state.Enabled = false;
            }

            // Dependency has been completed by _characterQuery.ToEntityArray
            // so we have to reset it here.
            state.Dependency = default;
        }
    }
}

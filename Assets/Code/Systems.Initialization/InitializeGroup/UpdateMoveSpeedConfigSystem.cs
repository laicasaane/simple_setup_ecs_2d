using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public sealed partial class UpdateMoveSpeedConfigSystem : SystemBase
    {
        private EntityArchetype _archetype;
        private EntityQuery _configQuery;
        private EntityQuery _assetRefQuery;

        protected override void OnCreate()
        {
            var types = new NativeArray<ComponentType>(2, Allocator.Temp) {
                [0] = ComponentType.ReadWrite<UpdateMoveSpeedCommandTag>(),
                [1] = ComponentType.ReadWrite<NeedsDestroyTag>(),
            };

            _archetype = EntityManager.CreateArchetype(types);

            _configQuery = SystemAPI.QueryBuilder()
                .WithAllRW<MoveSpeedConfig>()
                .Build();

            _assetRefQuery = SystemAPI.QueryBuilder()
                .WithAll<MoveSpeedConfigAssetRef>()
                .Build();

            RequireForUpdate(_configQuery);
            RequireForUpdate(_assetRefQuery);
        }

        protected override void OnUpdate()
        {
            // Before accessing the MoveSpeedConfig via GetSingletonRW
            // it is required to complete the Dependency
            // then reset it.
            Dependency.Complete();
            Dependency = default;

            var assetRef = _assetRefQuery.GetSingleton<MoveSpeedConfigAssetRef>();
            var configRW = _configQuery.GetSingletonRW<MoveSpeedConfig>();
            var asset = assetRef.value.Value;

            if (configRW.ValueRO.value.Equals(asset.value) == false)
            {
                configRW.ValueRW.value = asset.value;

                var entity = EntityManager.CreateEntity(_archetype);
                EntityManager.SetComponentEnabled<NeedsDestroyTag>(entity, false);
            }
        }
    }
}

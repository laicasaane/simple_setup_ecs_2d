using Unity.Burst;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(DestroySystemGroup), OrderLast = true)]
    public partial struct DestroyEntitiesSystem : ISystem
    {
        private EntityQuery _query;

        /// <inheritdoc cref="Documentation.DoNotPutBurstCompileHere" />
        public void OnCreate(ref SystemState state)
        {
            _query = SystemAPI.QueryBuilder()
                .WithAll<NeedsDestroyTag>()
                .Build();

            state.RequireForUpdate(_query);
        }

        /// <inheritdoc cref="Documentation.DoNotPutBurstCompileHere" />
        public void OnUpdate(ref SystemState state)
        {
            state.EntityManager.DestroyEntity(_query);

            // Dependency has been completed by EntityManager.DestroyEntity
            // so we have to reset it here.
            state.Dependency = default;
        }
    }
}

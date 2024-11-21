using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(DestroySystemGroup))]
    public sealed partial class DestroySpritePresenterSystem : SystemBase
    {
        private EntityQuery _commandQuery;
        private EntityQuery _poolerQuery;
        private EntityQuery _presenterQuery;

        protected override void OnCreate()
        {
            _commandQuery = SystemAPI.QueryBuilder()
                .WithDisabledRW<NeedsDestroyTag>()
                .WithAll<DestroySpritePresenterCommandTag>()
                .Build();

            _poolerQuery = SystemAPI.QueryBuilder()
                .WithAll<SpritePresenterPoolerRef>()
                .Build();

            _presenterQuery = SystemAPI.QueryBuilder()
                .WithDisabledRW<NeedsDestroyTag>()
                .WithAll<GameObjectInfo>()
                .Build();

            RequireForUpdate(_commandQuery);
            RequireForUpdate(_poolerQuery);
        }

        protected override void OnUpdate()
        {
            var pool = _poolerQuery.GetSingleton<SpritePresenterPoolerRef>().poolerRef.Value.Pool;
            var presenters = _presenterQuery
                .ToComponentDataArray<GameObjectInfo>(Allocator.Temp)
                .Reinterpret<PooledGameObject>()
                .Slice();

            var instanceIds = new NativeArray<int>(presenters.Length, Allocator.Temp);
            var transformIds = new NativeArray<int>(presenters.Length, Allocator.Temp);
            presenters.SliceWithStride<int>(PooledGameObject.OFFSET_INSTANCE_ID).CopyTo(instanceIds);
            presenters.SliceWithStride<int>(PooledGameObject.OFFSET_TRANSFORM_ID).CopyTo(transformIds);

            pool.Return(instanceIds, transformIds);

            // Dependency has been completed by _presenterQuery.ToEntityArray
            // so we have to reset it here.
            Dependency = default;
            Dependency = new SetNeedsDestroyJob().ScheduleParallel(_presenterQuery, Dependency);
            Dependency = new SetNeedsDestroyJob().ScheduleParallel(_commandQuery, Dependency);
        }
    }
}

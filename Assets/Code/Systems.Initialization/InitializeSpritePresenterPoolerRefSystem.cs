using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializationSystemGroup))]
    public sealed partial class InitializeSpritePresenterPoolerRefSystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithNone<SpritePresenterPoolerRef>()
                .Build();

            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            var pooler = Object.FindFirstObjectByType<SpritePresenterPooler>();

            if (pooler == false || pooler.Pool == null)
            {
                return;
            }

            var poolerRef = new SpritePresenterPoolerRef {
                poolerRef = pooler,
                transformArray = pooler.Pool.TransformArray,
                positions = pooler.Pool.Positions,
            };

            EntityManager.CreateSingleton(poolerRef, "SpritePresenterPoolerRef");
            CheckedStateRef.Enabled = false;
        }
    }
}

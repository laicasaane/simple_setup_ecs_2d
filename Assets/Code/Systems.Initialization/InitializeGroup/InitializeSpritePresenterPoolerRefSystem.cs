using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
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
            var indicator = Object.FindFirstObjectByType<SceneLoadedIndicator>();

            if (indicator == false || indicator.Loaded == false)
            {
                return;
            }

            var poolerGO = new GameObject("sprite-presenter-pooler");
            SceneManager.MoveGameObjectToScene(poolerGO, indicator.gameObject.scene);

            var pooler = poolerGO.AddComponent<SpritePresenterPooler>();
            pooler.Initialize();

            var poolerRef = new SpritePresenterPoolerRef {
                poolerRef = pooler,
                transformArray = pooler.Pool.TransformArray,
                positions = pooler.Pool.Positions,
            };

            EntityManager.CreateSingleton(poolerRef, nameof(SpritePresenterPoolerRef));
            CheckedStateRef.Enabled = false;
        }
    }
}

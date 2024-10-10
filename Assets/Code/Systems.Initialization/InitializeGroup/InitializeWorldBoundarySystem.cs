using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public sealed partial class InitializeWorldBoundarySystem : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithNone<WorldBoundary>()
                .Build();

            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            if (Camera.main == false)
            {
                return;
            }

            var cam = Camera.main;
            var camTrans = cam.transform;

#if UNITY_EDITOR
            UnityEditor.PlayModeWindow.GetRenderingResolution(out var resolutionWidth, out var resolutionHeight);
#else
            var resolution = Screen.currentResolution;
            var resolutionWidth = resolution.width;
            var resolutionHeight = resolution.height;
#endif

            var scaleWidthFactor = resolutionWidth / (float)resolutionHeight;
            var height = cam.orthographicSize * 2f;
            var width = height * scaleWidthFactor;

            var rect = new Rect(
                  new(camTrans.position.x - (width * 0.5f), camTrans.position.y - (height * 0.5f))
                , new(width, height)
            );

            var boundary = new WorldBoundary {
                min = new(rect.min, 0f),
                max = new(rect.max, 0f),
            };

            EntityManager.CreateSingleton(boundary, nameof(WorldBoundary));
            CheckedStateRef.Enabled = false;
        }
    }
}

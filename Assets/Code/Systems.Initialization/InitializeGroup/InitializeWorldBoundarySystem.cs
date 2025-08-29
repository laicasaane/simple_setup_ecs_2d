using Unity.Entities;
using Unity.Mathematics.Geometry;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public sealed partial class InitializeWorldBoundarySystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<WorldBoundary>();
        }

        protected override void OnUpdate()
        {
            if (Camera.main == false)
            {
                return;
            }
            ref var worldBoundary = ref SystemAPI.GetSingletonRW<WorldBoundary>().ValueRW;
            var elapsedTime = SystemAPI.Time.ElapsedTime;
            double nextInterval = worldBoundary.elapsed + worldBoundary.updateInterval;
            if (nextInterval < worldBoundary.elapsed)
            {
                return;
            }

            worldBoundary.elapsed = nextInterval + worldBoundary.updateInterval;

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
            
            var aabb = new MinMaxAABB { Min = new(rect.min, 0f), Max = new(rect.max, 0f), };
            worldBoundary.aabb = aabb;
        }
    }
}

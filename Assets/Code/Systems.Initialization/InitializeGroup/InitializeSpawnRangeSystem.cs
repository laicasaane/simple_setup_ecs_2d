using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public sealed partial class InitializeSpawnRangeSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var indicator = Object.FindFirstObjectByType<SpawnRangeIndicator>();

            if (indicator == false || indicator.Range == false)
            {
                return;
            }

            var offset = indicator.Range.transform.position;
            var size = indicator.Range.size;

            var spawnRange = new SpawnRange {
                min = new float3(offset.x - size.x / 2, offset.y - size.y / 2, 0f),
                max = new float3(offset.x + size.x / 2, offset.y + size.y / 2, 0f),
            };

            EntityManager.CreateSingleton(spawnRange, nameof(SpawnRange));
            CheckedStateRef.Enabled = false;
        }
    }
}

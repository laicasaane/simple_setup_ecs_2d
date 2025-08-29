using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public class WorldBoundaryAuthoring : MonoBehaviour
    {
        public float updateInterval = 1f;

        public class WorldBoundaryBaker : Baker<WorldBoundaryAuthoring>
        {
            public override void Bake(WorldBoundaryAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.None);
                AddComponent(entity, new WorldBoundary { updateInterval = authoring.updateInterval });
            }
        }
    }
}
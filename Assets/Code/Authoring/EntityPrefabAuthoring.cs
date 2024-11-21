using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class EntityPrefabAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public uint id;
        public bool version2;

        private void OnValidate()
        {
            if (prefab)
            {
                name = $"entity-{prefab.name}";
            }
        }

        private class Baker : Baker<EntityPrefabAuthoring>
        {
            public override void Bake(EntityPrefabAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                var prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new EntityPrefab {
                    prefab = prefab,
                    id = authoring.id,
                    version = VersionAPI.GetVersion(authoring.version2),
                });
            }
        }
    }
}

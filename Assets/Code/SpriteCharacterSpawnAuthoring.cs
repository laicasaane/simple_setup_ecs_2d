using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class SpriteCharacterSpawnAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public int amount;

        private void OnValidate()
        {
            if (prefab)
            {
                this.name = $"spawn-{prefab.name}";
            }
        }

        private class Baker : Baker<SpriteCharacterSpawnAuthoring>
        {
            public override void Bake(SpriteCharacterSpawnAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                var prefab = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new SpriteSpawnInfo {
                    prefab = prefab,
                    amount = authoring.amount,
                });
            }
        }
    }
}

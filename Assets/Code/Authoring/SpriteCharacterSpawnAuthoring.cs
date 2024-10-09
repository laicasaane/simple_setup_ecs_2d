using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class SpriteCharacterSpawnAuthoring : MonoBehaviour
    {
        public GameObject prefab;
        public ushort assetId;
        public ushort sheetId;
        public int amount;
        public bool canChangeSpriteSheet;

        private void OnValidate()
        {
            if (prefab)
            {
                name = $"spawn-{prefab.name}";
            }
        }

        private class Baker : Baker<SpriteCharacterSpawnAuthoring>
        {
            public override void Bake(SpriteCharacterSpawnAuthoring authoring)
            {
                var entity = GetEntity(authoring, TransformUsageFlags.None);
                var _ = GetEntity(authoring.prefab, TransformUsageFlags.Dynamic);

                AddComponent(entity, new SpriteSpawnInfo {
                    id = new(authoring.assetId, authoring.sheetId),
                    amount = authoring.amount,
                    canChangeSpriteSheet = authoring.canChangeSpriteSheet,
                });
            }
        }
    }
}

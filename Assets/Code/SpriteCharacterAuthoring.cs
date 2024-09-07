using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    internal sealed class SpriteCharacterAuthoring : MonoBehaviour
    {
        public ushort assetId;
        public ushort sheetId;

        private class Baker : Baker<SpriteCharacterAuthoring>
        {
            public override void Bake(SpriteCharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<TransformRef>(entity);
                AddComponent<SpriteRendererRef>(entity);
                AddComponent<SpriteIndex>(entity);
                AddComponent<SpriteInterval>(entity);
                AddComponent<SpriteElapsedTime>(entity);

                AddComponent(entity, new SpriteIndexPrevious { value = -1 });
                AddComponent(entity, new SpriteSheetInfo {
                    id = new(authoring.assetId, authoring.sheetId)
                });
            }
        }
    }
}

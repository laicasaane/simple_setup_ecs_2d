using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    internal sealed class SpriteCharacterAuthoring : MonoBehaviour
    {
        private class Baker : Baker<SpriteCharacterAuthoring>
        {
            public override void Bake(SpriteCharacterAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<GameObjectInfo>(entity);
                AddComponent<SpriteRendererRef>(entity);
                AddComponent<SpriteSheetInfo>(entity);
                AddComponent<SpriteInterval>(entity);
                AddComponent<SpriteElapsedTime>(entity);
                AddComponent<SpriteIndex>(entity);
                AddComponent(entity, new SpriteIndexPrevious { value = -1 });

                AddComponent<FaceDirection>(entity);
                AddComponent<MoveSpeed>(entity);
                AddComponent<CanMoveTag>(entity);
                SetComponentEnabled<CanMoveTag>(entity, false);

                AddComponent<CanChangeSpriteSheetTag>(entity);
                SetComponentEnabled<CanChangeSpriteSheetTag>(entity, false);

                AddComponent<NeedsInitComponentsTag>(entity);
                AddComponent<NeedsInitPresenterTag>(entity);

                AddComponent<NeedsDestroyTag>(entity);
                SetComponentEnabled<NeedsDestroyTag>(entity, false);
            }
        }
    }
}

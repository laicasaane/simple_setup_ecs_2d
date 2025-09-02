using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    internal sealed class SpriteCharacterV1Authoring : MonoBehaviour
    {
        private class Baker : Baker<SpriteCharacterV1Authoring>
        {
            public override void Bake(SpriteCharacterV1Authoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<OmitLinkedEntityGroupFromPrefabInstance>(entity);
                AddComponent<Version1Tag>(entity);
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

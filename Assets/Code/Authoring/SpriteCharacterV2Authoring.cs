using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    internal sealed class SpriteCharacterV2Authoring : MonoBehaviour
    {
        private class Baker : Baker<SpriteCharacterV2Authoring>
        {
            public override void Bake(SpriteCharacterV2Authoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<Version2Tag>(entity);
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

                AddComponent<NeedsDestroyTag>(entity);
                SetComponentEnabled<NeedsDestroyTag>(entity, false);
            }
        }
    }
}

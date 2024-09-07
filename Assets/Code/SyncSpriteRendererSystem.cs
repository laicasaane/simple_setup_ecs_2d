using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public sealed partial class SyncSpriteRendererSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            if (SpriteSheetVault.IsReady == false)
            {
                return;
            }

            foreach (var (rendererRef, spriteIndex, spriteIndexPrev, spriteSheetInfo)
                in SystemAPI.Query<SpriteRendererRef, SpriteIndex, SpriteIndexPrevious, SpriteSheetInfo>()
            )
            {
                var length = spriteSheetInfo.length;

                if (length < 1)
                {
                    continue;
                }

                if (spriteIndex.value == spriteIndexPrev.value)
                {
                    continue;
                }

                if (SpriteSheetVault.TryGetSheet(spriteSheetInfo.id, out var sheet) == false)
                {
                    continue;
                }

                var index = spriteIndex.value % length;
                var sprite = sheet.Sprites.Span[index];
                rendererRef.value.Value.sprite = sprite;
            }
        }
    }
}

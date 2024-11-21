using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public sealed partial class SyncSpriteRendererV1System : SystemBase
    {
        protected override void OnCreate()
        {
            var query = SystemAPI.QueryBuilder()
                .WithAll<NativeSpriteSheetVault>()
                .Build();

            RequireForUpdate(query);
        }

        protected override void OnUpdate()
        {
            foreach (var (rendererRef, spriteIndex, spriteIndexPrev, spriteSheetInfo, faceDirection)
                in SystemAPI.Query<SpriteRendererRef, SpriteIndex, SpriteIndexPrevious, SpriteSheetInfo, FaceDirection>()
                    .WithDisabled<NeedsInitComponentsTag>()
                    .WithDisabled<NeedsInitPresenterTag>()
                    .WithDisabled<NeedsDestroyTag>()
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
                var renderer = rendererRef.value.Value;
                renderer.sprite = sprite;
                renderer.flipX = faceDirection.GetFace() > 0;
            }
        }
    }
}

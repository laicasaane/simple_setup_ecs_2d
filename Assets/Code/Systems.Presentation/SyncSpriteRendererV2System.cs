using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public sealed partial class SyncSpriteRendererV2System : SystemBase
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
            var em = EntityManager;

            foreach (var (spriteIndex, spriteIndexPrev, spriteSheetInfo, faceDirection, entity)
                in SystemAPI.Query<SpriteIndex, SpriteIndexPrevious, SpriteSheetInfo, FaceDirection>()
                    .WithDisabled<NeedsInitComponentsTag>()
                    .WithDisabled<NeedsDestroyTag>()
                    .WithAll<Version2Tag>()
                    .WithEntityAccess()
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
                var renderer = em.GetComponentObject<SpriteRenderer>(entity);
                renderer.sprite = sprite;
                renderer.flipX = faceDirection.GetFace() > 0;
            }
        }
    }
}

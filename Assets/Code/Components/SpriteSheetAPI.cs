using System.Runtime.CompilerServices;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    public static class SpriteSheetAPI
    {
        public const int WALK_ID = 1;
        public const int RUN_ID = 2;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsMovable(SpriteSheetId id)
            => id.SheetId >= WALK_ID && id.SheetId <= RUN_ID;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Initialize(
              in SpriteSheetId id
            , in NativeSpriteSheet sheet
            , ref SpriteSheetInfo info
            , ref SpriteInterval interval
            , ref SpriteElapsedTime elapsedTime
            , ref SpriteIndex index
            , ref SpriteIndexPrevious indexPrev
            , EnabledRefRW<CanMoveTag> canMoveTag
        )
        {
            info.id = id;
            info.length = sheet.length;
            interval.value = sheet.spriteInterval;
            elapsedTime.value = 0f;
            index.value = 0;
            indexPrev.value = -1;
            canMoveTag.ValueRW = IsMovable(id);
        }
    }
}

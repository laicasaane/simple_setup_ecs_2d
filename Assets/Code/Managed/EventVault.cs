using System;

namespace SimpleSetupEcs2d
{
    public static class EventVault
    {
        public static event Action<int> OnChangeSpriteSheet;

        public static void ChangeSpriteSheet(int sheetId)
        {
            OnChangeSpriteSheet?.Invoke(sheetId);
        }
    }
}

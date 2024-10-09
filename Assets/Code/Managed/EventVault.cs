using System;

namespace SimpleSetupEcs2d
{
    public delegate void SpawnCharactersAction(int assetId, int amount, bool randomPosition);

    public static class EventVault
    {
        public static event Action<int> OnChangeSpriteSheet;

        public static event Action OnDestroyCharacters;

        public static event SpawnCharactersAction OnSpawnCharacters;

        public static event Action<bool> OnSetRandomAnimation;

        public static void ChangeSpriteSheet(int sheetId)
        {
            OnChangeSpriteSheet?.Invoke(sheetId);
        }

        public static void DestroyCharacters()
        {
            OnDestroyCharacters?.Invoke();
        }

        public static void SpawnCharacters(int assetId, int amount, bool randomPosition)
        {
            OnSpawnCharacters?.Invoke(assetId, amount, randomPosition);
        }

        public static void SetRandomAnimation(bool value)
        {
            OnSetRandomAnimation?.Invoke(value);
        }
    }
}

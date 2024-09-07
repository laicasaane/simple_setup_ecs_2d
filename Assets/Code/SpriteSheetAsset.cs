using System;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [CreateAssetMenu(fileName = "SpriteSheetAsset", menuName = "Test Setup/SpriteSheetAsset")]
    public sealed class SpriteSheetAsset : ScriptableObject
    {
        [SerializeField] private string _name;
        [SerializeField] private ushort _id;
        [SerializeField] private Sheet[] _sheets;

        public string Name => _name;

        public ushort Id => _id;

        public ReadOnlyMemory<Sheet> Sheets => _sheets;

        [Serializable]
        public class Sheet
        {
            [SerializeField] private string _name;
            [SerializeField] private ushort _id;
            [SerializeField] private float _fps;
            [SerializeField] private Sprite[] _sprites;

            public string Name => _name;

            public ushort Id => _id;

            public float Fps => _fps;

            public ReadOnlyMemory<Sprite> Sprites => _sprites;
        }
    }
}

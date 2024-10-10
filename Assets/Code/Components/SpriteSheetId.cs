using System;
using System.Runtime.InteropServices;

namespace SimpleSetupEcs2d
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct SpriteSheetId : IEquatable<SpriteSheetId>
    {
        [FieldOffset(0)] private readonly uint _raw;
        [FieldOffset(0)] public readonly ushort SheetId;
        [FieldOffset(2)] public readonly ushort AssetId;

        public SpriteSheetId(ushort assetId, ushort sheetId) : this()
        {
            SheetId = sheetId;
            AssetId = assetId;
        }

        public bool Equals(SpriteSheetId other)
            => _raw == other._raw;

        public override bool Equals(object obj)
            => obj is SpriteSheetId other && _raw == other._raw;

        public override int GetHashCode()
            => _raw.GetHashCode();

        public static bool operator ==(SpriteSheetId left, SpriteSheetId right)
            => left._raw == right._raw;

        public static bool operator !=(SpriteSheetId left, SpriteSheetId right)
            => left._raw != right._raw;
    }
}

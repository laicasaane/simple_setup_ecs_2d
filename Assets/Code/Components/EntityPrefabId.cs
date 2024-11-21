using System;
using System.Runtime.InteropServices;

namespace SimpleSetupEcs2d
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct EntityPrefabId : IEquatable<EntityPrefabId>
    {
        [FieldOffset(0)] public readonly uint Id;
        [FieldOffset(3)] public readonly byte Version;

        public EntityPrefabId(uint id, byte version) : this()
        {
            Id = id;
            Version = version;
        }

        public bool Equals(EntityPrefabId other)
            => Id == other.Id;

        public override bool Equals(object obj)
            => obj is EntityPrefabId other && Id == other.Id;

        public override int GetHashCode()
            => Id.GetHashCode();

        public static bool operator ==(EntityPrefabId left, EntityPrefabId right)
            => left.Id == right.Id;

        public static bool operator !=(EntityPrefabId left, EntityPrefabId right)
            => left.Id != right.Id;
    }
}

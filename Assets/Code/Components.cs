using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public struct SpriteSpawnInfo : IComponentData
    {
        public Entity prefab;
        public int amount;
    }

    public struct TransformRef : IComponentData
    {
        public UnityObjectRef<Transform> value;
    }

    public struct SpriteRendererRef : IComponentData
    {
        public UnityObjectRef<SpriteRenderer> value;
    }

    public struct SpriteSheetInfo : IComponentData
    {
        public SpriteSheetId id;
        public int length;
    }

    public struct SpriteIndex : IComponentData
    {
        public int value;
    }
    
    public struct SpriteIndexPrevious : IComponentData
    {
        public int value;
    }

    public struct SpriteInterval : IComponentData
    {
        public float value;
    }

    public struct SpriteElapsedTime : IComponentData
    {
        public float value;
    }
}

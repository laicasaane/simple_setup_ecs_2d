using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public struct NativeSpriteSheetVault : IComponentData
    {
        public NativeHashMap<SpriteSheetId, NativeSpriteSheet> map;
    }

    public struct NativeSpriteSheet
    {
        public int id;
        public int length;
        public float spriteInterval;
    }

    public struct SpriteSpawnInfo : IComponentData
    {
        public Entity prefab;
        public SpriteSheetId id;
        public int amount;
        public bool canChangeSpriteSheet;
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

    public readonly struct CanChangeSpriteSheetTag : IComponentData, IEnableableComponent { }

    public readonly struct NeedsInitPresenterTag : IComponentData, IEnableableComponent { }

    public readonly struct NeedsInitComponentsTag : IComponentData, IEnableableComponent { }
}

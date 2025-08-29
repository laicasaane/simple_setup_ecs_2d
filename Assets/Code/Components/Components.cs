using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Mathematics.Geometry;
using UnityEngine;
using UnityEngine.Jobs;

namespace SimpleSetupEcs2d
{
    public struct NativeSpriteSheetVault : IComponentData
    {
        public NativeArray<SpriteSheetId> sheetIds;
        public NativeHashMap<ushort, Range> assetIdToSheetIdRangeMap; // AssetId => Range in sheetIds
        public NativeHashMap<SpriteSheetId, NativeSpriteSheet> sheetIdToSheetMap;
    }

    public struct NativeSpriteSheet
    {
        public int id;
        public int length;
        public float spriteInterval;
    }

    public struct SpritePresenterPoolerRef : IComponentData
    {
        public UnityObjectRef<SpritePresenterPooler> poolerRef;
        public TransformAccessArray transformArray;
        public NativeList<float3> positions;
    }

    public struct EntityPrefab : IComponentData
    {
        public Entity prefab;
        public uint id;
        public byte version;
    }

    public struct EntityPrefabVault : IComponentData
    {
        public NativeHashMap<EntityPrefabId, Entity> value;
    }

    public struct RandomAnimation : IComponentData
    {
        public bool value;
    }

    public struct SpawnRange : IComponentData
    {
        public float3 min;
        public float3 max;
    }

    public struct WorldBoundary : IComponentData
    {
        public MinMaxAABB AABB;
    }

    public struct Randomizer : IComponentData
    {
        public Unity.Mathematics.Random value;
    }

    public struct SpriteSpawnInfo : IComponentData
    {
        public Entity prefab;
        public SpriteSheetId sheetId;
        public int amount;
        public bool canChangeSpriteSheet;
        public bool randomPosition;
    }

    public struct GameObjectInfo : IComponentData
    {
        public PooledGameObject value;
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

    public struct FaceDirection : IComponentData
    {
        public sbyte value;

        public readonly int GetFace()
            => math.select(-1, 1, value > 0);
    }

    public struct MoveSpeed : IComponentData
    {
        public float value;
    }

    public struct MoveSpeedConfig : IComponentData
    {
        public float2 value;
    }

    public struct MoveSpeedConfigAssetRef : IComponentData
    {
        public UnityObjectRef<MoveSpeedConfigAsset> value;
    }

    public readonly struct UpdateMoveSpeedCommandTag : IComponentData { }

    public readonly struct DestroySpritePresenterCommandTag : IComponentData { }

    public readonly struct DestroySpriteEntityCommandTag : IComponentData { }

    public readonly struct CanMoveTag : IComponentData, IEnableableComponent { }

    public readonly struct CanChangeSpriteSheetTag : IComponentData, IEnableableComponent { }

    public readonly struct NeedsInitPresenterTag : IComponentData, IEnableableComponent { }

    public readonly struct NeedsInitComponentsTag : IComponentData, IEnableableComponent { }

    public readonly struct NeedsDestroyTag : IComponentData, IEnableableComponent { }

    public readonly struct Version1Tag : IComponentData { }

    public readonly struct Version2Tag : IComponentData { }
}

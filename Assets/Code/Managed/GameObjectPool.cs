using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Jobs;

namespace SimpleSetupEcs2d
{
    [StructLayout(LayoutKind.Explicit)]
    public struct PooledGameObject
    {
        [FieldOffset(0)] public int instanceId;
        [FieldOffset(4)] public int transformId;
        [FieldOffset(8)] public int transformArrayIndex;
    }

    public sealed class GameObjectPool : IDisposable
    {
        private readonly int _prefabInstanceId;
        private readonly float3 _defaultPosition;
        private readonly quaternion _defaultRotation;
        private TransformAccessArray _transformArray;
        private NativeHashMap<int, PooledGameObject> _transformIdToObject;
        private NativeList<int> _unusedInstanceIds;
        private NativeList<int> _unusedTransformIds;
        private NativeList<float3> _positions;
        private NativeList<quaternion> _rotations;

        public GameObjectPool(
              GameObject prefab
            , int capacity
            , float3 defaultPosition
            , quaternion defaultRotation
        )
        {
            _prefabInstanceId = prefab.GetInstanceID();
            _defaultPosition = defaultPosition;
            _defaultRotation = defaultRotation;

            capacity = math.max(capacity, 4);

            TransformAccessArray.Allocate(capacity, -1, out _transformArray);
            _transformIdToObject = new(capacity, Allocator.Persistent);
            _unusedInstanceIds = new(capacity, Allocator.Persistent);
            _unusedTransformIds = new(capacity, Allocator.Persistent);
            _positions = new(capacity, Allocator.Persistent);
            _rotations = new(capacity, Allocator.Persistent);
        }

        public TransformAccessArray TransformArray => _transformArray;

        public NativeList<float3> Positions => _positions;

        public NativeList<quaternion> Rotations => _rotations;

        public void Dispose()
        {
            _transformArray.Dispose();
            _transformIdToObject.Dispose();
            _unusedInstanceIds.Dispose();
            _unusedTransformIds.Dispose();
            _positions.Dispose();
            _rotations.Dispose();
        }

        private void IncreaseCapacityBy(int amount)
        {
            var capacity = _transformArray.length + amount;

            if (_transformArray.capacity < capacity)
            {
                _transformArray.capacity = capacity;
            }

            if (_transformIdToObject.Capacity < capacity)
            {
                _transformIdToObject.Capacity = capacity;
            }

            if (_unusedInstanceIds.Capacity < capacity)
            {
                _unusedInstanceIds.Capacity = capacity;
            }

            if (_unusedTransformIds.Capacity < capacity)
            {
                _unusedTransformIds.Capacity = capacity;
            }

            if (_positions.Capacity < capacity)
            {
                _positions.Capacity = capacity;
            }

            if (_rotations.Capacity < capacity)
            {
                _rotations.Capacity = capacity;
            }
        }

        public void Prepool(int amount)
        {
            if (amount < 1)
            {
                return;
            }

            IncreaseCapacityBy(amount);

            var instanceIds = new NativeArray<int>(amount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var transformIds = new NativeArray<int>(amount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            GameObject.InstantiateGameObjects(_prefabInstanceId, amount, instanceIds, transformIds);
            GameObject.SetGameObjectsActive(instanceIds, false);

            _unusedInstanceIds.AddRange(instanceIds);
            _unusedTransformIds.AddRange(transformIds);
            _positions.AddReplicate(_defaultPosition, amount);
            _rotations.AddReplicate(_defaultRotation, amount);

            var array = _transformArray;
            var map = _transformIdToObject;

            for (var i = 0; i < amount; i++)
            {
                var instanceId = instanceIds[i];
                var transformId = transformIds[i];
                var arrayIndex = array.length;

                array.Add(transformId);
                map.Add(transformId, new PooledGameObject {
                    instanceId = instanceId,
                    transformId = transformId,
                    transformArrayIndex = arrayIndex,
                });
            }
        }

        private void RentInternal(Span<int> instanceIds, Span<int> transformIds, bool activate = false)
        {
            var amount = instanceIds.Length;

            Prepool(amount - _unusedInstanceIds.Length);

            var startIndex = _unusedInstanceIds.Length - amount;

            _unusedInstanceIds.AsArray().AsReadOnlySpan()[startIndex..].CopyTo(instanceIds);
            _unusedTransformIds.AsArray().AsReadOnlySpan()[startIndex..].CopyTo(transformIds);

            _unusedInstanceIds.RemoveRange(startIndex, amount);
            _unusedTransformIds.RemoveRange(startIndex, amount);

            if (activate)
            {
                GameObject.SetGameObjectsActive(instanceIds, true);
            }
        }

        public bool TryRent(int amount, ref NativeList<PooledGameObject> result, bool activate = false)
        {
            if (amount < 1)
            {
                return false;
            }

            if (result.IsCreated == false)
            {
                return false;
            }

            var instanceIds = new NativeArray<int>(amount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            var transformIds = new NativeArray<int>(amount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            RentInternal(instanceIds, transformIds, activate);

            var resultCapacity = result.Length + amount;

            if (result.Capacity < resultCapacity)
            {
                result.Capacity = resultCapacity;
            }

            var map = _transformIdToObject;

            for (var i = 0; i < amount; i++)
            {
                var transformId = transformIds[i];

                if (map.TryGetValue(transformId, out var item))
                {
                    result.AddNoResize(item);
                }
            }

            return true;
        }

        public void Return(Span<int> instanceIds, Span<int> transformIds)
        {
            var length = instanceIds.Length;

            if (length < 1)
            {
                return;
            }

            var startIndex = _unusedInstanceIds.Length;

            _unusedInstanceIds.AddReplicate(default, length);
            _unusedTransformIds.AddReplicate(default, length);

            instanceIds.CopyTo(_unusedInstanceIds.AsArray().AsSpan()[startIndex..]);
            transformIds.CopyTo(_unusedTransformIds.AsArray().AsSpan()[startIndex..]);

            GameObject.SetGameObjectsActive(instanceIds, false);
        }
    }
}

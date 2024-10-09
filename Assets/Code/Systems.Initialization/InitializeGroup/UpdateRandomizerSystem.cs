using System.Runtime.InteropServices;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

namespace SimpleSetupEcs2d
{
    [BurstCompile]
    [UpdateInGroup(typeof(InitializeSystemGroup), OrderFirst = true)]
    internal partial struct UpdateRandomizerSystem : ISystem
    {
        private EntityQuery _query;

        // Cannot Burst because of System.DateTime
        public void OnCreate(ref SystemState state)
        {
            uint randomSeed;

            unchecked { randomSeed = (uint)System.DateTime.Now.Ticks; }
            var union = new FloatUIntUnion { f = randomSeed * GetPhi() };
            var randomizer = new Randomizer { value = new Random(union.i) };

            state.EntityManager.CreateSingleton(randomizer, nameof(Randomizer));

            _query = SystemAPI.QueryBuilder()
                .WithAllRW<Randomizer>()
                .Build();

            state.RequireForUpdate(_query);
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            // Before accessing the Randomizer via GetSingletonRW
            // it is required to complete the Dependency
            // then reset it.
            state.Dependency.Complete();
            state.Dependency = default;

            var randomizerRW = _query.GetSingletonRW<Randomizer>();
            var union = new FloatUIntUnion { f = randomizerRW.ValueRO.value.state * GetPhi() };
            randomizerRW.ValueRW.value = new Random(union.i);
        }

        private static float GetPhi() => (1f + math.sqrt(5f)) / 2f;

        [StructLayout(LayoutKind.Explicit)]
        private struct FloatUIntUnion
        {
            [FieldOffset(0)] public float f;
            [FieldOffset(0)] public uint i;
        }
    }
}

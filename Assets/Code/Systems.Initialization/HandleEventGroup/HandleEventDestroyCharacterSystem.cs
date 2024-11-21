using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(HandleEventSystemGroup))]
    public sealed partial class HandleEventDestroyCharacterSystem : SystemBase
    {
        private EntityArchetype _archetypeV1;
        private EntityArchetype _archetypeV2;
        private Data? _value;
        private byte _version = VersionAPI.VERSION_1;

        protected override void OnCreate()
        {
            EventVault.OnDestroyCharacters += EventVault_OnDestroyCharacters;
            EventVault.OnSetVersion2 += EventVault_OnSetVersion2;

            var v1Types = new NativeArray<ComponentType>(2, Allocator.Temp) {
                [0] = ComponentType.ReadWrite<DestroySpritePresenterCommandTag>(),
                [1] = ComponentType.ReadWrite<NeedsDestroyTag>(),
            };

            _archetypeV1 = EntityManager.CreateArchetype(v1Types);

            var v2Types = new NativeArray<ComponentType>(2, Allocator.Temp) {
                [0] = ComponentType.ReadWrite<DestroySpriteEntityCommandTag>(),
                [1] = ComponentType.ReadWrite<NeedsDestroyTag>(),
            };

            _archetypeV2 = EntityManager.CreateArchetype(v2Types);
        }

        protected override void OnUpdate()
        {
            if (_value.HasValue == false)
            {
                return;
            }

            var value = _value.Value;
            _value = default;

            var entity = value.version == VersionAPI.VERSION_2
                ? EntityManager.CreateEntity(_archetypeV2)
                : EntityManager.CreateEntity(_archetypeV1)
                ;

            EntityManager.SetComponentEnabled<NeedsDestroyTag>(entity, false);

            // Dependency has been completed by the EntityManager.CreateEntity
            // so we can safely reset it.
            Dependency = default;
        }

        private void EventVault_OnDestroyCharacters()
        {
            _value = new Data {
                version = _version,
            };
        }

        private void EventVault_OnSetVersion2(bool value)
        {
            _value = new Data {
                version = _version,
            };

            _version = VersionAPI.GetVersion(value);
        }

        private struct Data
        {
            public byte version;
        }
    }
}

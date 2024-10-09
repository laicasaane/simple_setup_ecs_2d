using Unity.Collections;
using Unity.Entities;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(HandleEventSystemGroup))]
    public sealed partial class HandleEventDestroyCharacterSystem : SystemBase
    {
        private bool? _value;
        private EntityArchetype _archetype;

        protected override void OnCreate()
        {
            EventVault.OnDestroyCharacters += EventVault_OnDestroyCharacters;

            var types = new NativeArray<ComponentType>(2, Allocator.Temp) {
                [0] = ComponentType.ReadWrite<DestroyCommandTag>(),
                [1] = ComponentType.ReadWrite<NeedsDestroyTag>(),
            };

            _archetype = EntityManager.CreateArchetype(types);
        }

        protected override void OnUpdate()
        {
            if (_value.HasValue == false)
            {
                return;
            }

            _value = default;

            var entity = EntityManager.CreateEntity(_archetype);
            EntityManager.SetComponentEnabled<NeedsDestroyTag>(entity, false);

            // Dependency has been completed by the EntityManager.CreateEntity
            // so we can safely reset it.
            Dependency = default;
        }

        private void EventVault_OnDestroyCharacters()
        {
            _value = true;
        }
    }
}

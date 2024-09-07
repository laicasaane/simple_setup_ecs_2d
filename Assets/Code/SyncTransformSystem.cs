using Unity.Entities;
using Unity.Transforms;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(PresentationSystemGroup))]
    public sealed partial class SyncTransformSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            foreach (var (transformRef, localToWorld) in SystemAPI.Query<TransformRef, LocalToWorld>())
            {
                transformRef.value.Value.SetPositionAndRotation(localToWorld.Position, localToWorld.Rotation);
            }
        }
    }
}

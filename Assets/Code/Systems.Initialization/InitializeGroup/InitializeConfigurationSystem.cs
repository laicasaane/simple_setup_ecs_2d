using Unity.Entities;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    [UpdateInGroup(typeof(InitializeSystemGroup))]
    public sealed partial class InitializeConfigurationSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var indicator = Object.FindFirstObjectByType<ConfigurationIndicator>();

            if (indicator == false)
            {
                return;
            }

            if (indicator.moveSpeedConfig)
            {
                var assetRef = new MoveSpeedConfigAssetRef
                {
                    value = indicator.moveSpeedConfig
                };

                var config = new MoveSpeedConfig {
                    value = indicator.moveSpeedConfig.value,
                };

                EntityManager.CreateSingleton(assetRef, nameof(MoveSpeedConfigAssetRef));
                EntityManager.CreateSingleton(config, nameof(MoveSpeedConfig));
            }

            CheckedStateRef.Enabled = false;
        }
    }
}

using System.Collections;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class SceneLoadedIndicator : MonoBehaviour
    {
        public bool Loaded { get; private set; }

        private void Awake()
        {
            StartCoroutine(Load_Coroutine());
        }

        private IEnumerator Load_Coroutine()
        {
            while (SpriteSheetVault.IsReady == false)
            {
                yield return null;
            }

            Loaded = true;
        }
    }
}

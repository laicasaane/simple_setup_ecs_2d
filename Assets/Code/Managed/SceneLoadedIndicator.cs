using System.Collections;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class SceneLoadedIndicator : MonoBehaviour
    {
        public bool Loaded { get; private set; }

        private void Awake()
        {
            gameObject.AddComponent<SpritePresenterPooler>();

            StartCoroutine(Load_Coroutine());
        }

        private IEnumerator Load_Coroutine()
        {
            var pooler = GetComponent<SpritePresenterPooler>();
            pooler.Initialize();

            while (SpriteSheetVault.IsReady == false)
            {
                yield return null;
            }

            Loaded = true;
        }
    }
}

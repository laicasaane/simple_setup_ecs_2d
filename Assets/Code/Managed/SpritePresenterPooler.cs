using Unity.Mathematics;
using UnityEngine;

namespace SimpleSetupEcs2d
{
    public sealed class SpritePresenterPooler : MonoBehaviour
    {
        private GameObjectPool _pool;

        public GameObjectPool Pool => _pool;

        public void Initialize()
        {
            if (_pool != null)
            {
                return;
            }

            var prefab = new GameObject("prefab-sprite-presenter");
            var renderer = prefab.AddComponent<SpriteRenderer>();
            renderer.spriteSortPoint = SpriteSortPoint.Pivot;
            prefab.SetActive(false);

            _pool = new GameObjectPool(prefab, 4, float3.zero, quaternion.identity);
        }

        private void OnDestroy()
        {
            _pool?.Dispose();
        }
    }
}

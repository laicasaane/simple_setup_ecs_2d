using UnityEngine;

namespace SimpleSetupEcs2d
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class SpawnRangeIndicator : MonoBehaviour
    {
        [SerializeField] private BoxCollider2D _range;

        public BoxCollider2D Range => _range;

        private void OnValidate()
        {
            _range = GetComponentInChildren<BoxCollider2D>();
        }

        private void Awake()
        {
            if (_range == false)
            {
                _range = GetComponentInChildren<BoxCollider2D>();
            }
        }
    }
}

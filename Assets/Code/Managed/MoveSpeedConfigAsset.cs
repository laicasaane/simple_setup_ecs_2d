using UnityEngine;

namespace SimpleSetupEcs2d
{
    [CreateAssetMenu(fileName = nameof(MoveSpeedConfigAsset), menuName = nameof(MoveSpeedConfigAsset))]
    public class MoveSpeedConfigAsset : ScriptableObject
    {
        public Vector2 value;
    }
}

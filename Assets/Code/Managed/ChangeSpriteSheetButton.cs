using UnityEngine;
using UnityEngine.UI;

namespace SimpleSetupEcs2d
{
    public sealed class ChangeSpriteSheetButton : MonoBehaviour
    {
        public int sheetId;

        private void Awake()
        {
            var button = GetComponentInChildren<Button>();
            button.onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            EventVault.ChangeSpriteSheet(sheetId);
        }
    }
}

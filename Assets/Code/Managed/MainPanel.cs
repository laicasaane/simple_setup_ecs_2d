using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SimpleSetupEcs2d
{
    public class MainPanel : MonoBehaviour
    {
        [SerializeField] private CanvasGroup _panelAnimations;
        [SerializeField] private CanvasGroup _panelCharacterCount;

        [SerializeField] private Toggle _toggleRandomAnimation;
        [SerializeField] private Toggle _toggleMultiCharacters;

        [SerializeField] private TMP_Text _labelRandomAnimation;
        [SerializeField] private TMP_Text _labelMultiCharacters;

        [SerializeField] private TMP_InputField _inputCharacterCount;
        [SerializeField] private Button _buttonApplyCharacterCount;

        private void Awake()
        {
            _toggleRandomAnimation.onValueChanged.AddListener(ToggleRandomAnimation_OnValueChanged);
            _toggleMultiCharacters.onValueChanged.AddListener(ToggleMultiCharacter_OnValueChanged);
            _buttonApplyCharacterCount.onClick.AddListener(ButtonApplyCharacterCount_OnClick);

            SetCanvasGroup(_panelAnimations, false, flip: true);
            SetCanvasGroup(_panelCharacterCount, false, flip: false);
        }

        private void ToggleRandomAnimation_OnValueChanged(bool value)
        {
            _labelRandomAnimation.text = value ? "Random Animation" : "Single Animation";
            SetCanvasGroup(_panelAnimations, value, flip: true);

            EventVault.SetRandomAnimation(value);
        }

        private void ToggleMultiCharacter_OnValueChanged(bool value)
        {
            _labelMultiCharacters.text = value ? "Multi Characters" : "Single Character";
            SetCanvasGroup(_panelCharacterCount, value, flip: false);

            if (value && TryGetCharacterCount(out var count))
            {
                ResetCharacters(count, true);
            }
            else
            {
                ResetCharacters(1, false);
            }
        }

        private void ButtonApplyCharacterCount_OnClick()
        {
            if (TryGetCharacterCount(out var count))
            {
                ResetCharacters(count, true);
            }
        }

        private bool TryGetCharacterCount(out int result)
            => int.TryParse(_inputCharacterCount.text, out result);

        private static void SetCanvasGroup(CanvasGroup group, bool value, bool flip)
        {
            if (flip)
            {
                group.alpha = value ? 0 : 1;
                group.interactable = !value;
                group.blocksRaycasts = !value;
            }
            else
            {
                group.alpha = value ? 1 : 0;
                group.interactable = value;
                group.blocksRaycasts = value;
            }
        }

        private static void ResetCharacters(int amount, bool randomPosition)
        {
            EventVault.DestroyCharacters();

            if (amount > 0)
            {
                EventVault.SpawnCharacters(0, amount, randomPosition);
            }
        }
    }
}

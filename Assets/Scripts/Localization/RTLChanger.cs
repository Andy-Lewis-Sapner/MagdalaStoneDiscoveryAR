using TMPro;
using UnityEngine;

public class RTLChanger : MonoBehaviour {
    private TextMeshProUGUI _textMeshPro; // Cached reference
    [SerializeField] private bool allowChangingAlignment = true; // Allow changing alignment

    /**
     * <summary>Gets the TextMeshProUGUI component on the game object</summary>
     */
    private void Awake() {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    /**
     * <summary>Changes the text direction based on the saved locale</summary>
     */
    private void Start() {
        OnLocaleChanged(null, LocaleSelector.instance.localeId);
        LocaleSelector.OnLocaleChanged += OnLocaleChanged;
    }

    /**
     * <summary>Changes the text direction based on the selected locale</summary>
     */
    private void OnLocaleChanged(object sender, int localeId) {
        bool isRightToLeft = localeId != LocaleSelector.EnglishLocaleId && localeId != LocaleSelector.RussianLocaleId;
        
        _textMeshPro.isRightToLeftText = isRightToLeft;
        if (_textMeshPro.alignment != TextAlignmentOptions.Center && allowChangingAlignment)
            _textMeshPro.alignment = isRightToLeft ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
    }

    /**
     * <summary>Removes the event listener</summary>
     */
    private void OnDestroy() {
        LocaleSelector.OnLocaleChanged -= OnLocaleChanged;
    }
}
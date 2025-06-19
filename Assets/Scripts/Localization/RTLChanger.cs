using TMPro;
using UnityEngine;

public class RTLChanger : MonoBehaviour {
    private TextMeshProUGUI _textMeshPro; // Cached reference

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
        _textMeshPro.isRightToLeftText = localeId == 1;
        if (_textMeshPro.alignment != TextAlignmentOptions.Center) 
            _textMeshPro.alignment = localeId == 1 ? TextAlignmentOptions.Right : TextAlignmentOptions.Left;
    }

    /**
     * <summary>Removes the event listener</summary>
     */
    private void OnDestroy() {
        LocaleSelector.OnLocaleChanged -= OnLocaleChanged;
    }
}
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class FontChanger : MonoBehaviour {
    private TextMeshProUGUI _textMeshPro; // Reference to the TextMeshProUGUI component

    /**
     * <summary>Gets the TextMeshProUGUI component on the game object</summary>
     */
    private void Awake() {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }

    /**
     * <summary>Changes the font based on the selected locale</summary>
     */
    private void Start() {
        LocaleSelector.OnLocaleChanged += LocaleSelectorOnLocaleChanged;
        StartCoroutine(SetFontPerLanguage());
    }
    
    /**
     * <summary>Changes the font based on the newly selected locale</summary>
     */
    private void LocaleSelectorOnLocaleChanged(object sender, int localeId) {
        _textMeshPro.font = LocaleSelector.instance.GetFontAsset(localeId);
    }

    /**
     * <summary>Changes the font based on the saved locale</summary>
     */
    private IEnumerator SetFontPerLanguage() {
        yield return new WaitUntil(() => LocaleSelector.instance && LocaleSelector.instance.localeId != -1);
        _textMeshPro.font = LocaleSelector.instance.GetFontAsset(LocaleSelector.instance.localeId);
    }

    /**
     * <summary>Changes the font based on the saved locale</summary>
     */
    private void OnEnable() {
        StartCoroutine(SetFontPerLanguage());
    }

    /**
     * <summary>Removes the event listener</summary>
     */
    private void OnDestroy() {
        LocaleSelector.OnLocaleChanged -= LocaleSelectorOnLocaleChanged;
    }
}
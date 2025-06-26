using UnityEngine;
using UnityEngine.UI;

public class ClickSound : MonoBehaviour {
    private Button _button; // Reference to the button
    private Toggle _toggle; // Reference to the toggle

    /**
     * <summary>Gets the button/toggle reference</summary>
     */
    private void Awake() {
        _button = GetComponent<Button>();
        _toggle = GetComponent<Toggle>();
    }

    /**
     * <summary>Subscribes to the button/toggle click event</summary>
     */
    private void Start() {
        if (_button) _button.onClick.AddListener(PlayButtonClickSound);
        if (_toggle) _toggle.onValueChanged.AddListener(_ => PlayButtonClickSound());
    }

    /**
     * <summary>Plays the button click sound</summary>
     */
    private static void PlayButtonClickSound() {
        AudioManager.instance.PlayButtonClickSound();
    }

    /**
     * <summary>Unsubscribes from the button/toggle click event</summary>
     */
    private void OnDestroy() {
        if (_button) _button.onClick.RemoveListener(PlayButtonClickSound);
        if (_toggle) _toggle.onValueChanged.RemoveListener(_ => PlayButtonClickSound());
    }
}
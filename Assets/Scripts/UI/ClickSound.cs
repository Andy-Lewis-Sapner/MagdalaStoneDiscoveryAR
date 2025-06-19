using UnityEngine;
using UnityEngine.UI;

public class ClickSound : MonoBehaviour {
    private Button _button; // Reference to the button
    private Toggle _toggle; // Reference to the toggle

    /**
     * <summary>
     * This script is attached to buttons or toggles.
     * The button/toggle is connected to this script. 
     * </summary>
     */
    private void Awake() {
        _button = GetComponent<Button>();
        if (_button) {
            _button.onClick.AddListener(PlayButtonClickSound);  
        } else {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(_ => PlayButtonClickSound());
        }
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
        if (_button)
            _button.onClick.RemoveListener(PlayButtonClickSound);
        if (_toggle)
            _toggle.onValueChanged.RemoveListener(_ => PlayButtonClickSound());
    }
}
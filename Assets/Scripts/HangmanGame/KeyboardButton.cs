using System;
using TMPro;
using UnityEngine;

public class KeyboardButton : MonoBehaviour {
    public static event EventHandler<char> OnKeyboardButtonPressed; // Event for when a keyboard button is pressed
    [SerializeField] private TextMeshProUGUI key; // The text of the button (the key)

    /**
     * <summary>Event handler for when a keyboard button is pressed </summary>
     */
    public void OnPressed() {
        OnKeyboardButtonPressed?.Invoke(this, key.text[0]);
    }
}
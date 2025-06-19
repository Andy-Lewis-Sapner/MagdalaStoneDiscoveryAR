using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardController : MonoBehaviour {
    public static KeyboardController instance { get; private set; } // Singleton
    private const float AnimationDuration = 0.5f; // Duration of the keyboard fade animation
    
    [SerializeField] private CanvasGroup[] keyboards; // Array of keyboard CanvasGroups
    private Tween _keyboardFadeTween; // Tween for the keyboard fade animation
    private int _currentKeyboard; // Index of the currently active keyboard

    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
    }

    /**
     * <summary>Initializes the keyboards</summary>
     */
    private void Start() {
        foreach (CanvasGroup keyboard in keyboards) {
            keyboard.alpha = 0;
            keyboard.interactable = false;
            keyboard.blocksRaycasts = false;
        }
    }
    
    /**
     * <summary>Shows or hides the keyboard</summary>
     * <param name="show">Whether to show or hide the keyboard</param>
     */
    public void ShowKeyboard(bool show) {
        if (show) {
            _currentKeyboard = LocaleSelector.instance.localeId;
            CanvasGroup keyboard = keyboards[_currentKeyboard];

            foreach (Transform key in keyboard.transform)
                if (key.TryGetComponent(out Button button))
                    button.interactable = true;

            if (SettingsPanel.instance.toggleAnimationsIsOn) {
                _keyboardFadeTween = keyboard.DOFade(1, AnimationDuration).SetEase(Ease.OutBack).OnComplete(() => {
                    keyboard.interactable = true;
                    keyboard.blocksRaycasts = true;
                });
            } else {
                keyboard.alpha = 1;
                keyboard.interactable = true;
                keyboard.blocksRaycasts = true;
            }
        } else {
            CanvasGroup keyboard = keyboards[_currentKeyboard];
            keyboard.interactable = false;
            keyboard.blocksRaycasts = false;
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                _keyboardFadeTween = keyboard.DOFade(0, AnimationDuration).SetEase(Ease.OutBack);
            else
                keyboard.alpha = 0;
        }
    }

    /**
     * <summary>Kills the keyboard fade animation</summary>
     */
    private void OnDestroy() {
        _keyboardFadeTween?.Kill();
    }
}
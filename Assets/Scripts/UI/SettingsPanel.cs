using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : MonoBehaviour {
    public static SettingsPanel instance { get; private set; } // Singleton
    
    [SerializeField] private CanvasGroup canvasGroup; // The settings panel
    [SerializeField] private Toggle playVoiceForInformation; // Whether to play voice for information
    [SerializeField] private Toggle toggleAnimations; // Whether to play voice for information
    [SerializeField] private Toggle muteBackgroundMusic; // Whether to play voice for information
    [SerializeField] private Button englishLanguageButton; // The button to switch to English
    [SerializeField] private Button hebrewLanguageButton; // The button to switch to Hebrew
    [SerializeField] private Slider sliderVoiceVolume; // The slider for voice volume
    [SerializeField] private TextMeshProUGUI voiceVolumeText; // The text for voice volume
    
    public bool playVoiceForInformationIsOn => playVoiceForInformation.isOn; // Whether to play voice for information
    public bool toggleAnimationsIsOn => toggleAnimations.isOn; // Whether to play voice for information
    public bool isPanelVisible => canvasGroup.alpha > 0; // Whether the panel is visible
    
    private Tween _panelFadeTween; // The fade tween
    private bool _panelIsVisible; // Whether the panel is visible
    
    /**
     * <summary>Initializes the singleton and sets up the language buttons</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
        
        SetLanguageButtons();
        SetSavedValues();
    }

    /**
     * <summary>Sets up the language buttons</summary>
     */
    private void SetLanguageButtons() {
        englishLanguageButton.onClick.AddListener(() => LocaleSelector.instance.ChangeLocale(0));
        hebrewLanguageButton.onClick.AddListener(() => LocaleSelector.instance.ChangeLocale(1));
    }

    /**
     * <summary>Initializes the settings panel</summary>
     */
    private void Start() {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }

    /**
     * <summary>Sets the saved values</summary>
     */
    private void SetSavedValues() {
        sliderVoiceVolume.value = (int)(AudioManager.instance.audioVolume * 100f);
        voiceVolumeText.text = $"{sliderVoiceVolume.value}%";
        toggleAnimations.isOn = GameManager.instance.toggleAnimationsSetting;
        playVoiceForInformation.isOn = GameManager.instance.playVoiceSetting;
        muteBackgroundMusic.isOn = GameManager.instance.muteBackgroundMusic;
    }

    /**
     * <summary>Shows or hides the settings panel</summary>
     * <param name="show">Whether to show or hide the settings panel</param>
     */
    public void ShowSettingsPanel(bool show) {
        if (show) {
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            if (toggleAnimationsIsOn)
                _panelFadeTween =canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutBack);
            else
                canvasGroup.alpha = 1;
        } else {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            if (toggleAnimationsIsOn)
                _panelFadeTween = canvasGroup.DOFade(0, 0.5f).SetEase(Ease.OutBack);
            else
                canvasGroup.alpha = 0;
        }
    }

    /**
     * <summary>Sets the voice volume</summary>
     */
    public void SetVolume() {
        AudioManager.instance.audioVolume = sliderVoiceVolume.value / 100f;
        voiceVolumeText.text = $"{sliderVoiceVolume.value}%";
    }

    /**
     * <summary>Sets the toggle animations setting</summary>
     */
    public void OnToggleAnimationChanged() {
        GameManager.instance.toggleAnimationsSetting = toggleAnimationsIsOn;
    }
    
    /**
     * <summary>Sets the play voice for information setting</summary>
     */
    public void OnPlayVoiceForInformationChanged() {
        GameManager.instance.playVoiceSetting = playVoiceForInformationIsOn;
    }
    
    /**
     * <summary>Sets the mute background music setting</summary>
     */
    public void OnMuteBackgroundMusicChanged() {
        GameManager.instance.muteBackgroundMusic = muteBackgroundMusic.isOn;
        AudioManager.instance.MuteBackgroundMusic();
    }

    /**
     * <summary>Removes event listeners</summary>
     */
    private void OnDestroy() {
        englishLanguageButton.onClick.RemoveAllListeners();
        hebrewLanguageButton.onClick.RemoveAllListeners();
        _panelFadeTween?.Kill();
    }
}
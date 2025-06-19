using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
public class InformationManager : MonoBehaviour {
    public static InformationManager instance { get; private set; } // Singleton
    public static event EventHandler OnInformationPanelOpened; // Event when the information panel is opened
    public static event EventHandler OnInformationPanelClosed; // Event when the information panel is closed
    private const float AnimationDuration = 0.5f; // Duration of animations
    private const float CharactersPerSecondEnglish = 30f; // Characters per second for English
    private const float CharactersPerSecondHebrew = 45f; // Characters per second for Hebrew
    
    [Header("UI Elements")]
    [SerializeField] private CanvasGroup informationPanel; // Information panel
    [SerializeField] private TypeWriterEffect titleText; // Information title
    [SerializeField] private TypeWriterEffect informationText; // Information text
    [SerializeField] private Slider informationProgressSlider; // Progress slider
    
    [Header("Buttons")]
    [SerializeField] private CanvasGroup nextInformationButton; // Next information button
    [SerializeField] private Button playPauseButton; // Play/pause voice over button
    [SerializeField] private Button restartButton; // Restart voice over button

    private InformationSo _currentInformation; // Currently displayed information
    private Tween _panelFadeTween, _nextButtonFadeTween, _progressSliderFadeTween; // Tweens
    private int _currentInformationIndex; // Index of the currently displayed information

    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
    }

    /**
     * <summary>Prepares the information panel, adds event listeners and starts the game</summary>
     */
    private void Start() {
        informationPanel.alpha = 0;
        informationPanel.interactable = false;
        informationPanel.blocksRaycasts = false;
        informationProgressSlider.direction = LocaleSelector.instance.localeId == 0
            ? Slider.Direction.LeftToRight
            : Slider.Direction.RightToLeft;
        informationText.ChangeCharactersPerSecond(LocaleSelector.instance.localeId == 0
            ? CharactersPerSecondEnglish
            : CharactersPerSecondHebrew);
        
        LocaleSelector.OnLocaleChanged += ChangeInformationLanguage;
        TypeWriterEffect.CompleteTextRevealed += TypeWriterEffectOnCompleteTextRevealed;
        playPauseButton.onClick.AddListener(AudioManager.instance.PlayPauseClip);
        restartButton.onClick.AddListener(AudioManager.instance.RestartClip);
    }

    /**
     * <summary>Changes the language of the information</summary>
     * <param name="sender">The sender of the event.</param>
     * <param name="localeId">The ID of the locale that was changed.</param>
     */
    private void ChangeInformationLanguage(object sender, int localeId) {
        if (!_currentInformation || informationPanel.alpha == 0) return;
        SetNextButtonInitialState();
        titleText.SetText(_currentInformation.titles[localeId]);
        informationProgressSlider.direction =
            localeId == 0 ? Slider.Direction.LeftToRight : Slider.Direction.RightToLeft;
        informationText.ChangeCharactersPerSecond(localeId == 0 ? CharactersPerSecondEnglish : CharactersPerSecondHebrew);
        SetDialogue();
    }

    /**
     * <summary>Displays the information panel</summary>
     * <param name="information">The information to be displayed</param>
     */
    public void ShowInformation(InformationSo information) {
        _currentInformation = information;
        SetNextButtonInitialState();
        SetInitialInformation();
        ShowInformationPanel(true);
    }

    /**
     * <summary>Sets the initial state of the next information button</summary>
     */
    private void SetNextButtonInitialState() {
        nextInformationButton.alpha = 0;
        nextInformationButton.interactable = false;
    }

    /**
     * <summary>Sets the initial information</summary>
     */
    private void SetInitialInformation() {
        titleText.SetText(_currentInformation.titles[LocaleSelector.instance.localeId]);
        _currentInformationIndex = 0;
        informationProgressSlider.value = 0;
        informationProgressSlider.gameObject.SetActive(_currentInformation.informationDialogue.Length > 1);
        SetSliderValue();
        SetDialogue();
    }

    /**
     * <summary>Displays the next information</summary>
     */
    public void ShowNextInformation() {
        if (_currentInformationIndex >= _currentInformation.informationDialogue.Length - 1) return;
        _currentInformationIndex++;
        SetSliderValue();
        SetDialogue();

        if (SettingsPanel.instance.toggleAnimationsIsOn)
            _nextButtonFadeTween = nextInformationButton.DOFade(0, AnimationDuration).SetEase(Ease.OutBack);
        else 
            nextInformationButton.alpha = 0;
        nextInformationButton.interactable = false;
    }

    /**
     * <summary>Sets the progress slider value</summary>
     */
    private void SetSliderValue() {
        if (!informationProgressSlider.gameObject.activeSelf) return;
        
        float value = (float)(_currentInformationIndex + 1) / _currentInformation.informationDialogue.Length;
        if (SettingsPanel.instance.toggleAnimationsIsOn)
            _progressSliderFadeTween = DOTween.To(() => informationProgressSlider.value,
                x => informationProgressSlider.value = x, value, AnimationDuration);
        else
            informationProgressSlider.value = value;
    }
    
    /**
     * <summary>Handles the event when the text is revealed</summary>
     * <param name="sender">The sender of the event.</param>
     * <param name="e">The event arguments.</param>
     */
    private void TypeWriterEffectOnCompleteTextRevealed(object sender, EventArgs e) {
        if (!ReferenceEquals(sender, informationText)) return;
        if (_currentInformationIndex >= _currentInformation?.informationDialogue.Length - 1) return;
        if (SettingsPanel.instance.toggleAnimationsIsOn)
            _nextButtonFadeTween = nextInformationButton.DOFade(1, AnimationDuration).SetEase(Ease.OutBack);
        else 
            nextInformationButton.alpha = 1;
        nextInformationButton.interactable = true;
    }

    /**
     * <summary>Sets the dialogue</summary>
     */
    private void SetDialogue() {
        InformationSo.InformationDialoguePerLanguage dialogue = _currentInformation
            .informationDialogue[_currentInformationIndex].informationDialoguePerLanguages[LocaleSelector.instance.localeId];
        
        informationText.SetText(dialogue.dialogueText);
        AudioClip clip = dialogue.dialogueAudioClip;
        if (clip) AudioManager.instance.PlayNewClip(clip);
    }

    /**
     * <summary>Displays or hides the information panel</summary>
     * <param name="show">Whether to show or hide the information panel</param>
     */
    public void ShowInformationPanel(bool show) {
        if (show) {
            OnInformationPanelOpened?.Invoke(this, EventArgs.Empty);
            informationPanel.interactable = true;
            informationPanel.blocksRaycasts = true;
            
            if (SettingsPanel.instance.toggleAnimationsIsOn) 
                _panelFadeTween = informationPanel.DOFade(1f, AnimationDuration).SetEase(Ease.OutBack);
            else 
                informationPanel.alpha = 1f;
        } else {
            informationPanel.interactable = false;
            informationPanel.blocksRaycasts = false;
            _currentInformationIndex = 0;
            _currentInformation = null;
            AudioManager.instance.RemoveClip();

            if (SettingsPanel.instance.toggleAnimationsIsOn) {
                _panelFadeTween = informationPanel.DOFade(0f, AnimationDuration).SetEase(Ease.InBack)
                    .OnComplete(() => OnInformationPanelClosed?.Invoke(this, EventArgs.Empty));
            } else {
                informationPanel.alpha = 0f;
                OnInformationPanelClosed?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    /**
     * <summary>Removes event listeners, stops coroutines and fade tweens</summary>
     */
    private void OnDestroy() {
        LocaleSelector.OnLocaleChanged -= ChangeInformationLanguage;
        TypeWriterEffect.CompleteTextRevealed -= TypeWriterEffectOnCompleteTextRevealed;
        playPauseButton.onClick.RemoveListener(AudioManager.instance.PlayPauseClip);
        restartButton.onClick.RemoveListener(AudioManager.instance.RestartClip);
        _nextButtonFadeTween?.Kill();
        _panelFadeTween?.Kill();
        _progressSliderFadeTween?.Kill();
    }
}

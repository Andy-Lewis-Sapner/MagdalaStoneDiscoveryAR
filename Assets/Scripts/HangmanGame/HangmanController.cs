using System;
using System.Text;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HangmanController : MonoBehaviour {
    public static HangmanController instance { get; private set; } // Singleton
    public static event EventHandler OnGameDismissedOrEnded; // Event to notify when the game is dismissed or ended
    private const float AnimationDuration = 0.5f; // Duration of animations
    private readonly StringBuilder _wordBuilder = new(); // StringBuilder to build the word for display
    
    public bool isPanelVisible => gamePanel.alpha > 0; // Property to check if the game panel is visible

    [SerializeField] private CanvasGroup gamePanel; // CanvasGroup for the game panel
    [SerializeField] private TextMeshProUGUI wordText; // Text to display the word
    [SerializeField] private GameObject[] hangmanStages; // Array of hangman stages

    [Header("Hint Panel")] 
    [SerializeField] private RectTransform hintPanel; // RectTransform for the hint panel
    [SerializeField] private TextMeshProUGUI hintText; // Text to display the hint
    [SerializeField] private Button hintButton; // Button to show the hint
    [SerializeField] private GameObject hintLoadingSpinner; // Loading spinner for the hint
    
    [Header("Colors")]
    [SerializeField] private Color baseWordColor; // Base color for the word
    [SerializeField] private Color correctColor; // Color for correctly guessed letters
    [SerializeField] private Color incorrectColor; // Color for incorrectly guessed letters

    private InformationSo _currentInformation; // Information for the current game
    private Tween _panelFadeTween, _hintPanelTween; // Tweens for animations
    private bool _panelIsVisible; // Flag to track if the panel is visible
    private string _currentWord; // Current word for the game
    // Counters for incorrect and correct guesses
    // Last deleted stage - used to keep track of the last deleted stage
    private int _incorrectGuesses, _correctGuesses, _lastDeletedStage, _deleteStage = 1;

    /**
     * The method instantiates the singleton instance of the HangmanController class.
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
    }

    /**
     * Prepare the game by hiding the panel, setting the initial state,
     * and subscribing to events for locale changes and keyboard button presses.
     */
    private void Start() {
        gamePanel.alpha = 0;
        gamePanel.interactable = false;
        gamePanel.blocksRaycasts = false;
        hintText.text = string.Empty;
        hintLoadingSpinner.SetActive(false);
        hintPanel.localScale = Vector3.zero;
        
        LocaleSelector.OnLocaleChanged += OnLocaleChanged;
        KeyboardButton.OnKeyboardButtonPressed += OnKeyboardButtonPressed;
    }

    /**
     * <param name="sender">The sender of the event.</param>
     * <param name="localeId">The ID of the locale that was changed.</param>
     * <summary>When language is changed, the game is reinitialized.</summary>
     */
    private void OnLocaleChanged(object sender, int localeId) {
        if (!_currentInformation) return;
        if (_correctGuesses == _currentWord.Length || _lastDeletedStage == hangmanStages.Length) {
            wordText.text = _currentInformation.titles[localeId].ToUpper();
            KeyboardController.instance.ShowKeyboard(false);
            KeyboardController.instance.ShowKeyboard(true);
        } else {
            KeyboardController.instance.ShowKeyboard(false);
            InitializeGame(_currentInformation);
        }
    }

    /**
     * <param name="sender">The sender of the event.</param>
     * <param name="letter">The letter that was pressed.</param>
     * <summary>When a letter is pressed, check if it's in the word and update the UI accordingly.</summary>
     */
    private void OnKeyboardButtonPressed(object sender, char letter) {
        if (!_currentInformation) return;
        if (_correctGuesses == _currentWord.Length || _lastDeletedStage == hangmanStages.Length) return;
        
        bool letterInWord = false;
        for (int i = 0; i < _currentWord.Length; i++) {
            if (_currentWord[i] != letter) continue;
            letterInWord = true;
            
            if (wordText.text[i] != '_') continue;
            _correctGuesses++;
            wordText.text = wordText.text.Remove(i, 1).Insert(i, letter.ToString());
        }

        if (!letterInWord) {
            _incorrectGuesses++;
            if (_incorrectGuesses % _deleteStage == 0)
                hangmanStages[_lastDeletedStage++].SetActive(true);
        }
        
        if (sender is KeyboardButton keyboardButton && keyboardButton.TryGetComponent(out Button button))
            button.interactable = false;
        
        CheckOutcome();
    }

    /**
     * <summary>Checks if the game has ended and calls the appropriate method to dismiss or end the game.</summary>
     */
    private void CheckOutcome() {
        if (_correctGuesses == _currentWord.Length) {
            wordText.color = correctColor;
            Invoke(nameof(GameDismissedOrEnded), 1f);
        }

        if (_lastDeletedStage == hangmanStages.Length) {
            wordText.text = _currentWord;
            wordText.color = incorrectColor;
            Invoke(nameof(GameDismissedOrEnded), 1f);
        }
    }
    
    /**
     * <param name="informationSo">The information to be used for the game.</param>
     * <summary>Initializes the game with the provided information.</summary>
     */
    public void InitializeGame(InformationSo informationSo) {
        ResetToOriginalState();
        KeyboardController.instance.ShowKeyboard(true);
        ShowGamePanel(true);
        
        _currentInformation = informationSo;
        _currentWord = _currentInformation.titles[LocaleSelector.instance.localeId].ToUpper();
        
        int numberOfSpaces = _currentWord.Length - _currentWord.Replace(" ", "").Length;
        foreach (char character in _currentWord) _wordBuilder.Append(character == ' ' ? ' ' : '_');
        _correctGuesses += numberOfSpaces;
        
        wordText.text = _wordBuilder.ToString();
        _deleteStage = Mathf.CeilToInt((float)(_currentWord.Length - numberOfSpaces) / hangmanStages.Length);
    }

    /**
     * <summary>Resets the game to its original state.</summary>
     */
    private void ResetToOriginalState() {
        _correctGuesses = _incorrectGuesses = _lastDeletedStage = 0;
        _deleteStage = 1;
        _currentWord = wordText.text = string.Empty;
        _currentInformation = null;
        _wordBuilder.Clear();
        wordText.color = baseWordColor;
        
        foreach (GameObject hangmanStage in hangmanStages)
            hangmanStage.SetActive(false);
    }

    /**
     * <summary>Dismisses or ends the game.</summary>
     */
    public void GameDismissedOrEnded() {
        _currentInformation = null;
        OnGameDismissedOrEnded?.Invoke(this, EventArgs.Empty);
        KeyboardController.instance.ShowKeyboard(false);
        ShowGamePanel(false);
    }

    /**
     * <param name="show">Whether to show or hide the game panel.</param>
     * <summary>Shows or hides the game panel.</summary>
     */
    private void ShowGamePanel(bool show) {
        if (show) {
            hintButton.interactable = FirebaseAIManager.instance.modelInitialized;
            gamePanel.interactable = true;
            gamePanel.blocksRaycasts = true;
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                _panelFadeTween = gamePanel.DOFade(1, AnimationDuration).SetEase(Ease.OutBack);
            else
                gamePanel.alpha = 1;
        } else {
            gamePanel.interactable = false;
            gamePanel.blocksRaycasts = false;
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                _panelFadeTween = gamePanel.DOFade(0, AnimationDuration).SetEase(Ease.OutBack);
            else
                gamePanel.alpha = 0;
        }
    }

    /**
     * <summary>Gets a hint for the current information.</summary>
     */
    public async void GetHint() {
        try {
            hintText.text = string.Empty;
            hintButton.interactable = false;
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                _hintPanelTween = hintPanel.DOScale(Vector3.one, AnimationDuration).SetEase(Ease.OutBounce);
            else 
                hintPanel.localScale = Vector3.one;
            if (!_currentInformation) return;
        
            hintLoadingSpinner.SetActive(true);
            string hint = await FirebaseAIManager.instance.RequestHint(_currentInformation.titles[0]);
            hintLoadingSpinner.SetActive(false);
            hintText.text = hint;
        } catch (Exception) {
            // ignored
        }
    }

    /**
     * <summary>Closes the hint panel.</summary>
     */
    public void CloseHintPanel() {
        if (SettingsPanel.instance.toggleAnimationsIsOn)
            _hintPanelTween = hintPanel.DOScale(Vector3.zero, AnimationDuration).SetEase(Ease.InCirc);
        else
            hintPanel.localScale = Vector3.zero;
        hintButton.interactable = true;
    }

    /**
     * <summary>Unsubscribes from events and kills tweens if they exist.</summary>
     */
    private void OnDestroy() {
        KeyboardButton.OnKeyboardButtonPressed -= OnKeyboardButtonPressed;
        LocaleSelector.OnLocaleChanged -= OnLocaleChanged;
        _panelFadeTween?.Kill();
        _hintPanelTween?.Kill();
    }
}
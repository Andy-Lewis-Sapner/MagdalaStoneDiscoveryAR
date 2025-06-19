using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class TypeWriterEffect : MonoBehaviour, IPointerClickHandler {
    public static event EventHandler CompleteTextRevealed; // Event triggered when the text is fully revealed
    
    [Header("Typewriter Settings")] 
    [SerializeField] private float charactersPerSecond = 20f; // Characters per second
    [SerializeField] private float interpunctuationDelay = 0.5f; // Delay between punctuation
    
    [Header("Skip Options")]
    [SerializeField] private bool quickSkip; // Skip text quickly
    [SerializeField] [Min(1)] private int skipSpeedup = 5; // Skip speedup
    
    private TMP_Text _textBox; // Reference to the TMP_Text component
    private Coroutine _typeWriterCoroutine; // Coroutine for typing
    private Coroutine _sendCompleteTextRevealedCoroutine; // Coroutine for sending the CompleteTextRevealed event
    private int _currentVisibleCharacterIndex; // Index of the currently visible character
    private bool _currentlySkipping; // Flag to indicate if the text is currently being skipped

    private WaitForSeconds _skipDelay; // Delay between skipped characters
    private WaitForSeconds _simpleDelay; // Delay between simple characters
    private WaitForSeconds _interpunctuationDelay; // Delay between inter-punctuations

    /**
     * <summary>Initializes the typewriter effect delays</summary>
     */
    private void Awake() {
        _textBox = GetComponent<TMP_Text>();
        
        _simpleDelay = new WaitForSeconds(1 / charactersPerSecond);
        _interpunctuationDelay = new WaitForSeconds(interpunctuationDelay);
        _skipDelay = new WaitForSeconds(1 / (charactersPerSecond * skipSpeedup));
    }

    /**
     * <summary>Handles the click event and skips the text</summary>
     */
    public void OnPointerClick(PointerEventData eventData) {
        if (_textBox.maxVisibleCharacters == _textBox.textInfo.characterCount - 1) return;
        Skip();
    }

    /**
     * <summary>Sets the text to be revealed</summary>
     * <param name="text">The text to be revealed</param>
     */
    public void SetText(string text) {
        if (_typeWriterCoroutine != null)
            StopCoroutine(_typeWriterCoroutine);
        
        _textBox.text = text;
        if (!SettingsPanel.instance.toggleAnimationsIsOn) {
            _textBox.maxVisibleCharacters = text.Length;
            _sendCompleteTextRevealedCoroutine = StartCoroutine(SendCompleteTextRevealed());
            return;
        }
        
        _textBox.maxVisibleCharacters = 0;
        _currentVisibleCharacterIndex = 0;
        
        _typeWriterCoroutine = StartCoroutine(TypeWriter());
    }
    
    /**
     * <summary>Sends the CompleteTextRevealed event</summary>
     */
    private IEnumerator SendCompleteTextRevealed() {
        yield return null;
        CompleteTextRevealed?.Invoke(this, EventArgs.Empty);
    }

    /**
     * <summary>Reveals the text character by character</summary>
     */
    private IEnumerator TypeWriter() {
        TMP_TextInfo textInfo = _textBox.textInfo;
        
        while (_currentVisibleCharacterIndex < textInfo.characterCount) {
            int lastCharacterIndex = textInfo.characterCount - 1;
            if (_currentVisibleCharacterIndex == lastCharacterIndex) {
                _textBox.maxVisibleCharacters++;
                CompleteTextRevealed?.Invoke(this, EventArgs.Empty);
                yield break;
            }
            
            char character = textInfo.characterInfo[_currentVisibleCharacterIndex].character;
            _textBox.maxVisibleCharacters++;
            _currentVisibleCharacterIndex++;

            if (_currentlySkipping) yield return _skipDelay;
            else if (character is '?' or '.' or ',' or ':' or ';' or '!' or '-')
                yield return _interpunctuationDelay;
            else
                yield return _simpleDelay;
        }
    }

    /**
     * <summary>Skips the text</summary>
     */
    private void Skip() {
        if (_currentlySkipping) return;
        _currentlySkipping = true;

        if (!quickSkip) {
            StartCoroutine(SkipSpeedupReset());
            return;
        }
        
        StopCoroutine(_typeWriterCoroutine);
        _textBox.maxVisibleCharacters = _textBox.textInfo.characterCount;
        CompleteTextRevealed?.Invoke(this, EventArgs.Empty);
    }

    /**
     * <summary>Resets the skip speedup</summary>
     */
    private IEnumerator SkipSpeedupReset() {
        yield return new WaitUntil(() => _textBox.maxVisibleCharacters == _textBox.textInfo.characterCount - 1);
        _currentlySkipping = false;
    }

    /**
     * <summary>Changes the characters per second</summary>
     * <param name="newCharactersPerSecond">The new characters per second</param>
     */
    public void ChangeCharactersPerSecond(float newCharactersPerSecond) {
        charactersPerSecond = newCharactersPerSecond;
        _simpleDelay = new WaitForSeconds(1 / charactersPerSecond);
        _skipDelay = new WaitForSeconds(1 / (charactersPerSecond * skipSpeedup));
    }

    /**
     * <summary>Unsubscribes from events</summary>
     */
    private void OnDestroy() {
        if (_typeWriterCoroutine != null) StopCoroutine(_typeWriterCoroutine);
        if (_sendCompleteTextRevealedCoroutine != null) StopCoroutine(_sendCompleteTextRevealedCoroutine);
    }
}
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour {
    private const string LocaleKey = "Locale";
    
    public static LocaleSelector instance { get; private set; } // Singleton
    public static event EventHandler<int> OnLocaleChanged; // Event for language change
    private static int _localeId = -1; // Current language

    // Current language property
    public int localeId {
        get => _localeId;
        private set {
            if (_localeId == value) return;
            _localeId = value;
            // Save current language
            if (PlayerPrefs.GetInt(LocaleKey, -1) != value) PlayerPrefs.SetInt(LocaleKey, _localeId);
            // Invoke event
            OnLocaleChanged?.Invoke(this, _localeId);
        }
    }
    
    private bool _isCoroutineActive; // Flag for coroutine

    /**
     * <summary>Initialization of the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /**
     * <summary>Loads the current saved language</summary>
     */
    private void Start() {
        localeId = PlayerPrefs.GetInt(LocaleKey, 0);
        ChangeLocale(localeId);
    }

    /**
     * <summary>Changes the language of the game</summary>
     */
    public void ChangeLocale(int localeID) {
        if (_isCoroutineActive) return;
        StartCoroutine(SetLocale(localeID));
    }
    
    /**
     * <summary>Changes the language of the game in Localization Settings</summary>
     */
    private IEnumerator SetLocale(int localeID) {
        _isCoroutineActive = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        localeId = localeID;
        _isCoroutineActive = false;
    }
}
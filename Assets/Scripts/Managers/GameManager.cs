using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance { get; private set; }
    
    // All PlayerPrefs keys to save game progress and settings
    private const string CompletedSynagogueTour = "CompletedSynagogueTour";
    private const string CompletedStoneTour = "CompletedStoneTour";
    private const string CompletedQuiz = "CompletedQuiz"; 
    private const string ToggleAnimationsSetting = "ToggleAnimationsSetting";
    private const string PlayVoiceSetting = "PlayVoiceSetting";
    private const string MuteBackgroundMusicSetting = "MuteBackgroundMusicSetting";
    private const string VolumeSetting = "VolumeSetting";

    // Saves if synagogue tour has been completed
    public bool completedSynagogueTour {
        get => _completedSynagogueTour;
        set {
            if (value == _completedSynagogueTour) return;
            _completedSynagogueTour = value;
            PlayerPrefs.SetInt(CompletedSynagogueTour, value ? 1 : 0);
        }
    }

    // Saves if stone tour has been completed
    public bool completedStoneTour {
        get => _completedStoneTour;
        set {
            if (value == _completedStoneTour) return;
            _completedStoneTour = value;
            PlayerPrefs.SetInt(CompletedStoneTour, value ? 1 : 0);
        }
    }

    // Saves if quiz has been completed
    public bool completedQuiz {
        get => _completedQuiz;
        set {
            if (value == _completedQuiz) return;
            _completedQuiz = value;
            PlayerPrefs.SetInt(CompletedQuiz, value ? 1 : 0);
        }
    }

    // Saves toggle animations setting
    public bool toggleAnimationsSetting {
        get => _toggleAnimationsSetting;
        set {
            if (_toggleAnimationsSetting == value) return;
            _toggleAnimationsSetting = value;
            PlayerPrefs.SetInt(ToggleAnimationsSetting, value ? 1 : 0);
        }
    }
    
    // Saves play voice setting
    public bool playVoiceSetting {
        get => _playVoiceSetting;
        set {
            if (_playVoiceSetting == value) return;
            _playVoiceSetting = value;
            PlayerPrefs.SetInt(PlayVoiceSetting, value ? 1 : 0);
        }
    }
    
    // Saves mute background music
    public bool muteBackgroundMusic {
        get => _muteBackgroundMusic;
        set {
            if (_muteBackgroundMusic == value) return;
            _muteBackgroundMusic = value;
            PlayerPrefs.SetInt(MuteBackgroundMusicSetting, value ? 1 : 0);
        }
    }

    // Saves audio volume
    public float audioVolume {
        get => _audioVolume;
        set {
            if (Mathf.Approximately(_audioVolume, value)) return;
            _audioVolume = value;
            PlayerPrefs.SetFloat(VolumeSetting, value);
        }
    }

    // All game progress and settings
    private bool _completedStoneTour,
        _completedSynagogueTour,
        _completedQuiz,
        _toggleAnimationsSetting,
        _playVoiceSetting,
        _muteBackgroundMusic;
    private float _audioVolume;
    
    /**
     * <summary>Initializes the singleton and retrieves saved states</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
        RetrieveSavedStates();
    }

    /**
     * <summary>Retrieves saved game progress and settings</summary>
     */
    private void RetrieveSavedStates() {
        _completedStoneTour = PlayerPrefs.GetInt(CompletedStoneTour, 0) == 1;
        _completedSynagogueTour = PlayerPrefs.GetInt(CompletedSynagogueTour, 0) == 1;
        _completedQuiz = PlayerPrefs.GetInt(CompletedQuiz, 0) == 1;
        
        _toggleAnimationsSetting = PlayerPrefs.GetInt(ToggleAnimationsSetting, 1) == 1;
        _playVoiceSetting = PlayerPrefs.GetInt(PlayVoiceSetting, 1) == 1;
        _muteBackgroundMusic = PlayerPrefs.GetInt(MuteBackgroundMusicSetting, 0) == 1;
        
        _audioVolume = PlayerPrefs.GetFloat(VolumeSetting, 1f);
    }

    /**
     * <summary>Resets game progress</summary>
     */
    private static void ResetProgress() {
        PlayerPrefs.SetInt(CompletedSynagogueTour, 0);
        PlayerPrefs.SetInt(CompletedStoneTour, 0);
        PlayerPrefs.SetInt(CompletedQuiz, 0);
    }

    /**
     * <summary>Resets game progress and retrieves saved states</summary>
     */
    public void ResetProgressAndRetrieveStates() {
        ResetProgress();
        RetrieveSavedStates();
    }
}
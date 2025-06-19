using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance { get; private set; } // Singleton
    private const float FadeDuration = 0.5f; // Fade duration

    [SerializeField] private AudioSource audioSource; // Audio source for voice
    [SerializeField] private AudioSource backgroundAudioSource; // Audio source for background music
    [SerializeField] private AudioSource buttonClickAudioSource; // Audio source for clicks

    [Header("Audio Clips")] 
    [SerializeField] private AudioClip backgroundMusicForMenu; // Background music for menu
    [SerializeField] private AudioClip backgroundMusicForSynagogue; // Background music for synagogue
    [SerializeField] private AudioClip backgroundMusicForStone; // Background music for stone
    [SerializeField] private AudioClip backgroundMusicForQuiz; // Background music for quiz
    [SerializeField] private AudioClip correctAnswer; // Audio clip for the correct answer in the quiz
    [SerializeField] private AudioClip wrongAnswer;// Audio clip for wrong answer in the quiz
    
    private Coroutine _fadeMusicCoroutine; // Coroutine for fading music
    
    // Property for audio volume
    public float audioVolume {
        get => audioSource.volume;
        set {
            if (Mathf.Approximately(audioSource.volume, value)) return;
            SetVolumeInAudioSources(value);
            GameManager.instance.audioVolume = value;
        }
    }

    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /**
     * <summary>Sets up the audio volume</summary>
     */
    private void Start() {
        audioVolume = GameManager.instance.audioVolume;
        SetVolumeInAudioSources(audioVolume);
        backgroundAudioSource.mute = GameManager.instance.muteBackgroundMusic;
        SceneManagerOnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    /**
     * <summary>Sets the audio volume</summary>
     */
    private void SetVolumeInAudioSources(float value) {
        audioSource.volume = value;
        backgroundAudioSource.volume = 0.7f * value;
        buttonClickAudioSource.volume = value;
    }

    /**
     * <summary>Plays background music based on the loaded scene</summary>
     */
    private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) {
        audioSource.Stop();
        
        AudioClip clip = scene.name switch {
            nameof(SceneNames.MenuScene) => backgroundMusicForMenu,
            nameof(SceneNames.SynagogueScene) => backgroundMusicForSynagogue,
            nameof(SceneNames.StoneScene) => backgroundMusicForStone,
            nameof(SceneNames.StoneSceneNoAR) => backgroundMusicForStone,
            nameof(SceneNames.QuizScene) => backgroundMusicForQuiz,
            _ => null
        };

        if (clip) 
            _fadeMusicCoroutine = StartCoroutine(PlayBackgroundMusic(clip));
    }

    /**
     * <summary>Plays the correct answer audio clip</summary>
     */
    public void PlayCorrectAnswer() {
        PlayNewClip(correctAnswer, false);
    }

    /**
     * <summary>Plays the wrong answer audio clip</summary>
     */
    public void PlayWrongAnswer() {
        PlayNewClip(wrongAnswer, false);
    }

    /**
     * <summary>Transitions the background music to a new clip</summary>
     */
    private IEnumerator PlayBackgroundMusic(AudioClip clip) {
        float currentVolume = backgroundAudioSource.volume;
        float fadeStep = currentVolume / FadeDuration;
        
        while (backgroundAudioSource.volume > 0) {
            backgroundAudioSource.volume -= fadeStep * Time.deltaTime;
            yield return null;
        }
        
        backgroundAudioSource.volume = 0;
        backgroundAudioSource.Stop();
        backgroundAudioSource.clip = clip;
        backgroundAudioSource.Play();
        
        while (backgroundAudioSource.volume < currentVolume) {
            backgroundAudioSource.volume += fadeStep * Time.deltaTime;
            yield return null;
        }
        
        backgroundAudioSource.volume = currentVolume;
    }

    /**
     * <param name="clip">The audio clip to play</param>
     * <param name="voiceOver">Is the clip a voice over</param>
     * <summary>Plays a new audio clip</summary>
     */
    public void PlayNewClip(AudioClip clip, bool voiceOver = true) {
        audioSource.Stop();
        audioSource.clip = clip;
        if (!voiceOver || SettingsPanel.instance.playVoiceForInformationIsOn) audioSource.Play();
    }

    /**
     * <summary>Stops the audio clip and removes it</summary>
     */ 
    public void RemoveClip() {
        if (audioSource.isPlaying) audioSource.Stop();
        audioSource.clip = null;
    }

    /**
     * <summary>Plays or pauses the audio clip</summary>
     */
    public void PlayPauseClip() {
        if (!audioSource.clip || !SettingsPanel.instance.playVoiceForInformationIsOn) return;
        if (audioSource.isPlaying) audioSource.Pause();
        else audioSource.Play();
    }

    /**
     * <summary>Restarts the audio clip</summary>
     */
    public void RestartClip() {
        if (!audioSource.clip || !SettingsPanel.instance.playVoiceForInformationIsOn) return;
        audioSource.Stop();
        audioSource.Play();
    }
    
    /**
     * <summary>Plays the button click sound</summary>
     */
    public void PlayButtonClickSound() {
        buttonClickAudioSource.Stop();
        buttonClickAudioSource.Play();
    }

    /**
     * <summary>Mutes or unmutes the background music</summary>
     */
    public void MuteBackgroundMusic() {
        backgroundAudioSource.mute = GameManager.instance.muteBackgroundMusic;
    }

    /**
     * <summary>Unsubscribes from the scene loaded event and stops the fade music coroutine if it is running</summary>
     */
    private void OnDestroy() {
        SceneManager.sceneLoaded -= SceneManagerOnSceneLoaded;
        if (_fadeMusicCoroutine != null) StopCoroutine(_fadeMusicCoroutine);
    }
}
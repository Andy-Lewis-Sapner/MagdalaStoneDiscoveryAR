using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoneSceneProgress : MonoBehaviour {
    public static StoneSceneProgress instance { get; private set; } // Singleton
    private static readonly HashSet<string> DiscoveredSymbols = new(); // Stores discovered symbols
    
    [SerializeField] private int progressToDiscover; // Number of symbols to discover
    [SerializeField] private Slider progressBar; // Progress bar
    [SerializeField] private Button continueToQuizButton; // Button to continue the quiz
    [SerializeField] private GameObject instructionObject; // Instruction object

    private Tween _progressBarTween; // Progress bar tween
    private bool _shouldContinueToQuizBeActive; // Should the "continue to quiz" button be active
    private int _currentProgress; // Current progress

    /**
     * <summary>Initializes the singleton</summary>
     */
    private void Awake() {
        if (instance && instance != this) Destroy(gameObject);
        else instance = this;
    }

    /**
     * <summary>Starts the progress by subscribing to events</summary>
     */
    private void Start() {
        InformationManager.OnInformationPanelOpened += InformationManagerOnInformationPanelOpened;
        InformationManager.OnInformationPanelClosed += InformationManagerOnInformationPanelClosed;
        
        continueToQuizButton.gameObject.SetActive(false);
        progressBar.gameObject.SetActive(false);
        instructionObject.SetActive(false);

        if (SceneManager.GetActiveScene().name.Equals(nameof(SceneNames.StoneScene3D))) StartProgress();
    }

    /**
     * <summary>Show or hide the "continue to quiz" button</summary>
     */
    private void InformationManagerOnInformationPanelOpened(object sender, EventArgs e) {
        if (_shouldContinueToQuizBeActive) continueToQuizButton.gameObject.SetActive(false);
    } 
    
    /**
     * <summary>Show or hide the "continue to quiz" button</summary>
     */
    private void InformationManagerOnInformationPanelClosed(object sender, EventArgs e) {
        if (_shouldContinueToQuizBeActive) continueToQuizButton.gameObject.SetActive(true);
    }

    /**
     * <summary>Starts the progress</summary>
     */
    public void StartProgress() {
        if (progressBar.gameObject.activeSelf) return;
        
        DiscoveredSymbols.Clear();
        _currentProgress = 0;
        progressBar.value = 0f;
        progressBar.gameObject.SetActive(true);
        instructionObject.SetActive(true);
    }

    /**
     * <summary>Adds a symbol to the discovered symbols</summary>
     * <param name="itemName">The name of the symbol</param>
     */
    public void AddToDiscovered(string itemName) {
        if (!DiscoveredSymbols.Add(itemName)) return;
        _currentProgress++;
        if (_currentProgress <= progressToDiscover) {
            if (SettingsPanel.instance.toggleAnimationsIsOn)
                _progressBarTween = DOTween.To(() => progressBar.value, x => progressBar.value = x,
                    _currentProgress / (float)progressToDiscover, 0.5f);
            else
                progressBar.value = progressToDiscover != 0 ? _currentProgress / (float)progressToDiscover : 0f;
        }

        if (_currentProgress != progressToDiscover) return;
        instructionObject.SetActive(false);
        if (GameManager.instance.completedStoneTour) return;
        _shouldContinueToQuizBeActive = true;
        GameManager.instance.completedStoneTour = true;
    }

    /**
     * <summary>Unsubscribes from events</summary>
     */
    private void OnDestroy() {
        InformationManager.OnInformationPanelOpened -= InformationManagerOnInformationPanelOpened;
        InformationManager.OnInformationPanelClosed -= InformationManagerOnInformationPanelClosed;
        _progressBarTween?.Kill();
    }
}
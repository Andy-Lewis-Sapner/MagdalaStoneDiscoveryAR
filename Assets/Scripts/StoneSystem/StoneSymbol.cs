using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StoneSymbol : MonoBehaviour {
    private static readonly Dictionary<string, bool> StoneSymbolsAppearance = new(); // Dictionary to store the appearance state of each symbol
    private const float RotationSpeed = 10f; // The speed at which the symbol rotates
    
    [SerializeField] private InformationSo informationSo; // The information associated with the symbol
    [SerializeField] private float distance = 0.1f; // The distance between the symbol and the player
    [SerializeField] private float scaleChange = 200f; // The amount to scale the symbol (in the scene with no AR)
    
    private Transform _symbolGameObject; // The game object representing the symbol
    private Tween _scaleTween; // The tween for scaling the symbol
    private Vector3 _originalScale, _rotationAxis, _position, _forward; // The original scale and rotation axis
    private bool _subscribedToHangmanEvent, _isARScene; // Whether the symbol has been subscribed to the hangman event

    /**
     * <summary>Starts the progress by subscribing to events and setting up the symbol</summary>
     */
    private void Start() {
        InformationManager.OnInformationPanelClosed += DeactivateSymbolGameObject;
        _rotationAxis = informationSo.rotationAxis switch {
            RotationAxis.Up => Vector3.up,
            RotationAxis.Forward => Vector3.forward,
            RotationAxis.Right => Vector3.right,
            _ => Vector3.up
        };

        _isARScene = SceneManager.GetActiveScene().name == nameof(SceneNames.StoneSceneAR);
    }

    /**
     * <summary>Updates the symbol's position and rotation</summary>
     */
    private void Update() {
        if (!SettingsPanel.instance.toggleAnimationsIsOn) return;
        if (!_symbolGameObject) return;
        RotateSymbol();
    }

    /**
     * <summary>Rotates the symbol</summary>
     */
    private void RotateSymbol() {
        _symbolGameObject.Rotate(_rotationAxis * (RotationSpeed * Time.deltaTime));
    }

    /**
     * <summary>Deactivates the symbol game object</summary>
     */
    private void DeactivateSymbolGameObject(object sender, EventArgs eventArgs) {
        if (!_symbolGameObject) return;
        if (SettingsPanel.instance.toggleAnimationsIsOn) {
            _scaleTween?.Kill();
            _scaleTween = _symbolGameObject.DOScale(Vector3.zero, 0.5f).SetEase(Ease.InBack);
        } else {
            _symbolGameObject.localScale = Vector3.zero;
        }
        StoneSymbolsAppearance[informationSo.titles[0]] = false;
    }

    /**
     * <summary>Shows the information about the symbol</summary>
     */
    public void ShowInfoAboutSymbol(Vector3 position, Vector3 forward) {
        if (IsAnySymbolActive()) return;
        _position = position;
        _forward = forward;
        HangmanController.instance.InitializeGame(informationSo);
        HangmanController.OnGameDismissedOrEnded += HangmanControllerOnGameDismissedOrEnded;
        _subscribedToHangmanEvent = true;
    }

    /**
     * <summary>Handles the event when the hangman game is dismissed or ended</summary>
     */
    private void HangmanControllerOnGameDismissedOrEnded(object sender, EventArgs e) {
        HangmanController.OnGameDismissedOrEnded -= HangmanControllerOnGameDismissedOrEnded;
        _subscribedToHangmanEvent = false;
        InformationManager.instance.ShowInformation(informationSo);
        ShowSymbolGameObject();
        StoneSceneProgress.instance.AddToDiscovered(informationSo.titles[0]);
    }

    /**
     * <summary>Shows the symbol game object</summary>
     */
    private void ShowSymbolGameObject() {
        if (!_symbolGameObject) {
            if (!Camera.main) return;
            GameObject symbolGameObject = Instantiate(informationSo.symbolPrefab);
            _symbolGameObject = symbolGameObject.transform;
            _originalScale = _symbolGameObject.localScale;
            if (!_isARScene) _originalScale = scaleChange * _originalScale;
            _symbolGameObject.localScale = Vector3.zero;
        }
        
        if (_symbolGameObject.localScale == _originalScale) return;
        if (SettingsPanel.instance.toggleAnimationsIsOn) {
            _scaleTween?.Kill();
            _scaleTween = _symbolGameObject.DOScale(_originalScale, 0.5f).SetEase(Ease.OutBack);
        } else {
            _symbolGameObject.localScale = _originalScale;
        }
        
        _symbolGameObject.position = _position + _forward * distance;
        StoneSymbolsAppearance[informationSo.titles[0]] = true;
    }

    /**
     * <summary>Checks if any symbol is currently active</summary>
     */
    private static bool IsAnySymbolActive() {
        return StoneSymbolsAppearance.Values.Any(state => state);
    }

    /**
     * <summary>Unsubscribes from events</summary>
     */
    private void OnDestroy() {
        InformationManager.OnInformationPanelClosed -= DeactivateSymbolGameObject;
        if (_subscribedToHangmanEvent)
            HangmanController.OnGameDismissedOrEnded -= HangmanControllerOnGameDismissedOrEnded;
        _scaleTween?.Kill();
    }
}
